using Microsoft.Extensions.Logging;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public partial class CanDecoder(ILogger<CanDecoder> logger) : ICanDecoder
{
    public DecodedParameter? Decode(byte[] data, CommunicationCommand command, DecodingHints? hints = null)
    {
        // Must be at least header + infoNumber + 2 bytes value
        if (data.Length < CanConstants.StandardFrameLength)
        {
            LogInvalidCanFrameTooShort(logger, data.Length);
            return null;
        }

        // Validate command has expected header bytes
        if (command.Bytes.Length < 3)
        {
            LogCommunicationCommandInvalidHeader(logger, command.Bytes.Length);
            return null;
        }
        
        // Header check
        if (data[0] != command.Bytes[0] || data[1] != command.Bytes[1] || data[2] != command.Bytes[2])
        {
            LogInvalidCanFrameBadHeader(logger, data[0], data[1], data[2], command.Bytes[0], command.Bytes[1], command.Bytes[2]);
            return null;
        }

        var info = new InfoNumber(data[3], data[4]);

        ParameterDefinition? def = null;
        if(hints != null)
        {
            // Create a temporary definition from hints
            def = new ParameterDefinition(
                OriginalName: hints.ParameterName,
                InfoNumber: info,
                Type: hints.ParameterType,
                Factor: hints.Factor,
                BigEndian: hints.BigEndian
            );

            LogDecodingUnknownParameterWithHints(logger, hints.ParameterName);
        } else
        {
            // Try to get parameter definition from registry
            if (!CanParameterRegistry.Parameters.TryGetValue(info, out var def2))
            {
                LogUnknownParameterInfoNumber(logger, info);
                return null;
            }
            else
            {
                def = def2;
            }
        }

        try
        {
            object value = def!.Type switch
            {
                ParameterType.Int => DecodeInt(data, def),
                ParameterType.Float => DecodeFloat(data, def),
                ParameterType.Bool => DecodeBool(data),
                ParameterType.TimeRange => DecodeTimeRange(data),
                ParameterType.Enum => DecodeInt(data, def),
                _ => throw new NotSupportedException($"Type {def.Type} not supported")
            };

            LogDecodedParameter(logger, def.Name, value);
            return new DecodedParameter(def.Name, value, def);
        }
        catch (Exception ex)
        {
            LogErrorDecodingParameter(logger, ex, def!.Name);
            return null;
        }
    }

    private static int DecodeInt(byte[] data, ParameterDefinition def)
    {
        return CanFrameHelper.DecodeInt16(data, 5, def.BigEndian);
    }

    private static double DecodeFloat(byte[] data, ParameterDefinition def)
    {
        int raw = DecodeInt(data, def);
        return raw / def.Factor;
    }

    private static bool DecodeBool(byte[] data)
        => data[5] != 0;

    private static string DecodeTimeRange(byte[] data)
    {
        byte startQh = data[5];
        byte endQh = data[6];

        int startMinutes = CanFrameHelper.QuarterHourToMinutes(startQh);
        int endMinutes = CanFrameHelper.QuarterHourToMinutes(endQh);

        return $"{CanFrameHelper.FormatMinutesToTime(startMinutes)}-{CanFrameHelper.FormatMinutesToTime(endMinutes)}";
    }

    [LoggerMessage(LogLevel.Debug, "Invalid CAN frame: too short (length={Length})")]
    private static partial void LogInvalidCanFrameTooShort(ILogger logger, int length);

    [LoggerMessage(LogLevel.Error, "Communication command has invalid header (expected at least 3 bytes, got {Length})")]
    private static partial void LogCommunicationCommandInvalidHeader(ILogger logger, int length);

    [LoggerMessage(LogLevel.Debug, "Invalid CAN frame: bad header ({High:X2} {Mid:X2} {Low:X2}), expected ({ExpHigh:X2} {ExpMid:X2} {ExpLow:X2})")]
    private static partial void LogInvalidCanFrameBadHeader(ILogger logger, byte high, byte mid, byte low, byte expHigh, byte expMid, byte expLow);

    [LoggerMessage(LogLevel.Debug, "Unknown parameter InfoNumber: {InfoNumber}")]
    private static partial void LogUnknownParameterInfoNumber(ILogger logger, InfoNumber infoNumber);

    [LoggerMessage(LogLevel.Debug, "Decoding unknown parameter with hints: {ParameterName}")]
    private static partial void LogDecodingUnknownParameterWithHints(ILogger logger, string parameterName);

    [LoggerMessage(LogLevel.Debug, "Decoded {ParameterName}: {Value}")]
    private static partial void LogDecodedParameter(ILogger logger, string parameterName, object value);

    [LoggerMessage(LogLevel.Error, "Error decoding parameter {ParameterName}")]
    private static partial void LogErrorDecodingParameter(ILogger logger, Exception ex, string parameterName);
}
