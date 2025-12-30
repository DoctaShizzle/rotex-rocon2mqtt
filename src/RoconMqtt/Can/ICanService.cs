using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

/// <summary>
/// Defines methods for sending CAN bus requests and handling responses for device parameters asynchronously.
/// </summary>
/// <remarks>Implementations of this interface enable communication with devices over a CAN bus by providing
/// methods to send requests, listen for responses, and handle parameter decoding. All methods are asynchronous and
/// support cancellation via a CancellationToken.</remarks>
public interface ICanService
{
    /// <summary>
    /// Listens asynchronously for responses to a specified command and invokes a callback for each received response.
    /// </summary>
    /// <remarks>The method continues to listen for responses until the operation is cancelled via the
    /// provided token. The callback specified by <paramref name="responseAction"/> is invoked for each response that
    /// matches the specified command, device, and parameter.</remarks>
    /// <param name="commandType">The type of command to listen for. Determines which responses are processed.</param>
    /// <param name="deviceName">The name of the device from which to listen for responses. Cannot be null or empty.</param>
    /// <param name="parameterName">The name of the parameter associated with the command. Cannot be null or empty.</param>
    /// <param name="responseAction">A callback function to invoke for each decoded response parameter. The function receives a <see
    /// cref="DecodedParameter"/> representing the response.</param>
    /// <param name="token">A cancellation token that can be used to cancel the listening operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous listening operation. The task completes when listening is stopped or
    /// cancelled.</returns>
    Task ListenForResponses(CommandType commandType, string deviceName, string parameterName, Func<DecodedParameter, Task> responseAction, CancellationToken token = default);

    /// <summary>
    /// Sends an asynchronous command request to a specified device parameter.
    /// </summary>
    /// <param name="deviceName">The name of the target device to which the command is sent. Cannot be null or empty.</param>
    /// <param name="parameterName">The name of the parameter on the device to be controlled or queried. Cannot be null or empty.</param>
    /// <param name="commandType">The type of command to execute on the device parameter.</param>
    /// <param name="value">The value to be sent with the command. The expected type and meaning depend on the command and parameter. May be
    /// null if the command does not require a value.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendRequestAsync(string deviceName, string parameterName, CommandType commandType, object? value, CancellationToken token);

    /// <summary>
    /// Sends a command request to the specified device and waits asynchronously for a decoded response or until the
    /// operation times out or is canceled.
    /// </summary>
    /// <remarks>If the operation is canceled via the cancellation token or the timeout elapses before a
    /// response is received, the returned task completes without a result. This method is thread-safe and can be called
    /// concurrently for different devices or parameters.</remarks>
    /// <param name="deviceName">The name of the target device to which the command is sent. Cannot be null or empty.</param>
    /// <param name="parameterName">The name of the parameter to be accessed or modified on the device. Cannot be null or empty.</param>
    /// <param name="commandType">The type of command to execute on the device, such as read or write.</param>
    /// <param name="value">The value to be written to the device parameter if the command type requires it; otherwise, null.</param>
    /// <param name="timeoutMs">The maximum duration, in milliseconds, to wait for a response before the operation times out. Must be greater
    /// than zero.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a DecodedParameter object with the
    /// response from the device. The result is null if no response is received before the timeout or if the operation
    /// is canceled.</returns>
    Task<DecodedParameter> SendRequestAndWaitForResponseAsync(string deviceName, string parameterName, CommandType commandType, object? value, int timeoutMs, CancellationToken token);

    /// <summary>
    /// Sends a raw command request using InfoNumber and waits asynchronously for a decoded response or until the
    /// operation times out or is canceled.
    /// </summary>
    /// <remarks>This is a lower-level method that works directly with InfoNumbers instead of parameter names.
    /// Use this when you need direct control over the InfoNumber being queried.</remarks>
    /// <param name="deviceName">The name of the target device to which the command is sent. Cannot be null or empty.</param>
    /// <param name="infoNumber">The InfoNumber to query or set.</param>
    /// <param name="commandType">The type of command to execute on the device, such as GET or SET.</param>
    /// <param name="value">The value to be written if using SET command; otherwise, null or 0 for GET.</param>
    /// <param name="timeoutMs">The maximum duration, in milliseconds, to wait for a response before the operation times out. Must be greater
    /// than zero.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a DecodedParameter object with the
    /// response from the device.</returns>
    Task<DecodedParameter> SendRawRequestAndWaitForResponseAsync(string deviceName, InfoNumber infoNumber, CommandType commandType, object? value, int timeoutMs, CancellationToken token);

    /// <summary>
    /// Sends a raw command request using InfoNumber with encoding/decoding hints and waits asynchronously for a decoded response.
    /// </summary>
    Task<DecodedParameter> SendRawRequestAndWaitForResponseAsync(string deviceName, InfoNumber infoNumber, CommandType commandType, object? value, int timeoutMs, EncodingHints? encodingHints, DecodingHints? decodingHints, CancellationToken token);
}
