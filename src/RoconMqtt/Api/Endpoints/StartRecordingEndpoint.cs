using Microsoft.AspNetCore.Mvc;
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
                return Results.BadRequest(new
                {
                    error = "Recording already in progress. Stop the current recording before starting a new one."
                });
            }

            var filterInfo = options?.CanIdFilter != null && options.CanIdFilter.Count > 0
                ? string.Join(", ", options.CanIdFilter)
                : "all frames";

            return Results.Ok(new
            {
                status = "recording_started",
                message = "CAN bus recording started successfully",
                filter = filterInfo,
                description = options?.Description
            });
        })
        .WithName("StartCanRecording")
        .WithSummary("Start CAN bus recording")
        .WithDescription(
            "Starts recording all CAN bus traffic. Optionally filter by CAN IDs. " +
            "Only one recording can be active at a time.")
        .Produces(200)
        .Produces(400);
    }
}
