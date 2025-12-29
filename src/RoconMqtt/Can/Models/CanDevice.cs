using System;
using System.Collections.Generic;
using System.Text;

namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents a device with its communication profile
/// </summary>
public class CanDevice
{
    /// <summary>
    /// Device name (e.g., "HG1", "HC5", "HCM10")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Device type category
    /// </summary>
    public DeviceType Type { get; set; }

    /// <summary>
    /// Communication profile for this device
    /// </summary>
    public required CommunicationProfile Profile { get; set; }
}