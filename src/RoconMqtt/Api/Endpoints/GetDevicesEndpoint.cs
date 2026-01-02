using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to list all available devices
/// </summary>
public static class GetDevicesEndpoint
{
    public static RouteHandlerBuilder MapGetDevicesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/devices", (ICanParameterRegistry registry) =>
        {
            var devices = new List<DeviceInfo>();

            foreach (var device in registry.HeatGenerators)
            {
                devices.Add(device.ToDeviceInfo());
            }

            foreach (var device in registry.HeatingCircuits)
            {
                devices.Add(device.ToDeviceInfo());
            }

            foreach (var device in registry.HeatingCircuitModules)
            {
                devices.Add(device.ToDeviceInfo());
            }

            return TypedResults.Ok(new DevicesResponse { Devices = devices });
        })
        .WithName("GetDevices")
        .WithSummary("List all available devices")
        .WithDescription("Returns a list of all available devices (HG1-HG8, HC1-HC16, HCM1-HCM16)")
        .Produces<DevicesResponse>(StatusCodes.Status200OK);
    }
}
