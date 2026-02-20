# Balena Deployment Guide

This document outlines the plan to deploy RoconMqtt to Raspberry Pi using **balena.io**.

---

## Overview

Balena simplifies deployment and management of containerized applications on Raspberry Pi and other edge devices. This guide covers:

- Building a multi-stage Docker image for the .NET 10 application
- Configuring CAN interfaces via balena's host config system
- Setting up MQTT credentials and other runtime secrets
- Deploying to a fleet of devices

---

## Prerequisites

- A balena account (free tier available at https://balena.io)
- A Raspberry Pi 3B+ or newer (or other supported balena device type)
- Balena CLI installed (`npm install -g balena-cli`)
- MCP2515-based CAN HAT (or USB CAN adapter) installed on the Pi
- The git repository cloned locally

---

## Architecture & Files

The repository includes all necessary Docker and Balena files:

### 1. **Dockerfile** (multi-stage)

Location: `Dockerfile` (repo root)

- **Build stage**: Uses `mcr.microsoft.com/dotnet/sdk:10.0` to publish the RoconMqtt.csproj using the `RaspberryPi-ARM64` publish profile
- **Runtime stage**: Uses `mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine` (minimal, ~80 MB) to run the published binary
- Multi-stage keeps the final image small (~100–150 MB)

**Key features:**
- Publishes with `-c Release` and the existing `RaspberryPi-ARM64` profile (self-contained, trimmed)
- Sets `ASPNETCORE_URLS=http://+:5000` for the Kestrel endpoint
- Exposes port 5000
- Makes the binary executable

### 2. **.dockerignore** (repo root)

Excludes build artifacts, user files, and IDE files from the Docker build context for faster builds.

### 3. **docker-compose.yml** (repo root)

Defines the **roconmqtt** service:

- **roconmqtt** service: Runs the .NET application with:
  - `network_mode: "host"` – Required to access host SocketCAN (`can0`)
  - `cap_add: [NET_ADMIN]` – Allows the container to configure network interfaces (CAN bitrate setup)
  - `entrypoint` – Runs `ip link set can0 up type can bitrate 20000` before starting the app
  - `restart: always` – Auto-restart on crash
  - Environment variables for runtime config (ASPNETCORE_ENVIRONMENT, MQTT credentials, CAN settings)
  - Healthcheck endpoint

**Note**: CAN interface setup (`can0` bring-up) is handled by the container's entrypoint, not a separate service.

### 4. **.env.example** (repo root)

Example environment variable configuration for local Docker Compose testing. Copy to `.env` and fill in your values.

---

## Configuration via Balena

### Device Tree & Host Config (Fleet-level)

Balena provides `BALENA_HOST_CONFIG_*` environment variables to dynamically configure `/boot/firmware/config.txt` without manual SSH.

**Set these in balenaCloud dashboard (Fleet Configuration):**

```
BALENA_HOST_CONFIG_dtparam=spi=on
BALENA_HOST_CONFIG_dtoverlay=mcp2515-can0,oscillator=16000000,interrupt=25
```

This will:
1. Enable SPI on all devices in the fleet
2. Load the MCP2515 device tree overlay for the CAN channel
3. Auto-reboot if necessary (is non-disruptive)
4. Persist across device restarts

**Hardware notes:**
- Replace `interrupt=25` if your HAT uses a different GPIO pin
- See [HARDWARE.md](HARDWARE.md) for wiring and termination details

### Application & MQTT Config (Device or Fleet level)

**Copy-paste these environment variables into balenaCloud dashboard:**

Go to your fleet in balenaCloud → **Fleet Configuration** → **Fleet Variables** (or Device Variables for device-specific settings), and add:

#### Required Environment Variables

Copy this block and add each line as a separate variable in balenaCloud:

```
ASPNETCORE_ENVIRONMENT=Production
Can__MachineType=EHSHXXPXXA
Mqtt__Host=192.168.0.254
Mqtt__Username=rocon
Mqtt__Password=your-mqtt-password-here
```

**Change these values:**
- `Mqtt__Host`: Your MQTT broker IP address or hostname
- `Mqtt__Username`: Your MQTT broker username
- `Mqtt__Password`: Your MQTT broker password
- `Can__MachineType`: Machine type (use `EHSHXXPXXA` for Daikin Altherma EHSH heat pumps)

#### Optional Environment Variables (uncomment/add if needed)

**MQTT Configuration:**
```
Mqtt__Port=1883
Mqtt__ClientId=rocon-mqtt
Mqtt__Topic=rocon
Mqtt__PollingIntervalSeconds=30
Mqtt__ResponseTimeoutSeconds=5
Mqtt__ChangeThresholdPercent=2.0
Mqtt__TimeZoneId=Europe/Brussels
```

**CAN Configuration:**
```
Can__CanInterfaceName=can0
Can__TimeZoneId=Europe/Brussels
```

**Kestrel Web Server:**
```
Kestrel__Endpoints__Http__Url=http://0.0.0.0:5000
```

**SSH CAN (for remote CAN bus access):**
```
Can__Ssh__Host=192.168.0.100
Can__Ssh__Port=22
Can__Ssh__Username=pi
Can__Ssh__Password=your-ssh-password
```

**Note:** If SSH CAN variables are NOT set, the application uses local SocketCAN (`can0` on the device itself).

**SocketCAN (local)** is used by default if `Can__Ssh__*` variables are not set. The app will use the CAN interface specified by `Can__CanInterfaceName` (default: `can0`).

### Machine Types

The `Can__MachineType` setting determines which CAN registry file to load:
- `EHSHXXPXXA` → `can-registry-EHSHXXPXXA.json` (Daikin Altherma EHSH Heat Pump)
- More types will be added for other Rotex/Daikin models

Each registry file contains machine-specific configuration:
- Device list (HG1, HC1, HCM1, etc.)
- MQTT parameters to publish
- CAN parameter definitions
- Home Assistant metadata

This means you **don't need to configure** `Mqtt__Devices__*` or `Mqtt__Parameters__*` - they're defined per machine type in the registry.

For full configuration reference, see [CONFIGURATION.md](CONFIGURATION.md).

---

## Deployment Steps

### 1. Set Up Balena Project

```bash
# Install balena CLI
npm install -g balena-cli

# Log in
balena login

# Create a new application in balenaCloud
# - Name: e.g., "roconmqtt"
# - Device type: e.g., "raspberrypi4-64" (choose your Pi model)
# - Default branch: main (or your preferred branch)

# Verify the app exists
balena apps
```

### 2. Add Device to Fleet

- In balenaCloud dashboard, click "Add Device"
- Download and flash balenaOS to an SD card (balena CLI or Etcher)
- Power on the Pi; it will auto-register to your fleet within ~5 minutes
- Configure WiFi (if needed) via balenaCloud dashboard or QR code

### 3. Deploy Application

From the repo root:

```bash
# Push to balena (builds and deploys)
balena push roconmqtt

# Or: push to a specific device
balena push <device-uuid>
```

Balena builder (cloud) will:
1. Detect `docker-compose.yml`
2. Build the roconmqtt service for ARM64
3. Deploy to the device(s)
4. Start the container with CAN setup in the entrypoint

### 4. Configure Fleet Environment Variables

In balenaCloud dashboard:

1. **Fleet Config** (for `BALENA_HOST_CONFIG_*`):
   - Set SPI and MCP2515 overlays (see **Configuration via Balena** section)
   - Devices will reboot automatically if needed

2. **Environment Variables**:
   - Set MQTT credentials, CAN settings, polling intervals, etc.
   - The container will auto-restart and pick up new values

### 5. Monitor & Logs

In balenaCloud dashboard:

- **Device logs**: Real-time tail of container logs
- **Service status**: See if roconmqtt is running
- **Device metrics**: CPU, memory, network usage
- **Terminal access**: SSH directly into a container for debugging

Or via CLI:

```bash
# View logs
balena logs <device-uuid> -s roconmqtt

# SSH into the device (host OS)
balena ssh <device-uuid>

# SSH into a container
balena ssh <device-uuid> -s roconmqtt
```

---

## Security Considerations

### Secrets Management

**Do NOT** store MQTT passwords or CAN SSH credentials in the repository.

- Use balenaCloud **Fleet or Device environment variables** for all secrets
- Balena encrypts environment variables in transit and at rest
- Rotate credentials by updating environment variables in the dashboard

### Network Access

- By default, port 5000 (Kestrel) is **not** exposed outside the device
- To expose it for monitoring/health checks, add a firewall rule in balenaCloud (optional)
- CAN access is internal to the host; requires host networking mode (which the setup uses)

### User Permissions

- The `roconmqtt` service runs as root by default in the container
- To run as a non-root user, add a `USER` directive in the Dockerfile and ensure CAN group permissions are set (balena udev rules or similar)
- For now, root is acceptable if the device is on a private network

---

## Troubleshooting

### CAN Interfaces Not Appearing

1. Check device logs: "roconmqtt" service should start with CAN setup (or fail gracefully if can0 doesn't exist)
2. Verify `BALENA_HOST_CONFIG_*` variables are set in balenaCloud
3. Manually SSH into the device and run:
   ```bash
   ip link show can0
   dmesg | grep -i can
   ```

### Application Not Starting

1. Check roconmqtt service logs in balenaCloud dashboard
2. Verify MQTT credentials and host are correct in environment variables
3. Check CAN connectivity: if using SSH, ensure Can__Ssh__* are set correctly

### Port 5000 Not Reachable

The Kestrel server binds to `http://+:5000` inside the container and is accessible on `http://<device-ip>:5000` (if on the same network). To verify:

```bash
balena ssh <device-uuid> -s roconmqtt
curl http://localhost:5000/health
```

### Logs Filling Up

Mount a persistent volume for `/app/logs` in `docker-compose.yml` and manage log rotation via Serilog configuration. See [DEPLOYMENT.md](DEPLOYMENT.md#application-log-files).

### Machine Type Not Found

**Error:** `File not found: can-registry-{type}.json`

**Cause:** The specified `Can__MachineType` doesn't have a corresponding registry file.

**Fix:**
- Verify the machine type is correct (e.g., `EHSHXXPXXA`)
- Ensure the registry file exists in the image (check build logs)
- Available types: `EHSHXXPXXA` (more coming soon)

---

## Local Testing (Optional)

Before deploying to Balena, you can test the Docker setup locally:

1. **Copy `.env.example` to `.env`** and fill in your values:
   ```bash
   cp .env.example .env
   # Edit .env with your MQTT broker details
   ```

2. **Build and run:**
   ```bash
   docker-compose up --build
   ```

3. **Test the API:**
   ```bash
   curl http://localhost:5000/health
   ```

**Note:** CAN interface setup will run in the entrypoint (and may fail gracefully on non-Linux systems), but the app will start. On Linux/WSL with a CAN interface available, `can0` will be brought up automatically.

---

## References

- [Configuration Guide](CONFIGURATION.md) - Comprehensive configuration reference
- [RoconMqtt Deployment Guide](DEPLOYMENT.md) - Bare metal deployment
- [RoconMqtt Hardware Setup](HARDWARE.md) - CAN HAT wiring and setup
- [MQTT Configuration](MQTT.md) - MQTT and Home Assistant details
- [Balena Documentation](https://www.balena.io/docs/)
- [Balena Environment Variables](https://www.balena.io/docs/learn/manage/variables/)
- [Docker Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)

---

## Summary

| Component | Purpose |
|-----------|---------|
| **Dockerfile** | Multi-stage build: SDK stage publishes .NET app, runtime stage runs the binary |
| **.dockerignore** | Excludes build artifacts for faster Docker builds |
| **docker-compose.yml** | Defines roconmqtt service with NET_ADMIN capabilities; entrypoint sets up CAN interface |
| **.env.example** | Example environment variables for local Docker Compose testing |
| **can-registry-{type}.json** | Machine-specific CAN parameters and MQTT config (per heat pump model) |
| **BALENA_HOST_CONFIG_*** | Configure `/boot/firmware/config.txt` (SPI, MCP2515 overlays) at fleet level |
| **Environment Variables** | Set machine type, MQTT credentials, CAN settings per device or fleet |

Deployment is as simple as:
```bash
balena push roconmqtt
```

All configuration is managed through balenaCloud dashboard - no manual file editing needed!
