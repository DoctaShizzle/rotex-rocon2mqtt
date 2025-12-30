using Riok.Mapperly.Abstractions;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Models;

/// <summary>
/// Mapper for converting between domain models and API models using Mapperly
/// </summary>
[Mapper]
public static partial class ApiModelsMapper
{
    /// <summary>
    /// Maps a DecodedParameter to a ParameterResponse
    /// </summary>
    public static partial ParameterResponse ToParameterResponse(this DecodedParameter source);

    /// <summary>
    /// Maps a ParameterDefinition to ParameterMetadata
    /// </summary>
    public static partial ParameterMetadata ToParameterMetadata(this ParameterDefinition source);

    /// <summary>
    /// Maps a ParameterDefinition to ParameterInfo
    /// </summary>
    public static ParameterInfo ToParameterInfo(this ParameterDefinition source)
    {
        var result = source.ToParameterInfoCore();

        result.Metadata.

        return result;
    }

    /// <summary>
    /// Maps a ParameterDefinition to ParameterInfo
    /// </summary>
    private static partial ParameterInfo ToParameterInfoCore(this ParameterDefinition source);

    /// <summary>
    /// Maps a CanDevice to DeviceInfo
    /// </summary>
    [MapperIgnoreSource(nameof(CanDevice.Profile))]
    public static partial DeviceInfo ToDeviceInfo(this CanDevice source);

    /// <summary>
    /// Maps ParameterDefinition.Type (enum) to string for ParameterMetadata
    /// </summary>
    private static string MapParameterTypeToString(ParameterType type) => type.ToString();

    /// <summary>
    /// Maps CanDevice.Type (enum) to string for DeviceInfo
    /// </summary>
    private static string MapDeviceTypeToString(DeviceType type) => type.ToString();
}
