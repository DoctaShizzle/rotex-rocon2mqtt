using FastEndpoints;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to list all available parameters
/// </summary>
public sealed class GetParametersEndpoint : EndpointWithoutRequest<ParametersResponse>
{
    public override void Configure()
    {
        Get("/parameters");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all available parameters";
            s.Description = "Returns a list of all available parameters (~4800 parameters from the registry)";
            s.Response<ParametersResponse>(200, "Successfully retrieved parameter list");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var parameters = CanParameterRegistry.Parameters.Values
            .Select(p => p.ToParameterInfo())
            .ToList();

        var response = new ParametersResponse { Parameters = parameters };
        await Send.OkAsync(response, ct);
    }
}
