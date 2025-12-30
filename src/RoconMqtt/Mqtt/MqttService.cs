using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Mqtt;

public partial class MqttService : IAsyncDisposable, IMqttService
{
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;
    private readonly ILogger<MqttService> _logger;
    private bool _disposed;

    public bool IsConnected => _client.IsConnected;

    public MqttService(IOptions<MqttOptions> settings, ILogger<MqttService> logger)
    {
        _logger = logger;
        
        var factory = new MqttClientFactory();
        _client = factory.CreateMqttClient();

        var cfg = settings.Value;
        LogConfiguringMqttClient(_logger, cfg.Host, cfg.Port);

        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(cfg.Host, cfg.Port)
            .WithClientId(cfg.ClientId)
            .WithCleanSession();

        if (!string.IsNullOrEmpty(cfg.Username))
        {
            optionsBuilder.WithCredentials(cfg.Username, cfg.Password);
            LogMqttClientConfiguredWithAuth(_logger);
        }

        _options = optionsBuilder.Build();

        _client.DisconnectedAsync += async e =>
        {
            LogMqttDisconnected(_logger, e.Reason);

            // Simple exponential backoff
            var delay = TimeSpan.FromSeconds(2);
            var attemptCount = 0;
            
            while (!_client.IsConnected)
            {
                try
                {
                    attemptCount++;
                    LogMqttReconnectionAttempt(_logger, attemptCount, delay.TotalSeconds);
                    await Task.Delay(delay);
                    await _client.ConnectAsync(_options);
                    LogMqttReconnectedSuccessfully(_logger, attemptCount);
                }
                catch (Exception ex)
                {
                    LogMqttReconnectionAttemptFailed(_logger, ex, attemptCount);
                    delay *= 2;
                    if (delay > TimeSpan.FromSeconds(30))
                        delay = TimeSpan.FromSeconds(30);
                }
            }
        };
    }

    public async Task ConnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (!_client.IsConnected)
        {
            try
            {
                LogConnectingToMqttBroker(_logger);
                await _client.ConnectAsync(_options);
                LogConnectedToMqttBrokerSuccessfully(_logger);
            }
            catch (Exception ex)
            {
                LogFailedToConnectToMqttBroker(_logger, ex);
                throw;
            }
        }
    }

    public async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (!_client.IsConnected)
        {
            LogMqttClientNotConnected(_logger);
            await ConnectAsync();
        }

        try
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(true)
                .Build();

            LogPublishingToTopic(_logger, topic, payload.Length);
            await _client.PublishAsync(message, cancellationToken);
            LogSuccessfullyPublishedToTopic(_logger, topic);
        }
        catch (Exception ex)
        {
            LogFailedToPublishMessageToTopic(_logger, ex, topic);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            if (_client.IsConnected)
            {
                LogDisconnectingFromMqttBroker(_logger);
                await _client.DisconnectAsync();
            }
            
            _client.Dispose();
        }
        catch (Exception ex)
        {
            LogErrorDuringMqttDisconnection(_logger, ex);
        }
        
        GC.SuppressFinalize(this);
    }

    [LoggerMessage(EventId = 4001, Level = LogLevel.Debug, Message = "Configuring MQTT client for {Host}:{Port}")]
    private static partial void LogConfiguringMqttClient(ILogger logger, string host, int port);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Debug, Message = "MQTT client configured with authentication")]
    private static partial void LogMqttClientConfiguredWithAuth(ILogger logger);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Warning, Message = "MQTT disconnected. Reason: {Reason}. Attempting to reconnect...")]
    private static partial void LogMqttDisconnected(ILogger logger, object reason);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Debug, Message = "MQTT reconnection attempt {AttemptCount} - waiting {DelaySeconds}s")]
    private static partial void LogMqttReconnectionAttempt(ILogger logger, int attemptCount, double delaySeconds);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Information, Message = "Successfully reconnected to MQTT after {AttemptCount} attempts")]
    private static partial void LogMqttReconnectedSuccessfully(ILogger logger, int attemptCount);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Debug, Message = "MQTT reconnection attempt {AttemptCount} failed")]
    private static partial void LogMqttReconnectionAttemptFailed(ILogger logger, Exception exception, int attemptCount);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Information, Message = "Connecting to MQTT broker...")]
    private static partial void LogConnectingToMqttBroker(ILogger logger);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Information, Message = "Connected to MQTT broker successfully")]
    private static partial void LogConnectedToMqttBrokerSuccessfully(ILogger logger);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Error, Message = "Failed to connect to MQTT broker")]
    private static partial void LogFailedToConnectToMqttBroker(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 4010, Level = LogLevel.Debug, Message = "MQTT client not connected, attempting to connect before publishing")]
    private static partial void LogMqttClientNotConnected(ILogger logger);

    [LoggerMessage(EventId = 4011, Level = LogLevel.Debug, Message = "Publishing to topic {Topic} with payload length {PayloadLength}")]
    private static partial void LogPublishingToTopic(ILogger logger, string topic, int payloadLength);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Debug, Message = "Successfully published to topic {Topic}")]
    private static partial void LogSuccessfullyPublishedToTopic(ILogger logger, string topic);

    [LoggerMessage(EventId = 4013, Level = LogLevel.Error, Message = "Failed to publish message to topic {Topic}")]
    private static partial void LogFailedToPublishMessageToTopic(ILogger logger, Exception exception, string topic);

    [LoggerMessage(EventId = 4014, Level = LogLevel.Information, Message = "Disconnecting from MQTT broker")]
    private static partial void LogDisconnectingFromMqttBroker(ILogger logger);

    [LoggerMessage(EventId = 4015, Level = LogLevel.Error, Message = "Error during MQTT disconnection")]
    private static partial void LogErrorDuringMqttDisconnection(ILogger logger, Exception exception);
}
