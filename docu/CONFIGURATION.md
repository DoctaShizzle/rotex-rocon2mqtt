# Configuration Guide

This document explains how to configure RoconMqtt for different deployment scenarios.

---

## Overview

RoconMqtt supports two deployment modes:
1. **Bare metal** (direct on Raspberry Pi or Linux machine) - uses `appsettings.json`
2. **Containerized** (Docker, Balena) - uses environment variables to override `appsettings.json`

Both approaches can be combined: environment variables override values in `appsettings.json`.

---

## Configuration Hierarchy

.NET's configuration system merges settings from multiple sources in this order (later sources override earlier ones):

1. **`appsettings.json`** - Base configuration file with defaults
2. **`appsettings.{Environment}.json`** - Environment-specific overrides (Development, Production, etc.)
3. **Environment variables** - Highest priority, perfect for secrets and deployment-specific settings

This means you can:
- Keep stable defaults in `appsettings.json`
- Override sensitive values (passwords, IPs) with environment variables
- Use different configurations per environment without modifying code

---

## Environment Variable Format

Environment variables use **double underscores** (`__`) to represent nested JSON hierarchy:

```bash
# appsettings.json
{
  "Mqtt": {
    "Host": "192.168.0.254",
    "Port": 1883
  }
}

# Environment variable equivalent
Mqtt__Host=192.168.0.254
Mqtt__Port=1883
```

For arrays, use indexes:
```bash
Mqtt__Devices__0=HG1
Mqtt__Devices__1=HC1
```

---

## Machine Types and CAN Registry Files

RoconMqtt uses **machine-type-specific registry files** to support different Rotex/Daikin heat pump models.

### Configuration

Set the machine type in `appsettings.json` or via environment variable:

**appsettings.json:**
```json
{
  "Can": {
    "MachineType": "EHSHXXPXXA"
  }
}
```

**Environment variable:**
```bash
Can__MachineType=EHSHXXPXXA
```

### Registry File Naming

The application loads `can-registry-{MachineType}.json`:
- `Can__MachineType=EHSHXXPXXA` → loads `can-registry-EHSHXXPXXA.json`
- `Can__MachineType=altherma-ehbh` → loads `can-registry-altherma-ehbh.json`

### Registry File Structure

Each registry file contains:
- **Machine metadata** (type, description)
- **Device list** (HG1, HC1, etc.)
- **MQTT parameters** (which parameters to publish)
- **MQTT compound parameters** (calculated values like Timestamp)
- **CAN parameter definitions** (info numbers, data types, Home Assistant config)

**Example registry file header:**
```json
{
  "machineType": "EHSHXXPXXA",
  "machineDescription": "Daikin Altherma EHSH Heat Pump",
  "devices": ["HG1", "HC1", "HCM1"],
  "mqttParameters": [
    "OutdoorTemperature",
    "BufferTemperatureActual",
    ...
  ],
  "mqttCompoundParameters": ["Timestamp"],
  "parameters": [...]
}
```

**Benefits:**
- One registry file per heat pump model
- Easy to add new models without code changes
- Machine-specific parameters stay with the machine definition
- Deployment config (MQTT host, credentials) separate from machine config

---

## Configuration Sections

### 1. Kestrel (Web Server)

**Purpose:** Configure the HTTP endpoint for the REST API.

| Setting | Default | Environment Variable | Description |
|---------|---------|---------------------|-------------|
| `Kestrel:Endpoints:Http:Url` | `http://0.0.0.0:5000` | `Kestrel__Endpoints__Http__Url` | HTTP endpoint URL |

**Example:**
```bash
# Bind to all interfaces on port 5000
Kestrel__Endpoints__Http__Url=http://0.0.0.0:5000

# Bind to localhost only
Kestrel__Endpoints__Http__Url=http://127.0.0.1:5000
```

---

### 2. CAN Configuration

**Purpose:** Configure CAN bus access (local SocketCAN or remote SSH).

| Setting | Required | Default | Environment Variable | Description |
|---------|----------|---------|---------------------|-------------|
| `Can:MachineType` | ✅ | - | `Can__MachineType` | Machine type identifier (e.g., `EHSHXXPXXA`) |
| `Can:CanInterfaceName` | ✅ | `can0` | `Can__CanInterfaceName` | CAN interface name for SocketCAN |
| `Can:TimeZoneId` | ❌ | System | `Can__TimeZoneId` | IANA timezone for timestamps |
| `Can:Ssh:Host` | ❌ | - | `Can__Ssh__Host` | SSH host for remote CAN (leave empty for local) |
| `Can:Ssh:Port` | ❌ | `22` | `Can__Ssh__Port` | SSH port |
| `Can:Ssh:Username` | ❌ | - | `Can__Ssh__Username` | SSH username |
| `Can:Ssh:Password` | ❌ | - | `Can__Ssh__Password` | SSH password |

