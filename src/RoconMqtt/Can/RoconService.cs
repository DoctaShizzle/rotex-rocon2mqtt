using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;
using RoconMqtt.Mqtt;

namespace RoconMqtt.Can;

internal partial class RoconService(ICanReader canReader, ICanDecoder canDecoder, ICanEncoder canEncoder, IOptions<CanOptions> canOptions, ILogger<RoconService> logger) : IRoconService
{
    private readonly ICanReader _canReader = canReader ?? throw new ArgumentNullException(nameof(canReader));
    private readonly ICanDecoder _canDecoder = canDecoder ?? throw new ArgumentNullException(nameof(canDecoder));
    private readonly ICanEncoder _canEncoder = canEncoder ?? throw new ArgumentNullException(nameof(canEncoder));
    private readonly IOptions<CanOptions> _canOptions = canOptions ?? throw new ArgumentNullException(nameof(canOptions));
    private readonly ILogger<RoconService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);
        LogSendingRawCanFrame(_logger, canId, data.Length);
        await _canReader.SendFrameAsync(canId, data, token);
    }

    public async Task SendRequestAsync(string parameterName, object value, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(parameterName);
        ArgumentNullException.ThrowIfNull(value);

        LogSendingRequest(_logger, parameterName, value);

        // Find the parameter definition by name
        var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == parameterName);
        if (paramDef == null)
        {
            LogParameterNotFound(_logger, parameterName);
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry", nameof(parameterName));
        }

        // Determine which device/subsystem this parameter belongs to
        var subsystem = CanParameterRegistry.GetSubsystemForParameter(paramDef.InfoNumber);
        var device = subsystem switch
        {
            DeviceType.HeatGenerator => CanParameterRegistry.HeatGenerators[0],
            DeviceType.HeatingCircuit => CanParameterRegistry.HeatingCircuits[0],
            DeviceType.HeatingCircuitModule => CanParameterRegistry.HeatingCircuitModules[0],
            _ => throw new InvalidOperationException($"Unknown subsystem for parameter {parameterName}")
        };

        // Encode the value using SET command
        var frameData = _canEncoder.Encode(device.Profile.Set, paramDef.InfoNumber, value);

        // Send the frame using the configured send CAN ID
        LogEncodedFrameData(_logger, parameterName, _canOptions.Value.SendCanFrameId);
        await _canReader.SendFrameAsync(_canOptions.Value.SendCanFrameId, frameData, token);
        LogRequestSentSuccessfully(_logger, parameterName);
    }

    public async Task ListenForResponses(Func<DecodedParameter, Task> responseAction, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(responseAction);

        LogStartListeningForResponses(_logger, _canOptions.Value.ReceiveFromCanFrameId, _canOptions.Value.ReceiveToCanFrameId);

        try
        {
            await foreach (var frame in _canReader.ReadFramesAsync(token).Where(f => f.Id >= _canOptions.Value.ReceiveFromCanFrameId && f.Id <= _canOptions.Value.ReceiveToCanFrameId))
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

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information, Message = "Sending request for parameter {ParameterName} with value {Value}")]
    private static partial void LogSendingRequest(ILogger logger, string parameterName, object value);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Error, Message = "Parameter '{ParameterName}' not found in registry")]
    private static partial void LogParameterNotFound(ILogger logger, string parameterName);

    [LoggerMessage(EventId = 3004, Level = LogLevel.Debug, Message = "Encoded frame data for parameter {ParameterName}, sending with CAN ID 0x{SendCanFrameId:X8}")]
    private static partial void LogEncodedFrameData(ILogger logger, string parameterName, uint sendCanFrameId);

    [LoggerMessage(EventId = 3005, Level = LogLevel.Information, Message = "Request sent successfully for parameter {ParameterName}")]
    private static partial void LogRequestSentSuccessfully(ILogger logger, string parameterName);

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
}
