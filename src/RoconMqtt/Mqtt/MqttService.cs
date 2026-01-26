using Microsoft.Extensions.Options;
using MQTTnet;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Mqtt;

/// <summary>
/// Provides an asynchronous service for connecting to an MQTT broker, publishing messages, and managing connection
/// state.
/// </summary>
/// <remarks>MqttService handles connection management, including automatic reconnection with exponential backoff
/// if the connection to the broker is lost. It is designed for use in applications that require reliable MQTT messaging
/// and integrates with dependency injection and logging. The service should be disposed asynchronously when no longer
/// needed to ensure proper cleanup of resources. This class is thread-safe and supports concurrent access from multiple threads.</remarks>
public partial class MqttService : IAsyncDisposable, IMqttService
{
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;
    private readonly ILogger<MqttService> _logger;
    private bool _disposed;
    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
    private readonly ReaderWriterLockSlim _disposedLock = new ReaderWriterLockSlim();

    public bool IsConnected
    {
        get
        {
            _disposedLock.EnterReadLock();
            try
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                return _client.IsConnected;
            }
            finally
            {
                _disposedLock.ExitReadLock();
            }
        }
    }

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
            .WithCleanSession()
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(30));

        if (!string.IsNullOrEmpty(cfg.Username))
        {
            optionsBuilder.WithCredentials(cfg.Username, cfg.Password);
            LogMqttClientConfiguredWithAuth(_logger);
        }

        if (cfg.UseTls)
        {
            var tlsOptions = new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = !cfg.ValidateCertificate,
                IgnoreCertificateChainErrors = !cfg.ValidateCertificate,
                IgnoreCertificateRevocationErrors = !cfg.ValidateCertificate
            };
            optionsBuilder.WithTlsOptions(tlsOptions);
            LogMqttClientConfiguredWithTls(_logger, cfg.ValidateCertificate);
        }

        _options = optionsBuilder.Build();

        _client.DisconnectedAsync += async e =>
        {
            // Don't attempt reconnection if service is being disposed
            _disposedLock.EnterReadLock();
            try
            {
                if (_disposed)
                    return;
            }
            finally
            {
                _disposedLock.ExitReadLock();
            }

            LogMqttDisconnected(_logger, e.Reason, e.Exception?.GetType().Name ?? "None", e.Exception?.Message ?? "");

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
                    
                    // Acquire lock to prevent concurrent connection attempts
                    await _connectionLock.WaitAsync();
                    try
                    {
                        _disposedLock.EnterReadLock();
                        try
                        {
                            ObjectDisposedException.ThrowIf(_disposed, this);
                        }
                        finally
                        {
                            _disposedLock.ExitReadLock();
                        }

                        if (!_client.IsConnected)
                        {
                            LogAttemptingReconnect(_logger, attemptCount);
                            await _client.ConnectAsync(_options);
                            LogMqttReconnectedSuccessfully(_logger, attemptCount);
                            LogConnectionStateAfterReconnect(_logger, _client.IsConnected);
                        }
                    }
                    finally
                    {
                        _connectionLock.Release();
                    }
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
        _disposedLock.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
        finally
        {
            _disposedLock.ExitReadLock();
        }

        await _connectionLock.WaitAsync();
        try
        {
            _disposedLock.EnterReadLock();
            try
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
            }
            finally
            {
                _disposedLock.ExitReadLock();
            }

            if (!_client.IsConnected)
            {
                try
                {
                    LogConnectingToMqttBroker(_logger);
                    await _client.ConnectAsync(_options);
                    LogConnectedToMqttBrokerSuccessfully(_logger);
                    LogConnectionStateAfterConnect(_logger, _client.IsConnected);
                }
                catch (Exception ex)
                {
                    LogFailedToConnectToMqttBroker(_logger, ex);
                    throw;
                }
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default, bool retain = false)
    {
        _disposedLock.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
        finally
        {
            _disposedLock.ExitReadLock();
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            _disposedLock.EnterReadLock();
            try
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
            }
            finally
            {
                _disposedLock.ExitReadLock();
            }

            if (!_client.IsConnected)
            {
                LogMqttClientNotConnected(_logger);
                // Recursively call to ensure connection, but without holding the lock
                _connectionLock.Release();
                try
                {
                    await ConnectAsync();
                }
                finally
                {
                    await _connectionLock.WaitAsync(cancellationToken);
                }
            }

            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag(retain)
                    .Build();

                LogPublishingToTopic(_logger, topic, payload.Length);
                await _client.PublishAsync(message, cancellationToken);
                LogSuccessfullyPublishedToTopic(_logger, topic);
                LogConnectionStateAfterPublish(_logger, _client.IsConnected);
            }
            catch (Exception ex)
            {
                LogFailedToPublishMessageToTopic(_logger, ex, topic);
                throw;
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        // Early check to avoid acquiring disposed lock on subsequent calls
        if (_disposed)
            return;

        _disposedLock.EnterWriteLock();
        try
        {
            if (_disposed)
                return;

            _disposed = true;
        }
        finally
        {
            _disposedLock.ExitWriteLock();
        }

        // Wait for any ongoing operations to complete
        await _connectionLock.WaitAsync();
        try
        {
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
        }
        finally
        {
            _connectionLock.Release();
            _disposedLock.Dispose();
        }
        
        GC.SuppressFinalize(this);
    }

    [LoggerMessage(EventId = 4001, Level = LogLevel.Debug, Message = "Configuring MQTT client for {Host}:{Port}")]
    private static partial void LogConfiguringMqttClient(ILogger logger, string host, int port);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Debug, Message = "MQTT client configured with authentication")]
    private static partial void LogMqttClientConfiguredWithAuth(ILogger logger);

    [LoggerMessage(EventId = 4016, Level = LogLevel.Debug, Message = "MQTT client configured with TLS (certificate validation: {ValidateCertificate})")]
    private static partial void LogMqttClientConfiguredWithTls(ILogger logger, bool validateCertificate);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Warning, Message = "MQTT disconnected. Reason: {Reason}, Exception Type: {ExceptionType}, Details: {ExceptionMessage}. Attempting to reconnect...")]
    private static partial void LogMqttDisconnected(ILogger logger, object reason, string exceptionType, string exceptionMessage);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Debug, Message = "MQTT reconnection attempt {AttemptCount} - waiting {DelaySeconds}s")]
    private static partial void LogMqttReconnectionAttempt(ILogger logger, int attemptCount, double delaySeconds);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Information, Message = "Successfully reconnected to MQTT after {AttemptCount} attempts")]
    private static partial void LogMqttReconnectedSuccessfully(ILogger logger, int attemptCount);

    [LoggerMessage(EventId = 4019, Level = LogLevel.Debug, Message = "Attempting reconnect on attempt {AttemptCount}")]
    private static partial void LogAttemptingReconnect(ILogger logger, int attemptCount);

    [LoggerMessage(EventId = 4020, Level = LogLevel.Debug, Message = "Connection state after reconnect attempt: {IsConnected}")]
    private static partial void LogConnectionStateAfterReconnect(ILogger logger, bool isConnected);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Debug, Message = "MQTT reconnection attempt {AttemptCount} failed")]
    private static partial void LogMqttReconnectionAttemptFailed(ILogger logger, Exception exception, int attemptCount);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Information, Message = "Connecting to MQTT broker...")]
    private static partial void LogConnectingToMqttBroker(ILogger logger);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Information, Message = "Connected to MQTT broker successfully")]
    private static partial void LogConnectedToMqttBrokerSuccessfully(ILogger logger);

    [LoggerMessage(EventId = 4017, Level = LogLevel.Debug, Message = "Connection state after ConnectAsync: {IsConnected}")]
    private static partial void LogConnectionStateAfterConnect(ILogger logger, bool isConnected);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Error, Message = "Failed to connect to MQTT broker")]
    private static partial void LogFailedToConnectToMqttBroker(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 4010, Level = LogLevel.Debug, Message = "MQTT client not connected, attempting to connect before publishing")]
    private static partial void LogMqttClientNotConnected(ILogger logger);

    [LoggerMessage(EventId = 4011, Level = LogLevel.Debug, Message = "Publishing to topic {Topic} with payload length {PayloadLength}")]
    private static partial void LogPublishingToTopic(ILogger logger, string topic, int payloadLength);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Debug, Message = "Successfully published to topic {Topic}")]
    private static partial void LogSuccessfullyPublishedToTopic(ILogger logger, string topic);

    [LoggerMessage(EventId = 4018, Level = LogLevel.Debug, Message = "Connection state after publish: {IsConnected}")]
    private static partial void LogConnectionStateAfterPublish(ILogger logger, bool isConnected);

    [LoggerMessage(EventId = 4013, Level = LogLevel.Error, Message = "Failed to publish message to topic {Topic}")]
    private static partial void LogFailedToPublishMessageToTopic(ILogger logger, Exception exception, string topic);

    [LoggerMessage(EventId = 4014, Level = LogLevel.Information, Message = "Disconnecting from MQTT broker")]
    private static partial void LogDisconnectingFromMqttBroker(ILogger logger);

    [LoggerMessage(EventId = 4015, Level = LogLevel.Error, Message = "Error during MQTT disconnection")]
    private static partial void LogErrorDuringMqttDisconnection(ILogger logger, Exception exception);
}
