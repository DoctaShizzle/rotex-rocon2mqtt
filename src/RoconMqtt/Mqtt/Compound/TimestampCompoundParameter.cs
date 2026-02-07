using RoconMqtt.Can;

namespace RoconMqtt.Mqtt.Compound;

/// <summary>
/// Combines Hour and Minute parameters into a DateTime timestamp.
/// </summary>
public class TimestampCompoundParameter : ICompoundParameter
{
    private int? _hour;
    private int? _minute;
    private readonly TimeZoneInfo _timeZone;

    /// <summary>
    /// Initializes a new instance of the TimestampCompoundParameter class.
    /// </summary>
    /// <param name="timeZoneId">Optional IANA timezone identifier (e.g., "Europe/Brussels"). If null, system local timezone is used.</param>
    public TimestampCompoundParameter(string? timeZoneId = null)
    {
        _timeZone = string.IsNullOrWhiteSpace(timeZoneId) 
            ? TimeZoneInfo.Local 
            : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public string Name => "Timestamp";

    public IReadOnlyList<string> ComponentParameters => ["Hour", "Minute"];

    public DeviceType? DeviceType => Can.DeviceType.HeatGenerator;

    public string HomeAssistantComponent => "sensor";

    public string? UnitOfMeasurement => null;

    public string? DeviceClass => "timestamp";

    public string? StateClass => null;

    public bool TrySetComponent(string parameterName, object? value)
    {
        ArgumentNullException.ThrowIfNull(parameterName);

        switch (parameterName)
        {
            case "Hour":
                _hour = value switch
                {
                    int intValue => intValue,
                    double doubleValue => (int)doubleValue,
                    _ => null
                };
                break;

            case "Minute":
                _minute = value switch
                {
                    int intValue => intValue,
                    double doubleValue => (int)doubleValue,
                    _ => null
                };
                break;

            default:
                return false;
        }

        return _hour.HasValue && _minute.HasValue;
    }

    public object? GetValue()
    {
        if (!_hour.HasValue || !_minute.HasValue)
            return null;

        // Get current date in the configured timezone
        var nowUtc = DateTime.UtcNow;
        var nowInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, _timeZone);
        
        // Create timestamp with the specified hour and minute in the configured timezone
        var timestamp = new DateTime(nowInTimeZone.Year, nowInTimeZone.Month, nowInTimeZone.Day, 
            _hour.Value, _minute.Value, 0, DateTimeKind.Unspecified);
        
        // Get the UTC offset for this timezone at the timestamp date/time
        var offset = _timeZone.GetUtcOffset(timestamp);
        
        // Create DateTimeOffset with the correct offset
        var timestampWithOffset = new DateTimeOffset(timestamp, offset);
        
        // Return ISO 8601 formatted string with timezone offset for Home Assistant timestamp device class
        return timestampWithOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");
    }

    public void Reset()
    {
        _hour = null;
        _minute = null;
    }
}
