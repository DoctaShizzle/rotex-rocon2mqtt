using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt;
using RoconMqtt.Mqtt.Options;
using RoconMqtt.Mqtt.Resilience;
using Xunit;

namespace RoconMqtt.Tests;

public class RoconMqttPublisherTests
{
    private static DecodedParameter CreateDecodedParameter(string name, object value)
    {
        var definition = new ParameterDefinition(
            OriginalName: name,
            InfoNumber: new InfoNumber(0x00, 0x00),
            Type: ParameterType.Int,
            NameEnglish: name);
        return new DecodedParameter(name, value, definition);
    }

    private readonly Mock<ICanService> _canServiceMock;
    private readonly Mock<IMqttService> _mqttServiceMock;
    private readonly Mock<ICanParameterRegistry> _parameterRegistryMock;
    private readonly ResiliencePipelineFactory _resilienceFactory;
    private readonly Mock<ILogger<RoconMqttPublisher>> _loggerMock;
    private readonly MqttOptions _mqttOptions;
    private readonly IOptions<MqttOptions> _options;

    public RoconMqttPublisherTests()
    {
        _canServiceMock = new Mock<ICanService>();
        _mqttServiceMock = new Mock<IMqttService>();
        _parameterRegistryMock = new Mock<ICanParameterRegistry>();
        
        // Create real ResiliencePipelineFactory with disabled resilience for testing
        var resilienceOptions = Options.Create(new ResilienceOptions { Enabled = false });
        var resilienceLogger = new Mock<ILogger<ResiliencePipelineFactory>>();
        _resilienceFactory = new ResiliencePipelineFactory(resilienceOptions, resilienceLogger.Object);
        
        _loggerMock = new Mock<ILogger<RoconMqttPublisher>>();

        _mqttOptions = new MqttOptions
        {
            Enabled = true,
            Host = "localhost",
            Port = 1883,
            ClientId = "test-client",
            Topic = "test/topic",
            PollingIntervalSeconds = 1,
            ResponseTimeoutSeconds = 5,
            ChangeThresholdPercent = 1.0,
            Devices = ["Device1"],
            Parameters = ["Temperature"],
            CompoundParameters = [],
            HomeAssistant = new HomeAssistantOptions
            {
                Discovery = false,
                DiscoveryPrefix = "homeassistant",
                UniqueIdFormat = "{deviceId}_{parameterName}",
                ObjectIdFormat = "{deviceId}_{parameterName}",
                DeviceIdentifierFormat = "{deviceId}",
                DeviceNameFormat = "Device {deviceId}",
                DeviceManufacturer = "Test",
                DeviceModel = "Test Model",
                DeviceSoftwareVersion = "1.0",
                ValueTemplate = "{{ value_json.value }}"
            }
        };
        _options = Options.Create(_mqttOptions);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenParameterRegistryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            null!,
            _resilienceFactory,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCanServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RoconMqttPublisher(
            null!,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenResilienceFactoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            null!,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_ShouldInitializeWithEnabledState()
    {
        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        Assert.True(publisher.Enabled);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDisabledState_WhenOptionsAreDisabled()
    {
        _mqttOptions.Enabled = false;
        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        Assert.False(publisher.Enabled);
    }

    [Fact]
    public void Enabled_CanBeSetAndGet()
    {
        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        )
        {
            Enabled = false
        };
        Assert.False(publisher.Enabled);

        publisher.Enabled = true;
        Assert.True(publisher.Enabled);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldStopImmediately_WhenCancellationRequested()
    {
        _mqttOptions.Devices.Clear();
        _mqttOptions.Parameters.Clear();

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await publisher.StartAsync(cts.Token);
        await publisher.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldQueryDeviceIdentifiers_OnStartup()
    {
        _mqttOptions.PollingIntervalSeconds = 3600; // Long interval to avoid second poll
        
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "TEST123"));

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3)); // Wait for device identifier query
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        _canServiceMock.Verify(
            x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUseFallbackDeviceId_WhenDeviceIdentifierQueryFails()
    {
        _mqttOptions.PollingIntervalSeconds = 3600;
        
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DecodedParameter)null!);

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        _canServiceMock.Verify(
            x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSkipPollingCycle_WhenPublisherDisabled()
    {
        _mqttOptions.Enabled = false;
        _mqttOptions.PollingIntervalSeconds = 1;

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        // Should not query parameters when disabled
        _canServiceMock.Verify(
            x => x.SendRequestAndWaitForResponseAsync(
                It.IsAny<string>(),
                "Temperature",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleNoDevicesConfigured()
    {
        _mqttOptions.Devices.Clear();
        _mqttOptions.PollingIntervalSeconds = 3600;

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        // Verify no queries were made (except potentially DeviceIdentifier)
        // Since there are no devices, no queries should be made at all
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleNoParametersConfigured()
    {
        _mqttOptions.Parameters.Clear();
        _mqttOptions.PollingIntervalSeconds = 3600;

        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "TEST123"));

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        // Only DeviceIdentifier should be queried, not regular parameters
        _canServiceMock.Verify(
            x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleMultipleDevices()
    {
        _mqttOptions.Devices = ["Device1", "Device2"];
        _mqttOptions.PollingIntervalSeconds = 3600;

        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                It.IsAny<string>(),
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "TEST123"));

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        _canServiceMock.Verify(
            x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);

        _canServiceMock.Verify(
            x => x.SendRequestAndWaitForResponseAsync(
                "Device2",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleExceptionInPollingLoop()
    {
        _mqttOptions.PollingIntervalSeconds = 1;

        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "Device1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        await publisher.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        // Should continue running despite exception
    }
}
