#!/bin/bash
# ===========================
# RoconMqtt Development Runner
# ===========================
# This script sets required environment variables and runs the RoconMqtt
# application in Development mode with verbose logging.
#
# Usage: ./scripts/run-dev.sh

set -e

# ===========================
# CONFIGURATION - EDIT THESE VALUES
# ===========================

# Machine type (REQUIRED)
export Can__MachineType="EHSHXXPXXA"

# MQTT Broker (REQUIRED)
export Mqtt__Host="192.168.0.254"
export Mqtt__Username="rocon"
export Mqtt__Password="your-mqtt-password"

# MQTT Optional Settings
export Mqtt__Port="1883"
export Mqtt__ClientId="rocon-mqtt-dev"
export Mqtt__Topic="rocon"

# Web Server (Optional - defaults to http://localhost:5000)
# export Kestrel__Endpoints__Http__Url="http://localhost:5000"

# CAN Interface (Optional)
export Can__CanInterfaceName="can0"
export Can__TimeZoneId="Europe/Brussels"

# CAN SSH Remote Access (Optional - leave commented for local SocketCAN)
# export Can__Ssh__Host="192.168.0.100"
# export Can__Ssh__Username="pi"
# export Can__Ssh__Password="raspberry"

# Seq Logging (Optional - uncomment to enable)
# export Serilog__WriteTo__1__Name="Seq"
# export Serilog__WriteTo__1__Args__serverUrl="http://192.168.0.254:5341"

# Set Development environment
export ASPNETCORE_ENVIRONMENT="Development"

# ===========================
# RUN APPLICATION
# ===========================

echo "================================================"
echo "  RoconMqtt Development Runner"
echo "================================================"
echo ""
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "Machine Type: $Can__MachineType"
echo "MQTT Host: $Mqtt__Host"
echo "CAN Interface: $Can__CanInterfaceName"
if [ -n "$Can__Ssh__Host" ]; then
    echo "CAN Mode: SSH Remote ($Can__Ssh__Host)"
else
    echo "CAN Mode: Local SocketCAN"
fi
echo ""
echo "Press Ctrl+C to stop"
echo "================================================"
echo ""

# Navigate to project directory
cd "$(dirname "$0")/../src/RoconMqtt"

# Run the application
dotnet run --project RoconMqtt.csproj
