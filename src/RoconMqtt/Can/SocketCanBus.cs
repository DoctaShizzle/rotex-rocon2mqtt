using Microsoft.Extensions.Options;
using RoconMqtt.Can.Options;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using CanFrameModel = RoconMqtt.Can.Models.CanFrame;

namespace RoconMqtt.Can;

/// <summary>
/// SocketCAN implementation of the CAN bus interface.
/// Manages frame broadcasting to multiple subscribers using channels.
/// </summary>
public partial class SocketCanBus : ICanBus, IAsyncDisposable, IDisposable
{
    private readonly ICanSocket _canSocket;
    private readonly ILogger<SocketCanBus> _logger;
    private readonly CanOptions _options;
    private bool _disposed;

    // Broadcast infrastructure
    private readonly ConcurrentDictionary<Guid, FrameSubscriber> _subscribers = new();
    private readonly CancellationTokenSource _readerCts = new();
    private readonly Task? _readerTask;
    
    // Reconnection state
    private int _consecutiveErrors = 0;
    private const int MaxConsecutiveErrors = 10;

    public SocketCanBus(ICanSocket canSocket, ILogger<SocketCanBus> logger, IOptions<CanOptions> options)
    {
        _canSocket = canSocket ?? throw new ArgumentNullException(nameof(canSocket));
        _logger = logger;
        _options = options.Value;

        // Start the background reader task
        _readerTask = Task.Run(BackgroundReaderAsync, _readerCts.Token);
        LogSocketCanBusInitialized(_logger, _options.CanInterfaceName);
    }

