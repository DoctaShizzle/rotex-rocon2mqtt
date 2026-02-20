namespace RoconMqtt.Can.Configuration;

/// <summary>
/// Configuration options for CAN parameter registry loaded from appsettings.json
/// </summary>
public class CanRegistryOptions
{
    /// <summary>
    /// Machine type identifier (e.g., "EHSHXXPXXA")
    /// </summary>
    public string MachineType { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the machine type
    /// </summary>
    public string MachineDescription { get; set; } = string.Empty;

    /// <summary>
    /// List of device names to query (e.g., "HG1", "HC1", "HCM1")
    /// </summary>
    public List<string> Devices { get; set; } = [];

    /// <summary>
    /// List of parameter names to publish to MQTT
    /// </summary>
    public List<string> MqttParameters { get; set; } = [];

    /// <summary>
    /// List of compound parameter names to create for MQTT
    /// </summary>
    public List<string> MqttCompoundParameters { get; set; } = [];

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
