using System.Collections.Concurrent;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

/// <summary>
/// Background service for recording CAN bus traffic
/// </summary>
public sealed class CanRecordingService(
    ICanBus canBus,
    ILogger<CanRecordingService> logger) : BackgroundService
{
    private readonly ConcurrentBag<RecordedCanFrame> _recordedFrames = [];
    private DateTime? _recordingStartedAt;
    private CanRecordingOptions? _currentOptions;
    private bool _isRecording = false;
    private readonly SemaphoreSlim _recordingLock = new(1, 1);
    private CancellationTokenSource? _recordingCts;

    /// <summary>
    /// Check if recording is currently active
    /// </summary>
    public bool IsRecording => _isRecording;

    /// <summary>
    /// Start recording CAN bus traffic
    /// </summary>
    /// <param name="options">Recording options (filters, description)</param>
    /// <returns>True if recording started, false if already recording</returns>
    public async Task<bool> StartRecordingAsync(CanRecordingOptions? options = null)
    {
        await _recordingLock.WaitAsync();
        try
        {
            if (_isRecording)
            {
                logger.LogWarning("CAN recording already in progress");
                return false;
            }

            _recordedFrames.Clear();
            _recordingStartedAt = DateTime.UtcNow;
            _currentOptions = options;
            _isRecording = true;

            // Start the read loop only when recording begins
            _recordingCts = new CancellationTokenSource();
            _ = Task.Run(() => RecordFramesAsync(_recordingCts.Token), _recordingCts.Token);

            var filterInfo = options?.CanIdFilter != null && options.CanIdFilter.Count > 0
                ? $" with filter: {string.Join(", ", options.CanIdFilter)}"
                : " (all frames)";

            logger.LogInformation("CAN recording started at {StartTime}{Filter}",
                _recordingStartedAt.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                filterInfo);

            return true;
        }
        finally
        {
            _recordingLock.Release();
        }
    }

    /// <summary>
    /// Stop recording and return captured frames
    /// </summary>
    /// <returns>Recording result with all captured frames, or null if not recording</returns>
    public async Task<CanRecordingResult?> StopRecordingAsync()
    {
        await _recordingLock.WaitAsync();
        try
        {
            if (!_isRecording || _recordingStartedAt == null)
            {
                logger.LogWarning("No active CAN recording to stop");
                return null;
            }

            // Cancel the recording loop
            if(_recordingCts != null)
            {
                await _recordingCts.CancelAsync();
                _recordingCts.Dispose();
                _recordingCts = null;
            }

            var stoppedAt = DateTime.UtcNow;
            var frames = _recordedFrames.ToList();

            var result = new CanRecordingResult
            {
                StartedAt = _recordingStartedAt.Value,
                StoppedAt = stoppedAt,
                FrameCount = frames.Count,
                Description = _currentOptions?.Description,
                CanIdFilter = _currentOptions?.CanIdFilter,
                Frames = frames
            };

            _isRecording = false;
            _recordingStartedAt = null;
            _currentOptions = null;
            _recordedFrames.Clear();

            logger.LogInformation(
                "CAN recording stopped. Captured {FrameCount} frames in {Duration:F3} seconds",
                result.FrameCount,
                result.Duration.TotalSeconds);

            return result;
        }
        finally
        {
            _recordingLock.Release();
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CAN Recording Service started (passive mode - will activate on StartRecordingAsync)");

        // No longer starts reading immediately - waits for explicit StartRecordingAsync call
        // This prevents interference with RoconMqttPublisher's QueryDeviceIdentifiersAsync
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Internal method that performs the actual frame recording
    /// </summary>
    private async Task RecordFramesAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CAN frame recording loop");

        try
        {
            await foreach (var frame in canBus.ReadFramesAsync(cancellationToken))
            {
                // Apply CAN ID filter if configured
                if (_currentOptions?.CanIdFilter != null && _currentOptions.CanIdFilter.Count > 0)
                {
                    var canIdHex = $"0x{frame.Id:X}";
                    if (!_currentOptions.CanIdFilter.Contains(canIdHex, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                // Record the frame
                var recordedFrame = new RecordedCanFrame
                {
                    Timestamp = DateTime.UtcNow,
                    CanId = $"0x{frame.Id:X}",
                    Data = BitConverter.ToString(frame.Data).Replace("-", " "),
                    DataLength = frame.Data.Length
                };

                _recordedFrames.Add(recordedFrame);

                logger.LogDebug(
                    "Recorded frame: {CanId}#{Data}",
                    recordedFrame.CanId,
                    recordedFrame.Data);
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogDebug(ex, "CAN frame recording loop cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CAN frame recording loop");
        }
    }

    public override void Dispose()
    {
        _recordingCts?.Cancel();
        _recordingCts?.Dispose();
        _recordingLock.Dispose();
        base.Dispose();
    }
}
