using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

public static class StopRecordingEndpoint
{
    public static void MapStopRecordingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/can/recording/stop", async (
            [FromServices] CanRecordingService recordingService) =>
        {
            var result = await recordingService.StopRecordingAsync();

            if (result == null)
            {
                return Results.BadRequest(new
                {
                    error = "No active recording to stop"
                });
            }

            return Results.Ok(result);
        })
        .WithName("StopCanRecording")
        .WithSummary("Stop CAN bus recording and retrieve captured frames")
        .WithDescription(
            "Stops the active CAN bus recording and returns all captured frames in JSON format. " +
            "Returns error if no recording is active.")
        .Produces<CanRecordingResult>(200)
        .Produces(400);
    }
}
