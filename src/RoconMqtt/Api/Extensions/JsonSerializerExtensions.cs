using System.Text.Json;
using System.Text.Json.Nodes;

namespace RoconMqtt.Api.Extensions;

/// <summary>
/// Extension methods for JsonSerializer that use ApiJsonContext
/// </summary>
public static class JsonSerializerExtensions
{
    /// <summary>
    /// Serializes a value to a JsonNode using the ApiJsonContext for source-generated serialization
    /// </summary>
    public static JsonNode? SerializeToNodeWithApiJsonContext<TValue>(this TValue value)
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolverChain = { ApiJsonContext.Default }
        };
        return JsonSerializer.SerializeToNode(value, options);
    }
}
