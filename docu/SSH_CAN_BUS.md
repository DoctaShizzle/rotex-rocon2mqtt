# SSH CAN Bus Implementation

The `SshCanBus` provides remote CAN bus access via SSH using `candump` and `cansend` commands on a Linux system. This is particularly useful for:

- **Testing**: Develop and test without direct hardware access
- **Remote Monitoring**: Monitor CAN bus from a different machine
- **Development**: Work on Windows/Mac while accessing Linux CAN hardware

## Prerequisites

### Remote Linux System

The remote Linux system must have:

1. **can-utils package** installed:
   ```bash
   # Debian/Ubuntu
   sudo apt-get install can-utils
   
   # Fedora/RHEL
   sudo dnf install can-utils
   
   # Arch
   sudo pacman -S can-utils
   ```

2. **CAN interface configured** (e.g., `can0`):
   ```bash
   # Example: Setup virtual CAN interface for testing
   sudo modprobe vcan
   sudo ip link add dev can0 type vcan
   sudo ip link set up can0
   
   # Example: Setup real CAN interface
   sudo ip link set can0 type can bitrate 125000
   sudo ip link set up can0
   ```

3. **SSH access enabled**:
   ```bash
   sudo systemctl enable ssh
   sudo systemctl start ssh
   ```

4. **User permissions** for CAN access (add user to appropriate group):
   ```bash
   sudo usermod -a -G dialout $USER
   # or
   sudo usermod -a -G can $USER
   ```

## Configuration

### appsettings.json

```json
{
  "Can": {
    "CanInterfaceName": "can0",
    "Ssh": {
      "Host": "192.168.0.100",
      "Port": 22,
      "Username": "pi",
      "Password": "your-password",
      "ConnectionTimeoutMs": 30000
    }
  }
}
```

**Note**: SSH CAN bus is automatically enabled when the `Ssh` section is present. To use local SocketCAN, simply omit or remove the `Ssh` section.

### Configuration Options

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `Host` | Yes | - | SSH server hostname or IP address |
| `Port` | No | `22` | SSH server port |
| `Username` | Yes | - | SSH username |
| `Password` | Yes | - | SSH password |
| `ConnectionTimeoutMs` | No | `30000` | SSH connection timeout in milliseconds |

## Implementation Selection

The application automatically selects the CAN bus implementation based on configuration:

### Automatic Selection (Recommended)

```csharp
services.AddCanService(ctx.Configuration)
    .WithAutoCanBus();  // Auto-selects SSH or SocketCAN based on config
```

- If `Can:Ssh` section is present ? Uses `SshCanBus`
- Otherwise ? Uses `SocketCanBus`

### Manual Selection

You can also explicitly choose:

```csharp
// Force SSH CAN bus
services.AddCanService(ctx.Configuration)
    .WithSshCanBus();

// Force local SocketCAN
services.AddCanService(ctx.Configuration)
    .WithSocketCanBus();
```

## How It Works

### Reading Frames (`candump`)

1. Establishes SSH connection to remote host
2. Starts `candump` command in a shell stream: `candump can0`
3. Continuously reads and parses output
4. Yields `CanFrame` objects to the application

**candump output format:**
```
can0  180   [7]  D2 1D FA 0A 06 01 E0
?     ?     ?    ?? Data bytes (hex)
?     ?     ?? Data length
?     ?? CAN ID (hex)
?? Interface
```

### Sending Frames (`cansend`)

1. Ensures SSH connection is established
2. Executes `cansend` command: `cansend can0 180#D21DFA0A0601E0`
3. Checks exit status for errors
4. Logs success/failure

**cansend format:**
```
cansend can0 180#D21DFA0A0601E0
        ?    ?   ?? Data (hex)
        ?    ?? CAN ID (hex)
        ?? Interface
```

## Security Considerations

### ?? Production Use

**DO NOT** use password authentication in production! Instead:

#### Option 1: SSH Key-Based Authentication (Recommended)

Generate SSH key pair and configure SSH.NET to use it:

```csharp
// Future enhancement - add to SshOptions
public string? PrivateKeyPath { get; set; }
public string? PrivateKeyPassphrase { get; set; }
```

Then update `SshCanBus` to use key-based auth:

```csharp
if (!string.IsNullOrEmpty(_options.PrivateKeyPath))
{
    var keyFile = new PrivateKeyFile(_options.PrivateKeyPath, _options.PrivateKeyPassphrase);
    _sshClient = new SshClient(_options.Host, _options.Port, _options.Username, keyFile);
}
```

#### Option 2: Azure Key Vault / Secret Management

