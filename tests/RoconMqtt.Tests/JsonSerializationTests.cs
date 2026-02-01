using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Models;
using RoconMqtt.Tests.Helpers;
using System.Text.Json;
using Xunit;

namespace RoconMqtt.Tests;

/// <summary>
/// Tests to ensure all types used in JSON serialization are properly registered in ApiJsonContext.
/// These tests are critical for ARM/Linux deployments where reflection-based serialization is not available.
/// </summary>
public class JsonSerializationTests
{
    private readonly JsonSerializerOptions _options;

    public JsonSerializationTests()
    {
        // Use the same options that would be used in production on ARM/Linux
        _options = JsonSerializationTestHelper.GetApiJsonOptions();
    }

    [Fact]
    public void GetParameterRequest_CanSerializeAndDeserialize()
    {
        var request = new GetParameterRequest
        {
            DeviceName = "HG1",
            ParameterName = "cAUSSENTEMP",
            TimeoutMs = 5000
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<GetParameterRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(request.DeviceName, deserialized.DeviceName);
        Assert.Equal(request.ParameterName, deserialized.ParameterName);
        Assert.Equal(request.TimeoutMs, deserialized.TimeoutMs);
    }

    [Fact]
    public void GetParameterMetadataRequest_CanSerializeAndDeserialize()
    {
        var request = new GetParameterMetadataRequest
        {
            ParameterName = "cAUSSENTEMP"
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<GetParameterMetadataRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(request.ParameterName, deserialized.ParameterName);
    }

    [Fact]
    public void SetParameterRequest_CanSerializeAndDeserialize()
    {
        var request = new SetParameterRequest
        {
            DeviceName = "HC1",
            ParameterName = "cEINSTELL_SPEICHERSOLLTEMP",
            Value = 45.5,
            TimeoutMs = 10000
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<SetParameterRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(request.DeviceName, deserialized.DeviceName);
        Assert.Equal(request.ParameterName, deserialized.ParameterName);
        Assert.Equal(request.TimeoutMs, deserialized.TimeoutMs);
    }

    [Fact]
    public void GetParametersBulkRequest_CanSerializeAndDeserialize()
    {
        var request = new GetParametersBulkRequest
        {
            DeviceName = "HG1",
            ParameterNames = ["cAUSSENTEMP", "cVORLAUFTEMP", "cRUECKLAUFTEMP"],
            TimeoutMs = 15000
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<GetParametersBulkRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(request.DeviceName, deserialized.DeviceName);
        Assert.Equal(3, deserialized.ParameterNames.Count);
        Assert.Contains("cAUSSENTEMP", deserialized.ParameterNames);
        Assert.Equal(request.TimeoutMs, deserialized.TimeoutMs);
    }

    [Fact]
    public void RawParameterRequest_CanSerializeAndDeserialize()
    {
        var request = new RawParameterRequest
        {
            DeviceName = "HG1",
            InfoNumberHigh = 0x0A,
            InfoNumberLow = 0x06
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<RawParameterRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(request.DeviceName, deserialized.DeviceName);
        Assert.Equal(request.InfoNumberHigh, deserialized.InfoNumberHigh);
        Assert.Equal(request.InfoNumberLow, deserialized.InfoNumberLow);
    }

    [Fact]
    public void ParameterResponse_CanSerializeAndDeserialize()
    {
        var response = new ParameterResponse
        {
            Name = "cAUSSENTEMP",
            Value = 15.5,
            Metadata = new ParameterMetadata
            {
                Type = "Float",
                UnitOfMeasurement = "°C",
                InfoNumberHigh = 0x0A,
                InfoNumberLow = 0x06
            }
        };

        var json = JsonSerializer.Serialize(response, _options);
        var deserialized = JsonSerializer.Deserialize<ParameterResponse>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(response.Name, deserialized.Name);
        // Value is object type, which deserializes as JsonElement - use helper to extract value
        var actualValue = JsonSerializationTestHelper.GetDoubleValue(deserialized.Value);
        Assert.Equal(15.5, actualValue);
        Assert.Equal(response.Metadata.Type, deserialized.Metadata.Type);
        Assert.Equal(response.Metadata.UnitOfMeasurement, deserialized.Metadata.UnitOfMeasurement);
    }

    [Fact]
    public void ParameterMetadata_CanSerializeAndDeserialize()
    {
        var metadata = new ParameterMetadata
        {
            Type = "Float",
            Min = -50.0,
            Max = 100.0,
            Factor = 10.0,
            BigEndian = false,
            Writeable = false,
            InfoNumberHigh = 0x0A,
            InfoNumberLow = 0x06,
            UnitOfMeasurement = "°C",
            DeviceClass = "temperature",
            StateClass = "measurement",
            HomeAssistantComponent = "sensor"
        };

        var json = JsonSerializer.Serialize(metadata, _options);
        var deserialized = JsonSerializer.Deserialize<ParameterMetadata>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(metadata.Type, deserialized.Type);
        Assert.Equal(metadata.Min, deserialized.Min);
        Assert.Equal(metadata.Max, deserialized.Max);
        Assert.Equal(metadata.Factor, deserialized.Factor);
        Assert.Equal(metadata.UnitOfMeasurement, deserialized.UnitOfMeasurement);
        Assert.Equal(metadata.InfoNumberHigh, deserialized.InfoNumberHigh);
        Assert.Equal(metadata.InfoNumberLow, deserialized.InfoNumberLow);
    }

    [Fact]
    public void DevicesResponse_CanSerializeAndDeserialize()
    {
        var response = new DevicesResponse
        {
            Devices = 
            [
                new DeviceInfo { Name = "HG1", Type = "HeatGenerator" },
                new DeviceInfo { Name = "HC1", Type = "HeatingCircuit" }
            ]
        };

        var json = JsonSerializer.Serialize(response, _options);
        var deserialized = JsonSerializer.Deserialize<DevicesResponse>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized.Devices.Count);
        Assert.Equal("HG1", deserialized.Devices[0].Name);
        Assert.Equal("HC1", deserialized.Devices[1].Name);
    }

    [Fact]
    public void ParametersResponse_CanSerializeAndDeserialize()
    {
        var response = new ParametersResponse
        {
            Parameters = 
            [
                new ParameterInfo 
                { 
                    Name = "cAUSSENTEMP", 
                    OriginalName = "Outside Temperature",
                    Metadata = new ParameterMetadata { Type = "Float", UnitOfMeasurement = "°C" }
                },
                new ParameterInfo 
                { 
                    Name = "cVORLAUFTEMP", 
                    OriginalName = "Flow Temperature",
                    Metadata = new ParameterMetadata { Type = "Float", UnitOfMeasurement = "°C" }
                }
            ]
        };

        var json = JsonSerializer.Serialize(response, _options);
        var deserialized = JsonSerializer.Deserialize<ParametersResponse>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized.Parameters.Count);
        Assert.Equal("cAUSSENTEMP", deserialized.Parameters[0].Name);
        Assert.Equal("cVORLAUFTEMP", deserialized.Parameters[1].Name);
    }

    [Fact]
    public void ParametersBulkResponse_CanSerializeAndDeserialize()
    {
        var response = new ParametersBulkResponse
        {
            Results = 
            [
                new ParameterResponse 
                { 
                    Name = "cAUSSENTEMP", 
                    Value = 15.5,
                    Metadata = new ParameterMetadata 
                    { 
                        Type = "Float",
                        UnitOfMeasurement = "°C"
                    }
                }
            ],
            Errors = 
            [
                new ParameterError 
                { 
                    ParameterName = "cINVALID", 
                    ErrorMessage = "Parameter not found" 
                }
            ]
        };

        var json = JsonSerializer.Serialize(response, _options);
        var deserialized = JsonSerializer.Deserialize<ParametersBulkResponse>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Single(deserialized.Results);
        Assert.Single(deserialized.Errors);
        Assert.Equal("cAUSSENTEMP", deserialized.Results[0].Name);
        Assert.Equal("cINVALID", deserialized.Errors[0].ParameterName);
    }

    [Fact]
    public void MqttPublishingStatusRequest_CanSerializeAndDeserialize()
    {
        var request = new MqttPublishingStatusRequest
        {
            Enabled = true
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<MqttPublishingStatusRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.True(deserialized.Enabled);
    }

    [Fact]
    public void MqttPublishingStatusResponse_CanSerializeAndDeserialize()
    {
        var response = new MqttPublishingStatusResponse
        {
            Enabled = false
        };

        var json = JsonSerializer.Serialize(response, _options);
        var deserialized = JsonSerializer.Deserialize<MqttPublishingStatusResponse>(json, _options);

        Assert.NotNull(deserialized);
        Assert.False(deserialized.Enabled);
    }

    [Fact]
    public void HomeAssistantDiscoveryConfig_CanSerializeAndDeserialize()
    {
        var config = new HomeAssistantDiscoveryConfig
        {
            Name = "Outside Temperature",
            UniqueId = "rocon_hg1_cAUSSENTEMP",
            DefaultEntityId = "sensor.rocon_hg1_cAUSSENTEMP",
            StateTopic = "rocon/HG1/cAUSSENTEMP",
            ValueTemplate = "{{ value_json.value }}",
            UnitOfMeasurement = "°C",
            DeviceClass = "temperature",
            StateClass = "measurement",
            Device = new HomeAssistantDeviceInfo
            {
                Identifiers = ["rocon_hg1"],
                Name = "Rotex Rocon HG1",
                Manufacturer = "Rotex",
                Model = "Rocon G1",
                SwVersion = "1.0.0"
            }
        };

        var json = JsonSerializer.Serialize(config, _options);
        var deserialized = JsonSerializer.Deserialize<HomeAssistantDiscoveryConfig>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(config.Name, deserialized.Name);
        Assert.Equal(config.UniqueId, deserialized.UniqueId);
        Assert.Equal(config.StateTopic, deserialized.StateTopic);
        Assert.Equal(config.Device.Name, deserialized.Device.Name);
        Assert.Equal(config.Device.Manufacturer, deserialized.Device.Manufacturer);
    }

    [Fact]
    public void DecodedParameter_CanSerializeAndDeserialize()
    {
        var paramDef = new ParameterDefinition(
            OriginalName: "cAUSSENTEMP",
            InfoNumber: new InfoNumber(0x0A, 0x06),
            Type: ParameterType.Float,
            Factor: 10.0,
            UnitOfMeasurement: "°C"
        );

        var decoded = new DecodedParameter(
            Name: "cAUSSENTEMP",
            Value: 25.5,
            Definition: paramDef
        );

        var json = JsonSerializer.Serialize(decoded, _options);
        var deserialized = JsonSerializer.Deserialize<DecodedParameter>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(decoded.Name, deserialized.Name);
        // Value is object type, which deserializes as JsonElement - use helper to extract value
        var actualValue = JsonSerializationTestHelper.GetDoubleValue(deserialized.Value);
        Assert.Equal(25.5, actualValue);
        Assert.Equal(decoded.Definition.UnitOfMeasurement, deserialized.Definition.UnitOfMeasurement);
    }

    [Fact]
    public void ParameterDefinition_CanSerializeAndDeserialize()
    {
        var definition = new ParameterDefinition(
            OriginalName: "cAUSSENTEMP",
            InfoNumber: new InfoNumber(0x0A, 0x06),
            Type: ParameterType.Float,
            Factor: 10.0,
            Min: -50.0,
            Max: 100.0,
            Writeable: false,
            UnitOfMeasurement: "°C",
            HomeAssistantComponent: "sensor",
            DeviceClass: "temperature",
            StateClass: "measurement"
        );

        var json = JsonSerializer.Serialize(definition, _options);
        var deserialized = JsonSerializer.Deserialize<ParameterDefinition>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(definition.OriginalName, deserialized.OriginalName);
        Assert.Equal(definition.UnitOfMeasurement, deserialized.UnitOfMeasurement);
        Assert.Equal(definition.Type, deserialized.Type);
        Assert.Equal(definition.Factor, deserialized.Factor);
    }

    [Fact]
    public void InfoNumber_CanSerializeAndDeserialize()
    {
        var infoNumber = new InfoNumber(0x0A, 0x06);

        var json = JsonSerializer.Serialize(infoNumber, _options);
        var deserialized = JsonSerializer.Deserialize<InfoNumber>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(infoNumber.High, deserialized.High);
        Assert.Equal(infoNumber.Low, deserialized.Low);
    }

    [Fact]
    public void ParameterType_CanSerializeAndDeserialize()
    {
        var parameterType = ParameterType.Float;

        var json = JsonSerializer.Serialize(parameterType, _options);
        var deserialized = JsonSerializer.Deserialize<ParameterType>(json, _options);

        Assert.Equal(parameterType, deserialized);
    }

    [Fact]
    public void DictionaryOfIntString_CanSerializeAndDeserialize()
    {
        var dict = new Dictionary<int, string>
        {
            { 0, "Off" },
            { 1, "On" },
            { 2, "Auto" }
        };

        var json = JsonSerializer.Serialize(dict, _options);
        var deserialized = JsonSerializer.Deserialize<Dictionary<int, string>>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(3, deserialized.Count);
        Assert.Equal("Off", deserialized[0]);
        Assert.Equal("On", deserialized[1]);
        Assert.Equal("Auto", deserialized[2]);
    }

    [Fact]
    public void ReadOnlyDictionaryOfIntString_CanSerializeAndDeserialize()
    {
        var dict = new Dictionary<int, string>
        {
            { 0, "Off" },
            { 1, "On" }
        };
        IReadOnlyDictionary<int, string> readOnlyDict = dict;

        var json = JsonSerializer.Serialize(readOnlyDict, _options);
        var deserialized = JsonSerializer.Deserialize<IReadOnlyDictionary<int, string>>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized.Count);
        Assert.Equal("Off", deserialized[0]);
    }

    [Fact]
    public void ListOfString_CanSerializeAndDeserialize()
    {
        var list = new List<string> { "param1", "param2", "param3" };

        var json = JsonSerializer.Serialize(list, _options);
        var deserialized = JsonSerializer.Deserialize<List<string>>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(3, deserialized.Count);
        Assert.Contains("param1", deserialized);
    }

    [Fact]
    public void ProblemDetails_CanSerializeAndDeserialize()
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Not Found",
            Status = 404,
            Detail = "The requested parameter was not found"
        };

        var json = JsonSerializer.Serialize(problemDetails, _options);
        var deserialized = JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(problemDetails.Title, deserialized.Title);
        Assert.Equal(problemDetails.Status, deserialized.Status);
    }

    [Fact]
    public void ValidationProblemDetails_CanSerializeAndDeserialize()
    {
        var validationProblemDetails = new Microsoft.AspNetCore.Mvc.ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400
        };
        validationProblemDetails.Errors.Add("DeviceName", ["The DeviceName field is required."]);

        var json = JsonSerializer.Serialize(validationProblemDetails, _options);
        var deserialized = JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ValidationProblemDetails>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(validationProblemDetails.Title, deserialized.Title);
        Assert.Equal(validationProblemDetails.Status, deserialized.Status);
        Assert.Single(deserialized.Errors);
    }

    [Fact]
    public void SerializationWithoutReflectionResolver_ThrowsException()
    {
        // This test verifies that types NOT in ApiJsonContext will fail without reflection resolver
        var optionsWithoutContext = new JsonSerializerOptions
        {
            TypeInfoResolver = ApiJsonContext.Default
        };

        // Test with a type that IS registered - should work
        var registeredType = new GetParameterRequest { DeviceName = "HG1", ParameterName = "Test" };
        var json = JsonSerializer.Serialize(registeredType, optionsWithoutContext);
        Assert.NotNull(json);

        // Test with a completely custom type NOT in ApiJsonContext - should fail
        var unregisteredType = new UnregisteredTestClass { Value = "test" };
        var exception = Assert.Throws<NotSupportedException>(() => 
            JsonSerializer.Serialize(unregisteredType, optionsWithoutContext));
        
        // Verify the exception is about missing type metadata
        Assert.Contains("JsonTypeInfo", exception.Message);
    }

    private class UnregisteredTestClass
    {
        public string Value { get; set; } = string.Empty;
    }
}
