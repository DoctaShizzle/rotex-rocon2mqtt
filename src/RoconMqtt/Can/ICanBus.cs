using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

/// <summary>
/// Provides bidirectional communication with a CAN (Controller Area Network) bus.
/// </summary>
public interface ICanBus
{
    /// <summary>
    /// Asynchronously reads CAN frames from the bus.
    /// </summary>
    /// <param name="token">A cancellation token to stop reading frames.</param>
    /// <param name="canId">Optional CAN ID to filter frames. If null, all frames are returned.</param>
    /// <returns>An async enumerable of CAN frames received from the bus.</returns>
    IAsyncEnumerable<CanFrame> ReadFramesAsync(CancellationToken token, uint? canId = null);
    
    /// <summary>
    /// Asynchronously sends a CAN frame to the bus.
    /// </summary>
    /// <param name="canId">The CAN identifier for the frame.</param>
    /// <param name="data">The data payload of the frame.</param>
    /// <param name="token">A cancellation token to cancel the send operation.</param>
    Task SendFrameAsync(uint canId, byte[] data, CancellationToken token);
}
