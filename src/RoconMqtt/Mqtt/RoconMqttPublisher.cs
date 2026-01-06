using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Mqtt.Models;
using RoconMqtt.Mqtt.Options;
using RoconMqtt.Mqtt.Resilience;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RoconMqtt.Mqtt;

public partial class RoconMqttPublisher(ICanService roconService, IMqttService mqtt, IOptions<MqttOptions> options, ResiliencePipelineFactory resilienceFactory, ILogger<RoconMqttPublisher> logger) : BackgroundService
{
    private readonly MqttOptions _options = options.Value;
    private readonly ICanService _roconService = roconService ?? throw new ArgumentNullException(nameof(roconService));
    private readonly IMqttService _mqtt = mqtt;
    private readonly ILogger<RoconMqttPublisher> _logger = logger;
    private readonly ResiliencePipelineFactory _resilienceFactory = resilienceFactory ?? throw new ArgumentNullException(nameof(resilienceFactory));
    
    private readonly ConcurrentDictionary<string, object?> _lastPublishedValues = new();
    private readonly ConcurrentDictionary<string, string> _deviceIdentifiers = new();
    private bool _discoveryPublished = false;

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

        // small delay before discovery
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        // get data for discovery
        await QueryDeviceIdentifiersAsync(stoppingToken);

