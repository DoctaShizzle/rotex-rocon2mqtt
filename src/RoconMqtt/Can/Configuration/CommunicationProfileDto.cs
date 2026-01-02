using RoconMqtt.Can.Models;

namespace RoconMqtt.Can.Configuration;

/// <summary>
/// DTO for communication profile used in configuration files (appsettings.json)
/// </summary>
public class CommunicationProfileDto
{
    public required string Name { get; set; }
    public required CommunicationCommandDto Get { get; set; }
    public required CommunicationCommandDto Set { get; set; }
    public required CommunicationCommandDto Answer { get; set; }

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
