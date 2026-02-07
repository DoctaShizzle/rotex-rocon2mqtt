using RoconMqtt.Can;

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
    /// The device type restriction for this compound parameter (e.g., HeatGenerator, HeatingCircuit).
    /// If null, the parameter can be queried from any device type.
    /// </summary>
    DeviceType? DeviceType { get; }

    /// <summary>
    /// The Home Assistant component type (e.g., "sensor", "binary_sensor").
    /// </summary>
    string HomeAssistantComponent { get; }

    /// <summary>
    /// The unit of measurement for Home Assistant (e.g., "°C", "%"). Can be null if no unit applies.
    /// </summary>
    string? UnitOfMeasurement { get; }

    /// <summary>
    /// The Home Assistant device class (e.g., "temperature", "timestamp"). Can be null.
    /// </summary>
    string? DeviceClass { get; }

    /// <summary>
    /// The Home Assistant state class (e.g., "measurement", "total"). Can be null.
    /// </summary>
    string? StateClass { get; }

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
