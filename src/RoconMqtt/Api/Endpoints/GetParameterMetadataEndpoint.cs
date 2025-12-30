using FastEndpoints;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;

namespace RoconMqtt.Api.Endpoints;

/// <summary>
/// Request to get parameter metadata
/// </summary>
public sealed class GetParameterMetadataRequest
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;
}

/// <summary>
/// Endpoint to get metadata for a specific parameter
/// </summary>
public sealed class GetParameterMetadataEndpoint : Endpoint<GetParameterMetadataRequest, ParameterInfo>
{
    public override void Configure()
    {
        Get("/parameters/{ParameterName}/metadata");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get metadata for a specific parameter";
            s.Description = "Returns detailed information about a parameter including type, range, and writability";
            s.Response<ParameterInfo>(200, "Successfully retrieved parameter metadata");
            s.Response(404, "Parameter not found");
        });
    }

    public override async Task HandleAsync(GetParameterMetadataRequest req, CancellationToken ct)
    {
        var parameter = CanParameterRegistry.Parameters.Values
            .FirstOrDefault(p => p.Name.Equals(req.ParameterName, StringComparison.OrdinalIgnoreCase));

        if (parameter == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new ParameterInfo
        {
            Name = parameter.Name,
            Metadata = new ParameterMetadata
            {
                Type = parameter.Type.ToString(),
                Min = parameter.Min,
                Max = parameter.Max,
                Default = parameter.Default,
                Writeable = parameter.Writeable,
                Factor = parameter.Factor
            }
        };

        await Send.OkAsync(response, ct);
    }
}
