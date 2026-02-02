namespace RoconMqtt.Can.Models;

/// <summary>
/// Options for CAN bus recording
/// </summary>
public sealed class CanRecordingOptions
{
    /// <summary>
    /// Optional filter: only record frames with these CAN IDs (hex format, e.g., "0x180", "0x300")
    /// If null or empty, all frames are recorded
    /// </summary>
    public List<string>? CanIdFilter { get; set; }

    /// <summary>
    /// Optional description/label for this recording session
    /// </summary>
    public string? Description { get; set; }
}
