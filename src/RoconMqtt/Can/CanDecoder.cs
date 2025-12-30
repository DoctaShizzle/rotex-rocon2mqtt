using Microsoft.Extensions.Logging;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public class CanDecoder(ILogger<CanDecoder> logger) : ICanDecoder
{
    public DecodedParameter? Decode(byte[] data, CommunicationCommand command)
    {
        // Must be at least header + infoNumber + 2 bytes value
        if (data.Length < CanConstants.StandardFrameLength)
        {
            logger.LogDebug("Invalid CAN frame: too short (length={Length})", data.Length);
            return null;
        }

        // Validate command has expected header bytes
        if (command.Bytes.Length < 3)
        {
            logger.LogError("Communication command has invalid header (expected at least 3 bytes, got {Length})", 
                command.Bytes.Length);
            return null;
        }
        
        // Header check
        if (data[0] != command.Bytes[0] || data[1] != command.Bytes[1] || data[2] != command.Bytes[2])
        {
            logger.LogDebug("Invalid CAN frame: bad header ({High:X2} {Mid:X2} {Low:X2}), expected ({ExpHigh:X2} {ExpMid:X2} {ExpLow:X2})", 
                data[0], data[1], data[2], command.Bytes[0], command.Bytes[1], command.Bytes[2]);
            return null;
        }

        var info = new InfoNumber(data[3], data[4]);

        if (!CanParameterRegistry.Parameters.TryGetValue(info, out var def))
        {
            logger.LogDebug("Unknown parameter InfoNumber: {InfoNumber}", info);
            return null;
        }

        try
        {
            object value = def.Type switch
            {
                ParameterType.Int => DecodeInt(data, def),
                ParameterType.Float => DecodeFloat(data, def),
                ParameterType.Bool => DecodeBool(data),
                ParameterType.TimeRange => DecodeTimeRange(data),
                ParameterType.Enum => DecodeInt(data, def),
                _ => throw new NotSupportedException($"Type {def.Type} not supported")
            };

            logger.LogDebug("Decoded {ParameterName}: {Value}", def.Name, value);
            return new DecodedParameter(def.Name, value, def);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error decoding parameter {ParameterName}", def.Name);
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
}
