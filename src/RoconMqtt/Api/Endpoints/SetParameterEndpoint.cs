using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to set a parameter value on a device
/// </summary>
public static class SetParameterEndpoint
{
    public static RouteHandlerBuilder MapSetParameterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/parameters/set", async Task<Results<Ok<ParameterResponse>, BadRequest<ProblemDetails>, StatusCodeHttpResult>> (
            [FromBody] SetParameterRequest request,
            [FromServices] ICanService canService,
            CancellationToken ct) =>
        {
            try
            {
                var result = await canService.SendRequestAndWaitForResponseAsync(
                    request.DeviceName,
                    request.ParameterName,
                    CommandType.Set,
                    request.Value,
                    request.TimeoutMs,
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
        .WithName("SetParameter")
        .WithSummary("Set a parameter value on a device")
        .WithDescription("Sends a SET request to the specified device to write a parameter value")
        .Produces<ParameterResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status408RequestTimeout);
    }
}
