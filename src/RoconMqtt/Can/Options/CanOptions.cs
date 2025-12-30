namespace RoconMqtt.Can.Options;

public class CanOptions
{
    public required string CanInterfaceName { get; set; }
    public uint ReceiveFromCanFrameId { get; set; }
    public uint ReceiveToCanFrameId { get; set; }
}
