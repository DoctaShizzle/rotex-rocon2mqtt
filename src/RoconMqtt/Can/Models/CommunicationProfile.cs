namespace RoconMqtt.Can.Models;

/// <summary>
/// Defines the communication parameters for a device on the CAN bus
/// </summary>
public class CommunicationProfile
{
    /// <summary>
    /// Device name (e.g., "HG1", "HC1", "HCM1")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// CAN identifier and bytes for GET requests
    /// Format: [CanId, Byte0, Byte1, Byte2]
    /// </summary>
    public required CommunicationCommand Get { get; set; }

    /// <summary>
    /// CAN identifier and bytes for SET requests
    /// Format: [CanId, Byte0, Byte1, Byte2]
    /// </summary>
    public required CommunicationCommand Set { get; set; }

    /// <summary>
    /// Expected answer CAN identifier and header bytes
    /// Format: [CanId, Header0, Header1, Header2]
    /// </summary>
    public required CommunicationCommand Answer { get; set; }
}