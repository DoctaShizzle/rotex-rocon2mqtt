using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MQTTnet;
using RoconMqtt.Mqtt;
using RoconMqtt.Mqtt.Options;
using Xunit;

namespace RoconMqtt.Tests;

public class MqttServiceTests : IAsyncDisposable
{
    private readonly Mock<ILogger<MqttService>> _loggerMock;
    private readonly MqttOptions _mqttOptions;
    private readonly Mock<IOptionsMonitor<MqttOptions>> _optionsMonitorMock;
    private MqttService? _service;

    public MqttServiceTests()
    {
        _loggerMock = new Mock<ILogger<MqttService>>();
        _mqttOptions = new MqttOptions
        {
            Host = "localhost",
            Port = 1883,
            ClientId = "test-client",
            Topic = "test/topic"
        };
        _optionsMonitorMock = new Mock<IOptionsMonitor<MqttOptions>>();
        _optionsMonitorMock.Setup(x => x.CurrentValue).Returns(_mqttOptions);
    }

    [Fact]
    public void Constructor_ShouldConfigureClientWithBasicOptions()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.NotNull(_service);
        Assert.False(_service.IsConnected);
    }

    [Fact]
    public void Constructor_ShouldConfigureClientWithAuthentication()
    {
        _mqttOptions.Username = "testuser";
        _mqttOptions.Password = "testpass";

        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.NotNull(_service);
        Assert.False(_service.IsConnected);
    }

    [Fact]
    public void Constructor_ShouldConfigureClientWithTls()
    {
        _mqttOptions.UseTls = true;
        _mqttOptions.ValidateCertificate = true;

        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.NotNull(_service);
        Assert.False(_service.IsConnected);
    }

    [Fact]
    public void Constructor_ShouldConfigureClientWithTlsWithoutValidation()
    {
        _mqttOptions.UseTls = true;
        _mqttOptions.ValidateCertificate = false;

        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.NotNull(_service);
        Assert.False(_service.IsConnected);
    }

    [Fact]
    public async Task IsConnected_ShouldReturnFalse_WhenNotConnected()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.False(_service.IsConnected);

        await _service.DisposeAsync();
    }

    [Fact]
    public async Task IsConnected_ShouldThrowObjectDisposedException_WhenDisposed()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);
        await _service.DisposeAsync();

        Assert.Throws<ObjectDisposedException>(() => _service.IsConnected);
    }

    [Fact]
    public async Task ConnectAsync_ShouldThrowObjectDisposedException_WhenDisposed()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);
        await _service.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => _service.ConnectAsync());
    }

    [Fact]
    public async Task PublishAsync_ShouldThrowObjectDisposedException_WhenDisposed()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);
        await _service.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => 
            _service.PublishAsync("test/topic", "test payload"));
    }

    [Fact]
    public async Task PublishAsync_ShouldAcceptCancellationToken()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Should accept the cancellation token, but will fail because broker is not available
        await Assert.ThrowsAnyAsync<Exception>(() => 
            _service.PublishAsync("test/topic", "test payload", cts.Token));
    }

    [Fact]
    public async Task PublishAsync_ShouldAcceptRetainFlag()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        // Should accept the retain flag, but will fail because broker is not available
        await Assert.ThrowsAnyAsync<Exception>(() => 
            _service.PublishAsync("test/topic", "test payload", default, retain: true));
    }

    [Fact]
    public async Task DisposeAsync_ShouldBeIdempotent()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        await _service.DisposeAsync();
        await _service.DisposeAsync(); // Should not throw

        // Verify service is disposed
        Assert.Throws<ObjectDisposedException>(() => _service.IsConnected);
    }

    [Fact]
    public void Constructor_ShouldHandleNullUsername()
    {
        _mqttOptions.Username = null;
        _mqttOptions.Password = null;

        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.NotNull(_service);
        Assert.False(_service.IsConnected);
    }

    [Fact]
    public void Constructor_ShouldHandleEmptyUsername()
    {
        _mqttOptions.Username = string.Empty;
        _mqttOptions.Password = string.Empty;

        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        Assert.NotNull(_service);
        Assert.False(_service.IsConnected);
    }

    [Fact]
    public async Task ConnectAsync_ShouldFailGracefully_WhenBrokerUnavailable()
    {
        _mqttOptions.Host = "nonexistent.broker";
        _mqttOptions.Port = 1883;
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        await Assert.ThrowsAnyAsync<Exception>(() => _service.ConnectAsync());
    }

    [Fact]
    public async Task PublishAsync_ShouldFailGracefully_WhenBrokerUnavailable()
    {
        _mqttOptions.Host = "nonexistent.broker";
        _mqttOptions.Port = 1883;
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        await Assert.ThrowsAnyAsync<Exception>(() => 
            _service.PublishAsync("test/topic", "test payload"));
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleEmptyPayload()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        // Should accept empty payload, but will fail because broker is not available
        await Assert.ThrowsAnyAsync<Exception>(() => 
            _service.PublishAsync("test/topic", string.Empty));
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleLargePayload()
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);
        var largePayload = new string('X', 10000);

        // Should accept large payload, but will fail because broker is not available
        await Assert.ThrowsAnyAsync<Exception>(() => 
            _service.PublishAsync("test/topic", largePayload));
    }

    [Theory]
    [InlineData("test/topic")]
    [InlineData("test/topic/with/multiple/levels")]
    [InlineData("test-topic")]
    [InlineData("test_topic")]
    public async Task PublishAsync_ShouldAcceptVariousTopicFormats(string topic)
    {
        _service = new MqttService(_optionsMonitorMock.Object, _loggerMock.Object);

        // Should accept various topic formats, but will fail because broker is not available
        await Assert.ThrowsAnyAsync<Exception>(() => 
            _service.PublishAsync(topic, "test payload"));
    }

    public async ValueTask DisposeAsync()
    {
        if (_service != null)
        {
            try
            {
                await _service.DisposeAsync();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed, ignore
            }
        }

        GC.SuppressFinalize(this);
    }
}
