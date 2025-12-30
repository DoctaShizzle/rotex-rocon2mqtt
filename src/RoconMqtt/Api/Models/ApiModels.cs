using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Models;

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
    /// Timeout in milliseconds to wait for response (default: 1000)
    /// </summary>
    public int TimeoutMs { get; set; } = 1000;
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
    /// Timeout in milliseconds to wait for response (default: 1000)
    /// </summary>
    public int TimeoutMs { get; set; } = 1000;
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
    /// Parameter metadata
    /// </summary>
    public ParameterMetadata Metadata { get; set; } = new();
}
