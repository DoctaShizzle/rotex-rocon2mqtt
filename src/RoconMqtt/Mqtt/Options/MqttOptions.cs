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
    /// Interval in milliseconds between GET requests for each parameter.
    /// Default is 5000ms (5 seconds).
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Polling interval must be at least 100ms")]
    public int PollingIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Timeout in milliseconds to wait for ANSWER after sending GET request.
    /// Default is 1000ms (1 second).
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Response timeout must be at least 100ms")]
    public int ResponseTimeoutMs { get; set; } = 1000;
}
