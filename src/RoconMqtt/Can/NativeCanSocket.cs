using Microsoft.Extensions.Options;
using RoconMqtt.Can.Options;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RoconMqtt.Can;

/// <summary>
/// Native implementation of SocketCAN using raw .NET Socket APIs.
/// Handles low-level socket operations, P/Invoke, and CAN frame parsing.
/// </summary>
public sealed partial class NativeCanSocket : ICanSocket
{
    private Socket? _socket;
    private readonly Lock _socketLock = new();
    private readonly ILogger<NativeCanSocket> _logger;
    private readonly string _interfaceName;
    private bool _disposed;
    
    // CAN socket constants
    private const int AF_CAN = 29;
    private const int CAN_RAW = 1;
    private const int CAN_FRAME_SIZE = 16;

    public NativeCanSocket(ILogger<NativeCanSocket> logger, IOptions<CanOptions> options)
    {
        _logger = logger;
        _interfaceName = options.Value.CanInterfaceName;
        
        InitializeSocket();
    }

    private void InitializeSocket()
    {
        lock (_socketLock)
        {
            // Close existing socket if any
            try
            {
                _socket?.Close();
                _socket?.Dispose();
            }
            catch
            {
                // Ignore errors during close
            }
            
            // Create CAN socket using native socket() syscall
            // .NET Socket constructor doesn't support AF_CAN, so we must use P/Invoke
            const int SOCK_RAW = 3;
            var socketFd = socket(AF_CAN, SOCK_RAW, CAN_RAW);
            
            if (socketFd < 0)
            {
                var errno = Marshal.GetLastPInvokeError();
                throw new SocketException(errno, $"Failed to create AF_CAN socket. Error code: {errno}");
            }
            
            // Get interface index by name
            var ifIndex = GetInterfaceIndex(_interfaceName);
            
            // Bind to CAN interface using native bind() syscall
            var addr = new SockAddrCan
            {
                can_family = AF_CAN,
                can_ifindex = ifIndex
            };
            
            var result = bind(socketFd, ref addr, Marshal.SizeOf<SockAddrCan>());
            if (result < 0)
            {
                var errno = Marshal.GetLastPInvokeError();
                throw new SocketException(errno, $"Failed to bind to CAN interface '{_interfaceName}'. Error code: {errno}");
            }
            
            // Wrap the file descriptor in a SafeSocketHandle and create a .NET Socket
            var handle = new SafeSocketHandle((IntPtr)socketFd, ownsHandle: true);
            _socket = new Socket(handle);
            
            // Set socket to non-blocking mode (async I/O)
            _socket.Blocking = false;
        }
        
        LogSocketInitialized(_logger, _interfaceName);
    }

    public async Task<(uint canId, byte[] data)> ReceiveAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        var buffer = new byte[CAN_FRAME_SIZE];
        int bytesReceived;
        
        Socket? socketSnapshot;
        lock (_socketLock)
        {
            socketSnapshot = _socket;
        }
        
        if (socketSnapshot == null)
        {
            throw new InvalidOperationException("Socket is not initialized");
        }
        
        try
        {
            bytesReceived = await socketSnapshot.ReceiveAsync(buffer.AsMemory(), SocketFlags.None, cancellationToken);
        }
        catch (ObjectDisposedException)
        {
            throw new InvalidOperationException("Socket was disposed during read operation");
        }
        
        if (bytesReceived != CAN_FRAME_SIZE)
        {
            throw new InvalidOperationException($"Invalid CAN frame size: {bytesReceived} bytes (expected {CAN_FRAME_SIZE})");
        }
        
