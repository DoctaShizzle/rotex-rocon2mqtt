using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Models;

/// <summary>
/// Request to get parameter metadata
/// </summary>
public sealed class GetParameterMetadataRequest
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;
}

/// <summary>
/// Request to query a parameter value from a device
/// </summary>
public sealed class GetParameterRequest
{
    /// <summary>
    /// Device name (e.g., HG1, HC1, HCM1)
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// Parameter name (e.g., cAUSSENTEMP)
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;

    /// <summary>
    /// Timeout in milliseconds to wait for response (default: 30000)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;
}

/// <summary>
/// Request to set a parameter value on a device
/// </summary>
public sealed class SetParameterRequest
{
    /// <summary>
    /// Device name (e.g., HG1, HC1, HCM1)
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// Parameter name (e.g., cEINSTELL_SPEICHERSOLLTEMP)
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;

    /// <summary>
    /// Value to set (type depends on parameter definition)
    /// </summary>
    public object Value { get; set; } = null!;

    /// <summary>
    /// Timeout in milliseconds to wait for response (default: 30000)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;
}

/// <summary>
/// Request to query multiple parameter values from a device
/// </summary>
public sealed class GetParametersBulkRequest
{
    /// <summary>
    /// Device name (e.g., HG1, HC1, HCM1)
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// List of parameter names to query
    /// </summary>
    public List<string> ParameterNames { get; set; } = [];

    /// <summary>
    /// Timeout in milliseconds to wait for each response (default: 30000)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;
}

/// <summary>
/// Request to query/set a parameter using InfoNumber directly
/// </summary>
public sealed class RawParameterRequest
{
    /// <summary>
    /// Device name (e.g., HG1, HC1, HCM1)
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// InfoNumber high byte (e.g., 0x0A)
    /// </summary>
    public byte InfoNumberHigh { get; set; }

    /// <summary>
    /// InfoNumber low byte (e.g., 0x06)
    /// </summary>
    public byte InfoNumberLow { get; set; }

    /// <summary>
    /// Command type: Get or Set
    /// </summary>
    public string CommandType { get; set; } = "Get";

    /// <summary>
    /// Value to set (only for SET commands, optional for GET)
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Timeout in milliseconds to wait for response (default: 30000)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Parameter type for encoding/decoding unknown parameters (Int, Float, Bool, TimeRange, Enum). Default: Int
    /// </summary>
    public string? ParameterType { get; set; }

    /// <summary>
    /// Whether to use big-endian encoding/decoding for unknown parameters. Default: false
    /// </summary>
    public bool? BigEndian { get; set; }

    /// <summary>
    /// Factor to multiply/divide by when encoding/decoding float values for unknown parameters. Default: 1.0
    /// </summary>
    public double? Factor { get; set; }
}

/// <summary>
/// Response containing a decoded parameter value
/// </summary>
public sealed class ParameterResponse
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parameter value (type depends on parameter definition)
    /// </summary>
    public object Value { get; set; } = null!;

    /// <summary>
    /// Parameter metadata
    /// </summary>
    public ParameterMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Parameter metadata
/// </summary>
public sealed class ParameterMetadata
{
    /// <summary>
    /// Parameter type (Int, Float, Bool, TimeRange, Enum)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Minimum value (if applicable)
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Maximum value (if applicable)
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Default value
    /// </summary>
    public object? Default { get; set; }

    /// <summary>
    /// Whether the parameter can be written
    /// </summary>
    public bool Writeable { get; set; }

    /// <summary>
    /// Scaling factor (for numeric types)
    /// </summary>
    public double? Factor { get; set; }

    /// <summary>
    /// Big or Little endian
    /// </summary>
    public bool BigEndian { get; set; }

    /// <summary>
    /// InfoNumber high byte (e.g., 0x0A)
    /// </summary>
    public byte InfoNumberHigh { get; set; }

    /// <summary>
    /// InfoNumber low byte (e.g., 0x06)
    /// </summary>
    public byte InfoNumberLow { get; set; }

    /// <summary>
    /// Home Assistant component type
    /// </summary>
    public string? HomeAssistantComponent { get; set; }

    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string? UnitOfMeasurement { get; set; }

    /// <summary>
    /// Home Assistant device class
    /// </summary>
    public string? DeviceClass { get; set; }

    /// <summary>
    /// Home Assistant state class
    /// </summary>
    public string? StateClass { get; set; }
}

/// <summary>
/// Response containing list of available devices
/// </summary>
public sealed class DevicesResponse
{
    /// <summary>
    /// List of available devices
    /// </summary>
    public List<DeviceInfo> Devices { get; set; } = [];
}

/// <summary>
/// Information about a device
/// </summary>
public sealed class DeviceInfo
{
    /// <summary>
    /// Device name (e.g., HG1, HC1, HCM1)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Device type (HeatGenerator, HeatingCircuit, HeatingCircuitModule)
    /// </summary>
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Response containing list of available parameters
/// </summary>
public sealed class ParametersResponse
{
    /// <summary>
    /// List of available parameters
    /// </summary>
    public List<ParameterInfo> Parameters { get; set; } = [];
}

/// <summary>
/// Information about a parameter
/// </summary>
public sealed class ParameterInfo
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parameter original name
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// Parameter metadata
    /// </summary>
    public ParameterMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Response containing multiple parameter values
/// </summary>
public sealed class ParametersBulkResponse
{
    /// <summary>
    /// Successfully retrieved parameters
    /// </summary>
    public List<ParameterResponse> Results { get; set; } = [];

    /// <summary>
    /// Failed parameter queries with error messages
    /// </summary>
    public List<ParameterError> Errors { get; set; } = [];
}

/// <summary>
/// Error information for a failed parameter query
/// </summary>
public sealed class ParameterError
{
    /// <summary>
    /// Parameter name that failed
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
