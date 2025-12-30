using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;

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

    /// <inheritdoc/>
    public async Task SendRequestAsync(string deviceName, string parameterName, CommandType commandType, object? value, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(deviceName);
        ArgumentNullException.ThrowIfNull(parameterName);

        LogSendingRequest(_logger, deviceName, parameterName, commandType);

        // Find the parameter definition by name
        var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == parameterName || p.OriginalName == parameterName);
        if (paramDef == null)
        {
            LogParameterNotFound(_logger, parameterName);
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry", nameof(parameterName));
        }

        await SendRequestByInfoNumberAsync(deviceName, paramDef.InfoNumber, commandType, value, token);
        LogRequestSentSuccessfully(_logger, deviceName, parameterName, commandType);
    }

    /// <inheritdoc/>
    public async Task ListenForResponses(CommandType commandType, string deviceName, string parameterName, Func<DecodedParameter, Task> responseAction, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(responseAction);

        // Find the parameter definition by name
        var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == parameterName || p.OriginalName == parameterName);
        if (paramDef == null)
        {
            LogParameterNotFound(_logger, parameterName);
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry", nameof(parameterName));
        }

        await ListenForResponsesByInfoNumberAsync(commandType, deviceName, paramDef.InfoNumber, responseAction, token);
    }

    /// <inheritdoc/>
    public async Task<DecodedParameter> SendRequestAndWaitForResponseAsync(string deviceName, string parameterName, CommandType commandType, object? value, int timeoutMs, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(deviceName);
        ArgumentNullException.ThrowIfNull(parameterName);

        if (commandType == CommandType.Answer)
        {
            throw new ArgumentException("Cannot send ANSWER commands - they are responses from the controller", nameof(commandType));
        }

        // Find the parameter definition by name
        var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == parameterName || p.OriginalName == parameterName);
        if (paramDef == null)
        {
            LogParameterNotFound(_logger, parameterName);
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry", nameof(parameterName));
        }

        return await SendRawRequestAndWaitForResponseAsync(deviceName, paramDef.InfoNumber, commandType, value, timeoutMs, token);
    }

    /// <inheritdoc/>
    public async Task<DecodedParameter> SendRawRequestAndWaitForResponseAsync(string deviceName, InfoNumber infoNumber, CommandType commandType, object? value, int timeoutMs, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(deviceName);

        if (commandType == CommandType.Answer)
        {
            throw new ArgumentException("Cannot send ANSWER commands - they are responses from the controller", nameof(commandType));
        }

        var tcs = new TaskCompletionSource<DecodedParameter>();

        using var listenCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        var listenTask = Task.Run(async () =>
        {
            try
            {
                await ListenForResponsesByInfoNumberAsync(CommandType.Answer, deviceName, infoNumber, async decodedParameter =>
                {
                    tcs.TrySetResult(decodedParameter);
                    await listenCts.CancelAsync();
                    await Task.CompletedTask;
                }, listenCts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }, listenCts.Token);

        try
        {
            await SendRequestByInfoNumberAsync(deviceName, infoNumber, commandType, value, token);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            timeoutCts.CancelAfter(timeoutMs);

            var result = await tcs.Task.WaitAsync(timeoutCts.Token);
            return result;
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            LogRequestTimeoutByInfoNumber(_logger, deviceName, infoNumber.High, infoNumber.Low, commandType, timeoutMs);
            throw new TimeoutException($"Timeout waiting for response from {deviceName} for InfoNumber 0x{infoNumber.High:X2}{infoNumber.Low:X2} after {timeoutMs}ms");
        }
        finally
        {
            await listenCts.CancelAsync();
            try
            {
                await listenTask;
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <summary>
    /// Centralized method to send a request by InfoNumber
    /// </summary>
    private async Task SendRequestByInfoNumberAsync(string deviceName, InfoNumber infoNumber, CommandType commandType, object? value, CancellationToken token)
    {
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
        var frameData = _canEncoder.Encode(command, infoNumber, encodedValue);

        // Send the frame using the command's CAN ID
        LogEncodedFrameDataByInfoNumber(_logger, deviceName, infoNumber.High, infoNumber.Low, commandType, command.CanId);
        await _canBus.SendFrameAsync(command.CanId, frameData, token);
    }

    /// <summary>
    /// Centralized method to listen for responses by InfoNumber
    /// </summary>
    private async Task ListenForResponsesByInfoNumberAsync(CommandType commandType, string deviceName, InfoNumber infoNumber, Func<DecodedParameter, Task> responseAction, CancellationToken token)
    {
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
            CommandType.Answer => device.Profile.Answer,
            _ => throw new ArgumentException($"Unknown command type: {commandType}", nameof(commandType))
        };

        LogStartListeningForResponses(_logger, command.CanId, command.CanId);

        try
        {
            await foreach (var frame in _canBus.ReadFramesAsync(token, command.CanId))
            {
                LogProcessingFrame(_logger, frame.Id);
                
                // Decode using the specified command
                var decoded = _canDecoder.Decode(frame.Data, command);
                
                if (decoded != null)
                {
                    // Filter by InfoNumber
                    if (decoded.Definition.InfoNumber.High == infoNumber.High && 
                        decoded.Definition.InfoNumber.Low == infoNumber.Low)
                    {
                        LogSuccessfullyDecodedParameter(_logger, decoded.Name);
                        await responseAction(decoded);
                    }
                    else
                    {
                        LogInfoNumberMismatch(_logger, decoded.Definition.InfoNumber.High, decoded.Definition.InfoNumber.Low, infoNumber.High, infoNumber.Low);
                    }
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

    [LoggerMessage(EventId = 3004, Level = LogLevel.Debug, Message = "Encoded {CommandType} frame for device {DeviceName}, InfoNumber 0x{InfoHigh:X2}{InfoLow:X2}, sending with CAN ID 0x{CanId:X8}")]
    private static partial void LogEncodedFrameDataByInfoNumber(ILogger logger, string deviceName, byte infoHigh, byte infoLow, CommandType commandType, uint canId);

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

    [LoggerMessage(EventId = 3015, Level = LogLevel.Debug, Message = "InfoNumber mismatch: received 0x{ReceivedHigh:X2}{ReceivedLow:X2} but expected 0x{ExpectedHigh:X2}{ExpectedLow:X2}")]
    private static partial void LogInfoNumberMismatch(ILogger logger, byte receivedHigh, byte receivedLow, byte expectedHigh, byte expectedLow);

    [LoggerMessage(EventId = 3016, Level = LogLevel.Warning, Message = "Timeout waiting for {CommandType} response from device {DeviceName} for InfoNumber 0x{InfoHigh:X2}{InfoLow:X2} after {TimeoutMs}ms")]
    private static partial void LogRequestTimeoutByInfoNumber(ILogger logger, string deviceName, byte infoHigh, byte infoLow, CommandType commandType, int timeoutMs);
}
