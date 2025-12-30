using FastEndpoints;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to list all available devices
/// </summary>
public sealed class GetDevicesEndpoint : EndpointWithoutRequest<DevicesResponse>
{
    public override void Configure()
    {
        Get("/devices");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all available devices";
            s.Description = "Returns a list of all available devices (HG1-HG8, HC1-HC16, HCM1-HCM16)";
            s.Response<DevicesResponse>(200, "Successfully retrieved device list");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var devices = new List<DeviceInfo>();

        // Add all heat generators
        foreach (var device in CanParameterRegistry.HeatGenerators)
        {
            devices.Add(new DeviceInfo
            {
                Name = device.Name,
                Type = device.Type.ToString()
            });
        }

        // Add all heating circuits
        foreach (var device in CanParameterRegistry.HeatingCircuits)
        {
            devices.Add(new DeviceInfo
            {
                Name = device.Name,
                Type = device.Type.ToString()
            });
        }

        // Add all heating circuit modules
        foreach (var device in CanParameterRegistry.HeatingCircuitModules)
        {
            devices.Add(new DeviceInfo
            {
                Name = device.Name,
                Type = device.Type.ToString()
            });
        }

        var response = new DevicesResponse { Devices = devices };
        await Send.OkAsync(response, ct);
    }
}
