# Rocon G1 – CAN GET + ANSWER Cycle

Developer Documentation for GitHub Copilot / Claude

This document explains how the Rocon G1 controller communicates over CAN for reading parameters.  
It describes the **GET request format**, the **ANSWER response format**, and how a background service should implement the request/response cycle.

This documentation is intended for GitHub Copilot / Claude to generate correct code.

---

## 1. Overview

The Rocon G1 uses a simple request/response protocol over CAN:

- The client sends a **GET request** to CAN ID **0x69D**
- The controller replies on a subsystem?specific **ANSWER CAN ID**
- The response always starts with opcode **0xD2**
- The response echoes the **sequence bytes** from the request
- The response contains the **value** of the requested parameter

All parameters are defined in `CanParameterRegistry`, which is generated from `data.json`.

---

## 2. Subsystems and Their Communication Profiles

Each subsystem has:

- A **GET opcode** (byte in GET request)
- A **SET opcode** (byte in SET request)
- A **base ANSWER CAN ID** (controller response ID)
- A **sequence byte pair** (must match in request and response)

### Subsystems from `data.json`

#### Heat Generators (HG)

| Device | GET Opcode | GET Sequence | SET Opcode | SET Sequence | ANSWER CAN ID | ANSWER Header |
|--------|-----------|--------------|-----------|--------------|---------------|---------------|
| HG1 | `0x31` | `0x00, 0xFA` | `0x30` | `0x00, 0xFA` | `0x180` | `0xD2, 0x1D, 0xFA` |
| HG2 | `0x31` | `0x01, 0xFA` | `0x30` | `0x01, 0xFA` | `0x181` | `0xD2, 0x1D, 0xFA` |
| HG3 | `0x31` | `0x02, 0xFA` | `0x30` | `0x02, 0xFA` | `0x182` | `0xD2, 0x1D, 0xFA` |
| HG4 | `0x31` | `0x03, 0xFA` | `0x30` | `0x03, 0xFA` | `0x183` | `0xD2, 0x1D, 0xFA` |
| HG5 | `0x31` | `0x04, 0xFA` | `0x30` | `0x04, 0xFA` | `0x184` | `0xD2, 0x1D, 0xFA` |
| HG6 | `0x31` | `0x05, 0xFA` | `0x30` | `0x05, 0xFA` | `0x185` | `0xD2, 0x1D, 0xFA` |
| HG7 | `0x31` | `0x06, 0xFA` | `0x30` | `0x06, 0xFA` | `0x186` | `0xD2, 0x1D, 0xFA` |
| HG8 | `0x31` | `0x07, 0xFA` | `0x30` | `0x07, 0xFA` | `0x187` | `0xD2, 0x1D, 0xFA` |

#### Heating Circuits (HC)

| Device | GET Opcode | GET Sequence | SET Opcode | SET Sequence | ANSWER CAN ID | ANSWER Header |
|--------|-----------|--------------|-----------|--------------|---------------|---------------|
| HC1 | `0x61` | `0x00, 0xFA` | `0x60` | `0x00, 0xFA` | `0x300` | `0xD2, 0x1D, 0xFA` |
| HC2-HC16 | `0x61` | `0x01-0x0F, 0xFA` | `0x60` | `0x01-0x0F, 0xFA` | `0x301-0x30F` | `0xD2, 0x1D, 0xFA` |

*Pattern: HC sequence byte increments for each circuit (HC1=0x00, HC2=0x01, ... HC16=0x0F)*

#### Heating Circuit Modules (HCM)

| Device | GET Opcode | GET Sequence | SET Opcode | SET Sequence | ANSWER CAN ID | ANSWER Header |
|--------|-----------|--------------|-----------|--------------|---------------|---------------|
| HCM1 | `0xC1` | `0x00, 0xFA` | `0xC0` | `0x00, 0xFA` | `0x600` | `0xD2, 0x1D, 0xFA` |
| HCM2-HCM16 | `0xC1` | `0x01-0x0F, 0xFA` | `0xC0` | `0x01-0x0F, 0xFA` | `0x601-0x60F` | `0xD2, 0x1D, 0xFA` |

*Pattern: HCM sequence byte increments for each module (HCM1=0x00, HCM2=0x01, ... HCM16=0x0F)*

---

## 3. GET Request Frame Format

When requesting a parameter value from the controller:

### Frame Structure

```
CAN ID: 0x69D
Payload (8 bytes):
  [0]    = GET opcode (subsystem specific)
  [1]    = Sequence byte 1 (subsystem specific)
  [2]    = Sequence byte 2 (subsystem specific, usually 0xFA)
  [3]    = InfoNumber High byte
  [4]    = InfoNumber Low byte
  [5-7]  = Reserved / Unused
```

### Example: Read Parameter from HG1

