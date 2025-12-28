namespace RoconMqtt.Can;

public class CanFrameData(uint id, byte[] data)
{
    public uint Id { get; set; } = id;
    public byte[] Data { get; set; } = data;
}
