using FastEndpoints;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to set a parameter value on a device
/// </summary>
public sealed class SetParameterEndpoint(ICanService canService) : Endpoint<SetParameterRequest, ParameterResponse>
{
    private readonly ICanService _canService = canService;

    public override void Configure()
    {
        Post("/parameters/set");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Set a parameter value on a device";
            s.Description = "Sends a SET request to the specified device to write a parameter value";
            s.Response<ParameterResponse>(200, "Successfully set parameter value");
            s.Response(400, "Invalid request (device or parameter not found, or value out of range)");
            s.Response(408, "Timeout waiting for response from device");
        });
    }

    public override async Task HandleAsync(SetParameterRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _canService.SendRequestAndWaitForResponseAsync(
                req.DeviceName,
                req.ParameterName,
                CommandType.Set,
                req.Value,
                req.TimeoutMs,
                ct);

            var response = result.ToParameterResponse();

            await Send.OkAsync(response, ct);
        }
        catch (ArgumentException ex)
        {
            await Send.ErrorsAsync(400, ct);
            ThrowError(ex.Message);
        }
        catch (TimeoutException)
        {
            await Send.ErrorsAsync(408, ct);
            ThrowError("Timeout waiting for response from device");
        }
    }
}
