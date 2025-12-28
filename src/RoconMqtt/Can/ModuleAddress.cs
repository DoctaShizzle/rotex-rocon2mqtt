namespace RoconMqtt.Can;

public record CanFrame(uint Id, byte[] Data);

public record ModuleAddress(
    string Name,
    CanFrame Get,
    CanFrame Set,
    uint AnswerId,
    byte[] AnswerHeader // always D2 1D FA
);