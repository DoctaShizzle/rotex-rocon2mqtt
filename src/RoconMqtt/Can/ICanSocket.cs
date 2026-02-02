namespace RoconMqtt.Can;

/// <summary>
/// Low-level interface for CAN socket I/O operations.
/// </summary>
public interface ICanSocket : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Asynchronously receives a CAN frame from the socket.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A tuple containing the CAN ID and data bytes.</returns>
    Task<(uint canId, byte[] data)> ReceiveAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Asynchronously sends a CAN frame to the socket.
    /// </summary>
    /// <param name="canId">The CAN identifier.</param>
    /// <param name="data">The data bytes (max 8 bytes).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    Task SendAsync(uint canId, byte[] data, CancellationToken cancellationToken);
    
    /// <summary>
    /// Attempts to reconnect the socket after an error.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if reconnection succeeded, false otherwise.</returns>
    Task<bool> TryReconnectAsync(CancellationToken cancellationToken);
}
