namespace RoconMqtt.Can.Models;

/// <summary>
/// Response for starting CAN recording
/// </summary>
public sealed class CanRecordingStartResponse
{
    /// <summary>
    /// Status of the recording start operation
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Descriptive message
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Information about applied CAN ID filter
    /// </summary>
    public required string Filter { get; init; }

    /// <summary>
    /// Optional recording description
    /// </summary>
    public string? Description { get; init; }
}
