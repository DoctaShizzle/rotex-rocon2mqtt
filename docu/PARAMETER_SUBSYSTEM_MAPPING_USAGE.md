# Using Parameter-to-Subsystem Mapping in GET/ANSWER Cycle

## Quick Reference

The parameter registry now provides **automatic subsystem mapping** for every parameter. This eliminates the need to manually determine which subsystem (HG, HC, HCM) a parameter belongs to.

## Three Key Methods

### 1. `GetSubsystemForParameter(InfoNumber)` ? `DeviceType?`
**Purpose**: Determine which type of subsystem a parameter belongs to

```csharp
var infoNumber = new InfoNumber(0x0A, 0x06);
var subsystemType = CanParameterRegistry.GetSubsystemForParameter(infoNumber);

if (subsystemType == DeviceType.HeatGenerator)
{
    // This parameter is from a Heat Generator
}
```

### 2. `GetSubsystemProfile(InfoNumber)` ? `CommunicationProfile?`
**Purpose**: Get the complete communication profile for a parameter's subsystem

```csharp
var infoNumber = new InfoNumber(0x0A, 0x06);
var profile = CanParameterRegistry.GetSubsystemProfile(infoNumber);

if (profile != null)
{
    // profile.Get.CanId = 0x69D
    // profile.Get.Bytes = [0x31, 0x00, 0xFA]  (HG GET opcode)
    // profile.Answer.CanId = 0x180  (HG ANSWER CAN ID)
}
```

### 3. `GetDevice(string name)` ? `CanDevice?`
**Purpose**: Get a specific device instance (HG1, HC5, HCM10)

```csharp
var device = CanParameterRegistry.GetDevice("HG1");
if (device != null)
{
    // device.Profile contains the GET/SET/ANSWER commands
}
```

## Typical GET Cycle Flow

```
User Query
    ?
Parameter InfoNumber (e.g., 0x0A06)
    ?
[GetSubsystemProfile] ? Automatic subsystem lookup
    ?
Communication Profile (GET/SET/ANSWER commands)
    ?
Build CAN Frame with correct opcode
    ?
Send on 0x69D, wait for response on subsystem-specific CAN ID
    ?
Validate and decode response
```

## Real-World Example: Reading a Temperature Parameter

```csharp
public async Task<double?> ReadTemperatureAsync(string parameterName)
{
    // Step 1: Find parameter definition
    var paramDef = CanParameterRegistry.Parameters.Values
        .FirstOrDefault(p => p.Name == parameterName);
    
    if (paramDef == null)
    {
        _logger.LogWarning("Parameter not found: {Name}", parameterName);
        return null;
    }

    // Step 2: Automatically get subsystem profile
    var profile = CanParameterRegistry.GetSubsystemProfile(paramDef.InfoNumber);
    if (profile == null)
    {
        _logger.LogWarning("Unknown subsystem for parameter: {Name}", parameterName);
        return null;
    }

    // Step 3: Build GET request
    byte[] getRequest = new byte[8];
    getRequest[0] = profile.Get.Bytes[0];        // Correct GET opcode (0x31, 0x61, or 0xC1)
    getRequest[1] = profile.Get.Bytes[1];        // Sequence byte 1
    getRequest[2] = profile.Get.Bytes[2];        // Sequence byte 2
    getRequest[3] = paramDef.InfoNumber.High;
    getRequest[4] = paramDef.InfoNumber.Low;

    // Step 4: Send request and wait for answer
    _logger.LogDebug("Requesting {Name} from subsystem {Subsystem}", 
        parameterName, profile.Name);
    
    var answer = await _canBus.SendAndWaitAsync(
        profile.Get.CanId,        // Always 0x69D
        getRequest, 
        profile.Answer.CanId,     // Subsystem-specific (0x180, 0x300, or 0x600)
        timeoutMs: 1000
    );

    if (answer == null)
    {
        _logger.LogWarning("No response from {Subsystem} for {Name}", 
            profile.Name, parameterName);
        return null;
    }

    // Step 5: Validate and decode
    if (answer[0] != 0xD2 || answer[1] != 0x1D || answer[2] != 0xFA)
    {
        _logger.LogError("Invalid header in response for {Name}", parameterName);
        return null;
    }

    if (answer[3] != paramDef.InfoNumber.High || answer[4] != paramDef.InfoNumber.Low)
    {
        _logger.LogError("Response InfoNumber mismatch for {Name}", parameterName);
        return null;
    }

    // Step 6: Decode using CanDecoder
    var decoded = _canDecoder.Decode(answer);
    return decoded?.Value as double?;
}
```

## Subsystem Mapping Decision Tree

