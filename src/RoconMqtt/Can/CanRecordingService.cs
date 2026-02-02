using System.Collections.Concurrent;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

/// <summary>
/// Background service for recording CAN bus traffic
/// </summary>
public sealed class CanRecordingService : BackgroundService
{
    private readonly ICanBus _canBus;
    private readonly ILogger<CanRecordingService> _logger;

    private readonly ConcurrentBag<RecordedCanFrame> _recordedFrames = new();
    private DateTime? _recordingStartedAt;
    private CanRecordingOptions? _currentOptions;
    private bool _isRecording;
    private readonly SemaphoreSlim _recordingLock = new(1, 1);

    public CanRecordingService(
        ICanBus canBus,
        ILogger<CanRecordingService> logger)
    {
        _canBus = canBus;
        _logger = logger;
    }

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
                _logger.LogWarning("CAN recording already in progress");
                return false;
            }

            _recordedFrames.Clear();
            _recordingStartedAt = DateTime.UtcNow;
            _currentOptions = options;
            _isRecording = true;

            var filterInfo = options?.CanIdFilter != null && options.CanIdFilter.Count > 0
                ? $" with filter: {string.Join(", ", options.CanIdFilter)}"
                : " (all frames)";

            _logger.LogInformation("CAN recording started at {StartTime}{Filter}",
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
                _logger.LogWarning("No active CAN recording to stop");
                return null;
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

            _logger.LogInformation(
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CAN Recording Service started");

        try
        {
            await foreach (var frame in _canBus.ReadFramesAsync(stoppingToken))
            {
                if (!_isRecording)
                {
                    continue;
                }

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

                _logger.LogDebug(
                    "Recorded frame: {CanId}#{Data}",
                    recordedFrame.CanId,
                    recordedFrame.Data);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation("CAN Recording Service stopping (cancellation requested)", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CAN Recording Service", ex);
            throw;
        }

        _logger.LogInformation("CAN Recording Service stopped");
    }

    public override void Dispose()
    {
        _recordingLock.Dispose();
        base.Dispose();
    }
}
