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

    public static ParameterResponse ToParameterResponse(this DecodedParameter source)
    {
        var result = ToParameterResponseCore(source);

        MapParameterMetadata(source.Definition, result.Metadata);

        return result;
    }

    [MapperIgnoreSource(nameof(DecodedParameter.Definition))]
    [MapperIgnoreTarget(nameof(ParameterResponse.Metadata))]
    private static partial ParameterResponse ToParameterResponseCore(DecodedParameter source);

    /// <summary>
    /// Maps a ParameterDefinition to ParameterMetadata
    /// </summary>
    [MapperIgnoreSource(nameof(ParameterDefinition.Name))]
    [MapperIgnoreSource(nameof(ParameterDefinition.NameEnglish))]
    [MapperIgnoreSource(nameof(ParameterDefinition.OriginalName))]
    [MapperIgnoreSource(nameof(ParameterDefinition.TransferThreshold))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Display))]
    [MapperIgnoreSource(nameof(ParameterDefinition.WaterCircuit))]
    [MapperIgnoreSource(nameof(ParameterDefinition.EnumValues))]
    [MapperIgnoreSource(nameof(ParameterDefinition.DefaultTimeRange))]
    public static partial ParameterMetadata ToParameterMetadata(this ParameterDefinition source);

    /// <summary>
    /// Maps a ParameterDefinition to ParameterInfo
    /// </summary>
    public static ParameterInfo ToParameterInfo(this ParameterDefinition source)
    {
        var result = source.ToParameterInfoCore();

        MapParameterMetadata(source, result.Metadata);

        return result;
    }

    private static void MapParameterMetadata(ParameterDefinition source, ParameterMetadata target)
    {
        target.InfoNumberHigh = source.InfoNumber.High;
        target.InfoNumberLow = source.InfoNumber.Low;
        target.Type = source.Type.ToString();
        target.Factor = source.Factor;
        target.Writeable = source.Writeable;
        target.Min = source.Min;
        target.Max = source.Max;
        target.Default = source.Default;
        target.BigEndian = source.BigEndian;
    }

    /// <summary>
    /// Maps a ParameterDefinition to ParameterInfo
    /// </summary>
    [MapperIgnoreSource(nameof(ParameterDefinition.InfoNumber))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Type))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Factor))]
    [MapperIgnoreSource(nameof(ParameterDefinition.TransferThreshold))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Writeable))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Min))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Max))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Default))]
    [MapperIgnoreSource(nameof(ParameterDefinition.Display))]
    [MapperIgnoreSource(nameof(ParameterDefinition.WaterCircuit))]
    [MapperIgnoreSource(nameof(ParameterDefinition.BigEndian))]
    [MapperIgnoreSource(nameof(ParameterDefinition.EnumValues))]
    [MapperIgnoreSource(nameof(ParameterDefinition.DefaultTimeRange))]
    [MapperIgnoreSource(nameof(ParameterDefinition.NameEnglish))]
    [MapperIgnoreTarget(nameof(ParameterInfo.Metadata))]
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