Store credentials in a secure vault and retrieve at runtime:

```csharp
var password = await keyVaultClient.GetSecretAsync("ssh-password");
_sshClient = new SshClient(_options.Host, _options.Port, _options.Username, password);
```

#### Option 3: Environment Variables

```json
{
  "Can": {
    "Ssh": {
      "Password": "#{SSH_PASSWORD}#"
    }
  }
}
```

Replace tokens during deployment.

### ?? Development/Testing

For development, password auth is acceptable but:
- Use a dedicated test account with limited permissions
- Restrict SSH access to specific IPs/networks
- Use strong passwords
- Don't commit passwords to version control

## Logging

SSH CAN bus operations are logged with event IDs **2101-2117**:

| Event ID | Level | Description |
|----------|-------|-------------|
| 2101 | Information | Connecting to SSH host |
| 2102 | Information | SSH connected |
| 2103 | Error | SSH connection failed |
| 2104 | Information | SSH CAN bus initialized |
| 2105 | Error | candump not found on remote system |
| 2106 | Information | Starting candump |
| 2107 | Debug | candump started |
| 2108 | Debug | candump stopped |
| 2109 | Debug | Received CAN frame |
| 2110 | Debug | Sending CAN frame |
| 2111 | Debug | Frame sent successfully |
| 2112 | Error | Failed to send frame |
| 2113 | Error | Error reading from candump |
| 2114 | Warning | Failed to parse candump line |
| 2115 | Error | Error parsing candump line |
| 2116 | Error | cansend command failed |
| 2117 | Debug | Disposing SSH connection |

## Troubleshooting

### Connection Issues

**Error: "SSH connection failed"**
- Check network connectivity: `ping <host>`
- Verify SSH service is running: `systemctl status ssh`
- Check firewall rules
- Verify credentials

**Error: "candump command not found"**
```bash
# Install can-utils
sudo apt-get install can-utils

# Verify installation
which candump
which cansend
```

### Permission Issues

**Error: "cansend failed with exit code 1"**

User may not have permission to access CAN interface:
```bash
# Add user to appropriate group
sudo usermod -a -G dialout <username>

# Or grant direct permissions
sudo chmod 666 /dev/can0
```

### CAN Interface Issues

**Error: "Cannot find device can0"**

CAN interface is not configured:
```bash
# Check available interfaces
ip link show

# Setup interface
sudo ip link set can0 type can bitrate 125000
sudo ip link set up can0
```

## Performance Considerations

### Latency

SSH introduces network latency:
- Local SocketCAN: **< 1ms**
- SSH CAN (LAN): **5-20ms**
- SSH CAN (Internet): **50-500ms+**

For real-time applications, prefer local SocketCAN.

### Bandwidth

SSH compression helps, but high CAN bus traffic (>1000 frames/sec) may:
- Saturate network bandwidth
- Increase latency
- Cause frame drops

Consider filtering in `candump`:
```bash
# Filter by CAN ID
candump can0,180:7FF

# Multiple IDs
candump can0,180:7FF,300:7FF
```

## Example Configurations

### Local Development (Windows ? Linux VM)

```json
{
  "Can": {
    "CanInterfaceName": "can0",
    "Ssh": {
      "Host": "localhost",
      "Port": 2222,
      "Username": "vagrant",
      "Password": "vagrant"
    }
  }
}
```

### Remote Raspberry Pi

```json
{
  "Can": {
    "CanInterfaceName": "can0",
    "Ssh": {
      "Host": "raspberrypi.local",
      "Port": 22,
      "Username": "pi",
      "Password": "raspberry"
    }
  }
}
```

### Testing with Virtual CAN

```bash
# On remote Linux system
sudo modprobe vcan
sudo ip link add dev vcan0 type vcan
sudo ip link set up vcan0

# Test send
cansend vcan0 180#D21DFA0A0601E0

# Test receive
candump vcan0
```

```json
{
  "Can": {
    "CanInterfaceName": "vcan0",
    "Ssh": {
      "Host": "192.168.0.100",
      "Port": 22,
      "Username": "test",
      "Password": "test"
    }
  }
}
```

## Switching Between Local and Remote

Toggle between implementations without code changes:

**Use local SocketCAN:**
```json
{
  "Can": {
    "CanInterfaceName": "can0"
  }
}
```

**Use SSH CAN:**
```json
{
  "Can": {
    "CanInterfaceName": "can0",
    "Ssh": {
      "Host": "192.168.0.100",
      "Port": 22,
      "Username": "pi",
      "Password": "raspberry"
    }
  }
}
