namespace RoconMqtt.Can;

using SocketCANSharp;
using SocketCANSharp.Network;
using System.Runtime.CompilerServices;

public class SocketCanReader : ICanReader
{
    private readonly string _interfaceName = "can0";
    private readonly RawCanSocket _socket;

    public SocketCanReader()
    {
        _socket = new RawCanSocket();
        var iface = new CanNetworkInterface(0, _interfaceName, false);
        _socket.Bind(iface);
    }

    public async IAsyncEnumerable<CanFrame> ReadFramesAsync(
        [EnumeratorCancellation] CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _socket.Read(out SocketCANSharp.CanFrame frame);
            yield return new CanFrame(frame.CanId, frame.Data);
            await Task.Yield();
        }
    }

    public Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
        var frame = new SocketCANSharp.CanFrame(canId, data);
        _socket.Write(frame);
        return Task.CompletedTask;
    }
}
