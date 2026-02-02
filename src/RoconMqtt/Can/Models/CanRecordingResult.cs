namespace RoconMqtt.Can.Models;

/// <summary>
/// Result of a CAN bus recording session
/// </summary>
public sealed class CanRecordingResult
{
    /// <summary>
    /// Timestamp when recording started (UTC)
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// Timestamp when recording stopped (UTC)
    /// </summary>
    public required DateTime StoppedAt { get; init; }

    /// <summary>
    /// Duration of the recording
    /// </summary>
    public TimeSpan Duration => StoppedAt - StartedAt;

    /// <summary>
    /// Total number of frames captured
    /// </summary>
    public required int FrameCount { get; init; }

    /// <summary>
    /// Recording description (if provided)
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Applied CAN ID filter (if any)
    /// </summary>
    public List<string>? CanIdFilter { get; init; }

    /// <summary>
    /// All captured frames
    /// </summary>
    public required List<RecordedCanFrame> Frames { get; init; }

    /// <summary>
    /// Get candump-style formatted output
    /// </summary>
    public string ToCandumpFormat()
    {
        var header = $"# CAN Recording Session\n" +
                     $"# Started: {StartedAt:yyyy-MM-dd HH:mm:ss.fff} UTC\n" +
                     $"# Stopped: {StoppedAt:yyyy-MM-dd HH:mm:ss.fff} UTC\n" +
                     $"# Duration: {Duration.TotalSeconds:F3} seconds\n" +
                     $"# Frame Count: {FrameCount}\n";

        if (!string.IsNullOrWhiteSpace(Description))
        {
            header += $"# Description: {Description}\n";
        }

        if (CanIdFilter != null && CanIdFilter.Count > 0)
        {
            header += $"# Filter: {string.Join(", ", CanIdFilter)}\n";
        }

        header += "#\n";

        var frames = string.Join("\n", Frames.Select(f => f.ToString()));
        return header + frames;
    }
}
