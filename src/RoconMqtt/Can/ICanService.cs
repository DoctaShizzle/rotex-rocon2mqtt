using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public interface ICanService
{
    Task ListenForResponses(Func<DecodedParameter, Task> responseAction, CancellationToken token = default);
    Task SendRequestAsync(string deviceName, string parameterName, CommandType commandType, object? value, CancellationToken token);
}
