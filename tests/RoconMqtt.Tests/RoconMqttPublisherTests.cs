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
        
        // Setup registry device collections for GetDeviceTypeFromRegistry
        var heatGenerators = new List<CanDevice>
        {
            new() 
            { 
                Name = "HG1", 
                Type = DeviceType.HeatGenerator, 
                Profile = CreateTestProfile("HG1") 
            }
        };
        var heatingCircuits = new List<CanDevice>
        {
            new() 
            { 
                Name = "HC1", 
                Type = DeviceType.HeatingCircuit, 
                Profile = CreateTestProfile("HC1") 
            }
        };
        var heatingCircuitModules = new List<CanDevice>
        {
            new() 
            { 
                Name = "HCM1", 
                Type = DeviceType.HeatingCircuitModule, 
                Profile = CreateTestProfile("HCM1") 
            }
        };

        // Add test devices that match the test configurations
        heatGenerators.Add(new() { Name = "Device1", Type = DeviceType.HeatGenerator, Profile = CreateTestProfile("Device1") });
        heatGenerators.Add(new() { Name = "Device2", Type = DeviceType.HeatGenerator, Profile = CreateTestProfile("Device2") });

        _parameterRegistryMock.Setup(x => x.HeatGenerators).Returns(heatGenerators.AsReadOnly());
        _parameterRegistryMock.Setup(x => x.HeatingCircuits).Returns(heatingCircuits.AsReadOnly());
        _parameterRegistryMock.Setup(x => x.HeatingCircuitModules).Returns(heatingCircuitModules.AsReadOnly());
        
        // Setup parameter registry with test parameters that have deviceType defined
        var parameters = new Dictionary<InfoNumber, ParameterDefinition>
        {
            [new InfoNumber(0x00, 0x01)] = new ParameterDefinition(
                OriginalName: "Temperature",
                InfoNumber: new InfoNumber(0x00, 0x01),
                Type: ParameterType.Float,
                NameEnglish: "Temperature",
                DeviceType: DeviceType.HeatGenerator,
                HomeAssistantComponent: "sensor",
                UnitOfMeasurement: "°C",
                DeviceClass: "temperature",
                StateClass: "measurement"),
            [new InfoNumber(0x01, 0x48)] = new ParameterDefinition(
                OriginalName: "cGERAETE_KENNUNG",
                InfoNumber: new InfoNumber(0x01, 0x48),
                Type: ParameterType.Enum,
                NameEnglish: "DeviceIdentifier",
                DeviceType: null, // Universal parameter
                HomeAssistantComponent: "sensor")
        };
        _parameterRegistryMock.Setup(x => x.Parameters).Returns(parameters.AsReadOnly());
        
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
                UniqueIdFormat = "{deviceType}_{deviceId}_{parameterName}",
                ObjectIdFormat = "{deviceType}_{deviceId}_{parameterName}",
                ObjectIdentifierFormat = "rocon_{deviceType}_{deviceId}_{objectId}",
                DeviceIdentifierFormat = "{deviceType}_{deviceId}",
                DeviceNameFormat = "Device {deviceType} {deviceId}",
                DeviceManufacturer = "Test",
                DeviceModel = "Test Model",
                DeviceSoftwareVersion = "1.0",
                ValueTemplate = "{{ value_json.value }}"
            }
        };
        _options = Options.Create(_mqttOptions);
    }

    private static CommunicationProfile CreateTestProfile(string name)
    {
        return new CommunicationProfile
        {
            Name = name,
            Get = new CommunicationCommand { CanId = 0x100, Bytes = [0x00, 0x00, 0x00] },
            Set = new CommunicationCommand { CanId = 0x100, Bytes = [0x00, 0x00, 0x00] },
            Answer = new CommunicationCommand { CanId = 0x200, Bytes = [0x00, 0x00, 0x00] }
        };
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

    [Fact]
    public async Task HomeAssistantDiscovery_ShouldNotPublishDuplicates_WhenParameterRestrictedByDeviceType()
    {
        // Arrange: Setup parameter registry FIRST with OutdoorTemperature restricted to HeatGenerator
        var parameters = new Dictionary<InfoNumber, ParameterDefinition>
        {
            [new InfoNumber(0x00, 0x0C)] = new ParameterDefinition(
                OriginalName: "cAUSSENTEMP",
                InfoNumber: new InfoNumber(0x00, 0x0C),
                Type: ParameterType.Float,
                NameEnglish: "OutdoorTemperature",
                DeviceType: DeviceType.HeatGenerator, // Restricted to HeatGenerator only
                HomeAssistantComponent: "sensor",
                UnitOfMeasurement: "°C",
                DeviceClass: "temperature",
                StateClass: "measurement"),
            [new InfoNumber(0x01, 0x48)] = new ParameterDefinition(
                OriginalName: "cGERAETE_KENNUNG",
                InfoNumber: new InfoNumber(0x01, 0x48),
                Type: ParameterType.Enum,
                NameEnglish: "DeviceIdentifier",
                DeviceType: null, // Universal parameter
                HomeAssistantComponent: "sensor")
        };
        _parameterRegistryMock.Setup(x => x.Parameters).Returns(parameters.AsReadOnly());
        
        // Setup two devices (HG1 and HC1) and a parameter that only works with HeatGenerator
        _mqttOptions.Devices.Clear();
        _mqttOptions.Devices.Add("HG1");
        _mqttOptions.Devices.Add("HC1");
        _mqttOptions.Parameters.Clear();
        _mqttOptions.Parameters.Add("OutdoorTemperature");
        _mqttOptions.HomeAssistant.Discovery = true;
        _mqttOptions.PollingIntervalSeconds = 3600; // Long interval to prevent multiple polls
        
        
        
        // Setup device identifier responses for both devices
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "HG1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "12345678"));
        
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "HC1",
                "DeviceIdentifier",
                CommandType.Get,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "87654321"));
        
        // Track all MQTT publish calls
        var publishedTopics = new List<string>();
        _mqttServiceMock
            .Setup(x => x.PublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .Callback<string, string, CancellationToken, bool>((topic, payload, ct, retain) =>
            {
                publishedTopics.Add(topic);
            })
            .Returns(Task.CompletedTask);

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object,
            _mqttServiceMock.Object,
            _options,
            _parameterRegistryMock.Object,
            _resilienceFactory,
            _loggerMock.Object
        );

        using var cts = new CancellationTokenSource();
        
        // Act: Start publisher and wait for discovery to complete
        await publisher.StartAsync(cts.Token);
        await Task.Delay(TimeSpan.FromSeconds(4)); // Wait for discovery phase (2s delay + identifier query + discovery)
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        // Assert: Verify discovery config was published only once for OutdoorTemperature (for HG1 only)
        var discoveryTopics = publishedTopics
            .Where(t => t.Contains("/config") && t.Contains("outdoor_temperature"))
            .ToList();
        
        // Debug output if test fails
        if (discoveryTopics.Count != 1)
        {
            var allTopics = string.Join("\n", publishedTopics);
            Assert.Fail($"Expected 1 discovery topic for OutdoorTemperature, but found {discoveryTopics.Count}.\nAll published topics:\n{allTopics}");
        }
        
        // Should have exactly 1 discovery topic for OutdoorTemperature (for HG1 with deviceId 12345678)
        Assert.Single(discoveryTopics);
        
        // Verify the topic contains HG1's device ID (heatgenerator + 12345678)
        var discoveryTopic = discoveryTopics[0];
        Assert.Contains("heatgenerator", discoveryTopic);
        Assert.Contains("12345678", discoveryTopic);
        
        // Verify the topic does NOT contain HC1's device ID (87654321)
        Assert.DoesNotContain("heatingcircuit", discoveryTopic);
        Assert.All(publishedTopics, topic => 
        {
            // If topic mentions outdoor_temperature, it should NOT mention HC1's device
            if (topic.Contains("outdoor_temperature"))
            {
                Assert.DoesNotContain("87654321", topic);
            }
        });
    }
    
    [Fact]
    public async Task HomeAssistantDiscovery_UniversalParameter_ShouldPublishForAllDeviceTypes()
    {
        // Arrange: Setup DeviceIdentifier as universal parameter (no deviceType restriction)
        var parameters = new Dictionary<InfoNumber, ParameterDefinition>
        {
            [new InfoNumber(0x01, 0x48)] = new ParameterDefinition(
                OriginalName: "cGERAETE_KENNUNG",
                InfoNumber: new InfoNumber(0x01, 0x48),
                Type: ParameterType.Enum,
                NameEnglish: "DeviceIdentifier",
                DeviceType: null, // Universal parameter - no restriction
                HomeAssistantComponent: "sensor")
        };
        _parameterRegistryMock.Setup(x => x.Parameters).Returns(parameters.AsReadOnly());
        
        // Setup three different device types
        _mqttOptions.Devices.Clear();
        _mqttOptions.Devices.Add("HG1");
        _mqttOptions.Devices.Add("HC1");
        _mqttOptions.Devices.Add("HCM1");
        _mqttOptions.Parameters.Clear();
        _mqttOptions.Parameters.Add("DeviceIdentifier");
        _mqttOptions.HomeAssistant.Discovery = true;
        _mqttOptions.PollingIntervalSeconds = 3600;
        
        // Setup device identifier responses for all three devices
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "HG1", "DeviceIdentifier", CommandType.Get, null,
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "ID-HG1"));
        
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "HC1", "DeviceIdentifier", CommandType.Get, null,
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "ID-HC1"));
        
        _canServiceMock
            .Setup(x => x.SendRequestAndWaitForResponseAsync(
                "HCM1", "DeviceIdentifier", CommandType.Get, null,
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDecodedParameter("DeviceIdentifier", "ID-HCM1"));
        
        // Track all MQTT publish calls
        var publishedTopics = new List<string>();
        _mqttServiceMock
            .Setup(x => x.PublishAsync(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Callback<string, string, CancellationToken, bool>((topic, payload, ct, retain) =>
            {
                publishedTopics.Add(topic);
            })
            .Returns(Task.CompletedTask);

        var publisher = new RoconMqttPublisher(
            _canServiceMock.Object, _mqttServiceMock.Object, _options,
            _parameterRegistryMock.Object, _resilienceFactory, _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        
        // Act
        await publisher.StartAsync(cts.Token);
        await Task.Delay(TimeSpan.FromSeconds(4));
        await cts.CancelAsync();
        await publisher.StopAsync(CancellationToken.None);

        // Assert: DeviceIdentifier discovery should be published for ALL three device types
        var discoveryTopics = publishedTopics
            .Where(t => t.Contains("/config") && t.Contains("device_identifier"))
            .ToList();
        
        if (discoveryTopics.Count != 3)
        {
            var allTopics = string.Join("\n", publishedTopics);
            Assert.Fail($"Expected 3 discovery topics for DeviceIdentifier (one per device type), but found {discoveryTopics.Count}.\nAll topics:\n{allTopics}");
        }
        
        Assert.Equal(3, discoveryTopics.Count);
        
        // Verify one topic for each device type
        Assert.Contains(discoveryTopics, t => t.Contains("heatgenerator") && t.Contains("id-hg1"));
        Assert.Contains(discoveryTopics, t => t.Contains("heatingcircuit") && t.Contains("id-hc1"));
        Assert.Contains(discoveryTopics, t => t.Contains("heatingcircuitmodule") && t.Contains("id-hcm1"));
    }
}



