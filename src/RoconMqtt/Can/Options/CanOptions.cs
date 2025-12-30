namespace RoconMqtt.Can.Options;

public class CanOptions
{
    /// <summary>
    /// CAN interface name (e.g., "can0") - used by SocketCanBus for local CAN access
    /// </summary>
    public required string CanInterfaceName { get; set; }

    /// <summary>
    /// SSH connection configuration for remote CAN access via candump/cansend
    /// </summary>
    public SshOptions? Ssh { get; set; }
}

/// <summary>
/// SSH connection configuration for remote CAN bus access
/// </summary>
public class SshOptions
{
    /// <summary>
    /// SSH server hostname or IP address
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// SSH server port (default: 22)
    /// </summary>
    public int Port { get; set; } = 22;

    /// <summary>
    /// SSH username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// SSH password (consider using key-based authentication in production)
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Timeout for SSH connection in milliseconds (default: 30000ms = 30s)
    /// </summary>
    public int ConnectionTimeoutMs { get; set; } = 30000;
}
