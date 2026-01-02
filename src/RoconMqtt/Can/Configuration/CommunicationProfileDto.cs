using System.ComponentModel.DataAnnotations;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can.Configuration;

/// <summary>
/// DTO for communication profile used in configuration files (appsettings.json)
/// </summary>
public class CommunicationProfileDto
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public CommunicationCommandDto Get { get; set; } = null!;
    
    [Required]
    public CommunicationCommandDto Set { get; set; } = null!;
    
    [Required]
    public CommunicationCommandDto Answer { get; set; } = null!;

    /// <summary>
    /// Converts DTO to domain model
    /// </summary>
    public CommunicationProfile ToCommunicationProfile()
    {
        return new CommunicationProfile
        {
            Name = Name,
            Get = Get.ToCommunicationCommand(),
            Set = Set.ToCommunicationCommand(),
            Answer = Answer.ToCommunicationCommand()
        };
    }

    /// <summary>
    /// Creates DTO from domain model
    /// </summary>
    public static CommunicationProfileDto FromCommunicationProfile(CommunicationProfile profile)
    {
        return new CommunicationProfileDto
        {
            Name = profile.Name,
            Get = CommunicationCommandDto.FromCommunicationCommand(profile.Get),
            Set = CommunicationCommandDto.FromCommunicationCommand(profile.Set),
            Answer = CommunicationCommandDto.FromCommunicationCommand(profile.Answer)
        };
    }
}
