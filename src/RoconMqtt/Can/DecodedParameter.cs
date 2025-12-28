namespace RoconMqtt.Can;

public record DecodedParameter(
    string Name,
    object Value,
    ParameterDefinition Definition
);