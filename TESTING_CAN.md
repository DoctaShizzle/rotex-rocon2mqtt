# Testing CAN Communication

This document describes how to test the CAN communication end-to-end without MQTT.

## Running the Test

To run the CAN communication test:

```bash
dotnet run --project src/RoconMqtt/RoconMqtt.csproj -- test-can
```

Or if you're already in the project directory:

```bash
cd src/RoconMqtt
dotnet run -- test-can
```

## What the Test Does

The test program (`TestCanCommunication.cs`) performs the following actions:

1. **Loads Configuration**: Reads devices and parameters from `appsettings.json`
2. **Initializes Services**: Sets up the CAN reader, encoder, decoder, and ROCON service
3. **Starts Listener**: Begins listening for CAN responses in a background task
4. **Queries Parameters**: Loops through each device and parameter, sending GET requests
5. **Collects Responses**: Captures and displays responses as they arrive
6. **Displays Results**: Shows a summary table with all parameter values

## Configuration

The test uses the following configuration from `appsettings.json`:

- **Mqtt:Devices**: List of devices to query (e.g., `["HG1"]`)
- **Mqtt:Parameters**: List of parameter names to query
- **Mqtt:ResponseTimeoutMs**: How long to wait for responses (default: 1000ms)
- **Can:receiveFromCanFrameId**: Start of CAN ID range for responses
- **Can:receiveToCanFrameId**: End of CAN ID range for responses
- **Can:sendCanFrameId**: CAN ID to use for sending requests

## Example Output

```
=== ROCON CAN Communication Test ===

Devices: HG1
Parameters: 27 configured
Response timeout: 1000ms

=== Querying Parameters ===

--- Querying device: HG1 ---
[Response 1/27] OutdoorTemperature = 8.5
[Response 2/27] Hour = 14
[Response 3/27] Minute = 32
[Response 4/27] BufferTemperatureActual = 45.2
...

=== Results Summary ===

Total requests sent: 27
Total responses received: 25
Success rate: 92.6%

Device: HG1
--------------------------------------------------------------------------------
Parameter                      Value                     Type      
--------------------------------------------------------------------------------
OutdoorTemperature             8.50                      Float     
Hour                           14                        Int       
Minute                         32                        Int       
BufferTemperatureActual        45.20                     Float     
...
```

## Troubleshooting

### No Responses Received

- Ensure the CAN interface (can0) is up and configured
- Check that the heat pump controller is powered on and communicating
- Verify the CAN ID ranges in configuration match your system

### Partial Responses

- Some parameters may not be supported by your specific heat pump model
- Increase the response timeout in configuration if needed
- Check logs for any decoding errors

### Permission Errors

The SocketCAN interface may require elevated permissions:

```bash
sudo dotnet run --project src/RoconMqtt/RoconMqtt.csproj -- test-can
```

## Logging

To see more detailed logging, modify the logging level in the test code or add a temporary `appsettings.Test.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "RoconMqtt": "Debug"
    }
  }
}
```

Then set the environment variable:

```bash
export ASPNETCORE_ENVIRONMENT=Test
dotnet run -- test-can
```
