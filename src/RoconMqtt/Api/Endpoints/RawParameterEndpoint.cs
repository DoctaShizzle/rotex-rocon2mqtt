using FastEndpoints;
using RoconMqtt.Api.Models;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;

namespace RoconMqtt.Api.Endpoints;

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
    /// Timeout in milliseconds to wait for response (default: 1000)
    /// </summary>
    public int TimeoutMs { get; set; } = 1000;
}

/// <summary>
/// Endpoint to send raw CAN requests using InfoNumber
/// </summary>
public sealed class RawParameterEndpoint(ICanService canService) : Endpoint<RawParameterRequest, ParameterResponse>
{
    private readonly ICanService _canService = canService;

    public override void Configure()
    {
        Post("/parameters/raw");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Send a raw parameter request using InfoNumber";
            s.Description = "Lower-level endpoint that works directly with InfoNumbers. Sends GET/SET request and waits for ANSWER response.";
            s.Response<ParameterResponse>(200, "Successfully received response");
            s.Response(400, "Invalid request (device not found or invalid command type)");
            s.Response(408, "Timeout waiting for response from device");
            s.ExampleRequest = new RawParameterRequest
            {
                DeviceName = "HG1",
                InfoNumberHigh = 0x0A,
                InfoNumberLow = 0x06,
                CommandType = "Get",
                Value = null,
                TimeoutMs = 1000
            };
        });
    }

    public override async Task HandleAsync(RawParameterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceName))
        {
            ThrowError("DeviceName is required");
        }

        // Parse command type
        if (!Enum.TryParse<CommandType>(req.CommandType, ignoreCase: true, out var commandType))
        {
            ThrowError($"Invalid CommandType: {req.CommandType}. Valid values are: Get, Set");
        }

        if (commandType == CommandType.Answer)
        {
            ThrowError("Cannot use CommandType 'Answer' - it is only for responses from the controller");
        }

        try
        {
            var infoNumber = new InfoNumber(req.InfoNumberHigh, req.InfoNumberLow);
            
            var result = await _canService.SendRawRequestAndWaitForResponseAsync(
                req.DeviceName,
                infoNumber,
                commandType,
                req.Value,
                req.TimeoutMs,
                ct);

            var response = new ParameterResponse
            {
                Name = result.Name,
                Value = result.Value,
                Metadata = new ParameterMetadata
                {
                    Type = result.Definition.Type.ToString(),
                    Min = result.Definition.Min,
                    Max = result.Definition.Max,
                    Default = result.Definition.Default,
                    Writeable = result.Definition.Writeable,
                    Factor = result.Definition.Factor
                }
            };

            await Send.OkAsync(response, ct);
        }
        catch (ArgumentException ex)
        {
            ThrowError(ex.Message);
        }
        catch (TimeoutException ex)
        {
            await Send.ErrorsAsync(408, ct);
            ThrowError(ex.Message);
        }
    }
}
