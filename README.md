# Rotex Rocon G1 to MQTT Bridge

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A high-performance .NET service that bridges Rotex/Daikin Rocon G1 heating controllers to MQTT with automatic Home Assistant discovery support.

## ?? Features

- **CAN Bus Communication** - Native SocketCAN or remote SSH access to CAN interfaces
- **MQTT Publishing** - Automatic polling and publishing of heating system parameters
- **Home Assistant Integration** - Auto-discovery with device grouping and proper entity types
- **Compound Parameters** - Synthetic parameters combining multiple values (e.g., timestamp from date/time components)
- **Smart Change Detection** - Configurable threshold to reduce unnecessary MQTT traffic
- **Resilience** - Built-in retry logic, circuit breakers, and timeout handling with Polly
- **REST API** - Optional HTTP endpoints for manual queries and monitoring
- **Comprehensive Parameter Registry** - ~4,800 pre-configured parameters with metadata

## ?? Table of Contents

- [Quick Start](#quick-start)
- [Documentation](#documentation)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## ?? Quick Start

### Prerequisites

- .NET 10 SDK
- CAN interface (Raspberry Pi with CAN HAT or remote SSH access)
- MQTT broker (Mosquitto, HiveMQ, etc.)
- Rotex/Daikin Rocon G1 heating controller

### Installation

```bash
# Clone the repository
git clone https://github.com/DoctaShizzle/rotex-rocon2mqtt.git
cd rotex-rocon2mqtt

# Build the application
cd src/RoconMqtt
dotnet build

# Configure (edit appsettings.json)
nano appsettings.json

# Run
dotnet run
```

### Minimal Configuration

```json
{
  "Mqtt": {
    "Host": "192.168.1.100",
    "ClientId": "rocon-mqtt",
    "Topic": "rocon",
    "Devices": ["HG1"],
    "Parameters": ["cAUSSENTEMP", "cVORLAUFTEMP"]
  },
  "Can": {
    "CanInterfaceName": "can0"
  }
}
```

## ?? Documentation

Comprehensive documentation is available in the `docu/` folder:

### Core Documentation

| Document | Description |
|----------|-------------|
| **[MQTT.md](docu/MQTT.md)** | MQTT publisher configuration, Home Assistant integration, compound parameters, change detection |
| **[CAN.md](docu/CAN.md)** | CAN bus protocol, frame structure, parameter encoding/decoding, device subsystems |
| **[API.md](docu/API.md)** | REST API endpoints, request/response formats, examples |

### Setup & Deployment

| Document | Description |
|----------|-------------|
| **[DEPLOYMENT.md](docu/DEPLOYMENT.md)** | Deployment guide for Raspberry Pi, systemd service setup, monitoring |
| **[HARDWARE.md](docu/HARDWARE.md)** | Raspberry Pi CAN HAT setup, wiring, interface configuration |
| **[SSH_CAN_BUS.md](docu/SSH_CAN_BUS.md)** | Remote CAN access via SSH for development and testing |

## ??? System Requirements

### Development

- .NET 10 SDK
- Windows, Linux, or macOS

### Production (Raspberry Pi)

- Raspberry Pi 3B/4/5 (or compatible ARM64 Linux system)
- Debian/Ubuntu-based OS (Raspberry Pi OS recommended)
- CAN interface hardware (dual-channel MCP2515-based HAT recommended)
- Network access to MQTT broker

## ?? Installation

### Local Development

See [Quick Start](#quick-start) above.

### Raspberry Pi Deployment

Detailed step-by-step instructions are available in **[DEPLOYMENT.md](docu/DEPLOYMENT.md)**, including:

- Building for ARM64
- Transferring files
- systemd service configuration
- Log monitoring
- Security best practices

**Quick Deploy:**

```bash
# Build for Raspberry Pi (run on development machine)
.\scripts\publish-raspi.ps1

# Transfer to Pi and install
rsync -avz src/RoconMqtt/bin/publish/linux-arm64/ pi@raspberrypi:/tmp/roconmqtt/
ssh pi@raspberrypi
sudo mv /tmp/roconmqtt/* /opt/roconmqtt/
sudo systemctl start roconmqtt
```

## ?? Configuration

Configuration is managed through `appsettings.json`. See individual documentation files for detailed options:

- **MQTT Options** - See [MQTT.md](docu/MQTT.md#configuration)
- **CAN Options** - See [CAN.md](docu/CAN.md#configuration) and [SSH_CAN_BUS.md](docu/SSH_CAN_BUS.md#configuration)
- **API Options** - See [API.md](docu/API.md#configuration)

### Key Configuration Sections

```json
{
  "Mqtt": {
    "Enabled": true,
    "Host": "mqtt-broker.local",
    "Port": 1883,
    "ClientId": "rocon-mqtt",
    "Topic": "rocon",
    "Devices": ["HG1", "HC1"],
    "Parameters": ["cAUSSENTEMP", "cVORLAUFTEMP", "cRUECKLAUFTEMP"],
    "CompoundParameters": ["Timestamp"],
    "PollingIntervalSeconds": 30,
    "ChangeThresholdPercent": 1.0,
    "HomeAssistant": {
      "Discovery": true,
      "DiscoveryPrefix": "homeassistant"
    }
  },
  "Can": {
    "CanInterfaceName": "can0",
    "Ssh": {
      "Host": "192.168.1.250",
      "Username": "pi",
      "Password": "your-password"
    }
  }
}
```

## ?? Usage

### Running the Service

```bash
# Development mode (verbose logging)
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Production mode
dotnet run

# As systemd service (Raspberry Pi)
sudo systemctl start roconmqtt
sudo systemctl enable roconmqtt
```

### Monitoring

```bash
# View logs (systemd)
sudo journalctl -u roconmqtt -f

# View application logs
tail -f /opt/roconmqtt/logs/rocon-mqtt-$(date +%Y-%m-%d).txt

# Check service status
sudo systemctl status roconmqtt
```

### REST API

If enabled, the REST API is available at `http://localhost:5000`:

- Swagger UI: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/health`

```bash
# Query a parameter
curl "http://localhost:5000/api/parameters/get?DeviceName=HG1&ParameterName=cAUSSENTEMP"

# Set a parameter
curl -X POST "http://localhost:5000/api/parameters/set" \
  -H "Content-Type: application/json" \
  -d '{"deviceName":"HG1","parameterName":"cEINSTELL_SPEICHERSOLLTEMP","value":48.0}'
```

See **[API.md](docu/API.md)** for complete API documentation.

## ?? Home Assistant Integration

The service automatically publishes MQTT discovery messages for Home Assistant:

1. **Enable Discovery** in configuration:
   ```json
   "HomeAssistant": {
     "Discovery": true
   }
   ```

2. **Entities Auto-Created** - All configured parameters appear as entities
3. **Device Grouping** - Parameters grouped by device in Home Assistant UI
4. **Proper Metadata** - Units, device classes, state classes from registry

**Example entities created:**
- `sensor.rocon_12345678_caussententp` - Outside Temperature
- `sensor.rocon_12345678_cvorlauftemp` - Flow Temperature
- `sensor.rocon_12345678_timestamp` - System Timestamp

See **[MQTT.md](docu/MQTT.md#home-assistant-integration)** for detailed integration guide.

## ?? Architecture

```
???????????????????????
?  RoconMqttPublisher ?  Background Service
?   (Polling Loop)    ?
???????????????????????
           ?
           ???????????????????
           ?                 ?
    ???????????????   ???????????????
    ?  CanService ?   ? MqttService  ?
    ???????????????   ????????????????
           ?                 ?
    ???????????????????      ?
    ? CanParameterReg ?      ?
    ? CanEncoder      ?      ?
    ? CanDecoder      ?      ?
    ???????????????????      ?
           ?                 ?
    ???????????????   ????????????????
    ?  CAN Bus    ?   ? MQTT Broker  ?
    ? (SocketCAN/ ?   ?              ?
    ?    SSH)     ?   ?              ?
    ???????????????   ????????????????
           ?                 ?
    ???????????????   ??????????????????
    ? Rocon G1    ?   ? Home Assistant ?
    ? Controller  ?   ?                ?
    ???????????????   ??????????????????
```

## ??? Technology Stack

- **.NET 10.0** - Runtime framework
- **MQTTnet** - MQTT client library
- **SocketCANSharp** - Native Linux CAN interface
- **SSH.NET** - Remote CAN access
- **Polly** - Resilience and transient-fault handling
- **Serilog** - Structured logging
- **Minimal API** - REST API framework
- **Swashbuckle** - OpenAPI/Swagger documentation

## ?? Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Setup

```bash
# Clone the repository
git clone https://github.com/DoctaShizzle/rotex-rocon2mqtt.git
cd rotex-rocon2mqtt

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Run the application
cd src/RoconMqtt
dotnet run
```

### Guidelines

- Follow existing code style and conventions
- Add tests for new features
- Update documentation as needed
- Use meaningful commit messages

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Acknowledgments

- Rotex/Daikin for the Rocon G1 heating controller
- The .NET community for excellent libraries and tools
- Home Assistant community for integration standards

## ?? Support

- **Documentation**: See `docu/` folder
- **Issues**: [GitHub Issues](https://github.com/DoctaShizzle/rotex-rocon2mqtt/issues)
- **Discussions**: [GitHub Discussions](https://github.com/DoctaShizzle/rotex-rocon2mqtt/discussions)

## ?? Related Projects

- [Home Assistant](https://www.home-assistant.io/) - Open source home automation
- [Mosquitto](https://mosquitto.org/) - Open source MQTT broker
- [MQTTnet](https://github.com/dotnet/MQTTnet) - High performance MQTT library for .NET

---

**Made with ?? for the smart home community**
