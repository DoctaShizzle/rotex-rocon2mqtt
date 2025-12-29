using Microsoft.Extensions.Logging;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public partial class CanEncoder : ICanEncoder
{
    private readonly ILogger<CanEncoder> _logger;

    public CanEncoder(ILogger<CanEncoder> logger)
    {
        _logger = logger;
    }

    public byte[] Encode(CommunicationCommand command, InfoNumber info, object value)
    {
        // Validate command has header bytes
        if (command.Bytes.Length < 3)
        {
            throw new ArgumentException($"Communication command has invalid header (expected at least 3 bytes, got {command.Bytes.Length})");
        }
        
        var frame = new byte[CanConstants.StandardFrameLength];
        
        // Header from communication command
        frame[0] = command.Bytes[0];
        frame[1] = command.Bytes[1];
        frame[2] = command.Bytes[2];
        
        // InfoNumber
        frame[3] = info.High;
        frame[4] = info.Low;
        
        // Look up parameter definition to determine encoding rules
        var isBigEndian = false;
        var factor = 1.0;
        if (CanParameterRegistry.Parameters.TryGetValue(info, out var def))
        {
            isBigEndian = def.BigEndian;
            factor = def.Factor;
            LogEncodingParameter(_logger, def.Name, info);
        }
        else
        {
            LogParameterDefinitionNotFound(_logger, info);
        }
        
        // Value encoding
        if (value is int intValue)
        {
            LogEncodingInt(_logger, intValue);
            CanFrameHelper.EncodeInt16(frame, 5, intValue, isBigEndian);
        }
        else if (value is double doubleValue)
        {
            var intVal = (int)(doubleValue * factor);
            LogEncodingDouble(_logger, doubleValue, factor, intVal);
            CanFrameHelper.EncodeInt16(frame, 5, intVal, isBigEndian);
        }
        else if (value is bool boolValue)
        {
            LogEncodingBool(_logger, boolValue);
            frame[5] = boolValue ? (byte)1 : (byte)0;
            frame[6] = 0;
        }
        else if (value is string timeRangeValue && timeRangeValue.Contains('-'))
        {
            // TimeRange format: "HH:MM-HH:MM"
            // Encoded as quarter-hour indices (0-95 represents 00:00-23:45)
            var parts = timeRangeValue.Split('-');
            var startMinutes = CanFrameHelper.ParseTimeToMinutes(parts[0]);
            var endMinutes = CanFrameHelper.ParseTimeToMinutes(parts[1]);
            
            // Convert minutes to quarter-hour indices
            byte startQh = CanFrameHelper.MinutesToQuarterHour(startMinutes);
            byte endQh = CanFrameHelper.MinutesToQuarterHour(endMinutes);
            
            LogEncodingTimeRange(_logger, timeRangeValue, startQh, endQh);
            
            frame[5] = startQh;
            frame[6] = endQh;
        }
        else
        {
            LogUnsupportedValueType(_logger, value?.GetType().Name ?? "null");
        }
        
        return frame;
    }

    [LoggerMessage(EventId = 1001, Level = LogLevel.Debug, Message = "Encoding parameter {ParameterName} with InfoNumber {InfoNumber}")]
    private static partial void LogEncodingParameter(ILogger logger, string parameterName, InfoNumber infoNumber);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning, Message = "Parameter definition not found for InfoNumber {InfoNumber}")]
    private static partial void LogParameterDefinitionNotFound(ILogger logger, InfoNumber infoNumber);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Debug, Message = "Encoding int value: {Value}")]
    private static partial void LogEncodingInt(ILogger logger, int value);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Debug, Message = "Encoding double value: {Value} (factor: {Factor}, encoded as: {EncodedValue})")]
    private static partial void LogEncodingDouble(ILogger logger, double value, double factor, int encodedValue);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Debug, Message = "Encoding bool value: {Value}")]
    private static partial void LogEncodingBool(ILogger logger, bool value);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Debug, Message = "Encoding time range: {TimeRange} (start: {StartQh}, end: {EndQh})")]
    private static partial void LogEncodingTimeRange(ILogger logger, string timeRange, byte startQh, byte endQh);

    [LoggerMessage(EventId = 1007, Level = LogLevel.Warning, Message = "Unsupported value type for encoding: {ValueType}")]
    private static partial void LogUnsupportedValueType(ILogger logger, string valueType);
}
