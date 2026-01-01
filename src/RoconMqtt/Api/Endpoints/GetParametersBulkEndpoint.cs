using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to query multiple parameter values from a device
/// </summary>
public static class GetParametersBulkEndpoint
{
    public static RouteHandlerBuilder MapGetParametersBulkEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/parameters/get-bulk", async (
            [FromBody] GetParametersBulkRequest request,
            [FromServices] ICanService canService,
            [FromServices] IOptions<MqttOptions> mqttOptions,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.DeviceName))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request",
                    Detail = "DeviceName is required"
                });
            }

            if (request.ParameterNames == null || request.ParameterNames.Count == 0)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request",
                    Detail = "At least one parameter name must be specified"
                });
            }

            var response = new ParametersBulkResponse();

            foreach (var parameterName in request.ParameterNames)
            {
                if (ct.IsCancellationRequested)
                    break;

                try
                {
                    var result = await canService.SendRequestAndWaitForResponseAsync(
                        request.DeviceName,
                        parameterName,
                        CommandType.Get,
                        null,
                        request.TimeoutMs,
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

            return Results.Ok(response);
        })
        .WithName("GetParametersBulk")
        .WithSummary("Query multiple parameter values from a device")
        .WithDescription("Sends GET requests to the specified device to read multiple parameter values in one call")
        .Produces<ParametersBulkResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .AddOpenApiOperationTransformer(async (operation, context, ct) =>
        {
            var mqttOptions = context.ApplicationServices.GetRequiredService<IOptions<MqttOptions>>().Value;

            // Add request body example
            var exampleRequest = new GetParametersBulkRequest
            {
                DeviceName = "HG1",
                ParameterNames = mqttOptions.Parameters.Count != 0 ? mqttOptions.Parameters : ["OutdoorTemperature"],
                TimeoutMs = 30000
            };

            var mediaType = operation.RequestBody?.Content?.FirstOrDefault().Value;
            if (mediaType != null)
            {
                mediaType.Example = JsonSerializer.SerializeToNode(exampleRequest);
            }
        });
    }
}
