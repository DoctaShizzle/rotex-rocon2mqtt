using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

/// <summary>
/// Interface for CAN parameter registry providing access to parameters and devices
/// </summary>
public interface ICanParameterRegistry
{
    /// <summary>
    /// Gets all parameter definitions indexed by InfoNumber
    /// </summary>
    IReadOnlyDictionary<InfoNumber, ParameterDefinition> Parameters { get; }

    /// <summary>
    /// Gets all heat generator devices (HG1-HG8)
    /// </summary>
    IReadOnlyList<CanDevice> HeatGenerators { get; }

    /// <summary>
    /// Gets all heating circuit devices (HC1-HC16)
    /// </summary>
    IReadOnlyList<CanDevice> HeatingCircuits { get; }

    /// <summary>
    /// Gets all heating circuit module devices (HCM1-HCM16)
    /// </summary>
    IReadOnlyList<CanDevice> HeatingCircuitModules { get; }

    /// <summary>
    /// Determines the subsystem type for a given parameter based on its InfoNumber.High byte
    /// </summary>
    DeviceType? GetSubsystemForParameter(InfoNumber infoNumber);

    /// <summary>
    /// Gets a heat generator by name
    /// </summary>
    CanDevice? GetHeatGenerator(string name);

    /// <summary>
    /// Gets a heating circuit by name
    /// </summary>
    CanDevice? GetHeatingCircuit(string name);

    /// <summary>
    /// Gets a heating circuit module by name
    /// </summary>
    CanDevice? GetHeatingCircuitModule(string name);

    /// <summary>
    /// Gets a device by name, searching all device types
    /// </summary>
    CanDevice? GetDevice(string name);
}
