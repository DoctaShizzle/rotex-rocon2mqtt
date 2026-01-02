using RoconMqtt.Can.Models;

namespace RoconMqtt.Can.Configuration;

/// <summary>
/// DTO for CAN device used in configuration files (appsettings.json)
/// </summary>
public class CanDeviceDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required CommunicationProfileDto Profile { get; set; }

    /// <summary>
    /// Converts DTO to domain model
    /// </summary>
    public CanDevice ToCanDevice()
    {
        var deviceType = Enum.Parse<DeviceType>(Type, ignoreCase: true);
        
        return new CanDevice
        {
            Name = Name,
            Type = deviceType,
            Profile = Profile.ToCommunicationProfile()
        };
    }

    /// <summary>
    /// Creates DTO from domain model
    /// </summary>
    public static CanDeviceDto FromCanDevice(CanDevice device)
    {
        return new CanDeviceDto
        {
            Name = device.Name,
            Type = device.Type.ToString(),
            Profile = CommunicationProfileDto.FromCommunicationProfile(device.Profile)
        };
    }
}