To read parameter with InfoNumber `0x0A06` from **HG1**:

```
CAN ID: 0x69D
Payload:
  [0] = 0x31        (HG GET opcode)
  [1] = 0x00        (HG1 sequence byte 1)
  [2] = 0xFA        (HG1 sequence byte 2)
  [3] = 0x0A        (InfoNumber High)
  [4] = 0x06        (InfoNumber Low)
  [5] = 0x00        (Unused)
  [6] = 0x00        (Unused)
  [7] = 0x00        (Unused)
```

---

## 4. ANSWER Response Frame Format

The controller responds on the subsystem's **ANSWER CAN ID** with a 7-byte frame:

### Frame Structure

```
CAN ID: Subsystem-specific (e.g., 0x180 for HG1)
Payload (7 bytes):
  [0]    = Response opcode 0xD2 (always)
  [1]    = Response header byte 0x1D (always)
  [2]    = Response header byte 0xFA (always)
  [3]    = InfoNumber High byte (echoes request)
  [4]    = InfoNumber Low byte (echoes request)
  [5-6]  = Parameter value (16-bit, encoding depends on parameter type)
```

### Example: Response for HG1

Response to the HG1 parameter request above:

```
CAN ID: 0x180 (HG1's ANSWER ID)
Payload:
  [0] = 0xD2        (Response opcode)
  [1] = 0x1D        (Response header)
  [2] = 0xFA        (Response header)
  [3] = 0x0A        (InfoNumber High - echoes request)
  [4] = 0x06        (InfoNumber Low - echoes request)
  [5] = 0x01        (Value High byte)
  [6] = 0xE0        (Value Low byte)
```

---

## 5. Implementation Pattern: GET Request/Response Cycle

A background service implementing the GET cycle should:

### Step 1: Look Up Device and Parameter

```csharp
// Get parameter definition (e.g., 0x0A06)
var infoNumber = new InfoNumber(0x0A, 0x06);
if (!CanParameterRegistry.Parameters.TryGetValue(infoNumber, out var parameterDef))
{
    // Handle unknown parameter
}

// Automatically get subsystem profile based on InfoNumber.High byte
var subsystemProfile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
if (subsystemProfile == null)
{
    // Handle unknown subsystem
}

// Optional: Determine which specific device (HG1, HC5, etc.) to query
// This depends on your application logic - you might query all devices of that type
var subsystem = CanParameterRegistry.GetSubsystemForParameter(infoNumber);
```

### Step 2: Build GET Request Frame

```csharp
// Prepare 8-byte GET request using subsystem profile
byte[] getRequest = new byte[8];

getRequest[0] = subsystemProfile.Get.Bytes[0];      // GET opcode (0x31 for HG, 0x61 for HC, 0xC1 for HCM)
getRequest[1] = subsystemProfile.Get.Bytes[1];      // Sequence byte 1 (varies by specific device)
getRequest[2] = subsystemProfile.Get.Bytes[2];      // Sequence byte 2 (usually 0xFA)
getRequest[3] = infoNumber.High;                    // InfoNumber High
getRequest[4] = infoNumber.Low;                     // InfoNumber Low
getRequest[5] = 0x00;                               // Unused
getRequest[6] = 0x00;                               // Unused
getRequest[7] = 0x00;                               // Unused

// Send on standard CAN ID 0x69D
canBus.Send(subsystemProfile.Get.CanId, getRequest);
```

### Step 3: Wait for ANSWER Frame

```csharp
// Listen for response on subsystem-specific ANSWER CAN ID
uint expectedAnswerId = subsystemProfile.Answer.CanId;
byte[] answer = canBus.ReceiveTimeout(expectedAnswerId, timeoutMs: 1000);

if (answer == null || answer.Length < 7)
{
    // Handle timeout or short frame
}
```

### Step 4: Validate ANSWER Frame

```csharp
// Validate response header (always 0xD2, 0x1D, 0xFA)
if (answer[0] != 0xD2 || answer[1] != 0x1D || answer[2] != 0xFA)
{
    // Invalid response header
}

// Validate echoed InfoNumber
if (answer[3] != infoNumber.High || answer[4] != infoNumber.Low)
{
    // Response doesn't match request
}
```

### Step 5: Decode Parameter Value

```csharp
// Extract 16-bit value from response bytes [5-6]
// Use CanDecoder to convert to proper type based on parameterDef
byte[] decodedFrame = new byte[7];
decodedFrame[0] = answer[0];    // 0xD2
decodedFrame[1] = answer[1];    // 0x1D
decodedFrame[2] = answer[2];    // 0xFA
decodedFrame[3] = answer[3];    // InfoNumber High
decodedFrame[4] = answer[4];    // InfoNumber Low
decodedFrame[5] = answer[5];    // Value High
decodedFrame[6] = answer[6];    // Value Low

var decodedParameter = canDecoder.Decode(decodedFrame);
if (decodedParameter != null)
{
    // Use decodedParameter.Value
    object parameterValue = decodedParameter.Value;
}
```

