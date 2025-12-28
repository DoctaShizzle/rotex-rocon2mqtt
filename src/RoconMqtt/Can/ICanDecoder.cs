namespace RoconMqtt.Can;

public interface ICanDecoder
{
    DecodedParameter? Decode(byte[] data);
}