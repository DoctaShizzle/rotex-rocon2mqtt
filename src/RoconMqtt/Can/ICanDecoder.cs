using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public interface ICanDecoder
{
    /// <summary>
    /// Decodes a CAN frame using the specified communication command for header validation.
    /// </summary>
    /// <param name="data">CAN frame data (7 bytes for ANSWER, 8 bytes for GET/SET)</param>
    /// <param name="command">Communication command containing the expected header bytes</param>
    /// <param name="hints">Optional decoding hints for unknown parameters (ignored if parameter is in registry)</param>
    /// <returns>Decoded parameter or null if validation/decoding fails</returns>
    DecodedParameter? Decode(byte[] data, CommunicationCommand command, DecodingHints? hints = null);
}