        // trigger home assistant discovery
        if (_options.HomeAssistantDiscovery)
        {
            await PublishHomeAssistantDiscoveryAsync(stoppingToken);
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
                    await Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);
                    continue;
                }

                // Loop over all configured devices and parameters
                foreach (var deviceName in _options.Devices)
                {
                    // Query regular parameters
                    await QueryParametersAsync(deviceName, _options.Parameters, resiliencePipeline, stoppingToken);
                    
                    // Query compound parameters (handled transparently by CanService)
                    await QueryParametersAsync(deviceName, _options.CompoundParameters, resiliencePipeline, stoppingToken);
                }

                // Wait before next polling cycle
                await Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);
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

    /// <summary>
    /// Queries device identifiers for all configured devices asynchronously and updates the internal mapping with the
    /// results.
    /// </summary>
    /// <remarks>If a device identifier cannot be retrieved for a device, the device name is used as a
    /// fallback identifier. The internal mapping is updated for each device regardless of success or failure.</remarks>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task QueryDeviceIdentifiersAsync(CancellationToken token)
    {
        LogQueryingDeviceIdentifiers(_logger);

        foreach (var deviceName in _options.Devices)
        {
            try
            {
                var result = await _roconService.SendRequestAndWaitForResponseAsync(
                    deviceName,
                    "DeviceIdentifier",
                    CommandType.Get,
                    null,
                    (int)TimeSpan.FromSeconds(_options.ResponseTimeoutSeconds).TotalMilliseconds,
                    token);

                if (result != null)
                {
                    var deviceId = result.Value.ToString() ?? "unknown";
                    _deviceIdentifiers[deviceName] = deviceId;
                    LogDeviceIdentifierFound(_logger, deviceName, deviceId);
                }
                else
                {
                    _deviceIdentifiers[deviceName] = deviceName;
                    LogDeviceIdentifierNotFound(_logger, deviceName);
                }
            }
            catch (Exception ex)
            {
                _deviceIdentifiers[deviceName] = deviceName;
                LogErrorQueryingDeviceIdentifier(_logger, ex, deviceName);
            }
        }
    }

    /// <summary>
    /// Queries the specified parameters from a device asynchronously, applying the provided resilience pipeline to each
    /// query operation.
    /// </summary>
    /// <remarks>If the cancellation token is signaled, the operation stops processing further parameters.
    /// Exceptions such as circuit breaker and timeout are handled and logged for each parameter individually.</remarks>
    /// <param name="deviceName">The name of the device from which to query parameters. Cannot be null or empty.</param>
    /// <param name="parameterNames">A collection of parameter names to query from the device. Cannot be null or contain null or empty elements.</param>
    /// <param name="resiliencePipeline">The resilience pipeline used to execute each parameter query with fault-handling and retry logic. Cannot be
    /// null.</param>
    /// <param name="stoppingToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task QueryParametersAsync(string deviceName, ICollection<string> parameterNames, Polly.ResiliencePipeline resiliencePipeline, CancellationToken stoppingToken)
    {
        foreach (var parameterName in parameterNames)
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

    /// <summary>
    /// Publishes Home Assistant discovery configuration messages for all configured devices and parameters.
    /// </summary>
    /// <remarks>This method ensures that discovery messages are published only once per application lifetime.
    /// Subsequent calls have no effect if discovery has already been published.</remarks>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    private async Task PublishHomeAssistantDiscoveryAsync(CancellationToken token)
    {
        if (_discoveryPublished)
            return;

        LogPublishingHomeAssistantDiscovery(_logger);

        foreach (var deviceName in _options.Devices)
        {
            // Publish discovery for regular parameters
            await PublishDiscoveryForParametersAsync(deviceName, _options.Parameters, token);
            
            // Publish discovery for compound parameters
            await PublishDiscoveryForParametersAsync(deviceName, _options.CompoundParameters, token);
        }

        _discoveryPublished = true;
    }

    /// <summary>
    /// Publishes Home Assistant discovery configuration messages for the specified parameters of a device using MQTT.
    /// </summary>
    /// <param name="deviceName">The name of the device for which discovery configuration will be published.</param>
    /// <param name="parameterNames">A collection of parameter names to include in the discovery configuration messages. Cannot be null or empty.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    private async Task PublishDiscoveryForParametersAsync(string deviceName, ICollection<string> parameterNames, CancellationToken token)
    {
        foreach (var parameterName in parameterNames)
        {
            var discoveryConfig = CreateHomeAssistantDiscoveryConfig(deviceName, parameterName);
            var discoveryTopic = GetHomeAssistantDiscoveryTopic(deviceName, parameterName);
            var configJson = JsonSerializer.Serialize(discoveryConfig, _unIndentedJsonSerializerOptions);

            await _mqtt.PublishAsync(discoveryTopic, configJson, token, retain: true);
            LogPublishedDiscoveryConfig(_logger, discoveryTopic);
        }
    }

    private static readonly JsonSerializerOptions _unIndentedJsonSerializerOptions = new() { WriteIndented = false };

    /// <summary>
    /// Creates a Home Assistant MQTT discovery configuration for a specified device parameter.
    /// </summary>
    /// <remarks>The returned configuration includes fields such as name, unique_id, object_id, state_topic,
    /// value_template, unit_of_measurement, device_class, state_class, and a nested device object with identification
    /// and metadata. This configuration can be serialized to JSON and published to Home Assistant's MQTT discovery
    /// topic to enable automatic entity creation.</remarks>
    /// <param name="deviceName">The name of the device for which to generate the discovery configuration. Cannot be null or empty.</param>
    /// <param name="parameterName">The name of the parameter to include in the discovery configuration. Cannot be null or empty.</param>
    /// <returns>A Home Assistant MQTT discovery configuration for the specified device parameter.</returns>
    private HomeAssistantDiscoveryConfig CreateHomeAssistantDiscoveryConfig(string deviceName, string parameterName)
    {
        var stateTopic = GetStateTopic(deviceName, parameterName);
        var deviceId = _deviceIdentifiers[deviceName];
        var uniqueId = $"rocon_{deviceId}_{parameterName}";
        var objectId = $"{deviceId}_{parameterName}";

        var (component, unitOfMeasurement, deviceClass, stateClass) = GetParameterMetadata(parameterName);

        return new HomeAssistantDiscoveryConfig
        {
            Name = FormatParameterName(parameterName),
            UniqueId = uniqueId,
            ObjectId = objectId,
            StateTopic = stateTopic,
            ValueTemplate = "{{ value_json.value }}",
            UnitOfMeasurement = unitOfMeasurement,
            DeviceClass = deviceClass,
            StateClass = stateClass,
            Device = new HomeAssistantDeviceInfo
            {
                Identifiers = [$"rocon_{deviceId}"],
                Name = $"Rotex Device {deviceId}",
                Manufacturer = "Rotex",
                Model = "Rotex",
                SwVersion = "1.0"
            }
        };
    }

    /// <summary>
    /// Retrieves metadata describing the component type, unit of measurement, device class, and state class associated
    /// with a specified parameter name.
    /// </summary>
    /// <remarks>The returned metadata can be used to map parameters to standardized representations, such as
    /// those used in sensor or device integrations. The method supports both specific parameter names and pattern-based
    /// matching for common parameter types.</remarks>
    /// <param name="parameterName">The name of the parameter for which to obtain metadata. The value is case-insensitive and must correspond to a
    /// recognized parameter or pattern.</param>
    /// <returns>A tuple containing the component type, unit of measurement, device class, and state class for the specified
    /// parameter. If a particular metadata value is not applicable, the corresponding tuple element is null.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified parameter name is not recognized or does not match any known pattern.</exception>
#pragma warning disable S1192
    private static (string component, string? unitOfMeasurement, string? deviceClass, string? stateClass) GetParameterMetadata(string parameterName)
    {
        return parameterName switch
        {
            // Specific parameter mappings
            "DeviceIdentifier" => ("sensor", null, null, null),
            "Hour" => ("sensor", null, null, "measurement"),
            "Minute" => ("sensor", null, null, "measurement"),
            "Timestamp" => ("sensor", null, "timestamp", null),
            "TR" => ("sensor", "°C", "temperature", "measurement"),
            "V" => ("sensor", "L/h", null, "measurement"),
            "n" => ("sensor", "%", null, "measurement"),
            "ValveCH_DHW" => ("sensor", "%", null, "measurement"),
            "ValveCH_Bypass" => ("sensor", "%", null, "measurement"),
            "TVBH1" => ("sensor", "°C", "temperature", "measurement"),
            "TVBHMIX" => ("sensor", "°C", "temperature", "measurement"),
            "TVBH" => ("sensor", "°C", "temperature", "measurement"),
            "Defrost" => ("binary_sensor", null, null, null),
            
            // Pattern-based matching for remaining parameters
            var p when p.Contains("Temp", StringComparison.OrdinalIgnoreCase) => ("sensor", "°C", "temperature", "measurement"),
            var p when p.Contains("Pressure", StringComparison.OrdinalIgnoreCase) => ("sensor", "bar", "pressure", "measurement"),
            var p when p.Contains("Flow", StringComparison.OrdinalIgnoreCase) || p.Contains("VOLUMENSTROM", StringComparison.OrdinalIgnoreCase) => ("sensor", "L/min", null, "measurement"),
            var p when p.Contains("Power", StringComparison.OrdinalIgnoreCase) || p.Contains("LEISTUNG", StringComparison.OrdinalIgnoreCase) => ("sensor", "W", "power", "measurement"),
            var p when p.Contains("Energy", StringComparison.OrdinalIgnoreCase) => ("sensor", "kWh", "energy", "total_increasing"),
            var p when p.Contains("Humidity", StringComparison.OrdinalIgnoreCase) => ("sensor", "%", "humidity", "measurement"),
            var p when p.Contains("OperatingHours", StringComparison.OrdinalIgnoreCase) || p.Contains("LAUFZEIT", StringComparison.OrdinalIgnoreCase) => ("sensor", "h", "duration", "total_increasing"),
            var p when p.Contains("Active", StringComparison.OrdinalIgnoreCase) || p.Contains("STATUS", StringComparison.OrdinalIgnoreCase) => ("binary_sensor", null, null, null),
            var p when p.Contains("Mode", StringComparison.OrdinalIgnoreCase) || p.Contains("MODUS", StringComparison.OrdinalIgnoreCase) => ("sensor", null, null, null),
            var p when p.Contains("Error", StringComparison.OrdinalIgnoreCase) || p.Contains("FEHLER", StringComparison.OrdinalIgnoreCase) => ("sensor", null, "problem", null),
            var p when p.Contains("Curve", StringComparison.OrdinalIgnoreCase) || p.Contains("KURVE", StringComparison.OrdinalIgnoreCase) => ("sensor", null, null, "measurement"),
            
            // Throw exception for unrecognized parameters
            _ => throw new ArgumentException($"Unrecognized parameter name: {parameterName}. Please add metadata mapping for this parameter.", nameof(parameterName))
        };
    }
#pragma warning restore S1192

    private string GetHomeAssistantDiscoveryTopic(string deviceName, string parameterName)
    {
        var deviceId = _deviceIdentifiers[deviceName];
        var (component, _, _, _) = GetParameterMetadata(parameterName);
        var objectId = $"{deviceId}_{parameterName}".ToLowerInvariant().Replace(" ", "_");
        return $"{_options.HomeAssistantDiscoveryPrefix}/{component}/rocon_{objectId}/config";
    }

    private string GetStateTopic(string deviceName, string parameterName)
    {
        var deviceId = _deviceIdentifiers[deviceName];
        return $"{_options.Topic}/{deviceId}/{parameterName}/state";
    }

    /// <summary>
    /// Converts a parameter name from PascalCase to snake_case format.
    /// </summary>
    /// <remarks>This method is useful for generating entity names compatible with systems that require
    /// snake_case formatting, such as Home Assistant.</remarks>
    /// <param name="parameterName">The parameter name to convert. Must not be null or empty.</param>
    /// <returns>A string containing the parameter name converted to snake_case. The returned string is in lowercase.</returns>
    private static string FormatParameterName(string parameterName)
    {
        // Convert PascalCase to snake_case for Home Assistant entity names
        return string.Concat(parameterName.Select((c, i) =>
            i > 0 && char.IsUpper(c) && !char.IsUpper(parameterName[i - 1])
                ? $"_{c}"
                : c.ToString()
        )).ToLowerInvariant();
    }

    /// <summary>
    /// Sends a 'Get' command to the specified device for the given parameter and waits for a response, then publishes
    /// the result to MQTT if the value has changed.
    /// </summary>
    /// <remarks>If the retrieved value differs from the last published value, the result is serialized and
    /// published to the corresponding MQTT topic. If the value has not changed, publishing is skipped.</remarks>
    /// <param name="deviceName">The name of the device to which the 'Get' command is sent. Cannot be null or empty.</param>
    /// <param name="parameterName">The name of the parameter to retrieve from the device. Cannot be null or empty.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SendGetAndWaitForAnswer(string deviceName, string parameterName, CancellationToken token)
    {
        // Use CanService to coordinate request and response (handles compound parameters transparently)
        var result = await _roconService.SendRequestAndWaitForResponseAsync(
            deviceName, 
            parameterName, 
            CommandType.Get, 
            null, 
            (int)TimeSpan.FromSeconds(_options.ResponseTimeoutSeconds).TotalMilliseconds, 
            token);

        // Publish to MQTT using resolved device identifier
        var deviceId = _deviceIdentifiers[deviceName];
        var key = $"{deviceId}/{parameterName}";
        if (ShouldPublish(key, result))
        {
            var stateTopic = GetStateTopic(deviceName, parameterName);
            var json = JsonSerializer.Serialize(result);
            LogPublishingToMqtt(_logger, stateTopic, json);
            await _mqtt.PublishAsync(stateTopic, json, token);
            
            _lastPublishedValues[key] = result;
        }
        else
        {
            LogValueUnchangedSkippingPublish(_logger, deviceName, parameterName);
        }
    }


    /// <summary>
    /// Determines whether a new value for the specified key should be published based on the configured change
    /// threshold and the previous published value.
    /// </summary>
    /// <remarks>For numeric values, the method uses a relative change threshold to decide whether to publish.
    /// For other types, a standard equality comparison is used. If both the new value and the last published value are
    /// null, the method returns false.</remarks>
    /// <param name="key">The unique identifier for the value to evaluate. Cannot be null.</param>
    /// <param name="newValue">The new value to compare against the last published value. Can be null.</param>
    /// <returns>true if the new value differs sufficiently from the last published value to warrant publishing; otherwise,
    /// false.</returns>
    private bool ShouldPublish(string key, object? newValue)
    {
        if (!_lastPublishedValues.TryGetValue(key, out var lastValue))
        {
            return true;
        }

        if (newValue == null && lastValue == null)
            return false;
        if (newValue == null || lastValue == null)
            return true;

        if (newValue is double newDouble && lastValue is double lastDouble)
        {
            var threshold = _options.ChangeThresholdPercent / 100.0;
            var change = Math.Abs(newDouble - lastDouble);
            var referenceValue = Math.Max(Math.Abs(lastDouble), 0.001);
            return (change / referenceValue) > threshold;
        }

        if (newValue is int newInt && lastValue is int lastInt)
        {
            var threshold = _options.ChangeThresholdPercent / 100.0;
            var change = Math.Abs(newInt - lastInt);
            var referenceValue = Math.Max(Math.Abs(lastInt), 1);
            return (change / (double)referenceValue) > threshold;
        }

        return !newValue.Equals(lastValue);
    }

    #region Logging

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

    [LoggerMessage(EventId = 4012, Level = LogLevel.Debug, Message = "Value unchanged for device {DeviceName} parameter {ParameterName}, skipping publish")]
    private static partial void LogValueUnchangedSkippingPublish(ILogger logger, string deviceName, string parameterName);

    [LoggerMessage(EventId = 4013, Level = LogLevel.Information, Message = "Publishing Home Assistant MQTT Discovery configuration")]
    private static partial void LogPublishingHomeAssistantDiscovery(ILogger logger);

    [LoggerMessage(EventId = 4014, Level = LogLevel.Debug, Message = "Published discovery config to {Topic}")]
    private static partial void LogPublishedDiscoveryConfig(ILogger logger, string topic);

    [LoggerMessage(EventId = 4015, Level = LogLevel.Information, Message = "Querying device identifiers (cGERAETE_KENNUNG)")]
    private static partial void LogQueryingDeviceIdentifiers(ILogger logger);

    [LoggerMessage(EventId = 4016, Level = LogLevel.Information, Message = "Device identifier found for {DeviceName}: {DeviceId}")]
    private static partial void LogDeviceIdentifierFound(ILogger logger, string deviceName, string deviceId);

    [LoggerMessage(EventId = 4017, Level = LogLevel.Warning, Message = "Device identifier not found for {DeviceName}, using device name as fallback")]
    private static partial void LogDeviceIdentifierNotFound(ILogger logger, string deviceName);

    [LoggerMessage(EventId = 4018, Level = LogLevel.Error, Message = "Error querying device identifier for {DeviceName}")]
    private static partial void LogErrorQueryingDeviceIdentifier(ILogger logger, Exception exception, string deviceName);

    #endregion
}