```
InfoNumber.High = ?
    ?
    ?? 0x00–0x0F
    ?  ??? Heat Generator (HG)
    ?      GET: 0x31, SET: 0x30, Answer: 0x180
    ?      Examples: cAUSSENTEMP, cSPEICHERISTTEMP
    ?
    ?? 0x17–0x1B (TAKES PRECEDENCE)
    ?  ??? Heating Circuit Module (HCM)
    ?      GET: 0xC1, SET: 0xC0, Answer: 0x600
    ?      Examples: cW_WASSERPROG_1, cW_WASSERPROG_2
    ?
    ?? 0x14–0x1F (HCM range excluded by precedence above)
    ?  ??? Heating Circuit (HC)
    ?      GET: 0x61, SET: 0x60, Answer: 0x300
    ?      Examples: cHEIZPROG_1, cHEIZPROG_2
    ?
    ?? Other
       ??? Unknown subsystem (null)
```

## Important Notes

### ? No Hardcoding
Never hardcode GET/SET opcodes or Answer CAN IDs. Always use `GetSubsystemProfile()`.

```csharp
// ? DON'T DO THIS
byte getOpcode = 0x31;  // What if parameter is from HC?

// ? DO THIS
var profile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
byte getOpcode = profile?.Get.Bytes[0];  // Automatic, always correct
```

### ? HCM Range Precedence
Parameters with `InfoNumber.High` in 0x17–0x1B range are **HCM, not HC**.

```csharp
// InfoNumber 0x17:0x00 is HCM, not HC
var subsystem = CanParameterRegistry.GetSubsystemForParameter(new InfoNumber(0x17, 0x00));
// Result: DeviceType.HeatingCircuitModule (not HeatingCircuit)
```

### ? All Parameters Work Automatically
Every parameter registered in `CanParameterRegistry.Parameters` automatically has a subsystem.

```csharp
foreach (var (infoNumber, paramDef) in CanParameterRegistry.Parameters)
{
    // This works for ALL parameters, no exceptions
    var profile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
    // profile is always non-null because all parameters have valid InfoNumber.High
}
```

## Testing the Mapping

```csharp
[Theory]
[InlineData(0x0A, 0x06, DeviceType.HeatGenerator)]        // cEINSTELL_SPEICHERSOLLTEMP2
[InlineData(0x14, 0x10, DeviceType.HeatingCircuit)]       // cHEIZPROG_1_MO
[InlineData(0x17, 0x00, DeviceType.HeatingCircuitModule)]  // cW_WASSERPROG_1
[InlineData(0x00, 0x0C, DeviceType.HeatGenerator)]        // cAUSSENTEMP
public void TestSubsystemMapping(byte high, byte low, DeviceType expected)
{
    var infoNumber = new InfoNumber(high, low);
    var result = CanParameterRegistry.GetSubsystemForParameter(infoNumber);
    Assert.Equal(expected, result);
}

[Theory]
[InlineData(0x0A, 0x06, "0x31", "0x30")]  // HG opcodes
[InlineData(0x14, 0x10, "0x61", "0x60")]  // HC opcodes
[InlineData(0x17, 0x00, "0xC1", "0xC0")]  // HCM opcodes
public void TestOpcodeSelection(byte high, byte low, string expectedGetOpcode, string expectedSetOpcode)
{
    var infoNumber = new InfoNumber(high, low);
    var profile = CanParameterRegistry.GetSubsystemProfile(infoNumber);
    
    Assert.NotNull(profile);
    Assert.Equal(Convert.ToByte(expectedGetOpcode, 16), profile.Get.Bytes[0]);
    Assert.Equal(Convert.ToByte(expectedSetOpcode, 16), profile.Set.Bytes[0]);
}
```

## Migration Checklist

If you're updating an existing GET/ANSWER cycle implementation:

- [ ] Replace hardcoded opcodes with `profile.Get.Bytes[0]` and `profile.Set.Bytes[0]`
- [ ] Replace hardcoded Answer CAN IDs with `profile.Answer.CanId`
- [ ] Use `GetSubsystemProfile()` instead of manual subsystem lookup
- [ ] Verify GET requests are built correctly (order: opcode, seq1, seq2, high, low)
- [ ] Test with parameters from each subsystem (HG, HC, HCM)
- [ ] Verify Answer frames validate correctly

## Related Documentation

- **CAN_ENCODING_README.md** - Full parameter encoding/decoding details
- **CAN_GET_ANSWER_CYCLE.md** - Complete GET/ANSWER protocol specification
- **PARAMETER_SUBSYSTEM_MAPPING.md** - Technical implementation details
