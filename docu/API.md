# Rocon MQTT REST API Documentation

## Overview

The Rocon MQTT API provides REST endpoints for communicating with Rotex Rocon G1 heating controllers over CAN bus. This API allows you to read and write parameter values, query device information, and monitor system health.

**Base URL:** `http://localhost:5000/api`

**API Version:** 1.0.0

**Swagger/OpenAPI:** Available at `http://localhost:5000/swagger`

## Quick Start

1. **Start the application:**
   ```bash
   cd src/RoconMqtt
   dotnet run
   ```

2. **Access Swagger UI:** Navigate to `http://localhost:5000/swagger`

3. **Test an endpoint:**
   ```bash
   curl "http://localhost:5000/api/parameters/get?DeviceName=HG1&ParameterName=cAUSSENTEMP"
   ```

## Authentication

Currently, all endpoints are configured with `AllowAnonymous()`. You may want to add authentication in production environments.

---

## Endpoints

### 1. Get Parameter Value

**`GET /parameters/get`**

Query a single parameter value from a device.

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `DeviceName` | string | Yes | - | Device name (HG1-HG8, HC1-HC16, HCM1-HCM16) |
| `ParameterName` | string | Yes | - | Parameter name from registry |
| `TimeoutMs` | integer | No | 1000 | Response timeout in milliseconds |

**Example:**

```bash
curl "http://localhost:5000/api/parameters/get?DeviceName=HG1&ParameterName=cAUSSENTEMP"
```

**Response (200 OK):**

```json
{
  "name": "cAUSSENTEMP",
  "value": 15.5,
  "metadata": {
    "type": "Float",
    "min": -40.0,
    "max": 80.0,
    "default": 0.0,
    "writeable": false,
    "factor": 10.0
  }
}
```

**Error Responses:**
- **400 Bad Request** - Device or parameter not found
- **408 Request Timeout** - No response from device within timeout period

---

### 2. Get Multiple Parameters (Bulk)

**`POST /parameters/get-bulk`**

Query multiple parameters from a device in a single request. More efficient than individual GET requests.

**Request Body:**

```json
{
  "deviceName": "HG1",
  "parameterNames": [
    "cAUSSENTEMP",
    "cKESSELISTTEMP",
    "cSPEICHERISTTEMP"
  ],
  "timeoutMs": 1000
}
```

**Response (200 OK):**

```json
{
  "results": [
    {
      "name": "cAUSSENTEMP",
      "value": 15.5,
      "metadata": { /* ... */ }
    },
    {
      "name": "cKESSELISTTEMP",
      "value": 45.2,
      "metadata": { /* ... */ }
    }
  ],
  "errors": [
    {
      "parameterName": "cSPEICHERISTTEMP",
      "errorMessage": "Timeout waiting for response from device"
    }
  ]
}
```

**Behavior:**
- Always returns **200 OK**, even if some parameters fail
- Successful queries in `results` array
- Failed queries in `errors` array
- Parameters queried sequentially with individual timeouts

**Error Responses:**
- **400 Bad Request** - Device not found or no parameters specified

---

### 3. Set Parameter Value

**`POST /parameters/set`**

Write a parameter value to a device.

**Request Body:**

```json
{
  "deviceName": "HG1",
  "parameterName": "cEINSTELL_SPEICHERSOLLTEMP",
  "value": 48.0,
  "timeoutMs": 1000
}
```

**Response (200 OK):**

```json
{
  "name": "cEINSTELL_SPEICHERSOLLTEMP",
  "value": 48.0,
  "metadata": {
    "type": "Float",
    "min": 10.0,
    "max": 70.0,
    "default": 48.0,
    "writeable": true,
    "factor": 10.0
  }
}
```

**Error Responses:**
- **400 Bad Request** - Device/parameter not found, value out of range, or parameter not writeable
- **408 Request Timeout** - No response from device

---

### 4. Raw Parameter Request (Advanced)

**`POST /parameters/raw`**

Low-level endpoint that works directly with InfoNumbers instead of parameter names. Use for testing unknown parameters or when parameter is not in registry.

**Request Body:**

