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

## Architecture & Files to Create

### 1. **Dockerfile** (multi-stage)

Location: `Dockerfile` (repo root)

- **Build stage**: Uses `mcr.microsoft.com/dotnet/sdk:10.0` to publish the RoconMqtt.csproj using the `RaspberryPi-ARM64` publish profile
- **Runtime stage**: Uses `mcr.microsoft.com/dotnet/runtime-deps:10.0` (minimal, ~80 MB) to run the published binary
- Multi-stage keeps the final image small (~100–150 MB)

**Key points:**
- Publishes with `-c Release` and the existing `RaspberryPi-ARM64` profile (self-contained, trimmed)
- Sets `ASPNETCORE_URLS=http://+:5000` for the Kestrel endpoint
- Exposes port 5000
- Makes the binary executable

### 2. **.dockerignore** (repo root)

Excludes build artifacts, user files, and IDE files from the Docker build context.

```
bin/
obj/
.vscode/
.git/
*.user
*.suo
.DS_Store
```

### 3. **docker-compose.yml** (repo root; used by balena CLI)

Location: `docker-compose.yml` (repo root)

- **roconmqtt** service: Runs the .NET application with:
  - `network_mode: "host"` – Required to access host SocketCAN (`can0`)
  - `restart: always` – Auto-restart on crash
  - Environment variables for runtime config (ASPNETCORE_ENVIRONMENT, MQTT credentials)
  - Volume for persistent logs
  - Depends on `setup-can` service being ready

- **setup-can** service: Minimal Alpine container that:
  - Runs with `network_mode: "host"` and `privileged: true`
  - Brings up `can0` with bitrate 20000
  - Runs once (`restart: "no"`)
  - Executes before the main app starts

**Important**: Balena balenaCloud reads this file to orchestrate multi-service deployments; each service maps to a separate container on the device.

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

**Set these in balenaCloud dashboard (Environment Variables):**

```
ASPNETCORE_ENVIRONMENT=Production

Mqtt__Host=<your-mqtt-broker-ip>
Mqtt__Username=<mqtt-username>
Mqtt__Password=<mqtt-password>
Mqtt__ClientId=rocon-mqtt
Mqtt__Topic=rocon
Mqtt__PollingIntervalSeconds=30
Mqtt__ResponseTimeoutSeconds=1
Mqtt__ChangeThresholdPercent=1.0

Can__Ssh__Host=<can-device-ip>
Can__Ssh__Username=<ssh-user>
Can__Ssh__Password=<ssh-password>
```

**Or use SocketCAN directly** (no SSH) if the Raspberry Pi has CAN interfaceslocally attached:
- Leave `Can__Ssh__*` unset or empty
- The app will auto-detect `can0` and use SocketCAN via host networking

For full MQTT options (Home Assistant auto-discovery, TLS, etc.), see [MQTT.md](MQTT.md).

---

## Deployment Steps

### 1. Prepare Files Locally

Create the following files in the repo root (to be done in a future session):

- `Dockerfile` (multi-stage .NET build + runtime)
- `.dockerignore` (exclude build artifacts)
- `docker-compose.yml` (roconmqtt + setup-can services)

Reference the architecture section above for content guidelines.

### 2. Set Up Balena Project

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

### 3. Add Device to Fleet

- In balenaCloud dashboard, click "Add Device"
- Download and flash balenaOS to an SD card (balena CLI or Etcher)
- Power on the Pi; it will auto-register to your fleet within ~5 minutes
- Configure WiFi (if needed) via balenaCloud dashboard or QR code

### 4. Deploy Application

From the repo root:

```bash
# Push to balena (builds and deploys)
balena push roconmqtt

# Or: push to a specific device
balena push <device-uuid>
```

Balena builder (cloud) will:
1. Detect `docker-compose.yml`
2. Build both services (roconmqtt and setup-can) for ARM64
3. Deploy to the device(s)
4. Start the containers

### 5. Configure Fleet Environment Variables

In balenaCloud dashboard:

1. **Fleet Config** (for `BALENA_HOST_CONFIG_*`):
   - Set SPI and MCP2515 overlays (see **Configuration via Balena** section)
   - Devices will reboot automatically if needed

2. **Environment Variables**:
   - Set MQTT credentials, CAN settings, polling intervals, etc.
   - Containers will auto-restart and pick up new values

### 6. Monitor & Logs

In balenaCloud dashboard:

- **Device logs**: Real-time tail of all container logs
- **Service status**: See if roconmqtt and setup-can are running
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

1. Check device logs: "setup-can" service should log without errors
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

---

## Next Steps (Future Sessions)

1. **Create Dockerfile** – Multi-stage build using RaspberryPi-ARM64 profile
2. **Create .dockerignore** – Exclude build artifacts
3. **Create docker-compose.yml** – roconmqtt + setup-can services
4. **Test locally** (optional) – `docker-compose up` on a PC with Docker Desktop (requires Linux or WSL for full CAN testing)
5. **Create balenaCloud project** – Set up fleet and device type
6. **Deploy & monitor** – Push changes and verify logs in dashboard

---

## References

- [RoconMqtt Deployment Guide](DEPLOYMENT.md)
- [RoconMqtt Hardware Setup](HARDWARE.md)
- [MQTT Configuration](MQTT.md)
- [Balena Documentation](https://www.balena.io/docs/)
- [Balena Environment Variables](https://www.balena.io/docs/learn/manage/variables/)
- [Docker Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)

---

## Summary

| Component | Purpose |
|-----------|---------|
| **Dockerfile** | Multi-stage build: SDK stage publishes .NET app, runtime stage runs the binary |
| **.dockerignore** | Excludes build artifacts for faster Docker builds |
| **docker-compose.yml** | Defines roconmqtt and setup-can services; balena uses this to deploy both |
| **BALENA_HOST_CONFIG_*** | Configure `/boot/firmware/config.txt` (SPI, MCP2515 overlays) at fleet level |
| **Environment Variables** | Set MQTT credentials, CAN settings, and polling intervals per device or fleet |

Once the Docker files are created, deployment is as simple as:
```bash
balena push roconmqtt
```
