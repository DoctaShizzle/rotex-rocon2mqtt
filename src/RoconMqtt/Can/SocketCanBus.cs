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
    private readonly RawCanSocket _socket = new();
    private readonly ILogger<SocketCanBus> _logger;
    private readonly CanOptions _options;
    private bool _disposed;

    // Broadcast infrastructure
    private readonly ConcurrentDictionary<Guid, FrameSubscriber> _subscribers = new();
    private readonly CancellationTokenSource _readerCts = new();
    private Task? _readerTask;

    public SocketCanBus(ILogger<SocketCanBus> logger, IOptions<CanOptions> options)
    {
        _logger = logger;
        _options = options.Value;

        Init();
    }

    private void Init()
    {
        try
        {
            // Dynamically look up the interface index by name
            // This is necessary because the interface index varies based on system configuration
            using (var tempSocket = new RawCanSocket())
            {
                var iface = CanNetworkInterface.GetInterfaceByName(tempSocket.SafeHandle, _options.CanInterfaceName);
                _socket.Bind(iface);
            }
            LogSocketCanInitialized(_logger, _options.CanInterfaceName);

            // Start the background reader task
            _readerTask = Task.Run(BackgroundReaderAsync, _readerCts.Token);
        }
        catch (Exception ex)
        {
            LogSocketCanInitializationFailed(_logger, ex, _options.CanInterfaceName);
            throw;
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
                    // Note: This is a synchronous blocking call. We run it in a Task to allow cancellation checking.
                    var readTask = Task.Run(() =>
                    {
                        SocketCANSharp.CanFrame f;
                        _socket.Read(out f);
                        return f;
                    }, _readerCts.Token);

                    // Wait for read with periodic cancellation checks (timeout allows graceful cancellation)
                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_readerCts.Token);
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(1));
                    
                    try
                    {
                        frame = await readTask.WaitAsync(timeoutCts.Token);
                    }
                    catch (TimeoutException)
                    {
                        // Timeout is expected when no frames arrive - just loop and check cancellation
                        continue;
                    }
                }
                catch (OperationCanceledException)
                {
                    LogCanFrameReadingCancelled(_logger);
                    break;
                }
                catch (Exception ex) when (_disposed || _readerCts.Token.IsCancellationRequested)
                {
                    // Expected during shutdown
                    break;
                }
                catch (Exception ex)
                {
                    LogErrorReadingCanFrame(_logger, ex);
                    // Brief delay before retry to avoid tight error loop
                    try
                    {
                        await Task.Delay(100, _readerCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    continue;
                }

                LogReceivedCanFrame(_logger, frame.CanId, frame.Data.Length);

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

                await Task.Yield();
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
            LogSendingCanFrame(_logger, canId, data.Length);
            _socket.Write(frame);
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

        // Stop the background reader
        _readerCts.Cancel();
        if (_readerTask != null)
        {
            try
            {
                _readerTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch
            {
                // Ignore exceptions during shutdown
            }
        }

        // Close all subscriber channels
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.Channel.Writer.Complete();
        }
        _subscribers.Clear();

        _readerCts.Dispose();
        _socket.Dispose();
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

    [LoggerMessage(EventId = 2006, Level = LogLevel.Debug, Message = "Received CAN frame: ID=0x{CanId:X8}, DataLength={DataLength}")]
    private static partial void LogReceivedCanFrame(ILogger logger, uint canId, int dataLength);

    [LoggerMessage(EventId = 2007, Level = LogLevel.Debug, Message = "Sending CAN frame: ID=0x{CanId:X8}, DataLength={DataLength}")]
    private static partial void LogSendingCanFrame(ILogger logger, uint canId, int dataLength);

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
}