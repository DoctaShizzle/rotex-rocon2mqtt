using System.ComponentModel.DataAnnotations;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Can.Configuration;

/// <summary>
/// DTO for communication command used in configuration files (appsettings.json)
/// </summary>
public class CommunicationCommandDto
{
    [Required]
    public string CanId { get; set; } = null!;
    
    [Required]
    public string[] Bytes { get; set; } = null!;

    /// <summary>
    /// Converts DTO to domain model
    /// </summary>
    public CommunicationCommand ToCommunicationCommand()
    {
        var canId = Convert.ToUInt32(CanId, 16);
        var bytes = new byte[Bytes.Length];
        
        for (int i = 0; i < Bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(Bytes[i], 16);
        }
        
        return new CommunicationCommand
        {
            CanId = canId,
            Bytes = bytes
        };
    }

    /// <summary>
    /// Creates DTO from domain model
    /// </summary>
    public static CommunicationCommandDto FromCommunicationCommand(CommunicationCommand command)
    {
        return new CommunicationCommandDto
        {
            CanId = $"0x{command.CanId:X}",
            Bytes = command.Bytes.Select(b => $"0x{b:X2}").ToArray()
        };
    }
}
