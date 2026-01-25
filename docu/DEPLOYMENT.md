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

## Running in Development Mode (Command Line)

For testing and debugging, you can run the application directly from the command line with verbose logging.

### Prerequisites

- The application must be run with `sudo` to access CAN interfaces
- Ensure `can0` and `can1` are properly configured (see [HARDWARE.md](HARDWARE.md))

### Running the Application

Transfer the build output to your Raspberry Pi (as described in [Deployment Steps](#deployment-steps)), then:

```bash
cd /opt/roconmqtt
sudo ASPNETCORE_ENVIRONMENT=Development ./RoconMqtt
```

This will:
- Run with **verbose Debug logging** to help diagnose issues
- Output logs to both console and file (`/opt/roconmqtt/logs/rocon-mqtt-*.txt`)
- Show detailed error messages and state transitions

### Stopping the Application

Press `Ctrl+C` in the terminal to gracefully stop the application.

---

## Deployment Steps

### 1. Create User and Directory on Raspberry Pi

SSH into your Raspberry Pi and run:

```bash
# Create a dedicated user for the service and add to can group for SocketCAN access
sudo useradd -r -m -d /opt/roconmqtt -s /bin/false -G can rocon

# Create the application directory
sudo mkdir -p /opt/roconmqtt/logs
sudo chown -R rocon:rocon /opt/roconmqtt
```

**Note**: The `can` group membership allows the `rocon` user to access CAN interfaces without root privileges.

### 2. Transfer Files to Raspberry Pi

From your development machine:

```bash
# Using SCP
scp -r src/RoconMqtt/bin/publish/linux-arm64/* pi@<raspberry-pi-ip>:/tmp/roconmqtt/

# Or using rsync (recommended)
rsync -avz --progress src/RoconMqtt/bin/publish/linux-arm64/ pi@<raspberry-pi-ip>:/tmp/roconmqtt/
```

### 3. Move Files and Set Permissions

On the Raspberry Pi:

```bash
# Move files to final location
sudo mv /tmp/roconmqtt/* /opt/roconmqtt/
sudo chown -R rocon:rocon /opt/roconmqtt

# Make the executable runnable
sudo chmod +x /opt/roconmqtt/RoconMqtt
```

### 4. Configure the Application

Edit the configuration file:

```bash
sudo nano /opt/roconmqtt/appsettings.json
```

Update the configuration with your specific values. Here's a minimal example:

```json
{
  "Mqtt": {
    "Host": "192.168.0.254",  // Your MQTT broker IP
    "ClientId": "rocon-mqtt",
    "Topic": "rocon",
    "Username": "your-username",
    "Password": "your-password",
    "Devices": ["HG1"],
    "Parameters": ["cAUSSENTEMP", "cVORLAUFTEMP"],
    "CompoundParameters": ["Timestamp"],
    "PollingIntervalSeconds": 30,
    "ResponseTimeoutSeconds": 1,
    "ChangeThresholdPercent": 1.0
  },
  "Can": {
    "Ssh": {
      "Host": "192.168.0.250",  // Your CAN interface device IP
      "Username": "root",
      "Password": "your-password"
    }
  }
}
```

**Configuration Notes:**
- For complete MQTT options including Home Assistant auto-discovery, TLS, and more, see **[MQTT.md](MQTT.md)**
- For CAN bus configuration (SocketCAN vs SSH), see **[SSH_CAN_BUS.md](SSH_CAN_BUS.md)**

**?? Security Note**: For production, consider using environment variables or secrets management instead of storing passwords in plain text.

---

## Running as a systemd Service

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

Edit the service file to add SocketCAN group membership:

```bash
sudo nano /etc/systemd/system/roconmqtt.service
```

Add the following line in the `[Service]` section (near the `User=rocon` line):

```ini
SupplementaryGroups=can
```

This ensures the `rocon` user has access to CAN interfaces at runtime.

### 2. Enable and Start the Service

```bash
# Reload systemd to recognize the new service
sudo systemctl daemon-reload

# Enable the service to start on boot
sudo systemctl enable roconmqtt

# Start the service
sudo systemctl start roconmqtt
```

### 3. Verify the Service is Running

```bash
# Check service status
sudo systemctl status roconmqtt

# Follow live logs
sudo journalctl -u roconmqtt -f

# View recent logs
sudo journalctl -u roconmqtt -n 100
```

---

## Configuration

### Application Settings

For detailed MQTT configuration options including:
- Home Assistant auto-discovery
- Compound parameters
- Change detection thresholds
- TLS/SSL configuration
- Topic formats

See **[MQTT.md](MQTT.md)** for complete documentation.

### Environment Variables

The service runs in **Production** mode by default. To change the environment:

1. Edit the service file:
   ```bash
   sudo nano /etc/systemd/system/roconmqtt.service
   ```

2. Change the environment variable:
   ```ini
   Environment="ASPNETCORE_ENVIRONMENT=Development"
   ```

3. Reload and restart:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart roconmqtt
   ```

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
