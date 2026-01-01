using RoconMqtt.Api.Endpoints;

namespace RoconMqtt.Api;

public static class MinimalApiEndpoints
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapGetDevicesEndpoint();
        api.MapGetParametersEndpoint();
        api.MapGetParameterMetadataEndpoint();
        api.MapGetParameterEndpoint();
        api.MapSetParameterEndpoint();
        api.MapGetParametersBulkEndpoint();
        api.MapRawParameterEndpoint();
    }
}
