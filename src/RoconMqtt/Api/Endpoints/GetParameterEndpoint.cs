using FastEndpoints;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to query a parameter value from a device
/// </summary>
public sealed class GetParameterEndpoint(ICanService canService) : Endpoint<GetParameterRequest, ParameterResponse>
{
    private readonly ICanService _canService = canService;

    public override void Configure()
    {
        Get("/parameters/get");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Query a parameter value from a device";
            s.Description = "Sends a GET request to the specified device to read a parameter value";
            s.Response<ParameterResponse>(200, "Successfully retrieved parameter value");
            s.Response(400, "Invalid request (device or parameter not found)");
            s.Response(408, "Timeout waiting for response from device");
        });
    }

    public override async Task HandleAsync(GetParameterRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _canService.SendRequestAndWaitForResponseAsync(
                req.DeviceName,
                req.ParameterName,
                CommandType.Get,
                null,
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
