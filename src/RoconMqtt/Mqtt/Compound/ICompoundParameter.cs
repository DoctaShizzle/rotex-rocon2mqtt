namespace RoconMqtt.Mqtt.Compound;

/// <summary>
/// Represents a compound parameter that combines multiple CAN parameters into a single value.
/// </summary>
public interface ICompoundParameter
{
    /// <summary>
    /// The name of the compound parameter (e.g., "Timestamp").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The list of individual parameter names that make up this compound parameter.
    /// </summary>
    IReadOnlyList<string> ComponentParameters { get; }

    /// <summary>
    /// Tries to set a component value and returns whether the compound parameter is complete.
    /// </summary>
    /// <param name="parameterName">The name of the component parameter.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if all components are now set and the compound parameter is complete; otherwise false.</returns>
    bool TrySetComponent(string parameterName, object? value);

    /// <summary>
    /// Gets the combined value if all components are set.
    /// </summary>
    /// <returns>The combined value, or null if not all components are set.</returns>
    object? GetValue();

    /// <summary>
    /// Resets all component values.
    /// </summary>
    void Reset();
}
