namespace RoconMqtt.Can;

/// <summary>
/// Helper class for encoding and decoding CAN frame bytes.
/// Centralizes logic for byte operations, endianness handling, and time conversions.
/// </summary>
public static class CanFrameHelper
{
    /// <summary>
    /// Encodes a 16-bit integer into two bytes with specified endianness.
    /// </summary>
    public static void EncodeInt16(byte[] frame, int frameOffset, int value, bool bigEndian)
    {
        if (bigEndian)
        {
            frame[frameOffset] = (byte)((value >> 8) & 0xFF);      // high byte
            frame[frameOffset + 1] = (byte)(value & 0xFF);         // low byte
        }
        else
        {
            frame[frameOffset] = (byte)(value & 0xFF);             // low byte
            frame[frameOffset + 1] = (byte)((value >> 8) & 0xFF);  // high byte
        }
    }

    /// <summary>
    /// Decodes two bytes into a 16-bit integer with specified endianness.
    /// </summary>
    public static int DecodeInt16(byte[] data, int dataOffset, bool bigEndian)
    {
        var high = data[dataOffset];
        var low = data[dataOffset + 1];

        return bigEndian
            ? (high << 8) | low
            : (low << 8) | high;
    }

    /// <summary>
    /// Converts a time string "HH:MM" to minutes since midnight.
    /// </summary>
    public static int ParseTimeToMinutes(string time)
    {
        var parts = time.Trim().Split(':');
        var hours = int.Parse(parts[0]);
        var minutes = int.Parse(parts[1]);
        return hours * 60 + minutes;
    }

    /// <summary>
    /// Converts minutes since midnight to a time string "HH:MM".
    /// </summary>
    public static string FormatMinutesToTime(int minutes)
    {
        int hours = minutes / 60;
        int mins = minutes % 60;
        return $"{hours:D2}:{mins:D2}";
    }

    /// <summary>
    /// Converts minutes to quarter-hour index (0-95 represents 00:00-23:45).
    /// Each quarter-hour is 15 minutes.
    /// </summary>
    public static byte MinutesToQuarterHour(int minutes)
    {
        return (byte)(minutes / CanConstants.MinutesPerQuarterHour);
    }

    /// <summary>
    /// Converts quarter-hour index to minutes since midnight.
    /// Each quarter-hour is 15 minutes. Value is capped at MaxQuarterHourIndex.
    /// </summary>
    public static int QuarterHourToMinutes(byte quarterHour)
    {
        // Handle special "off" marker
        if (quarterHour == CanConstants.OffMarker)
            return 0;

        // Cap at max quarter-hour index
        if (quarterHour >= CanConstants.QuarterHourCapValue)
            quarterHour = CanConstants.MaxQuarterHourIndex;

        return quarterHour * CanConstants.MinutesPerQuarterHour;
    }
}
