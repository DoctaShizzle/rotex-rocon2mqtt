namespace RoconMqtt.Can.Models;

/// <summary>
/// Type of CAN communication command
/// </summary>
public enum CommandType
{
    /// <summary>
    /// GET command - request to read a parameter value from the controller
    /// </summary>
    Get,

    /// <summary>
    /// SET command - request to write a parameter value to the controller
    /// </summary>
    Set,

    /// <summary>
    /// ANSWER command - response from controller containing parameter value
    /// </summary>
    Answer
}