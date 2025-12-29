namespace RoconMqtt.Can.Models;

/// <summary>
/// Represents the CAN communication addressing for a specific module instance.
/// Contains the frames and identifiers needed for GET/SET/ANSWER communication.
/// </summary>
/// <param name="Name">Module name (e.g., "HG1", "HC5", "HCM10")</param>
/// <param name="Get">CAN frame template for GET requests (sent to query parameter values)</param>
/// <param name="Set">CAN frame template for SET requests (sent to write parameter values)</param>
/// <param name="AnswerId">CAN identifier on which the module responds (ANSWER frames)</param>
/// <param name="AnswerHeader">Expected header bytes in ANSWER frames (always 0xD2, 0x1D, 0xFA)</param>
public record ModuleAddress(
    string Name,
    CanFrame Get,
    CanFrame Set,
    uint AnswerId,
    byte[] AnswerHeader
);