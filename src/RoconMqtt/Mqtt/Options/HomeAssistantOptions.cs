using System.ComponentModel.DataAnnotations;

namespace RoconMqtt.Mqtt.Options;

/// <summary>
/// Home Assistant specific MQTT options.
/// </summary>
public class HomeAssistantOptions
{
    /// <summary>
    /// Enable Home Assistant MQTT Discovery.
    /// When enabled, the publisher will automatically publish discovery messages
    /// to allow Home Assistant to auto-configure entities.
    /// Default is true.
    /// </summary>
    public bool Discovery { get; set; } = true;

    /// <summary>
    /// Home Assistant MQTT Discovery prefix.
    /// Default is "homeassistant" which is the Home Assistant default.
    /// </summary>
    [Required(ErrorMessage = "Home Assistant discovery prefix is required")]
    public string DiscoveryPrefix { get; set; } = "homeassistant";

    /// <summary>
    /// Device identifier format string for Home Assistant discovery.
    /// The placeholder {0} will be replaced with the device ID.
    /// </summary>
    [Required(ErrorMessage = "Device identifier format is required")]
    public string DeviceIdentifierFormat { get; set; } = null!;

    /// <summary>
    /// Device name format string for Home Assistant discovery.
    /// The placeholder {0} will be replaced with the device ID.
    /// </summary>
    [Required(ErrorMessage = "Device name format is required")]
    public string DeviceNameFormat { get; set; } = null!;

    /// <summary>
    /// Device manufacturer name for Home Assistant discovery.
    /// </summary>
    [Required(ErrorMessage = "Device manufacturer is required")]
    public string DeviceManufacturer { get; set; } = null!;

    /// <summary>
    /// Device model name for Home Assistant discovery.
    /// </summary>
    [Required(ErrorMessage = "Device model is required")]
    public string DeviceModel { get; set; } = null!;

    /// <summary>
    /// Device software version for Home Assistant discovery.
    /// </summary>
    [Required(ErrorMessage = "Device software version is required")]
    public string DeviceSoftwareVersion { get; set; } = null!;

    /// <summary>
    /// Value template for Home Assistant discovery sensor entities.
    /// This template is used to extract the value from the MQTT message payload.
    /// </summary>
    [Required(ErrorMessage = "Value template is required")]
    public string ValueTemplate { get; set; } = null!;

    /// <summary>
    /// Unique ID format string for Home Assistant discovery entities.
    /// The placeholders {0} and {1} will be replaced with the device ID and parameter name respectively.
    /// </summary>
    [Required(ErrorMessage = "Unique ID format is required")]
    public string UniqueIdFormat { get; set; } = null!;

    /// <summary>
    /// Object ID format string for Home Assistant discovery entities.
    /// The placeholders {0} and {1} will be replaced with the device ID and parameter name respectively.
    /// </summary>
    [Required(ErrorMessage = "Object ID format is required")]
    public string ObjectIdFormat { get; set; } = null!;
}
