using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to query a parameter value from a device
/// </summary>
public static class GetParameterEndpoint
{
    public static RouteHandlerBuilder MapGetParameterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/parameters/get", async Task<Results<Ok<ParameterResponse>, BadRequest<ProblemDetails>, StatusCodeHttpResult>> (
            [FromQuery] string deviceName,
            [FromQuery] string parameterName,
            [FromQuery] int timeoutMs = 30000,
            [FromServices] ICanService canService = null!,
            CancellationToken ct = default) =>
        {
            try
            {
                var result = await canService.SendRequestAndWaitForResponseAsync(
                    deviceName,
                    parameterName,
                    CommandType.Get,
                    null,
                    timeoutMs,
                    ct);

                var response = result.ToParameterResponse();
                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request",
                    Detail = ex.Message
                });
            }
            catch (TimeoutException)
            {
                return TypedResults.StatusCode(StatusCodes.Status408RequestTimeout);
            }
        })
        .WithName("GetParameter")
        .WithSummary("Query a parameter value from a device")
        .WithDescription("Sends a GET request to the specified device to read a parameter value. Example: deviceName=HG1&parameterName=OutdoorTemperature&timeoutMs=30000")
        .Produces<ParameterResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status408RequestTimeout);
    }
}
