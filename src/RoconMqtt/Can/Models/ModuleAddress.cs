namespace RoconMqtt.Can.Models;

public record ModuleAddress(
    string Name,
    CanFrame Get,
    CanFrame Set,
    uint AnswerId,
    byte[] AnswerHeader // always D2 1D FA
);