using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using RoconMqtt.Api.Extensions;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to send raw CAN requests using InfoNumber
/// </summary>
public static class RawParameterEndpoint
{
    public static RouteHandlerBuilder MapRawParameterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/parameters/raw", async Task<Results<Ok<ParameterResponse>, BadRequest<ProblemDetails>, StatusCodeHttpResult>> (
            [FromBody] RawParameterRequest request,
            [FromServices] ICanService canService,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.DeviceName))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request",
                    Detail = "DeviceName is required"
                });
            }

            if (!Enum.TryParse<CommandType>(request.CommandType, ignoreCase: true, out var commandType))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request",
                    Detail = $"Invalid CommandType: {request.CommandType}. Valid values are: Get, Set"
                });
            }

            if (commandType == CommandType.Answer)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request",
                    Detail = "Cannot use CommandType 'Answer' - it is only for responses from the controller"
                });
            }

            EncodingHints? encodingHints = null;
            DecodingHints? decodingHints = null;

            if (request.ParameterType != null || request.BigEndian != null || request.Factor != null)
            {
                var paramType = ParameterType.Int;
                if (request.ParameterType != null && !Enum.TryParse<ParameterType>(request.ParameterType, ignoreCase: true, out paramType))
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Invalid request",
                        Detail = $"Invalid ParameterType: {request.ParameterType}. Valid values are: Int, Float, Bool, TimeRange, Enum"
                    });
                }

                encodingHints = new EncodingHints(
                    ParameterType: paramType,
                    BigEndian: request.BigEndian ?? false,
                    Factor: request.Factor ?? 1.0
                );

                decodingHints = new DecodingHints(
                    ParameterName: "UnknownParameter",
                    ParameterType: paramType,
                    BigEndian: request.BigEndian ?? false,
                    Factor: request.Factor ?? 1.0
                );
            }

            try
            {
                var infoNumber = new InfoNumber(request.InfoNumberHigh, request.InfoNumberLow);

                var result = await canService.SendRawRequestAndWaitForResponseAsync(
                    request.DeviceName,
                    infoNumber,
                    commandType,
                    request.Value,
                    request.TimeoutMs,
                    encodingHints,
                    decodingHints,
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
        .WithName("RawParameter")
        .WithSummary("Send a raw parameter request using InfoNumber")
        .WithDescription("Lower-level endpoint that works directly with InfoNumbers. Sends GET/SET request and waits for ANSWER response. Supports unknown parameters via optional hints.")
        .Produces<ParameterResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status408RequestTimeout)
        .AddOpenApiOperationTransformer(async (operation, context, ct) =>
        {
            var exampleRequest = new RawParameterRequest
            {
                DeviceName = "HG1",
                InfoNumberHigh = 0x0A,
                InfoNumberLow = 0x06,
                CommandType = "Get",
                Value = null,
                TimeoutMs = 30000,
                ParameterType = "Int",
                BigEndian = true,
                Factor = 1.0
            };

            var mediaType = operation.RequestBody?.Content?.FirstOrDefault().Value;
            if (mediaType != null)
            {
                mediaType.Example = exampleRequest.SerializeToNodeWithApiJsonContext();
            }
        });
    }
}