```json
{
  "deviceName": "HG1",
  "infoNumberHigh": 10,
  "infoNumberLow": 6,
  "commandType": "Get",
  "value": null,
  "timeoutMs": 1000
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `deviceName` | string | Yes | Device name |
| `infoNumberHigh` | byte | Yes | InfoNumber high byte (0-255) |
| `infoNumberLow` | byte | Yes | InfoNumber low byte (0-255) |
| `commandType` | string | Yes | "Get" or "Set" |
| `value` | object | No | Value for SET commands |
| `timeoutMs` | integer | No | Timeout in milliseconds (default: 1000) |

**Response (200 OK):**

```json
{
  "name": "cEINSTELL_SPEICHERSOLLTEMP",
  "value": 48.0,
  "metadata": { /* ... */ }
}
```

**Notes:**
- InfoNumber format: 0x0A06 ? High=10 (0x0A), Low=6 (0x06)
- Parameter name looked up from registry if available
- Bypasses parameter validation

**Error Responses:**
- **400 Bad Request** - Device not found or invalid command type
- **408 Request Timeout** - No response from device

---

### 5. List All Devices

**`GET /devices`**

Returns all available devices in the system.

**Response (200 OK):**

```json
{
  "devices": [
    {
      "name": "HG1",
      "type": "HeatGenerator"
    },
    {
      "name": "HC1",
      "type": "HeatingCircuit"
    },
    {
      "name": "HCM1",
      "type": "HeatingCircuitModule"
    }
  ]
}
```

**Device Types:**
- `HeatGenerator` - HG1-HG8 (core system parameters)
- `HeatingCircuit` - HC1-HC16 (heating schedules)
- `HeatingCircuitModule` - HCM1-HCM16 (water schedules)

---

### 6. List All Parameters

**`GET /parameters`**

Returns all available parameters from the registry (~4800 parameters).

**Response (200 OK):**

```json
{
  "parameters": [
    {
      "name": "cAUSSENTEMP",
      "metadata": {
        "type": "Float",
        "min": -40.0,
        "max": 80.0,
        "default": 0.0,
        "writeable": false,
        "factor": 10.0
      }
    }
  ]
}
```

**Note:** Response contains ~4800 parameters. Consider implementing pagination for production use.

---

### 7. Get Parameter Metadata

**`GET /parameters/{ParameterName}/metadata`**

Returns metadata for a specific parameter.

**Path Parameters:**
- `ParameterName` - Parameter name from registry

**Response (200 OK):**

```json
{
  "name": "cAUSSENTEMP",
  "metadata": {
    "type": "Float",
    "min": -40.0,
    "max": 80.0,
    "default": 0.0,
    "writeable": false,
    "factor": 10.0
  }
}
```

**Error Responses:**
- **404 Not Found** - Parameter not found in registry

---

### 8. Get MQTT Publishing Status

**`GET /mqtt/status`**

Returns the current MQTT publishing status (enabled or disabled).

**Response (200 OK):**

```json
{
  "enabled": true
}
```

**Example:**

```bash
curl "http://localhost:5000/api/mqtt/status"
```

**Use Case:**
- Check whether the MQTT publisher is currently active
- Monitor publishing state in monitoring dashboards

---

### 9. Update MQTT Publishing Status

**`PUT /mqtt/status`**

Enable or disable MQTT publishing at runtime without restarting the application.

**Request Body:**

```json
{
  "enabled": false
}
```

**Response (200 OK):**

```json
{
  "enabled": false
}
```

**Example - Disable Publishing:**

```bash
curl -X PUT "http://localhost:5000/api/mqtt/status" \
  -H "Content-Type: application/json" \
  -d '{"enabled": false}'
```

**Example - Enable Publishing:**

```bash
curl -X PUT "http://localhost:5000/api/mqtt/status" \
  -H "Content-Type: application/json" \
  -d '{"enabled": true}'
