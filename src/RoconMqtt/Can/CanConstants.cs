namespace RoconMqtt.Can;

public static class CanConstants
{
    // CAN Frame Header
    public const byte HeaderByte0 = 0xD2;
    public const byte HeaderByte1 = 0x1D;
    public const byte HeaderByte2 = 0xFA;
    
    // Frame Structure
    public const int StandardFrameLength = 7;
    public const int HeaderLength = 3;
    public const int InfoNumberLength = 2;
    public const int ValueLength = 2;
    
    // TimeRange Encoding
    public const int MinutesPerQuarterHour = 15;
    public const byte MaxQuarterHourIndex = 95; // Represents 23:45
    public const byte OffMarker = 0x80; // Special marker for inactive time slots
    public const byte QuarterHourCapValue = 96; // Value that gets capped to 95
}
