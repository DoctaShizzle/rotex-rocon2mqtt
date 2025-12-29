namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents a unique parameter identifier used in CAN frames.
/// The High byte determines which subsystem the parameter belongs to:
/// - 0x00-0x0F: Heat Generator (HG)
/// - 0x14-0x1F: Heating Circuit (HC)
/// - 0x17-0x1B: Heating Circuit Module (HCM) - takes precedence over HC
/// </summary>
/// <param name="High">High byte of the InfoNumber (determines subsystem routing)</param>
/// <param name="Low">Low byte of the InfoNumber (parameter identifier within subsystem)</param>
public record InfoNumber(byte High, byte Low);