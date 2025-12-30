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

### 8. Health Check

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

---

## Configuration

See [Configuration Documentation](../README.md#configuration) for:
- Kestrel endpoint configuration
- CAN bus settings (SocketCAN or SSH)
- MQTT publisher settings
- Resilience policies

---

## Related Documentation

- **[CAN.md](./CAN.md)** - CAN bus communication protocol, parameter encoding/decoding, device subsystems
- **[SSH_CAN_BUS.md](./SSH_CAN_BUS.md)** - Remote CAN bus access via SSH
- **[RESILIENCE.md](./RESILIENCE.md)** - Retry and circuit breaker policies
- **[README.md](../README.md)** - Project overview and setup

---

## Technology Stack

- **.NET 10.0** - Runtime framework
- **FastEndpoints 7.1.1** - REST API framework
- **NSwag** - OpenAPI/Swagger documentation
- **Serilog** - Structured logging

---

## Support

- **GitHub:** https://github.com/DoctaShizzle/rotex-rocon2mqtt
- **Issues:** Report bugs or request features via GitHub Issues
- **Logs:** Check `logs/rocon-mqtt-*.txt` for debugging
