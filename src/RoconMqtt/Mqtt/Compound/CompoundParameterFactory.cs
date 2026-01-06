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
    /// <returns>An instance of the compound parameter.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter name is not recognized.</exception>
    public static ICompoundParameter Create(string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        return parameterName switch
        {
            "Timestamp" => new TimestampCompoundParameter(),
            _ => throw new ArgumentException($"Unknown compound parameter: {parameterName}", nameof(parameterName))
        };
    }

    /// <summary>
    /// Gets all available compound parameter names.
    /// </summary>
    public static IReadOnlyList<string> AvailableCompoundParameters => ["Timestamp"];
}
