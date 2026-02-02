using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

public static class StartRecordingEndpoint
{
    public static void MapStartRecordingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/can/recording/start", async (
            [FromBody] CanRecordingOptions? options,
            [FromServices] CanRecordingService recordingService) =>
        {
            var started = await recordingService.StartRecordingAsync(options);

            if (!started)
            {
                var errorResponse = new ApiErrorResponse
                {
                    Error = "Recording already in progress. Stop the current recording before starting a new one."
                };
                return Results.BadRequest(errorResponse);
            }

            var filterInfo = options?.CanIdFilter != null && options.CanIdFilter.Count > 0
                ? string.Join(", ", options.CanIdFilter)
                : "all frames";

            var response = new CanRecordingStartResponse
            {
                Status = "recording_started",
                Message = "CAN bus recording started successfully",
                Filter = filterInfo,
                Description = options?.Description
            };

            return Results.Ok(response);
        })
        .WithName("StartCanRecording")
        .WithSummary("Start CAN bus recording")
        .WithDescription(
            "Starts recording all CAN bus traffic. Optionally filter by CAN IDs. " +
            "Only one recording can be active at a time.")
        .Produces<CanRecordingStartResponse>(200)
        .Produces<ApiErrorResponse>(400)
        .Produces(400);
    }
}
