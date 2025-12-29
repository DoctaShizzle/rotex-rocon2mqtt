using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public interface ICanDecoder
{
    DecodedParameter? Decode(byte[] data);
}