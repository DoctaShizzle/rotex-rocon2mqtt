using Microsoft.Extensions.Logging;
using SocketCANSharp;
using SocketCANSharp.Network;
using System.Runtime.CompilerServices;

namespace RoconMqtt.Can;

public partial class SocketCanReader : ICanReader
{
    private readonly string _interfaceName = "can0";
    private readonly RawCanSocket _socket;
    private readonly ILogger<SocketCanReader> _logger;

    public SocketCanReader(ILogger<SocketCanReader> logger)
    {
        _logger = logger;
        try
        {
            _socket = new RawCanSocket();
            var iface = new CanNetworkInterface(0, _interfaceName, false);
            _socket.Bind(iface);
            LogSocketCanInitialized(_logger, _interfaceName);
        }
        catch (Exception ex)
        {
            LogSocketCanInitializationFailed(_logger, ex, _interfaceName);
            throw;
        }
    }

    public async IAsyncEnumerable<CanFrame> ReadFramesAsync(
        [EnumeratorCancellation] CancellationToken token)
    {
        LogStartReadingCanFrames(_logger);
        
        while (!token.IsCancellationRequested)
        {
            SocketCANSharp.CanFrame frame;
            try
            {
                _socket.Read(out frame);
            }
            catch (OperationCanceledException)
            {
                LogCanFrameReadingCancelled(_logger);
                break;
            }
            catch (Exception ex)
            {
                LogErrorReadingCanFrame(_logger, ex);
                throw;
            }

            LogReceivedCanFrame(_logger, frame.CanId, frame.Data.Length);
            yield return new CanFrame(frame.CanId, frame.Data);
            await Task.Yield();
        }
    }

    public Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
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

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "SocketCAN reader initialized on interface {InterfaceName}")]
    private static partial void LogSocketCanInitialized(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Error, Message = "Failed to initialize SocketCAN reader on interface {InterfaceName}")]
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
}
