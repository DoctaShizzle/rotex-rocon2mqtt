namespace RoconMqtt.Can;

public class CanEncoder : ICanEncoder
{
    public byte[] Encode(InfoNumber info, object value)
    {
        var frame = new byte[CanConstants.StandardFrameLength];
        
        // Header
        frame[0] = CanConstants.HeaderByte0;
        frame[1] = CanConstants.HeaderByte1;
        frame[2] = CanConstants.HeaderByte2;
        
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
        }
        
        // Value encoding
        if (value is int intValue)
        {
            CanFrameHelper.EncodeInt16(frame, 5, intValue, isBigEndian);
        }
        else if (value is double doubleValue)
        {
            var intVal = (int)(doubleValue * factor);
            CanFrameHelper.EncodeInt16(frame, 5, intVal, isBigEndian);
        }
        else if (value is bool boolValue)
        {
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
            
            frame[5] = startQh;
            frame[6] = endQh;
        }
        
        return frame;
    }
}
