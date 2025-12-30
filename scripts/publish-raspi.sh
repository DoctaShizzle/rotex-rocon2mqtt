#!/bin/bash
# Publish script for Raspberry Pi (Linux ARM64)
# This script builds and publishes a self-contained deployment for Raspberry Pi

set -e

echo "Publishing RoconMqtt for Raspberry Pi (Linux ARM64)..."

# Navigate to project directory
cd "$(dirname "$0")/../src/RoconMqtt"

# Publish using the profile
dotnet publish -p:PublishProfile=RaspberryPi-ARM64

echo ""
echo "Publish successful!"
echo "Output location: bin/publish/linux-arm64/"
echo ""
echo "To deploy to Raspberry Pi, copy the contents to your device and run:"
echo "  chmod +x RoconMqtt (to allow execution)"
echo "  sudo ufw allow 5000 (to expose the web app publicly)"
echo ""
echo "Run in Production mode (default):"
echo "  ./RoconMqtt"
echo ""
echo "Run in Development mode (more logging):"
echo "  ASPNETCORE_ENVIRONMENT=Development ./RoconMqtt"
echo ""
echo "For systemd service installation, see: docu/DEPLOYMENT.md"
echo "Service file available at: scripts/roconmqtt.service"
