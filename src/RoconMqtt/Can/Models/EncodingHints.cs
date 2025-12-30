namespace RoconMqtt.Can.Models;

/// <summary>
/// Provides encoding hints for unknown parameters that are not in the registry.
/// When a parameter definition exists in the registry, these hints are ignored.
/// </summary>
/// <param name="ParameterType">Type of the parameter (default: Int)</param>
/// <param name="BigEndian">Whether to use big-endian encoding (default: false)</param>
/// <param name="Factor">Factor to multiply by when encoding float values (default: 1.0)</param>
public record EncodingHints(
    ParameterType ParameterType = ParameterType.Int,
    bool BigEndian = false,
    double Factor = 1.0
);
