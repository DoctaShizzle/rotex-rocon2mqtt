using Microsoft.Extensions.Options;
using RoconMqtt.Mqtt;

namespace RoconMqtt.Can;

internal class RoconService(ICanReader canReader, ICanDecoder canDecoder, ICanEncoder canEncoder, IOptions<CanOptions> canOptions) : IRoconService
{
    private readonly ICanReader _canReader = canReader ?? throw new ArgumentNullException(nameof(canReader));
    private readonly ICanDecoder _canDecoder = canDecoder ?? throw new ArgumentNullException(nameof(canDecoder));
    private readonly ICanEncoder _canEncoder = canEncoder ?? throw new ArgumentNullException(nameof(canEncoder));
    private readonly IOptions<CanOptions> _canOptions = canOptions ?? throw new ArgumentNullException(nameof(canOptions));

    public async Task SendFrameAsync(uint canId, byte[] data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);
        await _canReader.SendFrameAsync(canId, data, token);
    }

    public async Task SendRequestAsync(string parameterName, object value, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(parameterName);
        ArgumentNullException.ThrowIfNull(value);

        // Find the parameter definition by name
        var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == parameterName);
        if (paramDef == null)
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry", nameof(parameterName));

        // Encode the value
        var frameData = _canEncoder.Encode(paramDef.InfoNumber, value);

        // Send the frame using the configured send CAN ID
        await _canReader.SendFrameAsync(_canOptions.Value.SendCanFrameId, frameData, token);
    }

    public async Task ListenForResponses(Func<DecodedParameter, Task> responseAction, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(responseAction);

        await foreach (var frame in _canReader.ReadFramesAsync(token).Where(f => f.Id >= _canOptions.Value.ReceiveFromCanFrameId && f.Id <= _canOptions.Value.ReceiveToCanFrameId))
        {
            var decoded = _canDecoder.Decode(frame.Data);
            if (decoded != null)
            {
                await responseAction(decoded);
            }
        }
    }
}
