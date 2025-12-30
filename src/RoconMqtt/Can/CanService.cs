using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;
using RoconMqtt.Mqtt;

namespace RoconMqtt.Can;

internal partial class CanService(ICanBus canBus, ICanDecoder canDecoder, ICanEncoder canEncoder, IOptions<CanOptions> canOptions, ILogger<CanService> logger) : ICanService
{
    private readonly ICanBus _canBus = canBus ?? throw new ArgumentNullException(nameof(canBus));
    private readonly ICanDecoder _canDecoder = canDecoder ?? throw new ArgumentNullException(nameof(canDecoder));
    private readonly ICanEncoder _canEncoder = canEncoder ?? throw new ArgumentNullException(nameof(canEncoder));
    private readonly IOptions<CanOptions> _canOptions = canOptions ?? throw new ArgumentNullException(nameof(canOptions));
    private readonly ILogger<CanService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);
        LogSendingRawCanFrame(_logger, canId, data.Length);
        await _canBus.SendFrameAsync(canId, data, token);
    }

    public async Task SendRequestAsync(string deviceName, string parameterName, CommandType commandType, object? value, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(deviceName);
        ArgumentNullException.ThrowIfNull(parameterName);

        LogSendingRequest(_logger, deviceName, parameterName, commandType);

        // Find the parameter definition by name
        var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == parameterName);
        if (paramDef == null)
        {
            LogParameterNotFound(_logger, parameterName);
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry", nameof(parameterName));
        }

        // Get the specific device by name
        var device = CanParameterRegistry.GetDevice(deviceName);
        if (device == null)
        {
            LogDeviceNotFound(_logger, deviceName);
            throw new ArgumentException($"Device '{deviceName}' not found in registry", nameof(deviceName));
        }

        // Select the appropriate command based on CommandType
        var command = commandType switch
        {
            CommandType.Get => device.Profile.Get,
            CommandType.Set => device.Profile.Set,
            CommandType.Answer => throw new ArgumentException("Cannot send ANSWER commands - they are responses from the controller", nameof(commandType)),
            _ => throw new ArgumentException($"Unknown command type: {commandType}", nameof(commandType))
        };

        // For GET requests, value is ignored (use 0). For SET, use the provided value
        var encodedValue = commandType == CommandType.Get ? 0 : (value ?? 0);
        var frameData = _canEncoder.Encode(command, paramDef.InfoNumber, encodedValue);

        // Send the frame using the command's CAN ID
        LogEncodedFrameData(_logger, deviceName, parameterName, commandType, command.CanId);
        await _canBus.SendFrameAsync(command.CanId, frameData, token);
        LogRequestSentSuccessfully(_logger, deviceName, parameterName, commandType);
    }

    public async Task ListenForResponses(Func<DecodedParameter, Task> responseAction, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(responseAction);

        LogStartListeningForResponses(_logger, _canOptions.Value.ReceiveFromCanFrameId, _canOptions.Value.ReceiveToCanFrameId);

        try
        {
            await foreach (var frame in _canBus.ReadFramesAsync(token).Where(f => f.Id >= _canOptions.Value.ReceiveFromCanFrameId && f.Id <= _canOptions.Value.ReceiveToCanFrameId))
            {
                LogProcessingFrame(_logger, frame.Id);
                
                // Try to decode with each device's ANSWER command until one succeeds
                // Since all ANSWER frames share the same header (0xD2 0x1D 0xFA), we can use any device's Answer command
                var decoded = _canDecoder.Decode(frame.Data, CanParameterRegistry.HeatGenerators[0].Profile.Answer);
                
                if (decoded != null)
                {
                    LogSuccessfullyDecodedParameter(_logger, decoded.Name);
                    await responseAction(decoded);
                }
                else
                {
                    LogFailedToDecodeFrame(_logger, frame.Id);
                }
            }
        }
        catch (OperationCanceledException)
        {
            LogResponseListeningCancelled(_logger);
        }
    }

    [LoggerMessage(EventId = 3001, Level = LogLevel.Debug, Message = "Sending raw CAN frame with ID 0x{CanId:X8}, DataLength={DataLength}")]
    private static partial void LogSendingRawCanFrame(ILogger logger, uint canId, int dataLength);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information, Message = "Sending {CommandType} request for device {DeviceName}, parameter {ParameterName}")]
    private static partial void LogSendingRequest(ILogger logger, string deviceName, string parameterName, CommandType commandType);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Error, Message = "Parameter '{ParameterName}' not found in registry")]
    private static partial void LogParameterNotFound(ILogger logger, string parameterName);

    [LoggerMessage(EventId = 3004, Level = LogLevel.Debug, Message = "Encoded {CommandType} frame for device {DeviceName}, parameter {ParameterName}, sending with CAN ID 0x{CanId:X8}")]
    private static partial void LogEncodedFrameData(ILogger logger, string deviceName, string parameterName, CommandType commandType, uint canId);

    [LoggerMessage(EventId = 3005, Level = LogLevel.Information, Message = "{CommandType} request sent successfully for device {DeviceName}, parameter {ParameterName}")]
    private static partial void LogRequestSentSuccessfully(ILogger logger, string deviceName, string parameterName, CommandType commandType);

    [LoggerMessage(EventId = 3006, Level = LogLevel.Information, Message = "Starting to listen for responses (CAN ID range: 0x{FromId:X8} - 0x{ToId:X8})")]
    private static partial void LogStartListeningForResponses(ILogger logger, uint fromId, uint toId);

    [LoggerMessage(EventId = 3007, Level = LogLevel.Debug, Message = "Processing frame with ID 0x{FrameId:X8}")]
    private static partial void LogProcessingFrame(ILogger logger, uint frameId);

    [LoggerMessage(EventId = 3008, Level = LogLevel.Debug, Message = "Successfully decoded parameter: {ParameterName}")]
    private static partial void LogSuccessfullyDecodedParameter(ILogger logger, string parameterName);

    [LoggerMessage(EventId = 3009, Level = LogLevel.Debug, Message = "Failed to decode frame with ID 0x{FrameId:X8}")]
    private static partial void LogFailedToDecodeFrame(ILogger logger, uint frameId);

    [LoggerMessage(EventId = 3010, Level = LogLevel.Information, Message = "Response listening cancelled")]
    private static partial void LogResponseListeningCancelled(ILogger logger);

    [LoggerMessage(EventId = 3012, Level = LogLevel.Error, Message = "Device '{DeviceName}' not found in registry")]
    private static partial void LogDeviceNotFound(ILogger logger, string deviceName);
}
