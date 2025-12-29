namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents a CAN bus frame with identifier and data payload.
/// </summary>
/// <param name="Id">CAN identifier (11-bit or 29-bit)</param>
/// <param name="Data">Frame data payload (typically 7-8 bytes for this application)</param>
public record CanFrame(uint Id, byte[] Data);