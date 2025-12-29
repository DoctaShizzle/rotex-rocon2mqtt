namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents a decoded CAN parameter with its name, value, and metadata.
/// </summary>
/// <param name="Name">Human-readable parameter name (e.g., "cAUSSENTEMP")</param>
/// <param name="Value">Decoded parameter value (type depends on ParameterType: int, double, bool, string)</param>
/// <param name="Definition">Parameter definition containing metadata (type, factor, range, etc.)</param>
public record DecodedParameter(
    string Name,
    object Value,
    ParameterDefinition Definition
);