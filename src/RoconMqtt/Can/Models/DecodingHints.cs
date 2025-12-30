namespace RoconMqtt.Can.Models;

/// <summary>
/// Provides decoding hints for unknown parameters that are not in the registry.
/// When a parameter definition exists in the registry, these hints are ignored.
/// </summary>
/// <param name="ParameterName">Name to use for the decoded parameter (default: "UnknownParameter")</param>
/// <param name="ParameterType">Type of the parameter (default: Int)</param>
/// <param name="BigEndian">Whether to use big-endian decoding (default: false)</param>
/// <param name="Factor">Factor to divide by when decoding float values (default: 1.0)</param>
public record DecodingHints(
    string ParameterName = "UnknownParameter",
    ParameterType ParameterType = ParameterType.Int,
    bool BigEndian = false,
    double Factor = 1.0
);
