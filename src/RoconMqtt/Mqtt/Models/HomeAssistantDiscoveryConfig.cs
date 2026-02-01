using System.Text.Json.Serialization;

namespace RoconMqtt.Mqtt.Models;

/// <summary>
/// Represents the Home Assistant MQTT discovery configuration for a device parameter.
/// </summary>
public record HomeAssistantDiscoveryConfig
{
    /// <summary>
    /// Gets the friendly name of the entity.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    [JsonPropertyName("unique_id")]
    public required string UniqueId { get; init; }

    /// <summary>
    /// Gets the default entity ID for the entity (replaces deprecated object_id in home assistant).
    /// </summary>
    [JsonPropertyName("default_entity_id")]
    public required string DefaultEntityId { get; init; }

    /// <summary>
    /// Gets the MQTT topic where the state is published.
    /// </summary>
    [JsonPropertyName("state_topic")]
    public required string StateTopic { get; init; }

    /// <summary>
    /// Gets the template to extract the value from the state payload.
    /// </summary>
    [JsonPropertyName("value_template")]
    public required string ValueTemplate { get; init; }

    /// <summary>
    /// Gets the unit of measurement for the sensor.
    /// </summary>
    [JsonPropertyName("unit_of_measurement")]
    public string? UnitOfMeasurement { get; init; }

    /// <summary>
    /// Gets the device class for Home Assistant.
    /// </summary>
    [JsonPropertyName("device_class")]
    public string? DeviceClass { get; init; }

    /// <summary>
    /// Gets the state class for Home Assistant.
    /// </summary>
    [JsonPropertyName("state_class")]
    public string? StateClass { get; init; }

    /// <summary>
    /// Gets the device information.
    /// </summary>
    [JsonPropertyName("device")]
    public required HomeAssistantDeviceInfo Device { get; init; }
}

/// <summary>
/// Represents device information for Home Assistant MQTT discovery.
/// </summary>
public record HomeAssistantDeviceInfo
{
    /// <summary>
    /// Gets the device identifiers.
    /// </summary>
    [JsonPropertyName("identifiers")]
    public required string[] Identifiers { get; init; }

    /// <summary>
    /// Gets the device name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the device manufacturer.
    /// </summary>
    [JsonPropertyName("manufacturer")]
    public required string Manufacturer { get; init; }

    /// <summary>
    /// Gets the device model.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// Gets the software version.
    /// </summary>
    [JsonPropertyName("sw_version")]
    public required string SwVersion { get; init; }
}
