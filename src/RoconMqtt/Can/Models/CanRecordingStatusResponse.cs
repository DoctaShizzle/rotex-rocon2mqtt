namespace RoconMqtt.Can.Models;

/// <summary>
/// Response for CAN recording status
/// </summary>
public sealed class CanRecordingStatusResponse
{
    /// <summary>
    /// Whether CAN bus recording is currently active
    /// </summary>
    public required bool IsRecording { get; init; }

    /// <summary>
    /// Current recording status
    /// </summary>
    public required string Status { get; init; }
}
