using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Options;
using RoconMqtt.Mqtt.Resilience;
using System.Text.Json;

namespace RoconMqtt.Mqtt;

public partial class RoconMqttPublisher(ICanService roconService, IMqttService mqtt, IOptions<MqttOptions> options, ResiliencePipelineFactory resilienceFactory, ILogger<RoconMqttPublisher> logger) : BackgroundService
{
    private readonly MqttOptions _options = options.Value;
    private readonly ICanService _roconService = roconService ?? throw new ArgumentNullException(nameof(roconService));
    private readonly IMqttService _mqtt = mqtt;
    private readonly ILogger<RoconMqttPublisher> _logger = logger;
    private readonly ResiliencePipelineFactory _resilienceFactory = resilienceFactory ?? throw new ArgumentNullException(nameof(resilienceFactory));

    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// </summary>
    public bool Enabled { get; set; } = options.Value.Enabled;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogPublisherStarted(_logger);

        // Validate configuration
        if (_options.Devices.Count == 0)
        {
            LogNoDevicesConfigured(_logger);
        }
        if (_options.Parameters.Count == 0)
        {
            LogNoParametersConfigured(_logger);
        }

        // Create resilience pipeline
        var resiliencePipeline = _resilienceFactory.CreateQueryPipeline();

        // Main polling loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Check if publisher is enabled
                if (!_options.Enabled)
                {
                    LogPublisherDisabled(_logger);
                    await Task.Delay(_options.PollingIntervalMs, stoppingToken);
                    continue;
                }

                // Loop over all configured devices and parameters
                foreach (var deviceName in _options.Devices)
                {
                    foreach (var parameterName in _options.Parameters)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        try
                        {
                            await resiliencePipeline.ExecuteAsync(async ct =>
                            {
                                await SendGetAndWaitForAnswer(deviceName, parameterName, ct);
                            }, stoppingToken);
                        }
                        catch (BrokenCircuitException)
                        {
                            LogCircuitBreakerOpenForQuery(_logger, deviceName, parameterName);
                        }
                        catch (TimeoutException)
                        {
                            LogTimeoutWaitingForAnswer(_logger, deviceName, parameterName);
                        }
                        catch (Exception ex)
                        {
                            LogErrorQueryingDevice(_logger, ex, deviceName, parameterName);
                        }
                    }
                }

                // Wait before next polling cycle
                await Task.Delay(_options.PollingIntervalMs, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                LogCancellationRequested(_logger);
                break;
            }
            catch (Exception ex)
            {
                LogErrorInPollingLoop(_logger, ex);
            }
        }

        LogPublisherStopped(_logger);
    }

    private async Task SendGetAndWaitForAnswer(string deviceName, string parameterName, CancellationToken token)
    {
        // Use CanService to coordinate request and response
        var result = await _roconService.SendRequestAndWaitForResponseAsync(
            deviceName, 
            parameterName, 
            CommandType.Get, 
            null, 
            _options.ResponseTimeoutMs, 
            token);

        // Publish to MQTT
        var json = JsonSerializer.Serialize(result);
        LogPublishingToMqtt(_logger, _options.Topic, json);
        await _mqtt.PublishAsync(_options.Topic, json, token);
    }

    [LoggerMessage(EventId = 4001, Level = LogLevel.Information, Message = "RoconMqttPublisher started")]
    private static partial void LogPublisherStarted(ILogger logger);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Warning, Message = "No devices configured in Mqtt:Devices. Publisher will not query any devices.")]
    private static partial void LogNoDevicesConfigured(ILogger logger);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Warning, Message = "No parameters configured in Mqtt:Parameters. Publisher will not query any parameters.")]
    private static partial void LogNoParametersConfigured(ILogger logger);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Warning, Message = "Timeout waiting for ANSWER from device {DeviceName} for parameter {ParameterName}")]
    private static partial void LogTimeoutWaitingForAnswer(ILogger logger, string deviceName, string parameterName);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Error, Message = "Error querying device {DeviceName} for parameter {ParameterName}")]
    private static partial void LogErrorQueryingDevice(ILogger logger, Exception exception, string deviceName, string parameterName);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Information, Message = "RoconMqttPublisher cancellation requested")]
    private static partial void LogCancellationRequested(ILogger logger);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Error, Message = "Error in polling loop")]
    private static partial void LogErrorInPollingLoop(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Information, Message = "RoconMqttPublisher stopped")]
    private static partial void LogPublisherStopped(ILogger logger);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Debug, Message = "Publishing to MQTT topic {Topic}: {Message}")]
    private static partial void LogPublishingToMqtt(ILogger logger, string topic, string message);

    [LoggerMessage(EventId = 4010, Level = LogLevel.Warning, Message = "Circuit breaker open for device {DeviceName} parameter {ParameterName}, skipping query")]
    private static partial void LogCircuitBreakerOpenForQuery(ILogger logger, string deviceName, string parameterName);

    [LoggerMessage(EventId = 4011, Level = LogLevel.Debug, Message = "Publisher is disabled, skipping polling cycle")]
    private static partial void LogPublisherDisabled(ILogger logger);
}