namespace RoconMqtt;

using RoconMqtt.Api.Models;
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
public partial class ApiJsonContext : JsonSerializerContext;
