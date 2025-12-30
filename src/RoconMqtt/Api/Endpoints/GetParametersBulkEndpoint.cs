using FastEndpoints;
using Microsoft.Extensions.Options;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Request to query multiple parameter values from a device
/// </summary>
public sealed class GetParametersBulkRequest
{
    /// <summary>
    /// Device name (e.g., HG1, HC1, HCM1)
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// List of parameter names to query
    /// </summary>
    public List<string> ParameterNames { get; set; } = [];

    /// <summary>
    /// Timeout in milliseconds to wait for each response (default: 1000)
    /// </summary>
    public int TimeoutMs { get; set; } = 1000;
}

/// <summary>
/// Response containing multiple parameter values
/// </summary>
public sealed class ParametersBulkResponse
{
    /// <summary>
    /// Successfully retrieved parameters
    /// </summary>
    public List<ParameterResponse> Results { get; set; } = [];

    /// <summary>
    /// Failed parameter queries with error messages
    /// </summary>
    public List<ParameterError> Errors { get; set; } = [];
}

/// <summary>
/// Error information for a failed parameter query
/// </summary>
public sealed class ParameterError
{
    /// <summary>
    /// Parameter name that failed
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Endpoint to query multiple parameter values from a device
/// </summary>
public sealed class GetParametersBulkEndpoint(ICanService canService, IOptions<MqttOptions> mqttOptions) : Endpoint<GetParametersBulkRequest, ParametersBulkResponse>
{
    private readonly ICanService _canService = canService;
    private readonly MqttOptions _mqttOptions = mqttOptions.Value;

    public override void Configure()
    {
        Post("/parameters/get-bulk");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Query multiple parameter values from a device";
            s.Description = "Sends GET requests to the specified device to read multiple parameter values in one call";
            s.Response<ParametersBulkResponse>(200, "Successfully processed bulk request (check individual results/errors)");
            s.Response(400, "Invalid request (device not found or no parameters specified)");
            
            // Use configuration values for the example
            s.ExampleRequest = new GetParametersBulkRequest
            {
                DeviceName = _mqttOptions.Devices.FirstOrDefault() ?? "HG1",
                ParameterNames = _mqttOptions.Parameters.Count > 0 
                    ? _mqttOptions.Parameters 
                    : [CanParameterRegistry.Parameters.First().Value.Name],
                TimeoutMs = (int)TimeSpan.FromSeconds(_mqttOptions.ResponseTimeoutSeconds).TotalMilliseconds
            };
        });
    }

    public override async Task HandleAsync(GetParametersBulkRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceName))
        {
            ThrowError("DeviceName is required");
        }

        if (req.ParameterNames == null || req.ParameterNames.Count == 0)
        {
            ThrowError("At least one parameter name must be specified");
        }

        var response = new ParametersBulkResponse();

        foreach (var parameterName in req.ParameterNames)
        {
            if (ct.IsCancellationRequested)
                break;

            try
            {
                var result = await _canService.SendRequestAndWaitForResponseAsync(
                    req.DeviceName,
                    parameterName,
                    CommandType.Get,
                    null,
                    req.TimeoutMs,
                    ct);

                response.Results.Add(new ParameterResponse
                {
                    Name = result.Name,
                    Value = result.Value,
                    Metadata = new ParameterMetadata
                    {
                        Type = result.Definition.Type.ToString(),
                        Min = result.Definition.Min,
                        Max = result.Definition.Max,
                        Default = result.Definition.Default,
                        Writeable = result.Definition.Writeable,
                        Factor = result.Definition.Factor,
                        BigEndian = result.Definition.BigEndian,
                        InfoNumberHigh = result.Definition.InfoNumber.High,
                        InfoNumberLow = result.Definition.InfoNumber.Low,
                    }
                });
            }
            catch (ArgumentException ex)
            {
                response.Errors.Add(new ParameterError
                {
                    ParameterName = parameterName,
                    ErrorMessage = ex.Message
                });
            }
            catch (TimeoutException)
            {
                response.Errors.Add(new ParameterError
                {
                    ParameterName = parameterName,
                    ErrorMessage = "Timeout waiting for response from device"
                });
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ParameterError
                {
                    ParameterName = parameterName,
                    ErrorMessage = $"Unexpected error: {ex.Message}"
                });
            }
        }

        await Send.OkAsync(response, ct);
    }
}
