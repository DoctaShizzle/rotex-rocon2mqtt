# Deployment Guide

This guide describes how to deploy the RoconMqtt service to a Raspberry Pi or other Linux ARM64 system.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Building for Raspberry Pi](#building-for-raspberry-pi)
- [Deployment Steps](#deployment-steps)
- [Running as a systemd Service](#running-as-a-systemd-service)
- [Configuration](#configuration)
- [Monitoring and Logs](#monitoring-and-logs)
- [Troubleshooting](#troubleshooting)
- [Updating the Service](#updating-the-service)

---

## Prerequisites

### On Your Development Machine
- .NET 10 SDK installed
- PowerShell or Bash (for running publish scripts)

### On Your Raspberry Pi / Target Device
- Raspberry Pi 3/4/5 or compatible ARM64 Linux system
- SSH access enabled
- Sufficient disk space (~100 MB for the application)
- Network connectivity to:
  - CAN interface device (default: `192.168.0.250`)
  - MQTT broker (default: `192.168.0.254`)

---

## Building for Raspberry Pi

### Option 1: Using PowerShell (Windows)
```powershell
.\scripts\publish-raspi.ps1
```

### Option 2: Using Bash (Linux/macOS/WSL)
```bash
chmod +x ./scripts/publish-raspi.sh
./scripts/publish-raspi.sh
```

The build output will be located at:
```
src/RoconMqtt/bin/publish/linux-arm64/
```

---

## Development Testing (Bare Metal)

For quick development testing on your local machine or Raspberry Pi, use the provided development scripts.

### Quick Start Scripts

Two scripts are provided for easy development testing:

**Windows (PowerShell):**
```powershell
.\scripts\run-dev.ps1
```

**Linux/macOS:**
```bash
chmod +x ./scripts/run-dev.sh
./scripts/run-dev.sh
```

### Configuration

Edit the script before running to set your environment variables:

```powershell
# Required settings
$env:Can__MachineType = "EHSHXXPXXA"
$env:Mqtt__Host = "192.168.0.254"
$env:Mqtt__Username = "rocon"
$env:Mqtt__Password = "your-mqtt-password"

# Optional - SSH CAN (for remote CAN access)
# $env:Can__Ssh__Host = "192.168.0.100"
# $env:Can__Ssh__Username = "pi"
# $env:Can__Ssh__Password = "raspberry"
```

**Features:**
- Sets `ASPNETCORE_ENVIRONMENT=Development` for verbose logging
- Runs `dotnet run` with all required environment variables
- Shows configuration summary before starting
- Press `Ctrl+C` to stop

**SocketCAN vs SSH CAN:**
- If `Can__Ssh__*` variables are NOT set → Uses local SocketCAN (`can0` on the machine)
- If `Can__Ssh__*` variables ARE set → Connects to remote CAN device via SSH

### Manual Command Line Testing

Alternatively, run directly with environment variables:

**Windows (PowerShell):**
```powershell
cd src\RoconMqtt
$env:Can__MachineType = "EHSHXXPXXA"
$env:Mqtt__Host = "192.168.0.254"
$env:Mqtt__Username = "rocon"
$env:Mqtt__Password = "your-password"
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

**Linux/macOS:**
```bash
cd src/RoconMqtt
Can__MachineType="EHSHXXPXXA" \
Mqtt__Host="192.168.0.254" \
Mqtt__Username="rocon" \
Mqtt__Password="your-password" \
ASPNETCORE_ENVIRONMENT="Development" \
dotnet run
```

**On Raspberry Pi with SocketCAN (requires sudo):**
```bash
cd /opt/roconmqtt
sudo Can__MachineType="EHSHXXPXXA" \
     Mqtt__Host="192.168.0.254" \
     Mqtt__Username="rocon" \
     Mqtt__Password="your-password" \
     ASPNETCORE_ENVIRONMENT="Development" \
     ./RoconMqtt
```

**Note:** Local SocketCAN requires root/sudo permissions to access the CAN interface.

---

## Deployment Steps

### 1. Create User and Directory on Raspberry Pi

SSH into your Raspberry Pi and run:

```bash
# Create the can group for CAN interface access (if it doesn't exist)
sudo groupadd -f can

# Create a dedicated user for the service and add to can group for CAN interface access
sudo useradd -r -m -d /opt/roconmqtt -s /bin/false -G can rocon

# Create the application directory
sudo mkdir -p /opt/roconmqtt/logs
sudo chown -R rocon:rocon /opt/roconmqtt
```

**Note**: The `can` group membership allows the `rocon` user to access CAN interfaces without root privileges.

### 2. Configure CAN Group Permissions (udev rules)

To allow the `can` group to access CAN interfaces, create a udev rule:

```bash
# Create udev rules file
sudo nano /etc/udev/rules.d/90-can.rules
```

Add the following content:

```
# Grant can group access to CAN network interfaces
KERNEL=="can*", SUBSYSTEM=="net", GROUP="can", MODE="0660"
```

Save the file and reload udev rules:

```bash
# Reload udev rules
sudo udevadm control --reload-rules
sudo udevadm trigger

# Verify the rule is active (after CAN interfaces are up)
ls -l /sys/class/net/can*
```

**What this does:**
- When the CAN interface (`can0`) is created, it will be assigned to the `can` group
- Group members get read/write access (mode `0660`)
- The `rocon` user (member of `can` group) can now send and receive CAN frames without `sudo`

**Note**: Make sure your CAN interfaces are properly configured as described in **[HARDWARE.md](HARDWARE.md)** before testing permissions.

### 3. Transfer Files to Raspberry Pi

From your development machine:

```bash
# Using SCP
scp -r src/RoconMqtt/bin/publish/linux-arm64/* pi@<raspberry-pi-ip>:/tmp/roconmqtt/

# Or using rsync (recommended)
rsync -avz --progress src/RoconMqtt/bin/publish/linux-arm64/ pi@<raspberry-pi-ip>:/tmp/roconmqtt/
```

### 4. Move Files and Set Permissions

On the Raspberry Pi:

```bash
# Move files to final location
sudo mv /tmp/roconmqtt/* /opt/roconmqtt/
sudo chown -R rocon:rocon /opt/roconmqtt

# Make the executable runnable
sudo chmod +x /opt/roconmqtt
```

### 5. Configure the Application

**Important:** The application is designed to be configured via **environment variables** rather than editing `appsettings.json`. This keeps secrets out of configuration files and makes deployment consistent across environments.

Configuration will be set up in the systemd service (see [Running as a systemd Service](#running-as-a-systemd-service-production)).

**Required configuration:**
- `Can__MachineType` - Determines which CAN registry to load (e.g., `EHSHXXPXXA`)
- `Mqtt__Host` - Your MQTT broker IP address
- `Mqtt__Username` - MQTT broker username
- `Mqtt__Password` - MQTT broker password

**Note:** Devices and parameters are defined in the machine-specific CAN registry file (e.g., `can-registry-EHSHXXPXXA.json`) and do not need to be configured manually.

**Configuration documentation:**
- For complete configuration reference, see **[CONFIGURATION.md](CONFIGURATION.md)**
- For MQTT-specific options (Home Assistant, TLS, etc.), see **[MQTT.md](MQTT.md)**
- For CAN bus configuration (SocketCAN vs SSH), see **[CAN.md](CAN.md)**

---

## Running as a systemd Service (Production)

**Prerequisites:**
- Ensure CAN interface is configured as described in **[HARDWARE.md](HARDWARE.md)**
- The `setup-can.service` must be installed and enabled (creates `can0` interface)
- The RoconMqtt service has a hard dependency on `setup-can.service` to ensure CAN interface is ready before starting

### 1. Install and Configure the Service File

From your development machine, copy the service file:

```bash
scp scripts/roconmqtt.service pi@<raspberry-pi-ip>:/tmp/
```

On the Raspberry Pi:

```bash
sudo mv /tmp/roconmqtt.service /etc/systemd/system/
sudo chmod 644 /etc/systemd/system/roconmqtt.service
```

### 2. Configure Environment Variables

**Option A: Environment File (Recommended for Production)**

Create a secure environment file with all configuration:

```bash
sudo nano /opt/roconmqtt/.env
```

Add the following content (adjust values for your setup):

```ini
# Application Environment
ASPNETCORE_ENVIRONMENT=Production

# Machine Type (REQUIRED)
Can__MachineType=EHSHXXPXXA

# MQTT Configuration (REQUIRED)
Mqtt__Host=192.168.0.254
Mqtt__Username=rocon
Mqtt__Password=your-secure-mqtt-password

# MQTT Optional Settings
Mqtt__Port=1883
Mqtt__ClientId=rocon-mqtt
Mqtt__Topic=rocon
Mqtt__PollingIntervalSeconds=30
Mqtt__ResponseTimeoutSeconds=5
Mqtt__ChangeThresholdPercent=2.0
Mqtt__TimeZoneId=Europe/Brussels

# CAN Configuration
Can__CanInterfaceName=can0
Can__TimeZoneId=Europe/Brussels

# SSH CAN (Optional - for remote CAN access)
# Uncomment these if you need to access CAN via SSH:
# Can__Ssh__Host=192.168.0.100
# Can__Ssh__Port=22
# Can__Ssh__Username=pi
# Can__Ssh__Password=your-ssh-password

# Seq Logging (Optional - for centralized log management)
# Uncomment these to enable Seq logging:
# Serilog__WriteTo__1__Name=Seq
# Serilog__WriteTo__1__Args__serverUrl=http://192.168.0.254:5341

# Kestrel Web Server (Optional)
Kestrel__Endpoints__Http__Url=http://0.0.0.0:5000
```

Secure the file (important!):

```bash
sudo chmod 600 /opt/roconmqtt/.env
sudo chown rocon:rocon /opt/roconmqtt/.env
```

Update the systemd service file to use the environment file:

```bash
sudo nano /etc/systemd/system/roconmqtt.service
```

Add the `EnvironmentFile` line in the `[Service]` section:

```ini
[Service]
Type=notify
EnvironmentFile=/opt/roconmqtt/.env
WorkingDirectory=/opt/roconmqtt
ExecStart=/opt/roconmqtt/RoconMqtt
User=rocon
Group=rocon
SupplementaryGroups=can
Restart=always
RestartSec=10
```

**Option B: Direct Environment Variables in Service File**

Alternatively, add environment variables directly to the service file:

```bash
sudo nano /etc/systemd/system/roconmqtt.service
```

Add these lines in the `[Service]` section:

```ini
[Service]
Type=notify
# Required settings
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="Can__MachineType=EHSHXXPXXA"
Environment="Mqtt__Host=192.168.0.254"
Environment="Mqtt__Username=rocon"
Environment="Mqtt__Password=your-secure-password"

# Optional settings
Environment="Mqtt__Port=1883"
Environment="Mqtt__ClientId=rocon-mqtt"
Environment="Mqtt__Topic=rocon"
Environment="Can__CanInterfaceName=can0"

WorkingDirectory=/opt/roconmqtt
ExecStart=/opt/roconmqtt/RoconMqtt
User=rocon
Group=rocon
SupplementaryGroups=can
Restart=always
RestartSec=10
```

**Note:** Option A (environment file) is more secure and easier to update without editing the service file.

### 3. Enable and Start the Service

```bash
# Reload systemd to recognize the new service
sudo systemctl daemon-reload

# Enable the service to start on boot
sudo systemctl enable roconmqtt

# Start the service
sudo systemctl start roconmqtt
```

### 4. Verify the Service is Running

```bash
# Check service status
sudo systemctl status roconmqtt

# Follow live logs
sudo journalctl -u roconmqtt -f

# View recent logs
sudo journalctl -u roconmqtt -n 100
```

### Service Dependencies

The service file includes these critical dependencies:
```ini
After=network-online.target setup-can.service
Wants=network-online.target
Requires=setup-can.service
SupplementaryGroups=can
```

- `After=setup-can.service` - Waits for CAN interfaces to be configured
- `Requires=setup-can.service` - Hard dependency; service won't start if setup-can fails
- `SupplementaryGroups=can` - Grants access to CAN interfaces via the `can` group

**Important:** If you get socket errors indicating "network is down" on boot, verify that `setup-can.service` is installed and enabled (see [HARDWARE.md](HARDWARE.md)). The application uses native .NET sockets (AF_CAN) to communicate with the Linux SocketCAN kernel module.

### Verify Service Dependencies

Check that the service dependencies are correctly configured:

```bash
# Verify setup-can.service is enabled and active
sudo systemctl status setup-can.service

# Check if CAN interface is up
ip link show can0

# View dependency tree (shows startup order)
systemctl list-dependencies roconmqtt.service
```

**Expected output:**
- `setup-can.service` should be **active** and **enabled**
- `can0` should show state **UP** with correct bitrate (20000)
- Dependency tree should show `setup-can.service` before `roconmqtt.service`

**Troubleshooting startup issues:**

If you get "SocketCanException: network is down" after reboot:

```bash
# Check if setup-can ran successfully
sudo journalctl -u setup-can.service -n 50

# Manually verify CAN interface
ip link show can0

# If interface is down, check the setup script
cat /opt/roconmqtt/setup-can.sh

# Verify the service is enabled
sudo systemctl is-enabled setup-can.service
```

---

---

## Configuration Reference

### Configuration Sources (Priority Order)

The application loads configuration from multiple sources in this order (later sources override earlier ones):

1. `appsettings.json` - Default settings
2. `appsettings.{Environment}.json` - Environment-specific settings
3. Environment variables - Runtime overrides (highest priority)

### Environment Variables Format

Use double underscores (`__`) to represent nested settings:

```
Section__Subsection__Property
```

**Examples:**
- `Mqtt__Host` → `"Mqtt": { "Host": "..." }`
- `Can__Ssh__Username` → `"Can": { "Ssh": { "Username": "..." } }`
- `Kestrel__Endpoints__Http__Url` → `"Kestrel": { "Endpoints": { "Http": { "Url": "..." } } }`

### Required Settings

These settings MUST be configured (via environment variables or appsettings):

- `Can__MachineType` - Machine type to determine which CAN registry to load (e.g., `EHSHXXPXXA`)
- `Mqtt__Host` - MQTT broker hostname or IP address
- `Mqtt__Username` - MQTT broker username
- `Mqtt__Password` - MQTT broker password

### Complete Configuration Documentation

For a comprehensive list of all available settings, see:
- **[CONFIGURATION.md](CONFIGURATION.md)** - Complete environment variable reference
- **[MQTT.md](MQTT.md)** - MQTT-specific configuration (Home Assistant, TLS, etc.)
- **[CAN.md](CAN.md)** - CAN bus configuration (SocketCAN, SSH CAN)

### Environment-Specific Settings

- **Production** (`appsettings.Production.json`):
  - Log level: Warning (minimal logging)
  - Default environment if not specified

- **Development** (`appsettings.Development.json`):
  - Log level: Debug (verbose logging)
  - Use for troubleshooting

### Firewall Configuration

If you want to access the web API (Kestrel endpoint on port 5000):

```bash
sudo ufw allow 5000/tcp
sudo ufw reload
```

**Note**: The web API is optional and only needed if you want to expose HTTP endpoints for health checks or monitoring.

---

## Monitoring and Logs

### View Logs

```bash
# Live tail
sudo journalctl -u roconmqtt -f

# Last 100 lines
sudo journalctl -u roconmqtt -n 100

# Logs since yesterday
sudo journalctl -u roconmqtt --since yesterday

# Logs between specific times
sudo journalctl -u roconmqtt --since "2025-01-15 10:00" --until "2025-01-15 12:00"
```

### Application Log Files

Serilog also writes to files in:
```
/opt/roconmqtt/logs/
```

Files are named `rocon-mqtt-YYYY-MM-DD.txt` and roll daily.

### Service Control

```bash
# Start
sudo systemctl start roconmqtt

# Stop
sudo systemctl stop roconmqtt

# Restart
sudo systemctl restart roconmqtt

# Status
sudo systemctl status roconmqtt

# Enable auto-start on boot
sudo systemctl enable roconmqtt

# Disable auto-start
sudo systemctl disable roconmqtt
```

---

## Troubleshooting

### Service Won't Start

1. Check service status:
   ```bash
   sudo systemctl status roconmqtt -l
   ```

2. Check permissions:
   ```bash
   ls -la /opt/roconmqtt/
   # Ensure rocon:rocon owns the files
   ```

3. Test manual execution:
   ```bash
   sudo -u rocon /opt/roconmqtt/RoconMqtt
   ```

### Connection Issues

1. Verify network connectivity:
   ```bash
   # Test MQTT broker
   ping 192.168.0.254
   telnet 192.168.0.254 1883

   # Test CAN interface
   ping 192.168.0.250
   ssh root@192.168.0.250
   ```

2. Check configuration:
   ```bash
   cat /opt/roconmqtt/appsettings.json
   ```

3. Enable debug logging:
   - Switch to Development environment (see [Environment Variables](#environment-variables))
   - Or edit `appsettings.Production.json` to set log level to `Debug`

### High CPU/Memory Usage

The service has resource limits configured in the systemd service file:
- Memory: 512 MB max
- CPU: 50% max

To adjust these limits:
```bash
sudo nano /etc/systemd/system/roconmqtt.service
```

Modify:
```ini
MemoryMax=512M
CPUQuota=50%
```

Then reload and restart:
```bash
sudo systemctl daemon-reload
sudo systemctl restart roconmqtt
```

### View Resilience Policy Events

The application uses Polly for resilience (retries, circuit breaker). See [RESILIENCE.md](RESILIENCE.md) for details.

Check logs for event IDs:
- `5002`: Retry attempt
- `5003`: Circuit breaker opened
- `5004`: Circuit breaker closed

---

## Updating the Service

### 1. Build New Version

On your development machine:
```bash
.\scripts\publish-raspi.ps1
```

### 2. Stop the Service

On Raspberry Pi:
```bash
sudo systemctl stop roconmqtt
```

### 3. Backup Current Version (Optional)

```bash
sudo cp -r /opt/roconmqtt /opt/roconmqtt.backup.$(date +%Y%m%d)
```

### 4. Deploy New Files

From development machine:
```bash
rsync -avz --progress src/RoconMqtt/bin/publish/linux-arm64/ pi@<raspberry-pi-ip>:/tmp/roconmqtt/
```

On Raspberry Pi:
```bash
sudo cp -r /tmp/roconmqtt/* /opt/roconmqtt/
sudo chown -R rocon:rocon /opt/roconmqtt
sudo chmod +x /opt/roconmqtt/RoconMqtt
```

### 5. Restart the Service

```bash
sudo systemctl start roconmqtt
sudo systemctl status roconmqtt
```

### 6. Verify

```bash
sudo journalctl -u roconmqtt -f
```

---

## Advanced Configuration

### Using Environment Variables for Secrets

Instead of storing credentials in `appsettings.json`, use environment variables:

1. Edit the service file:
   ```bash
   sudo nano /etc/systemd/system/roconmqtt.service
   ```

2. Add environment variables:
   ```ini
   Environment="Mqtt__Username=your-username"
   Environment="Mqtt__Password=your-password"
   Environment="Can__Ssh__Password=your-ssh-password"
   ```

3. Remove credentials from `appsettings.json`

4. Reload and restart:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart roconmqtt
   ```

**Note**: Use double underscores (`__`) to represent nested configuration sections.

### Multiple Instances

To run multiple instances (e.g., for different heat pumps):

1. Copy and rename the service file:
   ```bash
   sudo cp /etc/systemd/system/roconmqtt.service /etc/systemd/system/roconmqtt-hg2.service
   ```

2. Edit the new service file to use a different working directory:
   ```ini
   WorkingDirectory=/opt/roconmqtt-hg2
   ```

3. Deploy the application to the new directory with different configuration

4. Enable and start:
   ```bash
   sudo systemctl enable roconmqtt-hg2
   sudo systemctl start roconmqtt-hg2
   ```

---

## Security Best Practices

1. **Use a dedicated user**: The service runs as user `rocon` with limited privileges
2. **Restrict file permissions**: Only `rocon` user can read/write application files
3. **Use secrets management**: Consider using environment variables or Azure Key Vault for credentials
4. **Enable firewall**: Only open necessary ports
5. **Keep system updated**: Regularly update Raspberry Pi OS and the application
6. **Monitor logs**: Watch for unusual activity or connection attempts

---

## Support

For issues or questions:
- Check logs: `sudo journalctl -u roconmqtt -f`
- Review documentation:
  - [MQTT.md](MQTT.md) - MQTT publisher and Home Assistant integration
  - [CAN.md](CAN.md) - CAN bus communication
  - [SSH_CAN_BUS.md](SSH_CAN_BUS.md) - Remote CAN access
  - [HARDWARE.md](HARDWARE.md) - Hardware setup
  - [RESILIENCE.md](RESILIENCE.md) - Retry and circuit breaker policies
- GitHub Issues: [https://github.com/DoctaShizzle/rotex-rocon2mqtt/issues](https://github.com/DoctaShizzle/rotex-rocon2mqtt/issues)
