using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to list all available parameters
/// </summary>
public static class GetParametersEndpoint
{
    public static RouteHandlerBuilder MapGetParametersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/parameters", () =>
        {
            var parameters = CanParameterRegistry.Parameters.Values
                .Select(p => p.ToParameterInfo())
                .ToList();

            return TypedResults.Ok(new ParametersResponse { Parameters = parameters });
        })
        .WithName("GetParameters")
        .WithSummary("List all available parameters")
        .WithDescription("Returns a list of all available parameters (~4800 parameters from the registry)")
        .Produces<ParametersResponse>(StatusCodes.Status200OK);
    }
}
