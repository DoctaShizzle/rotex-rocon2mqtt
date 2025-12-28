namespace RoconMqtt.Can;

public class CanOptions
{
    public uint ReceiveFromCanFrameId { get; set; }
    public uint ReceiveToCanFrameId { get; set; }
    public uint SendCanFrameId { get; set; }
}
