#!/bin/bash

# RoconMqtt Production Script
# This script runs the application in Release mode with Production logging
# Recommended to be used with systemd service

set -e

# Configuration
APP_NAME="rocon-mqtt"
APP_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BIN_PATH="$APP_DIR/bin/Release/net10.0/RoconMqtt"
LOG_DIR="$APP_DIR/logs"
PID_FILE="/var/run/$APP_NAME.pid"

echo "Starting RoconMqtt in Production mode..."
echo "Log level: Warning for all logs"
echo "Logs directory: $LOG_DIR"

# Ensure logs directory exists
mkdir -p "$LOG_DIR"

# Set production environment
export ASPNETCORE_ENVIRONMENT=Production

# Check if binary exists
if [ ! -f "$BIN_PATH" ]; then
    echo "Error: Binary not found at $BIN_PATH"
    echo "Please build the application first: dotnet publish -c Release"
    exit 1
fi

# Run the application
"$BIN_PATH"
