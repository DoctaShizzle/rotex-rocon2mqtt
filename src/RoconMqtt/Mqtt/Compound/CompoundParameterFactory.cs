namespace RoconMqtt.Mqtt.Compound;

/// <summary>
/// Factory for creating compound parameters.
/// </summary>
public static class CompoundParameterFactory
{
    /// <summary>
    /// Creates a compound parameter instance based on the parameter name.
    /// </summary>
    /// <param name="parameterName">The name of the compound parameter to create.</param>
    /// <param name="timeZoneId">Optional IANA timezone identifier (e.g., "Europe/Brussels"). If null, system local timezone is used.</param>
    /// <returns>An instance of the compound parameter.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter name is not recognized.</exception>
    public static ICompoundParameter Create(string parameterName, string? timeZoneId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        return parameterName switch
        {
            "Timestamp" => new TimestampCompoundParameter(timeZoneId),
            _ => throw new ArgumentException($"Unknown compound parameter: {parameterName}", nameof(parameterName))
        };
    }

    /// <summary>
    /// Gets all available compound parameter names.
    /// </summary>
    public static IReadOnlyList<string> AvailableCompoundParameters => ["Timestamp"];
}
