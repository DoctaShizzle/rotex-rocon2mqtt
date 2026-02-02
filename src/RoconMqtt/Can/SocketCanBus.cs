using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;
using SocketCANSharp;
using SocketCANSharp.Network;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using CanFrameModel = RoconMqtt.Can.Models.CanFrame;

namespace RoconMqtt.Can;

/// <summary>
/// SocketCAN implementation of the CAN bus interface.
/// </summary>
public partial class SocketCanBus : ICanBus, IDisposable
{
    private RawCanSocket _socket;
    private readonly object _socketLock = new();
    private readonly ILogger<SocketCanBus> _logger;
    private readonly CanOptions _options;
    private bool _disposed;

    // Broadcast infrastructure
    private readonly ConcurrentDictionary<Guid, FrameSubscriber> _subscribers = new();
    private readonly CancellationTokenSource _readerCts = new();
    private Task? _readerTask;
    
    // Reconnection state
    private int _consecutiveErrors = 0;
    private const int MaxConsecutiveErrors = 10;
    private const int MaxInitRetries = 5;
    private const int InitRetryDelayMs = 1000;

    public SocketCanBus(ILogger<SocketCanBus> logger, IOptions<CanOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _socket = new RawCanSocket();

        Init();
    }

    private void Init()
    {
        Exception? lastException = null;
        
        for (int attempt = 1; attempt <= MaxInitRetries; attempt++)
        {
            try
            {
                LogInitializationAttempt(_logger, attempt, MaxInitRetries, _options.CanInterfaceName);
                
                // Dynamically look up the interface index by name
                // This is necessary because the interface index varies based on system configuration
                using (var tempSocket = new RawCanSocket())
                {
                    var iface = CanNetworkInterface.GetInterfaceByName(tempSocket.SafeHandle, _options.CanInterfaceName);
                    lock (_socketLock)
                    {
                        // Ensure socket is in a clean state before binding
                        // This helps recover from previous unclean shutdowns
                        try
                        {
                            _socket?.Close();
                        }
                        catch
                        {
                            // Ignore errors during close - socket might already be closed
                        }
                        
                        // Recreate socket to ensure clean state
                        _socket?.Dispose();
                        _socket = new RawCanSocket();
                        
                        _socket.Bind(iface);
                    }
                }
                
                LogSocketCanInitialized(_logger, _options.CanInterfaceName);

                // Start the background reader task
                _readerTask = Task.Run(BackgroundReaderAsync, _readerCts.Token);
                
                // Success - exit retry loop
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                LogInitializationAttemptFailed(_logger, ex, attempt, MaxInitRetries, _options.CanInterfaceName);
                
                if (attempt < MaxInitRetries)
                {
                    // Calculate exponential backoff delay
                    var delayMs = InitRetryDelayMs * (int)Math.Pow(2, attempt - 1);
                    LogRetryingInitialization(_logger, delayMs, _options.CanInterfaceName);
                    Thread.Sleep(delayMs);
                }
            }
        }
        
        // All retries failed
        LogSocketCanInitializationFailed(_logger, lastException!, _options.CanInterfaceName);
        throw new InvalidOperationException($"Failed to initialize SocketCAN bus on interface {_options.CanInterfaceName} after {MaxInitRetries} attempts", lastException);
    }
    
    /// <summary>
    /// Attempts to reconnect the CAN socket after a fatal error.
    /// </summary>
    private async Task<bool> TryReconnectSocketAsync(CancellationToken token)
    {
        LogAttemptingSocketReconnection(_logger, _options.CanInterfaceName);
        
        try
        {
            // Dispose old socket
            lock (_socketLock)
            {
                _socket?.Dispose();
                _socket = new RawCanSocket();
            }
            
            // Delay before reconnection attempt (exponential backoff)
            var delayMs = Math.Min(1000 * Math.Pow(2, Math.Min(_consecutiveErrors, 5)), 30000);
            await Task.Delay((int)delayMs, token);
            
            // Try to rebind
            using (var tempSocket = new RawCanSocket())
            {
                var iface = CanNetworkInterface.GetInterfaceByName(tempSocket.SafeHandle, _options.CanInterfaceName);
                lock (_socketLock)
                {
                    _socket.Bind(iface);
                }
            }
            
            LogSocketReconnected(_logger, _options.CanInterfaceName);
            _consecutiveErrors = 0; // Reset error counter on success
            return true;
        }
        catch (Exception ex)
        {
            LogSocketReconnectionFailed(_logger, ex, _options.CanInterfaceName);
            return false;
        }
    }

