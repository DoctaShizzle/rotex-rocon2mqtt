namespace RoconMqtt.Can.Configuration;

/// <summary>
/// Configuration options for CAN parameter registry loaded from appsettings.json
/// </summary>
public class CanRegistryOptions
{
    /// <summary>
    /// List of parameter definitions
    /// </summary>
    public List<ParameterDefinitionDto> Parameters { get; set; } = [];

    /// <summary>
    /// List of heat generator devices (HG1-HG8)
    /// </summary>
    public List<CanDeviceDto> HeatGenerators { get; set; } = [];

    /// <summary>
    /// List of heating circuit devices (HC1-HC16)
    /// </summary>
    public List<CanDeviceDto> HeatingCircuits { get; set; } = [];

    /// <summary>
    /// List of heating circuit module devices (HCM1-HCM16)
    /// </summary>
    public List<CanDeviceDto> HeatingCircuitModules { get; set; } = [];
}
