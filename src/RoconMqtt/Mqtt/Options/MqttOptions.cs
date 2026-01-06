using System.ComponentModel.DataAnnotations;

namespace RoconMqtt.Mqtt.Options;

public class MqttOptions
{
    /// <summary>
    /// Enable or disable the MQTT publisher, on start of the application.
    /// Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// mqtt broker host
    /// </summary>
    [Required(ErrorMessage = "MQTT Host is required")]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// mqtt broker port, 1883 by default
    /// </summary>
    [Range(1, 65535, ErrorMessage = "MQTT Port must be between 1 and 65535")]
    public int Port { get; set; } = 1883;

    /// <summary>
    /// Enable TLS/SSL for MQTT connection.
    /// Default is false (unencrypted connection on port 1883).
    /// Set to true for encrypted connection (typically port 8883).
    /// </summary>
    public bool UseTls { get; set; } = false;

    /// <summary>
    /// Validate TLS/SSL certificate when UseTls is enabled.
    /// Default is true for security.
    /// Set to false only for development/testing with self-signed certificates.
    /// </summary>
    public bool ValidateCertificate { get; set; } = true;

    /// <summary>
    /// Gets or sets the unique identifier for the client application.
    /// </summary>
    [Required(ErrorMessage = "MQTT ClientId is required")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// optional mqtt username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// optional mqtt password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the topic associated with the current instance.
    /// </summary>
    [Required(ErrorMessage = "MQTT Topic is required")]
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// List of device names to query (e.g., "HG1", "HC1", "HCM1").
    /// If empty or null, no devices will be queried.
    /// </summary>
    public List<string> Devices { get; set; } = [];

    /// <summary>
    /// List of parameter names to query (e.g., "cAUSSENTEMP", "cTAG").
    /// If empty or null, no parameters will be queried.
    /// </summary>
    public List<string> Parameters { get; set; } = [];

    /// <summary>
    /// List of compound parameter names to create (e.g., "Timestamp").
    /// Compound parameters combine multiple individual parameters into a single value.
    /// If empty or null, no compound parameters will be created.
    /// </summary>
    public List<string> CompoundParameters { get; set; } = [];

    /// <summary>
    /// Interval in seconds between GET requests for each parameter.
    /// Default is 5 seconds.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Polling interval must be at least 1 second")]
    public int PollingIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Timeout in seconds to wait for ANSWER after sending GET request.
    /// Default is 1 second.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Response timeout must be at least 1 second")]
    public int ResponseTimeoutSeconds { get; set; } = 1;

    /// <summary>
    /// Enable Home Assistant MQTT Discovery.
    /// When enabled, the publisher will automatically publish discovery messages
    /// to allow Home Assistant to auto-configure entities.
    /// Default is true.
    /// </summary>
    public bool HomeAssistantDiscovery { get; set; } = true;

    /// <summary>
    /// Home Assistant MQTT Discovery prefix.
    /// Default is "homeassistant" which is the Home Assistant default.
    /// </summary>
    [Required(ErrorMessage = "Home Assistant discovery prefix is required")]
    public string HomeAssistantDiscoveryPrefix { get; set; } = "homeassistant";

    /// <summary>
    /// Percentage change threshold (0-100). 
    /// Only publish if value changes by this percentage or more.
    /// Set to 0 to publish all changes.
    /// Default is 0.
    /// </summary>
    [Range(0, 100, ErrorMessage = "Change threshold must be between 0 and 100")]
    public double ChangeThresholdPercent { get; set; } = 0;
}
