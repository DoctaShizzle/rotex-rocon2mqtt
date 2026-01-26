namespace RoconMqtt;

using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Models;
using System.Text.Json.Serialization;

/// <summary>
/// JSON serialization context for Linux ARM64 STJ source-gen
/// </summary>
[JsonSerializable(typeof(GetParameterRequest))]
[JsonSerializable(typeof(GetParameterMetadataRequest))]
[JsonSerializable(typeof(SetParameterRequest))]
[JsonSerializable(typeof(GetParametersBulkRequest))]
[JsonSerializable(typeof(RawParameterRequest))]
[JsonSerializable(typeof(ParameterResponse))]
[JsonSerializable(typeof(ParameterMetadata))]
[JsonSerializable(typeof(DevicesResponse))]
[JsonSerializable(typeof(DeviceInfo))]
[JsonSerializable(typeof(ParametersResponse))]
[JsonSerializable(typeof(ParameterInfo))]
[JsonSerializable(typeof(ParametersBulkResponse))]
[JsonSerializable(typeof(ParameterError))]
[JsonSerializable(typeof(MqttPublishingStatusRequest))]
[JsonSerializable(typeof(MqttPublishingStatusResponse))]
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ValidationProblemDetails))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<ParameterResponse>))]
[JsonSerializable(typeof(List<ParameterError>))]
[JsonSerializable(typeof(List<DeviceInfo>))]
[JsonSerializable(typeof(List<ParameterInfo>))]
[JsonSerializable(typeof(HomeAssistantDiscoveryConfig))]
[JsonSerializable(typeof(HomeAssistantDeviceInfo))]
[JsonSerializable(typeof(DecodedParameter))]
[JsonSerializable(typeof(ParameterDefinition))]
[JsonSerializable(typeof(InfoNumber))]
[JsonSerializable(typeof(ParameterType))]
[JsonSerializable(typeof(Dictionary<int, string>))]
[JsonSerializable(typeof(IReadOnlyDictionary<int, string>))]
public partial class ApiJsonContext : JsonSerializerContext;
