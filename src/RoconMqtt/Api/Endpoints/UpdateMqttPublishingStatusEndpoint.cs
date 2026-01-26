using RoconMqtt.Api.Models;
using RoconMqtt.Mqtt;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoints to control MQTT publishing status
/// </summary>
public static class UpdateMqttPublishingStatusEndpoint
{
    public static RouteGroupBuilder MapUpdateMqttPublishingStatus(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/mqtt");

        group.MapPut("/status", (MqttPublishingStatusRequest request, RoconMqttPublisher publisher) =>
        {
            publisher.Enabled = request.Enabled;
            return TypedResults.Ok(new MqttPublishingStatusResponse { Enabled = publisher.Enabled });
        })
        .WithName("UpdateMqttPublishingStatus")
        .WithSummary("Update MQTT publishing status")
        .WithDescription("Enable or disable MQTT publishing")
        .Produces<MqttPublishingStatusResponse>(StatusCodes.Status200OK);

        return group;
    }
}
