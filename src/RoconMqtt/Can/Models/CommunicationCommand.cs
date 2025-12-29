namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents a single CAN communication command with identifier and data bytes
/// </summary>
public class CommunicationCommand
{
    /// <summary>
    /// CAN identifier (11-bit or 29-bit)
    /// </summary>
    public uint CanId { get; set; }

    /// <summary>
    /// Byte values for the command
    /// </summary>
    public required byte[] Bytes { get; set; }

    /// <summary>
    /// Parses a communication command from array of hex strings
    /// </summary>
    /// <param name="parts">Array with first element as CanId, rest as bytes</param>
    public static CommunicationCommand Parse(string[] parts)
    {
        if (parts == null || parts.Length < 1)
            throw new ArgumentException("Parts must contain at least the CanId");

        var canId = Convert.ToUInt32(parts[0], 16);
        var bytes = new byte[parts.Length - 1];

        for (int i = 1; i < parts.Length; i++)
        {
            bytes[i - 1] = Convert.ToByte(parts[i], 16);
        }

        return new CommunicationCommand
        {
            CanId = canId,
            Bytes = bytes
        };
    }
}