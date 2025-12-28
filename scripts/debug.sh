#!/bin/bash

# RoconMqtt Debug Script
# This script runs the application in Debug mode with Development logging

set -e

echo "Starting RoconMqtt in Debug mode..."
echo "Log level: Debug for RoconMqtt namespace, Warning for frameworks"

export ASPNETCORE_ENVIRONMENT=Development
dotnet run --configuration Debug