**SocketCAN (local) example:**
```bash
Can__MachineType=EHSHXXPXXA
Can__CanInterfaceName=can0
Can__TimeZoneId=Europe/Brussels
```

**SSH CAN (remote) example:**
```bash
Can__MachineType=EHSHXXPXXA
Can__Ssh__Host=192.168.0.100
Can__Ssh__Username=pi
Can__Ssh__Password=raspberry
```

---

### 3. MQTT Configuration

**Purpose:** Configure MQTT broker connection and publishing behavior.

| Setting | Required | Default | Environment Variable | Description |
|---------|----------|---------|---------------------|-------------|
| `Mqtt:Enabled` | ❌ | `true` | `Mqtt__Enabled` | Enable/disable MQTT publisher |
| `Mqtt:Host` | ✅ | - | `Mqtt__Host` | MQTT broker hostname/IP |
| `Mqtt:Port` | ❌ | `1883` | `Mqtt__Port` | MQTT broker port |
| `Mqtt:UseTls` | ❌ | `false` | `Mqtt__UseTls` | Enable TLS/SSL |
| `Mqtt:ValidateCertificate` | ❌ | `true` | `Mqtt__ValidateCertificate` | Validate TLS certificate |
| `Mqtt:ClientId` | ✅ | - | `Mqtt__ClientId` | MQTT client identifier |
| `Mqtt:Username` | ❌ | - | `Mqtt__Username` | MQTT username |
| `Mqtt:Password` | ❌ | - | `Mqtt__Password` | MQTT password |
| `Mqtt:Topic` | ✅ | - | `Mqtt__Topic` | MQTT topic prefix |
| `Mqtt:PollingIntervalSeconds` | ❌ | `30` | `Mqtt__PollingIntervalSeconds` | How often to poll CAN parameters |
| `Mqtt:ResponseTimeoutSeconds` | ❌ | `5` | `Mqtt__ResponseTimeoutSeconds` | CAN response timeout |
| `Mqtt:ChangeThresholdPercent` | ❌ | `0` | `Mqtt__ChangeThresholdPercent` | Only publish if value changes by % |
| `Mqtt:TimeZoneId` | ❌ | System | `Mqtt__TimeZoneId` | IANA timezone for timestamps |

**Example:**
```bash
Mqtt__Enabled=true
Mqtt__Host=192.168.0.254
Mqtt__Port=1883
Mqtt__ClientId=rocon-mqtt
Mqtt__Username=rocon
Mqtt__Password=your-password
Mqtt__Topic=rocon
Mqtt__PollingIntervalSeconds=30
Mqtt__ResponseTimeoutSeconds=5
Mqtt__ChangeThresholdPercent=2.0
Mqtt__TimeZoneId=Europe/Brussels
```

#### MQTT Devices and Parameters

**Important:** `Devices`, `Parameters`, and `CompoundParameters` are now defined in the **CAN registry file** (e.g., `can-registry-EHSHXXPXXA.json`), not in `appsettings.json`.

This means:
- Each machine type has its own parameter list
- You don't need to manually configure which parameters to publish
- Parameters are tied to the machine type, not the deployment

**To override** (optional, rarely needed):
```bash
# Override devices
Mqtt__Devices__0=HG1
Mqtt__Devices__1=HC1

# Override parameters (long list, better to edit registry file)
Mqtt__Parameters__0=OutdoorTemperature
Mqtt__Parameters__1=BufferTemperatureActual
```

---

### 4. Home Assistant MQTT Discovery

**Purpose:** Auto-configure sensors in Home Assistant via MQTT discovery.

| Setting | Default | Environment Variable | Description |
|---------|---------|---------------------|-------------|
| `Mqtt:HomeAssistant:Discovery` | `true` | `Mqtt__HomeAssistant__Discovery` | Enable HA discovery |
| `Mqtt:HomeAssistant:DiscoveryPrefix` | `homeassistant` | `Mqtt__HomeAssistant__DiscoveryPrefix` | HA discovery prefix |

Most HA settings have sensible defaults. See [MQTT.md](MQTT.md) for full list.

---

## Deployment Scenarios

### Scenario 1: Bare Metal (Raspberry Pi)

