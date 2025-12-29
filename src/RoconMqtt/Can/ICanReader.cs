using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public interface ICanReader
{
    IAsyncEnumerable<CanFrame> ReadFramesAsync(CancellationToken token);
    Task SendFrameAsync(uint canId, byte[] data, CancellationToken token);
}
