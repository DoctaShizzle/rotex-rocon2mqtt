# Publish script for Raspberry Pi (Linux ARM64)
# This script builds and publishes a self-contained deployment for Raspberry Pi

$ErrorActionPreference = "Stop"

Write-Host "Publishing RoconMqtt for Raspberry Pi (Linux ARM64)..." -ForegroundColor Cyan

# Navigate to project directory
$projectPath = Join-Path $PSScriptRoot "..\src\RoconMqtt"
Push-Location $projectPath

try {
    # Publish using the profile
    dotnet publish -p:PublishProfile=RaspberryPi-ARM64
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nPublish successful!" -ForegroundColor Green
        Write-Host "Output location: bin\publish\linux-arm64\" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "==============================================" -ForegroundColor Cyan
        Write-Host "  Deployment Options" -ForegroundColor Cyan
        Write-Host "==============================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "1. Balena (Containerized):" -ForegroundColor Green
        Write-Host "   balena push <fleet-name>" -ForegroundColor White
        Write-Host "   See: docu\BALENA_DEPLOYMENT.md" -ForegroundColor Gray
        Write-Host ""
        Write-Host "2. Bare Metal (Raspberry Pi):" -ForegroundColor Green
        Write-Host "   Transfer files: rsync -avz bin/publish/linux-arm64/ pi@<ip>:/opt/roconmqtt/" -ForegroundColor White
        Write-Host "   Configure: Edit environment variables (see DEPLOYMENT.md)" -ForegroundColor White
        Write-Host "   See: docu\DEPLOYMENT.md" -ForegroundColor Gray
        Write-Host ""
        Write-Host "3. Development Testing:" -ForegroundColor Green
        Write-Host "   Quick Start: .\scripts\run-dev.ps1 (edit settings first)" -ForegroundColor White
        Write-Host "   Manual: Set environment variables and run ./RoconMqtt" -ForegroundColor White
        Write-Host ""
        Write-Host "==============================================" -ForegroundColor Cyan
        Write-Host "  Configuration" -ForegroundColor Cyan
        Write-Host "==============================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Required environment variables:" -ForegroundColor Yellow
        Write-Host "  Can__MachineType=EHSHXXPXXA" -ForegroundColor White
        Write-Host "  Mqtt__Host=<broker-ip>" -ForegroundColor White
        Write-Host "  Mqtt__Username=<username>" -ForegroundColor White
        Write-Host "  Mqtt__Password=<password>" -ForegroundColor White
        Write-Host ""
        Write-Host "For complete configuration reference:" -ForegroundColor Yellow
        Write-Host "  docu\CONFIGURATION.md" -ForegroundColor Gray
        Write-Host ""
    } else {
        Write-Host "`nPublish failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} finally {
    Pop-Location
}
