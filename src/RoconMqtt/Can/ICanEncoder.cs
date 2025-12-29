using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public interface ICanEncoder
{
    /// <summary>
    /// Encodes a parameter value into a CAN frame using the specified communication command format.
    /// </summary>
    /// <param name="command">Communication command containing the header bytes and CAN ID to use</param>
    /// <param name="info">Parameter InfoNumber</param>
    /// <param name="value">Parameter value to encode</param>
    /// <returns>7-byte CAN frame (for ANSWER) or 8-byte frame (for GET/SET)</returns>
    byte[] Encode(CommunicationCommand command, InfoNumber info, object value);
}
