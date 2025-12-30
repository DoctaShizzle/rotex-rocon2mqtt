using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Options;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RoconMqtt.Mqtt;

public class RoconMqttPublisher(ICanService roconService, IMqttService mqtt, IOptions<MqttOptions> options, ILogger<RoconMqttPublisher> logger) : BackgroundService
{
    private readonly MqttOptions _options = options.Value;
    private readonly ICanService _roconService = roconService ?? throw new ArgumentNullException(nameof(roconService));
    private readonly IMqttService _mqtt = mqtt;
    private readonly ILogger<RoconMqttPublisher> _logger = logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<DecodedParameter>> _pendingRequests = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RoconMqttPublisher started");

        // Validate configuration
        if (_options.Devices.Count == 0)
        {
            _logger.LogWarning("No devices configured in Mqtt:Devices. Publisher will not query any devices.");
        }
        if (_options.Parameters.Count == 0)
        {
            _logger.LogWarning("No parameters configured in Mqtt:Parameters. Publisher will not query any parameters.");
        }
        
        // Start listening for responses in background
        _ = Task.Run(() => ListenForResponses(stoppingToken), stoppingToken);

        // Main polling loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Loop over all configured devices and parameters
                foreach (var deviceName in _options.Devices)
                {
                    foreach (var parameterName in _options.Parameters)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        try
                        {
                            await SendGetAndWaitForAnswer(deviceName, parameterName, stoppingToken);
                        }
                        catch (TimeoutException)
                        {
                            _logger.LogWarning("Timeout waiting for ANSWER from device {Device} for parameter {Parameter}", 
                                deviceName, parameterName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error querying device {Device} for parameter {Parameter}", 
                                deviceName, parameterName);
                        }
                    }
                }

                // Wait before next polling cycle
                await Task.Delay(_options.PollingIntervalMs, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("RoconMqttPublisher cancellation requested");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in polling loop");
            }
        }

        _logger.LogInformation("RoconMqttPublisher stopped");
    }

    private async Task SendGetAndWaitForAnswer(string deviceName, string parameterName, CancellationToken token)
    {
        var requestKey = $"{deviceName}:{parameterName}";
        var tcs = new TaskCompletionSource<DecodedParameter>();

        // Register pending request
        _pendingRequests[requestKey] = tcs;

        try
        {
            // Send GET request
            await _roconService.SendRequestAsync(deviceName, parameterName, CommandType.Get, null, token);

            // Wait for ANSWER with timeout
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            timeoutCts.CancelAfter(_options.ResponseTimeoutMs);

            var result = await tcs.Task.WaitAsync(timeoutCts.Token);

            // Publish to MQTT
            var json = JsonSerializer.Serialize(result);
            _logger.LogDebug("Publishing to MQTT topic {Topic}: {Message}", _options.Topic, json);
            await _mqtt.PublishAsync(_options.Topic, json, token);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            // Timeout occurred
            throw new TimeoutException($"Timeout waiting for response from {deviceName} for {parameterName}");
        }
        finally
        {
            // Clean up pending request
            _pendingRequests.TryRemove(requestKey, out _);
        }
    }

    private Task ListenForResponses(CancellationToken token)
    {
        return _roconService.ListenForResponses(async decodedParameter =>
        {
            try
            {
                // Check if this is a response to a pending request
                // Try to match based on parameter name
                var matchingRequest = _pendingRequests.FirstOrDefault(kvp => 
                    kvp.Key.EndsWith($":{decodedParameter.Name}"));

                if (matchingRequest.Key != null)
                {
                    // Complete the pending request
                    matchingRequest.Value.TrySetResult(decodedParameter);
                }
                else
                {
                    // Unsolicited response - still publish it
                    var json = JsonSerializer.Serialize(decodedParameter);
                    _logger.LogDebug("Publishing unsolicited response to MQTT topic {Topic}: {Message}", 
                        _options.Topic, json);
                    await _mqtt.PublishAsync(_options.Topic, json, token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling response for parameter {Parameter}", decodedParameter.Name);
            }
        }, token);
    }
}