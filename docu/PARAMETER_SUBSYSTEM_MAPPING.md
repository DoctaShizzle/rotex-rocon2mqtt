# Parameter-to-Subsystem Mapping Implementation

## Overview

Added automatic subsystem mapping to `CanParameterRegistry` that dynamically determines which subsystem (Heat Generator, Heating Circuit, or Heating Circuit Module) a parameter belongs to, based on its `InfoNumber.High` byte.

## Changes Made

### 1. Code Changes: `CanParameterRegistry.cs`

**New Methods Added:**

#### `GetSubsystemForParameter(InfoNumber infoNumber) ? DeviceType?`
- Determines the subsystem type for any parameter based on `InfoNumber.High` byte
- Mapping rules:
  - `0x00–0x0F` ? `DeviceType.HeatGenerator`
  - `0x14–0x1F` ? `DeviceType.HeatingCircuit` (with HCM override below)
  - `0x17–0x1B` ? `DeviceType.HeatingCircuitModule` (takes precedence over HC range)
  - Other ranges ? `null` (unknown)

#### `GetSubsystemProfile(InfoNumber infoNumber) ? CommunicationProfile?`
- Returns the complete communication profile for a parameter's subsystem
- Automatically sets:
  - **GET opcode** (0x31 for HG, 0x61 for HC, 0xC1 for HCM)
  - **SET opcode** (0x30 for HG, 0x60 for HC, 0xC0 for HCM)
  - **Answer CAN ID** (0x180 for HG, 0x300 for HC, 0x600 for HCM)
  - **Sequence bytes** (standardized, device-specific)

### 2. Documentation Updates

#### `CAN_ENCODING_README.md`
- Added **Subsystem Determination** table with InfoNumber ranges
- Added **Dynamic Parameter-to-Subsystem Mapping** code examples
- Updated GET Cycle example to show automatic profile lookup
- Clarified overlapping ranges (HCM 0x17–0x1B takes precedence over HC)

#### `CAN_GET_ANSWER_CYCLE.md`
- Updated **Step 1: Look Up Device and Parameter** to use `GetSubsystemProfile()`
- Updated **Step 2: Build GET Request Frame** to use subsystem profile opcodes
- Updated **Full Request/Response Cycle Example** with new `CanGetCycleService` implementation
- Shows how to query parameters without hardcoding device-specific mappings

## Benefits

### ? **No Hardcoding**
- Parameters are no longer manually assigned to subsystems
- Subsystem membership is dynamically determined from `InfoNumber.High`

### ? **Extensible**
- Adding new parameters automatically works with GET/ANSWER cycle
- No code changes needed when new parameters are added to registry

### ? **Self-Documenting**
- `InfoNumber.High` ranges clearly indicate subsystem membership
- Code comments explain the mapping rules

### ? **Automatic Opcode Selection**
- GET/SET opcodes are automatically selected based on subsystem
- No risk of using wrong opcode for a parameter

## Usage Examples

### Example 1: Get Subsystem for a Parameter
```csharp
var infoNumber = new InfoNumber(0x0A, 0x06);  // cEINSTELL_SPEICHERSOLLTEMP2
var subsystem = CanParameterRegistry.GetSubsystemForParameter(infoNumber);
// Result: DeviceType.HeatGenerator (0x0A is in 0x00–0x0F range)
```

### Example 2: Get Communication Profile
```csharp
var profile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
// Result: CommunicationProfile with:
//   - GET opcode: 0x31 (Heat Generator)
//   - SET opcode: 0x30 (Heat Generator)
//   - Answer CAN ID: 0x180 (Heat Generator base)
```

### Example 3: Build GET Request Automatically
```csharp
var profile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
byte[] getRequest = new byte[8];
getRequest[0] = profile.Get.Bytes[0];  // Correct opcode (never hardcoded)
getRequest[3] = infoNumber.High;
getRequest[4] = infoNumber.Low;
canBus.Send(profile.Get.CanId, getRequest);
```

## Subsystem Mapping Reference

| InfoNumber.High | Subsystem | GET Opcode | SET Opcode | Answer CAN ID | Example Parameters |
|---|---|---|---|---|---|
| 0x00–0x0F | Heat Generator | 0x31 | 0x30 | 0x180–0x187 | cAUSSENTEMP, cSPEICHERISTTEMP, cKESSELISTTEMP |
| 0x14–0x1F | Heating Circuit | 0x61 | 0x60 | 0x300–0x30F | cHEIZPROG_1, cHEIZPROG_2 |
| 0x17–0x1B | Heating Circuit Module | 0xC1 | 0xC0 | 0x600–0x60F | cW_WASSERPROG_1, cW_WASSERPROG_2 |

**Note**: The 0x17–0x1B range overlaps with HC (0x14–0x1F). HCM takes precedence for this range.

## Next Steps

1. **Update GET Cycle Service**: Use `GetSubsystemProfile()` in the background service that reads parameters
2. **Update SET Command Handler**: Use `GetSubsystemProfile()` when writing parameters
3. **Parameter Discovery**: Consider generating this mapping from `data.json` to ensure consistency

## Code Quality

- ? Build: **Successful**
- ? No breaking changes to existing APIs
- ? All parameters continue to work as before
- ? New functionality is purely additive
