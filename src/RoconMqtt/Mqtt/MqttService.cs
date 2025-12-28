using Microsoft.Extensions.Options;
using MQTTnet;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Mqtt;

public class MqttService : IAsyncDisposable, IMqttService
{
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;

    public bool IsConnected => _client.IsConnected;

    public MqttService(IOptions<MqttOptions> settings)
    {
        var factory = new MqttClientFactory();
        _client = factory.CreateMqttClient();

        var cfg = settings.Value;

        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(cfg.Host, cfg.Port)
            .WithClientId(cfg.ClientId)
            .WithCleanSession();

        if (!string.IsNullOrEmpty(cfg.Username))
        {
            optionsBuilder
            .WithCredentials(cfg.Username, cfg.Password);
        }

        _options = optionsBuilder.Build();

        _client.DisconnectedAsync += async e =>
        {
            Console.WriteLine("MQTT disconnected. Reconnecting...");

            // Simple exponential backoff
            var delay = TimeSpan.FromSeconds(2);
            while (!_client.IsConnected)
            {
                try
                {
                    await Task.Delay(delay);
                    await _client.ConnectAsync(_options);
                    Console.WriteLine("Reconnected to MQTT.");
                }
                catch
                {
                    delay *= 2;
                    if (delay > TimeSpan.FromSeconds(30))
                        delay = TimeSpan.FromSeconds(30);
                }
            }
        };
    }

    public async Task ConnectAsync()
    {
        if (!_client.IsConnected)
            await _client.ConnectAsync(_options);
    }

    public async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
    {
        if (!_client.IsConnected)
            await ConnectAsync();

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(true)
            .Build();

        await _client.PublishAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_client.IsConnected)
            await _client.DisconnectAsync();
    }
}
