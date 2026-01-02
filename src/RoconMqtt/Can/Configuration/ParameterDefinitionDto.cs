using System.ComponentModel.DataAnnotations;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can.Configuration;

/// <summary>
/// DTO for parameter definition used in configuration files (appsettings.json)
/// </summary>
public class ParameterDefinitionDto
{
    [Required]
    public string OriginalName { get; set; } = null!;
    
    [Required]
    public string InfoNumberHigh { get; set; } = null!;
    
    [Required]
    public string InfoNumberLow { get; set; } = null!;
    
    [Required]
    public string Type { get; set; } = null!;
    
    public double Factor { get; set; } = 1.0;
    public double? TransferThreshold { get; set; }
    public bool Writeable { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double? Default { get; set; }
    public bool Display { get; set; } = true;
    public bool WaterCircuit { get; set; }
    public bool BigEndian { get; set; }
    public Dictionary<int, string>? EnumValues { get; set; }
    public string? DefaultTimeRange { get; set; }
    public string? NameEnglish { get; set; }

    /// <summary>
    /// Converts DTO to domain model
    /// </summary>
    public ParameterDefinition ToParameterDefinition()
    {
        var high = Convert.ToByte(InfoNumberHigh, 16);
        var low = Convert.ToByte(InfoNumberLow, 16);
        var infoNumber = new InfoNumber(high, low);
        
        var parameterType = Enum.Parse<ParameterType>(Type, ignoreCase: true);
        
        return new ParameterDefinition(
            OriginalName: OriginalName,
            InfoNumber: infoNumber,
            Type: parameterType,
            Factor: Factor,
            TransferThreshold: TransferThreshold,
            Writeable: Writeable,
            Min: Min,
            Max: Max,
            Default: Default,
            Display: Display,
            WaterCircuit: WaterCircuit,
            BigEndian: BigEndian,
            EnumValues: EnumValues,
            DefaultTimeRange: DefaultTimeRange,
            NameEnglish: NameEnglish
        );
    }

    /// <summary>
    /// Creates DTO from domain model
    /// </summary>
    public static ParameterDefinitionDto FromParameterDefinition(ParameterDefinition parameter)
    {
        return new ParameterDefinitionDto
        {
            OriginalName = parameter.OriginalName,
            InfoNumberHigh = $"0x{parameter.InfoNumber.High:X2}",
            InfoNumberLow = $"0x{parameter.InfoNumber.Low:X2}",
            Type = parameter.Type.ToString(),
            Factor = parameter.Factor,
            TransferThreshold = parameter.TransferThreshold,
            Writeable = parameter.Writeable,
            Min = parameter.Min,
            Max = parameter.Max,
            Default = parameter.Default,
            Display = parameter.Display,
            WaterCircuit = parameter.WaterCircuit,
            BigEndian = parameter.BigEndian,
            EnumValues = parameter.EnumValues?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            DefaultTimeRange = parameter.DefaultTimeRange,
            NameEnglish = parameter.NameEnglish
        };
    }
}
