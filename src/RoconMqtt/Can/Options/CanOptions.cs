using System.ComponentModel.DataAnnotations;

namespace RoconMqtt.Can.Options;

public class CanOptions
{
    /// <summary>
    /// Machine type identifier (e.g., "EHSHXXPXXA") - determines which can-registry file to load
    /// </summary>
    [Required(ErrorMessage = "Machine type is required")]
    public string MachineType { get; set; } = string.Empty;

    /// <summary>
    /// CAN interface name (e.g., "can0") - used by SocketCanBus for local CAN access
    /// </summary>
    [Required(ErrorMessage = "CAN interface name is required")]
    public string CanInterfaceName { get; set; } = string.Empty;

    /// <summary>
    /// SSH connection configuration for remote CAN access via candump/cansend
    /// </summary>
    public SshOptions? Ssh { get; set; }

    /// <summary>
    /// IANA timezone identifier for compound parameter timestamps (e.g., "Europe/Brussels", "America/New_York").
    /// If not specified, the system's local timezone will be used.
    /// See https://en.wikipedia.org/wiki/List_of_tz_database_time_zones for valid values.
    /// </summary>
    public string? TimeZoneId { get; set; }
}

/// <summary>
/// SSH connection configuration for remote CAN bus access
/// </summary>
public class SshOptions
{
    /// <summary>
    /// SSH server hostname or IP address
    /// </summary>
    [Required(ErrorMessage = "SSH Host is required")]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SSH server port (default: 22)
    /// </summary>
    [Range(1, 65535, ErrorMessage = "SSH Port must be between 1 and 65535")]
    public int Port { get; set; } = 22;

    /// <summary>
    /// SSH username
    /// </summary>
    [Required(ErrorMessage = "SSH Username is required")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SSH password (consider using key-based authentication in production)
    /// </summary>
    [Required(ErrorMessage = "SSH Password is required")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Timeout for SSH connection in milliseconds (default: 30000ms = 30s)
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Connection timeout must be at least 100ms")]
    public int ConnectionTimeoutMs { get; set; } = 30000;
}
