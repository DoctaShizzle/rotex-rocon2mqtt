namespace RoconMqtt.Mqtt.Models;

/// <summary>
/// Represents the payload published to MQTT state topics for Home Assistant.
/// Contains only the essential parameter information without metadata.
/// </summary>
/// <param name="Name">Human-readable parameter name (e.g., "cAUSSENTEMP")</param>
/// <param name="Value">Decoded parameter value (type depends on parameter: int, double, bool, string, DateTime)</param>
public record MqttStatePayload(
    string Name,
    object Value
);