        return ParseCanFrame(buffer);
    }

    public async Task SendAsync(uint canId, byte[] data, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (data.Length > 8)
        {
            throw new ArgumentException($"CAN data too long: {data.Length} bytes (max 8)", nameof(data));
        }
        
        var frameBytes = CreateCanFrame(canId, data);
        
        Socket? socketSnapshot;
        lock (_socketLock)
        {
            socketSnapshot = _socket;
        }
        
        if (socketSnapshot == null)
        {
            throw new InvalidOperationException("Socket is not initialized");
        }
        
        await socketSnapshot.SendAsync(frameBytes.AsMemory(), SocketFlags.None, cancellationToken);
    }

    public async Task<bool> TryReconnectAsync(CancellationToken cancellationToken)
    {
        LogAttemptingReconnection(_logger, _interfaceName);
        
        try
        {
            // Small delay before reconnection
            await Task.Delay(1000, cancellationToken);
            
            // Reinitialize socket
            InitializeSocket();
            
            LogReconnectionSucceeded(_logger, _interfaceName);
            return true;
        }
        catch (Exception ex)
        {
            LogReconnectionFailed(_logger, ex, _interfaceName);
            return false;
        }
    }

    #region P/Invoke and Native Structures

    /// <summary>
    /// Creates a socket using the native socket() syscall.
    /// </summary>
    [DllImport("libc", SetLastError = true)]
    private static extern int socket(int domain, int type, int protocol);

    /// <summary>
    /// Binds a socket to an address using the native bind() syscall.
    /// </summary>
    [DllImport("libc", SetLastError = true)]
    private static extern int bind(int sockfd, ref SockAddrCan addr, int addrlen);

    /// <summary>
    /// Gets the network interface index by name using if_nametoindex.
    /// </summary>
    [DllImport("libc", SetLastError = true)]
    private static extern uint if_nametoindex(string ifname);
    
    /// <summary>
    /// Represents the sockaddr_can structure for binding to CAN interface.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct SockAddrCan
    {
        public ushort can_family;   // AF_CAN
        public int can_ifindex;      // Interface index
        public uint rx_id;           // RX filter ID (not used for raw sockets)
        public uint tx_id;           // TX filter ID (not used for raw sockets)
    }
    
    /// <summary>
    /// Gets the network interface index by name.
    /// </summary>
    private static int GetInterfaceIndex(string interfaceName)
    {
        var index = if_nametoindex(interfaceName);
        if (index == 0)
        {
            var errno = Marshal.GetLastPInvokeError();
            throw new InvalidOperationException($"Failed to get interface index for '{interfaceName}'. Error code: {errno}");
        }
        return (int)index;
    }

    #endregion

    #region CAN Frame Parsing

    /// <summary>
    /// Parses a raw CAN frame (16 bytes) into CAN ID and data.
    /// </summary>
    private static (uint canId, byte[] data) ParseCanFrame(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < CAN_FRAME_SIZE)
        {
            throw new InvalidOperationException($"Invalid CAN frame size: {buffer.Length} bytes (expected {CAN_FRAME_SIZE})");
        }
        
        // Parse CAN ID (first 4 bytes, little endian)
        var canId = BitConverter.ToUInt32(buffer.Slice(0, 4));
        
        // Parse data length (byte 4)
        var dlc = buffer[4];
        if (dlc > 8)
        {
            throw new InvalidOperationException($"Invalid CAN DLC: {dlc} (max 8)");
        }
        
        // Parse data (bytes 8-15, use only DLC bytes)
        var data = buffer.Slice(8, dlc).ToArray();
        
        return (canId, data);
    }
    
    /// <summary>
    /// Creates a raw CAN frame (16 bytes) from CAN ID and data.
    /// </summary>
    private static byte[] CreateCanFrame(uint canId, byte[] data)
    {
        if (data.Length > 8)
        {
            throw new ArgumentException($"CAN data too long: {data.Length} bytes (max 8)", nameof(data));
        }
        
        var frame = new byte[CAN_FRAME_SIZE];
        
        // Write CAN ID (first 4 bytes, little endian)
        BitConverter.TryWriteBytes(frame.AsSpan(0, 4), canId);
        
        // Write DLC (byte 4)
        frame[4] = (byte)data.Length;
        
        // Bytes 5-7 are padding (leave as 0)
        
        // Write data (bytes 8-15)
        data.CopyTo(frame.AsSpan(8));
        
        return frame;
    }

    #endregion

    #region Disposal

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (_socketLock)
        {
            try
            {
                _socket?.Close();
                _socket?.Dispose();
            }
            catch (Exception ex)
            {
                LogErrorDisposingSocket(_logger, ex);
            }
            
            _socket = null;
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        lock (_socketLock)
        {
            try
            {
                _socket?.Close();
                _socket?.Dispose();
            }
            catch (Exception ex)
            {
                LogErrorDisposingSocket(_logger, ex);
            }
            
            _socket = null;
        }

        _disposed = true;
        await ValueTask.CompletedTask;
    }

    #endregion

    #region Logging

    [LoggerMessage(EventId = 2100, Level = LogLevel.Information, Message = "Native CAN socket initialized on interface {InterfaceName}")]
    private static partial void LogSocketInitialized(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2101, Level = LogLevel.Information, Message = "Attempting to reconnect CAN socket on interface {InterfaceName}")]
    private static partial void LogAttemptingReconnection(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2102, Level = LogLevel.Information, Message = "Successfully reconnected CAN socket on interface {InterfaceName}")]
    private static partial void LogReconnectionSucceeded(ILogger logger, string interfaceName);

    [LoggerMessage(EventId = 2103, Level = LogLevel.Error, Message = "Failed to reconnect CAN socket on interface {InterfaceName}")]
    private static partial void LogReconnectionFailed(ILogger logger, Exception exception, string interfaceName);

    [LoggerMessage(EventId = 2104, Level = LogLevel.Warning, Message = "Error disposing CAN socket")]
    private static partial void LogErrorDisposingSocket(ILogger logger, Exception exception);

    #endregion
}
