namespace RoconMqtt;

using FastEndpoints;
using RoconMqtt.Api.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Fix Linux ARM64 STJ source-gen issue for FastEndpoints
/// </summary>
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(GetParameterRequest))]
[JsonSerializable(typeof(SetParameterRequest))]
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
