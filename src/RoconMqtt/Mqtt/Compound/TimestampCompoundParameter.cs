namespace RoconMqtt.Mqtt.Compound;

/// <summary>
/// Combines Hour and Minute parameters into a DateTime timestamp.
/// </summary>
public class TimestampCompoundParameter : ICompoundParameter
{
    private int? _hour;
    private int? _minute;

    public string Name => "Timestamp";

    public IReadOnlyList<string> ComponentParameters => ["Hour", "Minute"];

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

        var now = DateTimeOffset.Now;
        var timestamp = new DateTimeOffset(now.Year, now.Month, now.Day, _hour.Value, _minute.Value, 0, now.Offset);
        
        // Return ISO 8601 formatted string for Home Assistant timestamp device class
        return timestamp.ToString("O");
    }

    public void Reset()
    {
        _hour = null;
        _minute = null;
    }
}
