using FastEndpoints;
using Microsoft.Extensions.Options;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Api.Endpoints;

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

                response.Results.Add(result.ToParameterResponse());
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
