using RoconMqtt.Api.Models;
using RoconMqtt.Mqtt;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoints to control MQTT publishing status
/// </summary>
public static class GetMqttPublishingStatusEndpoint
{
    public static RouteGroupBuilder MapGetMqttPublishingStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/mqtt");

        group.MapGet("/status", (RoconMqttPublisher publisher) =>
        {
            return TypedResults.Ok(new MqttPublishingStatusResponse { Enabled = publisher.Enabled });
        })
        .WithName("GetMqttPublishingStatus")
        .WithSummary("Get MQTT publishing status")
        .WithDescription("Returns whether MQTT publishing is currently enabled or disabled")
        .Produces<MqttPublishingStatusResponse>(StatusCodes.Status200OK);

        return group;
    }
}
