using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Endpoint to get metadata for a specific parameter
/// </summary>
public static class GetParameterMetadataEndpoint
{
    public static RouteHandlerBuilder MapGetParameterMetadataEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/parameters/{parameterName}/metadata", Results<Ok<ParameterInfo>, NotFound> (
            [FromRoute] string parameterName) =>
        {
            var parameter = CanParameterRegistry.Parameters.Values
                .FirstOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));

            if (parameter == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(parameter.ToParameterInfo());
        })
        .WithName("GetParameterMetadata")
        .WithSummary("Get metadata for a specific parameter")
        .WithDescription("Returns detailed information about a parameter including type, range, and writability")
        .Produces<ParameterInfo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
