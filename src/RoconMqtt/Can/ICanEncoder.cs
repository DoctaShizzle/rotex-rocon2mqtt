namespace RoconMqtt.Can;

public interface ICanEncoder
{
    byte[] Encode(InfoNumber info, object value);
}
