using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoconMqtt.Can;
using RoconMqtt.Mqtt.Options;
using System.Text.Json;

namespace RoconMqtt.Mqtt;

public class RoconMqttPublisher(IRoconService roconService, IMqttService mqtt, IOptions<MqttOptions> options, ILogger<RoconMqttPublisher> logger) : BackgroundService
{
    private readonly MqttOptions _options = options.Value;
    private readonly IRoconService _roconService = roconService ?? throw new ArgumentNullException(nameof(roconService));
    private readonly IMqttService _mqtt = mqtt;
    private readonly ILogger<RoconMqttPublisher> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RoconMqttPublisher started");
        
        _ = Task.Run(() => ListenForResponses(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Request cAUSSENTEMP (outside temperature)
                await _roconService.SendRequestAsync("cAUSSENTEMP", 0, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, "RoconMqttPublisher cancellation requested");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while requesting sensor data");
            }
        }

        _logger.LogInformation("RoconMqttPublisher stopped");
    }

    private Task ListenForResponses(CancellationToken token)
    {
        return _roconService.ListenForResponses(async decodedParameter =>
        {
            try
            {
                var json = JsonSerializer.Serialize(decodedParameter);
                
                _logger.LogDebug("Publishing to MQTT topic {Topic}: {Message}", _options.Topic, json);

                await _mqtt.PublishAsync(_options.Topic, json, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to MQTT");
            }
        }, token);
    }
}