    /// <summary>
    /// Background task that continuously reads from the CAN socket and broadcasts frames to all subscribers
    /// </summary>
    private async Task BackgroundReaderAsync()
    {
        LogBackgroundReaderStarted(_logger);

        try
        {
            while (!_readerCts.Token.IsCancellationRequested)
            {
                SocketCANSharp.CanFrame frame;
                try
                {
                    // Read frame from socket (blocking call)
                    // Note: We don't use timeout here because abandoned tasks can cause lock contention
                    frame = await Task.Run(() =>
                    {
                        SocketCANSharp.CanFrame f;
                        lock (_socketLock)
                        {
                            _socket.Read(out f);
                        }
                        return f;
                    }, _readerCts.Token);
                    
                    _consecutiveErrors = 0; // Reset on successful read
                }
                catch (OperationCanceledException)
                {
                    LogCanFrameReadingCancelled(_logger);
                    break;
                }
                catch (Exception ex) when (_disposed || _readerCts.Token.IsCancellationRequested)
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
                        
                        var reconnected = await TryReconnectSocketAsync(_readerCts.Token);
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
                    continue;
                }

                // Format candump command for logging
                var dataHex = BitConverter.ToString(frame.Data).Replace("-", " ");
                var candumpOutput = $"{_options.CanInterfaceName}  {frame.CanId:X3}   [{frame.Data.Length}]  {dataHex}";
                
                LogReceivedCanFrame(_logger, frame.CanId, frame.Data.Length, candumpOutput);

                // Broadcast to all subscribers
                var canFrameModel = new CanFrameModel(frame.CanId, frame.Data);
                foreach (var subscriber in _subscribers.Values)
                {
                    // Apply per-subscriber filter
                    if (subscriber.CanIdFilter.HasValue && subscriber.CanIdFilter.Value != frame.CanId)
                    {
                        continue;
                    }

                    // Try to write to subscriber channel (non-blocking)
                    subscriber.Channel.Writer.TryWrite(canFrameModel);
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
            await foreach (var frame in subscriber.Channel.Reader.ReadAllAsync(token))
            {
                yield return frame;
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

    public Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        try
        {
            var frame = new SocketCANSharp.CanFrame(canId, data);
            
            // Format cansend command for logging
            var dataHex = Convert.ToHexString(data);
            var cansendCommand = $"cansend {_options.CanInterfaceName} {canId:X3}#{dataHex}";
            
            LogSendingCanFrame(_logger, canId, data.Length, cansendCommand);
            lock (_socketLock)
            {
                _socket.Write(frame);
            }
            LogCanFrameSentSuccessfully(_logger);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            LogFailedToSendCanFrame(_logger, ex, canId);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

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

        // Ensure socket is properly closed and disposed
        lock (_socketLock)
        {
            try
            {
                _socket?.Close();
            }
            catch (Exception ex)
            {
                LogErrorClosingSocket(_logger, ex);
            }

            try
            {
                _socket?.Dispose();
            }
            catch (Exception ex)
            {
                LogErrorDisposingSocket(_logger, ex);
            }
        }

        // Dispose CancellationTokenSource LAST - after background task is guaranteed to be stopped
        // This prevents ObjectDisposedException when the task is still accessing the token
        try
        {
            _readerCts.Dispose();
        }
        catch (Exception ex)
        {
            LogErrorDisposingCancellationTokenSource(_logger, ex);
        }

        LogSocketCanBusDisposed(_logger);
        GC.SuppressFinalize(this);
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
    private static partial void LogSocketCanInitialized(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Error, Message = "Failed to initialize SocketCAN bus on interface {InterfaceName}")]
    private static partial void LogSocketCanInitializationFailed(ILogger logger, Exception exception, string interfaceName);

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

    [LoggerMessage(EventId = 2021, Level = LogLevel.Information, Message = "Attempting to reconnect CAN socket on interface {InterfaceName}")]
    private static partial void LogAttemptingSocketReconnection(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2022, Level = LogLevel.Information, Message = "Successfully reconnected CAN socket on interface {InterfaceName}")]
    private static partial void LogSocketReconnected(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2023, Level = LogLevel.Error, Message = "Failed to reconnect CAN socket on interface {InterfaceName}")]
    private static partial void LogSocketReconnectionFailed(ILogger logger, Exception exception, string interfaceName);

    [LoggerMessage(EventId = 2024, Level = LogLevel.Debug, Message = "Disposing SocketCAN bus")]
    private static partial void LogDisposingSocketCanBus(ILogger logger);

    [LoggerMessage(EventId = 2025, Level = LogLevel.Warning, Message = "Error waiting for background reader shutdown during disposal")]
    private static partial void LogErrorWaitingForReaderShutdown(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2026, Level = LogLevel.Warning, Message = "Error closing CAN socket during disposal")]
    private static partial void LogErrorClosingSocket(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2027, Level = LogLevel.Warning, Message = "Error disposing CAN socket during disposal")]
    private static partial void LogErrorDisposingSocket(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2028, Level = LogLevel.Information, Message = "SocketCAN bus disposed successfully")]
    private static partial void LogSocketCanBusDisposed(ILogger logger);

    [LoggerMessage(EventId = 2029, Level = LogLevel.Information, Message = "Attempting to initialize CAN interface {InterfaceName} (attempt {Attempt}/{MaxAttempts})")]
    private static partial void LogInitializationAttempt(ILogger logger, int attempt, int maxAttempts, string interfaceName);

    [LoggerMessage(EventId = 2030, Level = LogLevel.Warning, Message = "Failed to initialize CAN interface {InterfaceName} on attempt {Attempt}/{MaxAttempts}")]
    private static partial void LogInitializationAttemptFailed(ILogger logger, Exception exception, int attempt, int maxAttempts, string interfaceName);

    [LoggerMessage(EventId = 2031, Level = LogLevel.Information, Message = "Retrying CAN interface {InterfaceName} initialization in {DelayMs}ms")]
    private static partial void LogRetryingInitialization(ILogger logger, int delayMs, string interfaceName);

    [LoggerMessage(EventId = 2032, Level = LogLevel.Warning, Message = "Error disposing CancellationTokenSource during disposal")]
    private static partial void LogErrorDisposingCancellationTokenSource(ILogger logger, Exception exception);
}