```

**Use Cases:**
- Temporarily stop MQTT publishing during maintenance
- Dynamically control publishing based on external conditions
- Pause publishing while testing or debugging
- Reduce system load by disabling publishing on demand

**Notes:**
- Changes take effect immediately in the next polling cycle
- Setting persists until application restart (reverts to `appsettings.json` value)
- When disabled, the publisher continues running but skips all queries and MQTT publishes
- Does not affect the REST API endpoints - they remain operational

---

### 10. Start CAN Bus Recording

**`POST /api/can/recording/start`**

Start recording all CAN bus traffic for reverse engineering and analysis.

**Request Body:**

```json
{
  "canIdFilter": ["0x180", "0x300"],
  "description": "Testing thermostat temperature changes"
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `canIdFilter` | string[] | No | Optional filter: only record frames with these CAN IDs (hex format). If null/empty, all frames recorded |
| `description` | string | No | Optional description/label for this recording session |

**Response (200 OK):**

```json
{
  "status": "recording_started",
  "message": "CAN bus recording started successfully",
  "filter": "0x180, 0x300",
  "description": "Testing thermostat temperature changes"
}
```

**Error Response (400 Bad Request):**

```json
{
  "error": "Recording already in progress. Stop the current recording before starting a new one."
}
```

**Example - Record All Frames:**

```bash
curl -X POST "http://localhost:5000/api/can/recording/start" \
  -H "Content-Type: application/json" \
  -d '{}'
```

**Example - Record Specific CAN IDs:**

```bash
curl -X POST "http://localhost:5000/api/can/recording/start" \
  -H "Content-Type: application/json" \
  -d '{
    "canIdFilter": ["0x180", "0x300", "0x600"],
    "description": "Recording HG1, HC1, and HCM1 responses"
  }'
```

---

### 11. Stop CAN Bus Recording (JSON)

**`POST /api/can/recording/stop`**

Stop the active CAN bus recording and retrieve all captured frames as JSON.

**Response (200 OK):**

```json
{
  "startedAt": "2024-12-20T14:30:00.000Z",
  "stoppedAt": "2024-12-20T14:35:00.000Z",
  "duration": "00:05:00",
  "frameCount": 1523,
  "description": "Testing thermostat temperature changes",
  "canIdFilter": ["0x180", "0x300"],
  "frames": [
    {
      "timestamp": "2024-12-20T14:30:00.123Z",
      "canId": "0x180",
      "data": "D2 1D FA 00 0C 01 E0",
      "dataLength": 7
    },
    {
      "timestamp": "2024-12-20T14:30:01.456Z",
      "canId": "0x300",
      "data": "D2 1D FA 01 12 00 FA",
      "dataLength": 7
    }
  ]
}
```

**Error Response (400 Bad Request):**

```json
{
  "error": "No active recording to stop"
}
```

**Example:**

```bash
curl -X POST "http://localhost:5000/api/can/recording/stop"
```

---

### 12. Stop CAN Bus Recording & Export (candump format)

**`POST /api/can/recording/stop/export`**

Stop the active CAN bus recording and export all captured frames in candump text format.

**Response (200 OK):**

```
# CAN Recording Session
# Started: 2024-12-20 14:30:00.000 UTC
# Stopped: 2024-12-20 14:35:00.000 UTC
# Duration: 300.000 seconds
# Frame Count: 1523
# Description: Testing thermostat temperature changes
# Filter: 0x180, 0x300
#
(2024-12-20 14:30:00.123) 0x180#D2 1D FA 00 0C 01 E0
(2024-12-20 14:30:01.456) 0x300#D2 1D FA 01 12 00 FA
(2024-12-20 14:30:02.789) 0x180#D2 1D FA 00 0D 02 A0
...
```

**Error Response (400 Bad Request):**

```json
{
  "error": "No active recording to stop"
}
```

**Example:**

```bash
curl -X POST "http://localhost:5000/api/can/recording/stop/export" > recording.log
```

---

### 13. Get CAN Recording Status

**`GET /api/can/recording/status`**

Check if CAN bus recording is currently active.

**Response (200 OK):**

```json
{
  "isRecording": true,
  "status": "recording"
}
```

**Example:**

```bash
curl "http://localhost:5000/api/can/recording/status"
```

---

### 14. Health Check

**`GET /health`**

*(Note: Not under `/api` prefix)*

Returns health status of the API and its dependencies.

**Response (200 OK):**

```json
{
  "status": "Healthy",
  "mqttConnected": true,
  "version": "1.0.0"
}
```

---

## Data Models

### ParameterResponse

```typescript
{
  name: string,
  value: any,  // Type varies by parameter
  metadata: ParameterMetadata
}
```

### ParameterMetadata

```typescript
{
  type: "Int" | "Float" | "Bool" | "TimeRange" | "Enum",
  min: number | null,
  max: number | null,
  default: any | null,
  writeable: boolean,
  factor: number | null
}
```

### ParametersBulkResponse

```typescript
{
  results: ParameterResponse[],
  errors: ParameterError[]
}
```

### ParameterError

```typescript
{
  parameterName: string,
  errorMessage: string
}
```

### DeviceInfo

```typescript
{
  name: string,
  type: "HeatGenerator" | "HeatingCircuit" | "HeatingCircuitModule"
}
```

### MqttPublishingStatusRequest

```typescript
{
  enabled: boolean
}
```

### MqttPublishingStatusResponse

```typescript
{
  enabled: boolean
}
```

### CanRecordingOptions

```typescript
{
  canIdFilter?: string[],  // Optional: filter by CAN IDs (e.g., ["0x180", "0x300"])
  description?: string     // Optional: description for this recording
}
```

### CanRecordingResult

```typescript
{
  startedAt: string,        // ISO 8601 timestamp (UTC)
  stoppedAt: string,        // ISO 8601 timestamp (UTC)
  duration: string,         // TimeSpan format (HH:MM:SS)
  frameCount: number,       // Total frames captured
  description?: string,     // Recording description (if provided)
  canIdFilter?: string[],   // Applied filter (if any)
  frames: RecordedCanFrame[]
}
```

### RecordedCanFrame

```typescript
{
  timestamp: string,  // ISO 8601 timestamp (UTC)
  canId: string,      // Hex format (e.g., "0x180")
  data: string,       // Hex bytes (e.g., "D2 1D FA 00 0C 01 E0")
  dataLength: number  // Number of data bytes
}
```

---

## Parameter Types

| Type | Description | Example |
|------|-------------|---------|
| **Int** | Raw 16-bit signed integer | Day value (0-31) |
| **Float** | Integer scaled by factor | Temperature: 48.0°C = 480 (factor 10) |
| **Bool** | Boolean value | Active/inactive status |
| **TimeRange** | Time range `"HH:MM-HH:MM"` | `"06:00-22:00"` |
| **Enum** | Enumerated values | Program switch (1, 3, 4, 5, 11, 12, 17) |

---

## Common Parameters

| Parameter Name | Type | Writeable | Description |
|----------------|------|-----------|-------------|
| `cAUSSENTEMP` | Float | No | Outside temperature (°C) |
| `cKESSELISTTEMP` | Float | No | Boiler actual temperature (°C) |
| `cSPEICHERISTTEMP` | Float | No | Storage actual temperature (°C) |
| `cEINSTELL_SPEICHERSOLLTEMP` | Float | Yes | Storage target temperature (°C) |
| `cRAUMISTTEMP` | Float | No | Room actual temperature (°C) |
| `cWW_AKTIV` | Bool | No | Warm water active status |
| `cSTUNDE` | Int | No | Current hour (0-23) |
| `cMINUTE` | Int | No | Current minute (0-59) |

---

## Error Handling

### Standard Error Format

```json
{
  "errors": {
    "GeneralErrors": [
      "Error message"
    ],
    "FieldName": [
      "Field-specific error"
    ]
  }
}
```

### Common Scenarios

| Status Code | Scenario | Solution |
|-------------|----------|----------|
| **400** | Device not found | Check `/api/devices` for available devices |
| **400** | Parameter not found | Check `/api/parameters` for available parameters |
| **400** | Value out of range | Check parameter metadata for min/max |
| **400** | Parameter not writeable | Verify `writeable` flag in metadata |
| **408** | Timeout | Check device connectivity, increase `timeoutMs` |

---

## Example Use Cases

### Reading Temperature

```bash
curl "http://localhost:5000/api/parameters/get?DeviceName=HG1&ParameterName=cAUSSENTEMP"
```

### Bulk Reading Multiple Values

```bash
curl -X POST "http://localhost:5000/api/parameters/get-bulk" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "HG1",
    "parameterNames": ["cAUSSENTEMP", "cKESSELISTTEMP", "cSPEICHERISTTEMP"]
  }'
```

### Setting Temperature

```bash
curl -X POST "http://localhost:5000/api/parameters/set" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "HG1",
    "parameterName": "cEINSTELL_SPEICHERSOLLTEMP",
    "value": 50.0
  }'
```

### Raw Access by InfoNumber

```bash
curl -X POST "http://localhost:5000/api/parameters/raw" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "HG1",
    "infoNumberHigh": 10,
    "infoNumberLow": 6,
    "commandType": "Get"
  }'
```

### Listing Available Devices

```bash
curl "http://localhost:5000/api/devices"
```

### Getting Parameter Metadata

```bash
curl "http://localhost:5000/api/parameters/cAUSSENTEMP/metadata"
```

### Controlling MQTT Publishing

**Check Publishing Status:**
```bash
curl "http://localhost:5000/api/mqtt/status"
```

**Disable Publishing:**
```bash
curl -X PUT "http://localhost:5000/api/mqtt/status" \
  -H "Content-Type: application/json" \
  -d '{"enabled": false}'
```

**Re-enable Publishing:**
```bash
curl -X PUT "http://localhost:5000/api/mqtt/status" \
  -H "Content-Type: application/json" \
  -d '{"enabled": true}'
```

### Recording CAN Bus Traffic

**Use Case:** Reverse engineering parameters for different Rotex Rocon hardware variants.

**Workflow:**

1. **Disable MQTT publishing to avoid interference:**
   ```bash
   curl -X PUT "http://localhost:5000/api/mqtt/status" \
     -H "Content-Type: application/json" \
     -d '{"enabled": false}'
   ```

2. **Start recording (with optional filtering):**
   ```bash
   curl -X POST "http://localhost:5000/api/can/recording/start" \
     -H "Content-Type: application/json" \
     -d '{
       "canIdFilter": ["0x180", "0x300"],
       "description": "Testing temperature setpoint changes"
     }'
   ```

3. **Manipulate the thermostat** (change temperature, mode, etc.)

4. **Check recording status:**
   ```bash
   curl "http://localhost:5000/api/can/recording/status"
   ```

5. **Stop recording and save to file:**
   ```bash
   curl -X POST "http://localhost:5000/api/can/recording/stop/export" > recording.log
   ```

6. **Analyze the recording** to identify:
   - InfoNumber patterns (bytes 3-4 in ANSWER frames)
   - Value encoding (bytes 5-6)
   - CAN ID mapping to devices
   - Parameter types and ranges

7. **Re-enable MQTT publishing:**
   ```bash
   curl -X PUT "http://localhost:5000/api/mqtt/status" \
     -H "Content-Type: application/json" \
     -d '{"enabled": true}'
   ```

**Analysis Tips:**
- Look for ANSWER frames (header: `D2 1D FA`)
- Compare before/after values when changing thermostat settings
- Track which CAN IDs respond to specific parameter changes
- Document InfoNumber patterns for new parameter types

---

## Configuration

See [Configuration Documentation](../README.md#configuration) for:
- Kestrel endpoint configuration
- CAN bus settings (SocketCAN or SSH)
- MQTT publisher settings
- Resilience policies

---

## Related Documentation

- **[MQTT.md](./MQTT.md)** - MQTT publisher, Home Assistant integration, compound parameters
- **[CAN.md](./CAN.md)** - CAN bus communication protocol, parameter encoding/decoding, device subsystems
- **[SSH_CAN_BUS.md](./SSH_CAN_BUS.md)** - Remote CAN bus access via SSH
- **[DEPLOYMENT.md](./DEPLOYMENT.md)** - Deployment guide for Raspberry Pi
- **[HARDWARE.md](./HARDWARE.md)** - Hardware setup and CAN interface configuration
- **[RESILIENCE.md](./RESILIENCE.md)** - Retry and circuit breaker policies
- **[README.md](../README.md)** - Project overview and setup

---

## Technology Stack

- **.NET 10.0** - Runtime framework
- **Minimal API** - REST API framework
- **Microsoft.AspNetCore.OpenApi** - OpenAPI document generation
- **Swashbuckle.AspNetCore.SwaggerUI** - Swagger UI (consuming OpenAPI document)
- **Serilog** - Structured logging

---

## Support

- **GitHub:** https://github.com/DoctaShizzle/rotex-rocon2mqtt
- **Issues:** Report bugs or request features via GitHub Issues
- **Logs:** Check `logs/rocon-mqtt-*.txt` for debugging
