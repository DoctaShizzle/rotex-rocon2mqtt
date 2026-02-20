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
echo "=============================================="
echo "  Deployment Options"
echo "=============================================="
echo ""
echo "1. Balena (Containerized):"
echo "   balena push <fleet-name>"
echo "   See: docu/BALENA_DEPLOYMENT.md"
echo ""
echo "2. Bare Metal (Raspberry Pi):"
echo "   Transfer files: rsync -avz bin/publish/linux-arm64/ pi@<ip>:/opt/roconmqtt/"
echo "   Configure: Edit environment variables (see DEPLOYMENT.md)"
echo "   See: docu/DEPLOYMENT.md"
echo ""
echo "3. Development Testing:"
echo "   Quick Start: ../../scripts/run-dev.sh (edit settings first)"
echo "   Manual: Set environment variables and run ./RoconMqtt"
echo ""
echo "=============================================="
echo "  Configuration"
echo "=============================================="
echo ""
echo "Required environment variables:"
echo "  Can__MachineType=EHSHXXPXXA"
echo "  Mqtt__Host=<broker-ip>"
echo "  Mqtt__Username=<username>"
echo "  Mqtt__Password=<password>"
echo ""
echo "For complete configuration reference:"
echo "  docu/CONFIGURATION.md"
echo ""
