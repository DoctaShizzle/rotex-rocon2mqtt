#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Run RoconMqtt in development mode with environment variables.

.DESCRIPTION
    This script sets required environment variables and runs the RoconMqtt
    application in Development mode with verbose logging.
    
    Configure the variables below before running, or set them in your
    PowerShell profile for persistence.

.EXAMPLE
    .\scripts\run-dev.ps1

.NOTES
    Requires .NET 10 SDK installed.
    Run from the repository root.
#>

# ===========================
# CONFIGURATION - EDIT THESE VALUES
# ===========================

# Machine type (REQUIRED)
$env:Can__MachineType = "EHSHXXPXXA"

# MQTT Broker (REQUIRED)
$env:Mqtt__Host = "192.168.0.254"
$env:Mqtt__Username = "rocon"
$env:Mqtt__Password = "your-mqtt-password"

# MQTT Optional Settings
$env:Mqtt__Port = "1883"
$env:Mqtt__ClientId = "rocon-mqtt-dev"
$env:Mqtt__Topic = "rocon"

# Web Server (Optional - defaults to http://localhost:5000)
# $env:Kestrel__Endpoints__Http__Url = "http://localhost:5000"

# CAN Interface (Optional)
$env:Can__CanInterfaceName = "can0"
$env:Can__TimeZoneId = "Europe/Brussels"

# CAN SSH Remote Access (Optional - leave commented for local SocketCAN)
# $env:Can__Ssh__Host = "192.168.0.100"
# $env:Can__Ssh__Username = "pi"
# $env:Can__Ssh__Password = "raspberry"

# Seq Logging (Optional - uncomment to enable)
# $env:Serilog__WriteTo__1__Name = "Seq"
# $env:Serilog__WriteTo__1__Args__serverUrl = "http://192.168.0.254:5341"

# Set Development environment
$env:ASPNETCORE_ENVIRONMENT = "Development"

# ===========================
# RUN APPLICATION
# ===========================

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  RoconMqtt Development Runner" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Environment: $env:ASPNETCORE_ENVIRONMENT" -ForegroundColor Green
Write-Host "Machine Type: $env:Can__MachineType" -ForegroundColor Green
Write-Host "MQTT Host: $env:Mqtt__Host" -ForegroundColor Green
Write-Host "CAN Interface: $env:Can__CanInterfaceName" -ForegroundColor Green
if ($env:Can__Ssh__Host) {
    Write-Host "CAN Mode: SSH Remote ($env:Can__Ssh__Host)" -ForegroundColor Yellow
} else {
    Write-Host "CAN Mode: Local SocketCAN" -ForegroundColor Green
}
Write-Host ""
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to project directory
$projectPath = Join-Path $PSScriptRoot "..\src\RoconMqtt"
Set-Location $projectPath

# Run the application
dotnet run --project RoconMqtt.csproj
