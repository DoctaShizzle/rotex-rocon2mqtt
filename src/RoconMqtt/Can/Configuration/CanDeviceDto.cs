using System.ComponentModel.DataAnnotations;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can.Configuration;

/// <summary>
/// DTO for CAN device used in configuration files (appsettings.json)
/// </summary>
public class CanDeviceDto
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Type { get; set; } = null!;
    
    [Required]
    public CommunicationProfileDto Profile { get; set; } = null!;

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