    /// <summary>
    /// Background task that continuously reads from the CAN socket and broadcasts frames to all subscribers.
    /// Uses true async I/O without blocking any threads.
    /// </summary>
    private async Task BackgroundReaderAsync()
    {
        LogBackgroundReaderStarted(_logger);

        try
        {
            while (!_readerCts.Token.IsCancellationRequested)
            {
                // If no subscribers are listening, don't perform I/O
                // This conserves resources when idle
                if (_subscribers.IsEmpty)
                {
                    try
                    {
                        await Task.Delay(10, _readerCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    continue;
                }
                
                try
                {
                    // Receive CAN frame from low-level socket
                    var (canId, data) = await _canSocket.ReceiveAsync(_readerCts.Token);
                    
                    _consecutiveErrors = 0; // Reset on successful read
                    
                    // Format candump command for logging
                    var dataHex = BitConverter.ToString(data).Replace("-", " ");
                    var candumpOutput = $"{_options.CanInterfaceName}  {canId:X3}   [{data.Length}]  {dataHex}";
                    
                    LogReceivedCanFrame(_logger, canId, data.Length, candumpOutput);

                    // Broadcast to all subscribers
                    var canFrameModel = new CanFrameModel(canId, data);
                    foreach (var subscriber in _subscribers.Values)
                    {
                        // Apply per-subscriber filter
                        if (subscriber.CanIdFilter.HasValue && subscriber.CanIdFilter.Value != canId)
                        {
                            continue;
                        }

                        // Try to write to subscriber channel (non-blocking)
                        subscriber.Channel.Writer.TryWrite(canFrameModel);
                    }
                }
                catch (OperationCanceledException)
                {
                    LogCanFrameReadingCancelled(_logger);
                    break;
                }
                catch (Exception) when (_disposed || _readerCts.Token.IsCancellationRequested)
                {
                    // Expected during shutdown
                    LogCanFrameReadingCancelled(_logger);
                    break;
                }
                catch (Exception ex)
                {
                    _consecutiveErrors++;
                    LogErrorReadingCanFrame(_logger, ex);
                    
                    // If too many consecutive errors, attempt reconnection
                    if (_consecutiveErrors >= MaxConsecutiveErrors)
                    {
                        LogTooManyConsecutiveErrors(_logger, _consecutiveErrors);
                        
                        var reconnected = await _canSocket.TryReconnectAsync(_readerCts.Token);
                        if (!reconnected)
                        {
                            // Failed to reconnect, continue with exponential backoff
                            var delayMs = Math.Min(1000 * Math.Pow(2, Math.Min(_consecutiveErrors - MaxConsecutiveErrors, 5)), 30000);
                            try
                            {
                                await Task.Delay((int)delayMs, _readerCts.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Brief delay before retry to avoid tight error loop
                        try
                        {
                            await Task.Delay(100, _readerCts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
        catch (Exception ex)
        {
            // Unexpected error that caused reader to stop
            LogUnexpectedErrorInBackgroundReader(_logger, ex);
        }
        finally
        {
            LogBackgroundReaderStopped(_logger);
        }
    }

    public async IAsyncEnumerable<CanFrameModel> ReadFramesAsync([EnumeratorCancellation] CancellationToken token, uint? canId = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        LogStartReadingCanFrames(_logger);

        // Create a new subscriber
        var subscriber = new FrameSubscriber
        {
            CanIdFilter = canId
        };

        // Register subscriber
        if (!_subscribers.TryAdd(subscriber.Id, subscriber))
        {
            throw new InvalidOperationException("Failed to register frame subscriber");
        }

        if (canId.HasValue)
        {
            LogSubscriberRegisteredWithFilter(_logger, subscriber.Id, canId.Value);
        }
        else
        {
            LogSubscriberRegistered(_logger, subscriber.Id);
        }

        try
        {
            // Read from subscriber's channel until cancelled
            // Use WaitToReadAsync to allow responsive cancellation
            while (!token.IsCancellationRequested && await subscriber.Channel.Reader.WaitToReadAsync(token))
            {
                // Try to read all available items without blocking
                while (subscriber.Channel.Reader.TryRead(out var frame))
                {
                    yield return frame;
                    
                    // Check cancellation between frames for responsiveness
                    if (token.IsCancellationRequested)
                        yield break;
                }
            }
        }
        finally
        {
            // Unregister subscriber on completion/cancellation
            if (_subscribers.TryRemove(subscriber.Id, out _))
            {
                subscriber.Channel.Writer.Complete();
                LogSubscriberUnregistered(_logger, subscriber.Id);
            }
        }
    }

    public async Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        try
        {
            // Format cansend command for logging
            var dataHex = Convert.ToHexString(data);
            var cansendCommand = $"cansend {_options.CanInterfaceName} {canId:X3}#{dataHex}";
            
            LogSendingCanFrame(_logger, canId, data.Length, cansendCommand);
            
            // Delegate to low-level socket
            await _canSocket.SendAsync(canId, data, token);
            
            LogCanFrameSentSuccessfully(_logger);
        }
        catch (Exception ex)
        {
            LogFailedToSendCanFrame(_logger, ex, canId);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            LogDisposingSocketCanBus(_logger);

            // Stop the background reader
            _readerCts.Cancel();
            if (_readerTask != null)
            {
                try
                {
                    _readerTask.Wait(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    LogErrorWaitingForReaderShutdown(_logger, ex);
                }
            }

            // Close all subscriber channels
            foreach (var subscriber in _subscribers.Values)
            {
                try
                {
                    subscriber.Channel.Writer.Complete();
                }
                catch
                {
                    // Ignore errors during channel cleanup
                }
            }
            _subscribers.Clear();

            // Dispose the CAN socket
            try
            {
                _canSocket?.Dispose();
            }
            catch (Exception ex)
            {
                LogErrorDisposingSocket(_logger, ex);
            }

            // Dispose CancellationTokenSource LAST
            try
            {
                _readerCts.Dispose();
            }
            catch (Exception ex)
            {
                LogErrorDisposingCancellationTokenSource(_logger, ex);
            }

            LogSocketCanBusDisposed(_logger);
        }

        _disposed = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        LogDisposingSocketCanBus(_logger);

        // Stop the background reader
        await _readerCts.CancelAsync().ConfigureAwait(false);
        if (_readerTask != null)
        {
            try
            {
                await _readerTask.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogErrorWaitingForReaderShutdown(_logger, ex);
            }
        }

        // Close all subscriber channels
        foreach (var subscriber in _subscribers.Values)
        {
            try
            {
                subscriber.Channel.Writer.Complete();
            }
            catch
            {
                // Ignore errors during channel cleanup
            }
        }
        _subscribers.Clear();

        // Dispose the CAN socket
        try
        {
            await _canSocket.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogErrorDisposingSocket(_logger, ex);
        }

        // Dispose CancellationTokenSource LAST
        try
        {
            _readerCts.Dispose();
        }
        catch (Exception ex)
        {
            LogErrorDisposingCancellationTokenSource(_logger, ex);
        }

        LogSocketCanBusDisposed(_logger);
    }

    /// <summary>
    /// Represents a subscriber to the CAN frame broadcast
    /// </summary>
    private sealed class FrameSubscriber
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Channel<CanFrameModel> Channel { get; }
        public uint? CanIdFilter { get; set; }

        public FrameSubscriber()
        {
            Channel = System.Threading.Channels.Channel.CreateUnbounded<CanFrameModel>();
        }
    }

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "SocketCAN bus initialized on interface {InterfaceName}")]
    private static partial void LogSocketCanBusInitialized(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Debug, Message = "Starting to read CAN frames")]
    private static partial void LogStartReadingCanFrames(ILogger logger);

    [LoggerMessage(EventId = 2004, Level = LogLevel.Debug, Message = "CAN frame reading cancelled")]
    private static partial void LogCanFrameReadingCancelled(ILogger logger);

    [LoggerMessage(EventId = 2005, Level = LogLevel.Error, Message = "Error reading CAN frame")]
    private static partial void LogErrorReadingCanFrame(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2006, Level = LogLevel.Debug, Message = "Received CAN frame: ID=0x{CanId:X3}, DataLength={DataLength} | {CandumpOutput}")]
    private static partial void LogReceivedCanFrame(ILogger logger, uint canId, int dataLength, string candumpOutput);

    [LoggerMessage(EventId = 2007, Level = LogLevel.Debug, Message = "Sending CAN frame: ID=0x{CanId:X3}, DataLength={DataLength} | Command: {CansendCommand}")]
    private static partial void LogSendingCanFrame(ILogger logger, uint canId, int dataLength, string cansendCommand);

    [LoggerMessage(EventId = 2008, Level = LogLevel.Debug, Message = "CAN frame sent successfully")]
    private static partial void LogCanFrameSentSuccessfully(ILogger logger);

    [LoggerMessage(EventId = 2009, Level = LogLevel.Error, Message = "Failed to send CAN frame with ID 0x{CanId:X8}")]
    private static partial void LogFailedToSendCanFrame(ILogger logger, Exception exception, uint canId);

    [LoggerMessage(EventId = 2014, Level = LogLevel.Information, Message = "Background CAN frame reader started")]
    private static partial void LogBackgroundReaderStarted(ILogger logger);

    [LoggerMessage(EventId = 2015, Level = LogLevel.Information, Message = "Background CAN frame reader stopped")]
    private static partial void LogBackgroundReaderStopped(ILogger logger);

    [LoggerMessage(EventId = 2016, Level = LogLevel.Debug, Message = "Subscriber {SubscriberId} registered")]
    private static partial void LogSubscriberRegistered(ILogger logger, Guid subscriberId);

    [LoggerMessage(EventId = 2017, Level = LogLevel.Debug, Message = "Subscriber {SubscriberId} registered with CAN ID filter 0x{CanId:X}")]
    private static partial void LogSubscriberRegisteredWithFilter(ILogger logger, Guid subscriberId, uint canId);

    [LoggerMessage(EventId = 2018, Level = LogLevel.Debug, Message = "Subscriber {SubscriberId} unregistered")]
    private static partial void LogSubscriberUnregistered(ILogger logger, Guid subscriberId);

    [LoggerMessage(EventId = 2019, Level = LogLevel.Error, Message = "Unexpected error in background CAN reader caused reader to stop")]
    private static partial void LogUnexpectedErrorInBackgroundReader(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2020, Level = LogLevel.Warning, Message = "Too many consecutive errors ({ErrorCount}) reading CAN frames, attempting socket reconnection")]
    private static partial void LogTooManyConsecutiveErrors(ILogger logger, int errorCount);

    [LoggerMessage(EventId = 2024, Level = LogLevel.Debug, Message = "Disposing SocketCAN bus")]
    private static partial void LogDisposingSocketCanBus(ILogger logger);

    [LoggerMessage(EventId = 2025, Level = LogLevel.Warning, Message = "Error waiting for background reader shutdown during disposal")]
    private static partial void LogErrorWaitingForReaderShutdown(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2027, Level = LogLevel.Warning, Message = "Error disposing CAN socket during disposal")]
    private static partial void LogErrorDisposingSocket(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2028, Level = LogLevel.Information, Message = "SocketCAN bus disposed successfully")]
    private static partial void LogSocketCanBusDisposed(ILogger logger);

    [LoggerMessage(EventId = 2032, Level = LogLevel.Warning, Message = "Error disposing CancellationTokenSource during disposal")]
    private static partial void LogErrorDisposingCancellationTokenSource(ILogger logger, Exception exception);
}



