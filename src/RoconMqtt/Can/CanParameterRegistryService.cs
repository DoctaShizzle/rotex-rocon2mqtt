using Microsoft.Extensions.Options;
using RoconMqtt.Can.Configuration;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

/// <summary>
/// Service that provides access to CAN parameter registry loaded from configuration
/// </summary>
public class CanParameterRegistryService : ICanParameterRegistry
{
    private readonly IReadOnlyDictionary<InfoNumber, ParameterDefinition> _parameters;
    private readonly IReadOnlyList<CanDevice> _heatGenerators;
    private readonly IReadOnlyList<CanDevice> _heatingCircuits;
    private readonly IReadOnlyList<CanDevice> _heatingCircuitModules;

    public CanParameterRegistryService(IOptions<CanRegistryOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        
        var config = options.Value;

        // Convert DTOs to domain models and build dictionary
        _parameters = config.Parameters
            .Select(dto => dto.ToParameterDefinition())
            .ToDictionary(p => p.InfoNumber);

        _heatGenerators = config.HeatGenerators
            .Select(dto => dto.ToCanDevice())
            .ToList();

        _heatingCircuits = config.HeatingCircuits
            .Select(dto => dto.ToCanDevice())
            .ToList();

        _heatingCircuitModules = config.HeatingCircuitModules
            .Select(dto => dto.ToCanDevice())
            .ToList();
    }

    public IReadOnlyDictionary<InfoNumber, ParameterDefinition> Parameters => _parameters;

    public IReadOnlyList<CanDevice> HeatGenerators => _heatGenerators;

    public IReadOnlyList<CanDevice> HeatingCircuits => _heatingCircuits;

    public IReadOnlyList<CanDevice> HeatingCircuitModules => _heatingCircuitModules;

    public DeviceType? GetSubsystemForParameter(InfoNumber infoNumber)
    {
        var high = infoNumber.High;

        // Heat Generator: 0x00–0x0F
        if (high is >= 0x00 and <= 0x0F)
            return DeviceType.HeatGenerator;

        // Heating Circuit Module: 0x17–0x1B (subset of HC)
        if (high is >= 0x17 and <= 0x1B)
            return DeviceType.HeatingCircuitModule;

        // Heating Circuit: 0x14–0x1F (broader range)
        if (high is >= 0x14 and <= 0x1F)
            return DeviceType.HeatingCircuit;

        return null;
    }

    public CanDevice? GetHeatGenerator(string name) =>
        _heatGenerators.FirstOrDefault(d => d.Name == name);

    public CanDevice? GetHeatingCircuit(string name) =>
        _heatingCircuits.FirstOrDefault(d => d.Name == name);

    public CanDevice? GetHeatingCircuitModule(string name) =>
        _heatingCircuitModules.FirstOrDefault(d => d.Name == name);

    public CanDevice? GetDevice(string name)
    {
        return GetHeatGenerator(name)
            ?? GetHeatingCircuit(name)
            ?? GetHeatingCircuitModule(name);
    }
}
