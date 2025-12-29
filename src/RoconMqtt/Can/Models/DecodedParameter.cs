namespace RoconMqtt.Can.Models;

public record DecodedParameter(
    string Name,
    object Value,
    ParameterDefinition Definition
);