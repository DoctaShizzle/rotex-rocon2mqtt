using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;
using SocketCANSharp;
using SocketCANSharp.Network;
using System.Runtime.CompilerServices;
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
        }
        catch (Exception ex)
        {
            LogSocketCanInitializationFailed(_logger, ex, _options.CanInterfaceName);
            throw;
        }
    }

    public async IAsyncEnumerable<CanFrameModel> ReadFramesAsync([EnumeratorCancellation] CancellationToken token, uint? canId = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        LogStartReadingCanFrames(_logger);

        // Apply hardware-level CAN filter if specified
        if (canId.HasValue)
        {
            try
            {
                var filter = new CanFilter(canId.Value, 0x7FF);
                _socket.CanFilters = [filter];
                LogCanFilterApplied(_logger, canId.Value);
            }
            catch (Exception ex)
            {
                LogCanFilterFailed(_logger, ex, canId.Value);
                // Fall back to software filtering if hardware filtering fails
            }
        }
        else
        {
            // Clear any existing filters to receive all frames
            try
            {
                _socket.CanFilters = [];
                LogCanFiltersCleared(_logger);
            }
            catch (Exception ex)
            {
                LogCanFilterClearFailed(_logger, ex);
            }
        }
        
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

            // Additional software filter as fallback (in case hardware filter not supported)
            if (canId.HasValue && frame.CanId != canId.Value)
            {
                await Task.Yield();
                continue;
            }

            LogReceivedCanFrame(_logger, frame.CanId, frame.Data.Length);
            yield return new CanFrameModel(frame.CanId, frame.Data);
            await Task.Yield();
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

        _socket.Dispose();
        GC.SuppressFinalize(this);
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

    [LoggerMessage(EventId = 2010, Level = LogLevel.Debug, Message = "Applied CAN hardware filter for ID 0x{CanId:X}")]
    private static partial void LogCanFilterApplied(ILogger logger, uint canId);

    [LoggerMessage(EventId = 2011, Level = LogLevel.Warning, Message = "Failed to apply CAN hardware filter for ID 0x{CanId:X}, falling back to software filtering")]
    private static partial void LogCanFilterFailed(ILogger logger, Exception exception, uint canId);

    [LoggerMessage(EventId = 2012, Level = LogLevel.Debug, Message = "Cleared CAN hardware filters to receive all frames")]
    private static partial void LogCanFiltersCleared(ILogger logger);

    [LoggerMessage(EventId = 2013, Level = LogLevel.Warning, Message = "Failed to clear CAN hardware filters")]
    private static partial void LogCanFilterClearFailed(ILogger logger, Exception exception);
}
