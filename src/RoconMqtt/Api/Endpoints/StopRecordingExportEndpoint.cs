using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

public static class StopRecordingExportEndpoint
{
    public static void MapStopRecordingExportEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/can/recording/stop/export", async (
            [FromServices] CanRecordingService recordingService) =>
        {
            var result = await recordingService.StopRecordingAsync();

            if (result == null)
            {
                var errorResponse = new ApiErrorResponse
                {
                    Error = "No active recording to stop"
                };
                return Results.BadRequest(errorResponse);
            }

            var candumpOutput = result.ToCandumpFormat();

            return Results.Content(candumpOutput, "text/plain", System.Text.Encoding.UTF8);
        })
        .WithName("StopCanRecordingExport")
        .WithSummary("Stop CAN bus recording and export as candump format")
        .WithDescription(
            "Stops the active CAN bus recording and returns captured frames in candump text format. " +
            "Returns error if no recording is active.")
        .Produces<string>(200, "text/plain")
        .Produces<ApiErrorResponse>(400);
    }
}
