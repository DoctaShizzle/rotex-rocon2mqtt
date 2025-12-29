using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public record ParameterDefinition(
    string Name,
    InfoNumber InfoNumber,
    ParameterType Type,
    double Factor = 1.0,
    double? TransferThreshold = null,
    bool Writeable = false,
    double? Min = null,
    double? Max = null,
    double? Default = null,
    bool Display = true,
    bool WaterCircuit = false,
    bool BigEndian = false,
    IReadOnlyDictionary<int, string>? EnumValues = null,
    string? DefaultTimeRange = null,
    string? NameEnglish = null
);