**Configuration approach:** Use `appsettings.json` with sensitive values in environment variables.

**appsettings.json:**
```json
{
  "Can": {
    "MachineType": "EHSHXXPXXA",
    "CanInterfaceName": "can0"
  },
  "Mqtt": {
    "Enabled": true,
    "Host": "192.168.0.254",
    "Port": 1883,
    "ClientId": "rocon-mqtt",
    "Topic": "rocon"
  }
}
```

**Environment variables (set in systemd service or shell):**
```bash
export Mqtt__Username=rocon
export Mqtt__Password=your-password
```

**Run:**
```bash
./RoconMqtt
```

---

### Scenario 2: Docker Compose (Local Testing)

**Configuration approach:** Use `.env` file for all deployment-specific settings.

**Create `.env` file** (copy from `.env.example`):
```bash
Can__MachineType=EHSHXXPXXA
Mqtt__Host=192.168.0.254
Mqtt__Username=rocon
Mqtt__Password=your-password
```

**Run:**
```bash
docker-compose up
```

Docker Compose automatically reads the `.env` file.

---

### Scenario 3: Balena (Production)

**Configuration approach:** Set Fleet or Device environment variables in balenaCloud dashboard.

1. **Fleet Configuration** (hardware, same for all devices):
   ```
   BALENA_HOST_CONFIG_dtparam=spi=on
   BALENA_HOST_CONFIG_dtoverlay=mcp2515-can0,oscillator=16000000,interrupt=25
   ```

2. **Fleet or Device Environment Variables** (deployment-specific):
   ```
   ASPNETCORE_ENVIRONMENT=Production
   Can__MachineType=EHSHXXPXXA
   Mqtt__Host=192.168.0.254
   Mqtt__Username=rocon
   Mqtt__Password=your-password
   ```

3. **Deploy:**
   ```bash
   balena push my-fleet-name
   ```

See [BALENA_DEPLOYMENT.md](BALENA_DEPLOYMENT.md) for detailed Balena setup.

---

## Security Best Practices

### 1. Secrets Management

**❌ DO NOT:**
- Commit passwords to Git
- Store credentials in `appsettings.json` in version control

**✅ DO:**
- Use environment variables for all secrets (MQTT password, SSH password)
- Use `.env` file for local development (add to `.gitignore`)
- Use Balena environment variables for cloud deployment
- Use systemd `EnvironmentFile` for bare metal production

### 2. Example `.gitignore`

```
.env
appsettings.Development.json
appsettings.Production.json
```

Keep `appsettings.json` with default/example values only.

---

## Troubleshooting

### "Configuration section 'Can' is missing"

**Cause:** `appsettings.json` doesn't have a `Can` section.

**Fix:** Ensure `appsettings.json` contains:
```json
{
  "Can": {
    "MachineType": "EHSHXXPXXA",
    "CanInterfaceName": "can0"
  }
}
```

### "Either 'Can:MachineType' or 'Can:RegistryFilePath' must be configured"

**Cause:** Neither `Can:MachineType` nor `Can:RegistryFilePath` is set.

**Fix:** Set `Can__MachineType` environment variable or add to `appsettings.json`.

### "File not found: can-registry-{type}.json"

**Cause:** Registry file for the specified machine type doesn't exist.

**Fix:** 
- Check spelling of `Can:MachineType`
- Ensure `can-registry-{type}.json` exists in the application directory
- Available types: `EHSHXXPXXA` (more coming soon)

### Environment variables not taking effect

**Cause:** Incorrect format or application not restarted.

**Fix:**
- Use double underscores: `Mqtt__Host`, not `Mqtt:Host`
- Restart the application after changing environment variables
- For Docker: recreate containers (`docker-compose up --force-recreate`)

---

## Reference: All Configuration Settings

See individual documentation files for complete details:
- [CAN.md](CAN.md) - CAN bus configuration
- [MQTT.md](MQTT.md) - MQTT and Home Assistant configuration
- [DEPLOYMENT.md](DEPLOYMENT.md) - Bare metal deployment
- [BALENA_DEPLOYMENT.md](BALENA_DEPLOYMENT.md) - Balena deployment

---

## Summary

| Deployment Mode | Configuration Method |
|----------------|---------------------|
| **Bare metal** | `appsettings.json` + environment variables for secrets |
| **Docker Compose** | `.env` file (auto-loaded) |
| **Balena** | Fleet/Device environment variables in balenaCloud dashboard |

All modes support environment variables to override `appsettings.json` - use what works best for your workflow!
