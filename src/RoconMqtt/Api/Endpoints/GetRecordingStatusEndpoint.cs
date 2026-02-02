using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

public static class GetRecordingStatusEndpoint
{
    public static void MapGetRecordingStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/can/recording/status", (
            [FromServices] CanRecordingService recordingService) =>
        {
            var isRecording = recordingService.IsRecording;

            var response = new CanRecordingStatusResponse
            {
                IsRecording = isRecording,
                Status = isRecording ? "recording" : "idle"
            };

            return Results.Ok(response);
        })
        .WithName("GetCanRecordingStatus")
        .WithSummary("Get CAN recording status")
        .WithDescription("Check if CAN bus recording is currently active")
        .Produces<CanRecordingStatusResponse>(200);
    }
}