---

## 6. Full Request/Response Cycle Example

```csharp
public class CanGetCycleService
{
    private readonly ICanBus _canBus;
    private readonly ICanDecoder _canDecoder;
    private readonly ILogger<CanGetCycleService> _logger;

    public async Task<object?> GetParameterAsync(InfoNumber infoNumber, int timeoutMs = 1000)
    {
        // Step 1: Lookup parameter and subsystem profile
        if (!CanParameterRegistry.Parameters.TryGetValue(infoNumber, out var parameterDef))
        {
            _logger.LogWarning("Parameter not found: {InfoNumber}", infoNumber);
            return null;
        }

        var subsystemProfile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
        if (subsystemProfile == null)
        {
            _logger.LogWarning("Unknown subsystem for parameter: {InfoNumber}", infoNumber);
            return null;
        }

        // Step 2: Build GET request
        byte[] getRequest = BuildGetRequest(subsystemProfile, infoNumber);

        // Step 3: Send and wait for ANSWER
        var answer = await _canBus.SendAndWaitAsync(
            subsystemProfile.Get.CanId, 
            getRequest, 
            subsystemProfile.Answer.CanId, 
            timeoutMs
        );
        
        if (answer == null)
        {
            _logger.LogWarning("No response for parameter {ParameterName} ({InfoNumber})", parameterDef.Name, infoNumber);
            return null;
        }

        // Step 4: Validate
        if (!ValidateAnswerFrame(answer, infoNumber))
        {
            _logger.LogWarning("Invalid response for parameter {ParameterName}", parameterDef.Name);
            return null;
        }

        // Step 5: Decode
        byte[] decodedFrame = answer.Take(7).ToArray();
        var decoded = _canDecoder.Decode(decodedFrame);

        if (decoded != null)
        {
            _logger.LogInformation("Read {ParameterName} = {Value}", decoded.Name, decoded.Value);
            return decoded.Value;
        }

        return null;
    }

    private byte[] BuildGetRequest(CommunicationProfile profile, InfoNumber infoNumber)
    {
        byte[] request = new byte[8];
        request[0] = profile.Get.Bytes[0];
        request[1] = profile.Get.Bytes[1];
        request[2] = profile.Get.Bytes[2];
        request[3] = infoNumber.High;
        request[4] = infoNumber.Low;
        return request;
    }

    private bool ValidateAnswerFrame(byte[] answer, InfoNumber infoNumber)
    {
        return answer.Length >= 7
            && answer[0] == 0xD2
            && answer[1] == 0x1D
            && answer[2] == 0xFA
            && answer[3] == infoNumber.High
            && answer[4] == infoNumber.Low;
    }
}
```

---

## 7. Timing and Concurrency Considerations

- **GET requests** always target CAN ID **0x69D** (broadcast)
- **ANSWER responses** come on subsystem-specific IDs (non-overlapping)
- Multiple GET requests can be pending simultaneously (no blocking required)
- Use **CAN ID + timeout** to match responses to requests
- Consider request/response timeout of **500-1000ms** for reliable operation

---

## 8. Error Handling

| Error Case | Handling |
|-----------|----------|
| Device not found | Check device name in `CanParameterRegistry` |
| Parameter not found | Check InfoNumber in `CanParameterRegistry.Parameters` |
| No response (timeout) | Log warning, return null, retry with backoff |
| Invalid response header | Log error, may indicate communication issue |
| Mismatched InfoNumber in response | Discard frame, wait for next response |
| Decoding error | Log error with parameter name, return null |

---

## 9. Reference: Communication Profiles

All devices and their communication profiles are accessible via:

```csharp
// Access all devices
var heatGenerators = CanParameterRegistry.HeatGenerators;      // HG1-HG8
var heatingCircuits = CanParameterRegistry.HeatingCircuits;      // HC1-HC16
var modules = CanParameterRegistry.HeatingCircuitModules;        // HCM1-HCM16

// Lookup specific device
var hg1 = CanParameterRegistry.GetHeatGenerator("HG1");
var hc5 = CanParameterRegistry.GetHeatingCircuit("HC5");
var hcm10 = CanParameterRegistry.GetHeatingCircuitModule("HCM10");

// Access communication profile
var getCmd = hg1.Profile.Get;    // CAN ID 0x69D, bytes [0x31, 0x00, 0xFA]
var answerCmd = hg1.Profile.Answer;  // CAN ID 0x180, bytes [0xD2, 0x1D, 0xFA]
```

See `CAN_ENCODING_README.md` for parameter definitions and data type encoding details.
