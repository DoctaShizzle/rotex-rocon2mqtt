# RoconMqtt Linux Deployment Guide

## Quick Start - Debug Mode

```bash
chmod +x debug.sh
./debug.sh
```

This will:
- Set `ASPNETCORE_ENVIRONMENT=Development`
- Run in Debug configuration
- Output all Debug logs from RoconMqtt namespace
- Display output in the terminal

---

## Production Mode - Manual

```bash
chmod +x run-production.sh
./run-production.sh
```

This will:
- Set `ASPNETCORE_ENVIRONMENT=Production`
- Run the Release binary
- Log only Warning level and above
- Create logs directory if needed

---

## Production Mode - Systemd Service (Recommended)

### 1. Build the Release Binary

```bash
dotnet publish -c Release -o bin/Release
```

### 2. Copy Files to Production Location

```bash
sudo mkdir -p /opt/rocon-mqtt
sudo cp -r bin/Release/net10.0/* /opt/rocon-mqtt/
sudo cp run-production.sh /opt/rocon-mqtt/
sudo cp src/RoconMqtt/appsettings*.json /opt/rocon-mqtt/

# Make scripts executable
sudo chmod +x /opt/rocon-mqtt/run-production.sh
```

### 3. Create Application User (Optional but Recommended)

```bash
sudo useradd -r -s /bin/false rocon
sudo chown -R rocon:rocon /opt/rocon-mqtt
```

### 4. Install Systemd Service

```bash
sudo cp rocon-mqtt.service /etc/systemd/system/
sudo systemctl daemon-reload
```

### 5. Manage the Service

```bash
# Start the service
sudo systemctl start rocon-mqtt

# Enable auto-start on boot
sudo systemctl enable rocon-mqtt

# Check status
sudo systemctl status rocon-mqtt

# View logs (real-time)
sudo journalctl -u rocon-mqtt -f

# View logs (last 100 lines)
sudo journalctl -u rocon-mqtt -n 100

# Stop the service
sudo systemctl stop rocon-mqtt

# Restart the service
sudo systemctl restart rocon-mqtt
```

---

## Directory Structure (Production)

```
/opt/rocon-mqtt/
??? RoconMqtt              # Main executable
??? *.dll                  # Dependency libraries
??? appsettings.json       # Base configuration
??? appsettings.Production.json
??? run-production.sh      # Production startup script
??? logs/
    ??? rocon-mqtt-YYYYMMDD.txt
```

---

## Troubleshooting

### Check if service is running
```bash
sudo systemctl status rocon-mqtt
```

### View recent errors
```bash
sudo journalctl -u rocon-mqtt -n 50 --no-pager
```

### Check permissions
```bash
ls -la /opt/rocon-mqtt/
```

### Restart after configuration changes
```bash
sudo systemctl restart rocon-mqtt
```

### Check port bindings (if applicable)
```bash
sudo netstat -tlnp | grep rocon
```

---

## File Permissions

After deployment, verify permissions:

```bash
# Scripts should be executable
sudo chmod +x /opt/rocon-mqtt/run-production.sh
sudo chmod +x /opt/rocon-mqtt/RoconMqtt

# Configuration and logs directory writable
sudo chmod 755 /opt/rocon-mqtt
sudo chmod 755 /opt/rocon-mqtt/logs
```

---

## Environment Variables

Both scripts set `ASPNETCORE_ENVIRONMENT`:
- **Debug mode**: `Development`
- **Production mode**: `Production`

To override or add additional variables:

**For manual runs:**
```bash
export MY_VAR=value
./run-production.sh
```

**For systemd service:**
Edit `/etc/systemd/system/rocon-mqtt.service` and add:
```ini
Environment="MY_VAR=value"
Environment="ANOTHER_VAR=another_value"
```

Then reload and restart:
```bash
sudo systemctl daemon-reload
sudo systemctl restart rocon-mqtt
```

---

## Logs Location

- **Debug mode**: Console output + `logs/` directory
- **Production mode**: `logs/` directory (configured in appsettings.Production.json)
- **Systemd service**: `journalctl` (systemd journal)

View combined logs:
```bash
sudo journalctl -u rocon-mqtt -f
```
