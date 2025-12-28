namespace RoconMqtt.Mqtt.Options;

public class MqttOptions
{
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
}
