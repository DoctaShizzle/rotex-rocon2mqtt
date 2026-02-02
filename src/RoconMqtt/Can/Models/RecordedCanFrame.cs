namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents a single recorded CAN frame with timestamp
/// </summary>
public sealed class RecordedCanFrame
{
    /// <summary>
    /// Timestamp when the frame was captured (UTC)
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    /// CAN identifier (hex format)
    /// </summary>
    public required string CanId { get; init; }

    /// <summary>
    /// Raw frame data as hex string
    /// </summary>
    public required string Data { get; init; }

    /// <summary>
    /// Data length
    /// </summary>
    public required int DataLength { get; init; }

    /// <summary>
    /// Format as candump-style output
    /// </summary>
    public override string ToString()
    {
        // Format: (timestamp) canId#data
        var timeOffset = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        return $"({timeOffset}) {CanId}#{Data}";
    }
}
