using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Mqtt.Compound;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to list all available parameters
/// </summary>
public static class GetParametersEndpoint
{
    public static RouteHandlerBuilder MapGetParametersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/parameters", (ICanParameterRegistry registry) =>
        {
            var parameters = registry.Parameters.Values
                .Select(p => p.ToParameterInfo())
                .ToList();

            // Add compound parameters
            foreach (var compoundParameterName in CompoundParameterFactory.AvailableCompoundParameters)
            {
                parameters.Add(new ParameterInfo
                {
                    Name = compoundParameterName,
                    OriginalName = compoundParameterName,
                    Metadata = new ParameterMetadata
                    {
                        Type = "Compound",
                        Writeable = false,
                        InfoNumberHigh = 0,
                        InfoNumberLow = 0
                    }
                });
            }

            return TypedResults.Ok(new ParametersResponse { Parameters = parameters });
        })
        .WithName("GetParameters")
        .WithSummary("List all available parameters")
        .WithDescription("Returns a list of all available parameters (~4800 parameters from the registry) plus compound parameters")
        .Produces<ParametersResponse>(StatusCodes.Status200OK);
    }
}
