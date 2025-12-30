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
        Write-Host "`nTo deploy to Raspberry Pi, copy the contents to your device and run:" -ForegroundColor Cyan
        Write-Host "  chmod +x RoconMqtt (to allow execution)" -ForegroundColor White
        Write-Host "  sudo ufw allow 5000 (to expose the web app publicly)" -ForegroundColor White
        Write-Host "`nRun in Production mode (default):" -ForegroundColor Cyan
        Write-Host "  ./RoconMqtt" -ForegroundColor White
        Write-Host "`nRun in Development mode (more logging):" -ForegroundColor Cyan
        Write-Host "  ASPNETCORE_ENVIRONMENT=Development ./RoconMqtt" -ForegroundColor White
        Write-Host "`nFor systemd service installation, see: docu\DEPLOYMENT.md" -ForegroundColor Yellow
        Write-Host "Service file available at: scripts\roconmqtt.service" -ForegroundColor Yellow
    } else {
        Write-Host "`nPublish failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} finally {
    Pop-Location
}
