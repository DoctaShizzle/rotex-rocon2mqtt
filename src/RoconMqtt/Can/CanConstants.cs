namespace RoconMqtt.Can;

/// <summary>
/// Protocol-level constants for CAN bus communication with Rocon G1 controller.
/// </summary>
public static class CanConstants
{
    /// <summary>
    /// Standard length of a CAN frame payload for parameter communication.
    /// Format: [Header:3 bytes][InfoNumber:2 bytes][Value:2 bytes] = 7 bytes total
    /// </summary>
    public const int StandardFrameLength = 7;
    
    /// <summary>
    /// Number of minutes in one quarter-hour time unit.
    /// Used for encoding/decoding TimeRange parameters.
    /// </summary>
    public const int MinutesPerQuarterHour = 15;
    
    /// <summary>
    /// Maximum valid quarter-hour index (represents 23:45).
    /// Valid range is 0-95, covering a full 24-hour period in 15-minute increments.
    /// </summary>
    public const byte MaxQuarterHourIndex = 95;
    
    /// <summary>
    /// Special marker value indicating an inactive or disabled time slot.
    /// When encountered in TimeRange decoding, it's treated as 00:00.
    /// </summary>
    public const byte OffMarker = 0x80;
    
    /// <summary>
    /// Quarter-hour values at or above this value are capped to MaxQuarterHourIndex.
    /// This prevents invalid time values beyond 23:45.
    /// </summary>
    public const byte QuarterHourCapValue = 96;
}
