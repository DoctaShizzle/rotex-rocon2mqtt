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
    public required string Host { get; set; }

    /// <summary>
    /// mqtt broker port, 1883 by default
    /// </summary>
    public int Port { get; set; } = 1883;

    /// <summary>
    /// Gets or sets the unique identifier for the client application.
    /// </summary>
    public required string ClientId { get; set; }

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
    public required string Topic { get; set; }

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
    public int PollingIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Timeout in milliseconds to wait for ANSWER after sending GET request.
    /// Default is 1000ms (1 second).
    /// </summary>
    public int ResponseTimeoutMs { get; set; } = 1000;
}
