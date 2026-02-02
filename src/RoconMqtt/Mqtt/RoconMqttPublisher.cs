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

public partial class RoconMqttPublisher(ICanService roconService, IMqttService mqtt, IOptions<MqttOptions> options, ICanParameterRegistry parameterRegistry, ResiliencePipelineFactory resilienceFactory, ILogger<RoconMqttPublisher> logger) : BackgroundService
{
    private readonly MqttOptions _options = options.Value;
    private readonly ICanParameterRegistry _parameterRegistry = parameterRegistry ?? throw new ArgumentNullException(nameof(parameterRegistry));
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

        // Create resilience pipeline
        var resiliencePipeline = _resilienceFactory.CreateQueryPipeline();

        // Main polling loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Check if publisher is enabled
                if (!Enabled)
                {
                    LogPublisherDisabled(_logger);
                    await Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);
                    continue;
                }

                // trigger home assistant discovery (skips if it's already done)
                if (_options.HomeAssistant.Discovery)
                {
                    await PublishHomeAssistantDiscoveryAsync(stoppingToken);
                }

                // Loop over all configured devices and parameters
                foreach (var deviceName in _options.Devices)
                {
                    // Skip devices without valid identifiers
                    if (!_deviceIdentifiers.ContainsKey(deviceName))
                    {
                        LogSkippingDeviceWithoutIdentifier(_logger, deviceName);
                        continue;
                    }
                    
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
    /// <remarks>Only devices with successfully retrieved identifiers are added to the internal mapping.
    /// Devices that fail to return a valid identifier are excluded from all subsequent operations.</remarks>
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

                if (result != null && result.Value != null)
                {
                    var deviceId = result.Value.ToString();
                    if (!string.IsNullOrWhiteSpace(deviceId))
                    {
                        _deviceIdentifiers[deviceName] = deviceId;
                        LogDeviceIdentifierFound(_logger, deviceName, deviceId);
                    }
                    else
                    {
                        LogDeviceIdentifierInvalid(_logger, deviceName);
                    }
                }
                else
                {
                    LogDeviceIdentifierNotFound(_logger, deviceName);
                }
            }
            catch (Exception ex)
            {
                LogErrorQueryingDeviceIdentifier(_logger, ex, deviceName);
            }
        }
        
        // Log summary of successfully initialized devices
        if (_deviceIdentifiers.Count == 0)
        {
            LogNoValidDeviceIdentifiers(_logger);
        }
        else
        {
            LogDeviceIdentifiersInitialized(_logger, _deviceIdentifiers.Count, _options.Devices.Count);
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
            // Skip devices without valid identifiers
            if (!_deviceIdentifiers.ContainsKey(deviceName))
            {
                LogSkippingDiscoveryForDevice(_logger, deviceName);
                continue;
            }
            
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
    /// <remarks>Parameters that are not found in the registry or lack required Home Assistant metadata are skipped
    /// with a warning log. This is common for compound parameters that may not have Home Assistant component definitions.</remarks>
    /// <param name="deviceName">The name of the device for which discovery configuration will be published.</param>
    /// <param name="parameterNames">A collection of parameter names to include in the discovery configuration messages. Cannot be null or empty.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    private async Task PublishDiscoveryForParametersAsync(string deviceName, ICollection<string> parameterNames, CancellationToken token)
    {
        foreach (var parameterName in parameterNames)
        {
            try
            {
                var discoveryConfig = CreateHomeAssistantDiscoveryConfig(deviceName, parameterName);
                var discoveryTopic = GetHomeAssistantDiscoveryTopic(deviceName, parameterName);
                var configJson = JsonSerializer.Serialize(discoveryConfig, _unIndentedJsonSerializerOptions);

                await _mqtt.PublishAsync(discoveryTopic, configJson, token, retain: true);
                LogPublishedDiscoveryConfig(_logger, discoveryTopic);
            }
            catch (ArgumentException ex)
            {
                LogSkippingDiscoveryForParameter(_logger, parameterName, ex.Message);
            }
        }
    }

    private static readonly JsonSerializerOptions _unIndentedJsonSerializerOptions = new() { WriteIndented = false, TypeInfoResolver = ApiJsonContext.Default };

    private const string deviceIdKey = "deviceId";
    private const string objectIdKey = "objectId";

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
        var parameterNameSnakeCase = ToSnakeCase(parameterName);
        var uniqueId = FormatTemplate(_options.HomeAssistant.UniqueIdFormat, new() { { deviceIdKey, deviceId }, { "parameterName", parameterNameSnakeCase } });
        var objectId = FormatTemplate(_options.HomeAssistant.ObjectIdFormat, new() { { deviceIdKey, deviceId }, { "parameterName", parameterNameSnakeCase } });

        var (component, unitOfMeasurement, deviceClass, stateClass) = GetParameterMetadata(parameterName);

        // Format default_entity_id as {component}.{objectId}
        var defaultEntityId = $"{component}.{objectId}";

        return new HomeAssistantDiscoveryConfig
        {
            Name = FormatParameterDisplayName(parameterName),
            UniqueId = uniqueId,
            DefaultEntityId = defaultEntityId,
            StateTopic = stateTopic,
            ValueTemplate = _options.HomeAssistant.ValueTemplate,
            UnitOfMeasurement = unitOfMeasurement,
            DeviceClass = deviceClass,
            StateClass = stateClass,
            Device = new HomeAssistantDeviceInfo
            {
                Identifiers = [FormatTemplate(_options.HomeAssistant.DeviceIdentifierFormat, new() { { deviceIdKey, deviceId } })],
                Name = FormatTemplate(_options.HomeAssistant.DeviceNameFormat, new() { { deviceIdKey, deviceId } }),
                Manufacturer = _options.HomeAssistant.DeviceManufacturer,
                Model = _options.HomeAssistant.DeviceModel,
                SwVersion = _options.HomeAssistant.DeviceSoftwareVersion
            }
        };
    }

    /// <summary>
    /// Retrieves metadata describing the component type, unit of measurement, device class, and state class associated
    /// with a specified parameter name from the parameter registry.
    /// </summary>
    /// <remarks>The method looks up the parameter in the registry by NameEnglish. For compound parameters,
    /// metadata is retrieved from the compound parameter definition. If a regular parameter is not found or is 
    /// missing required metadata fields, an error is logged indicating the misconfiguration.</remarks>
    /// <param name="parameterName">The name of the parameter (NameEnglish) for which to obtain metadata.</param>
    /// <returns>A tuple containing the component type, unit of measurement, device class, and state class for the specified
    /// parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified parameter is not found in the registry or if required
    /// metadata is missing.</exception>
    private (string component, string? unitOfMeasurement, string? deviceClass, string? stateClass) GetParameterMetadata(string parameterName)
    {
        // Check if this is a compound parameter (synthetic, not in registry)
        if (IsCompoundParameter(parameterName))
        {
            var compound = RoconMqtt.Mqtt.Compound.CompoundParameterFactory.Create(parameterName, _options.TimeZoneId);
            LogUsingCompoundParameterMetadata(_logger, parameterName);
            return (compound.HomeAssistantComponent, compound.UnitOfMeasurement, compound.DeviceClass, compound.StateClass);
        }

        // Find the parameter by NameEnglish in the registry
        var paramDef = _parameterRegistry.Parameters.Values.FirstOrDefault(p => p.NameEnglish == parameterName);

        if (paramDef == null)
        {
            LogParameterNotFoundInRegistry(_logger, parameterName);
            throw new ArgumentException($"Parameter '{parameterName}' not found in registry. Please ensure it is properly configured in can-registry.json.", nameof(parameterName));
        }

        if (paramDef.HomeAssistantComponent == null)
        {
            LogMissingHomeAssistantMetadata(_logger, parameterName, "HomeAssistantComponent");
            throw new ArgumentException($"Parameter '{parameterName}' is missing HomeAssistantComponent in registry configuration.", nameof(parameterName));
        }

        return (paramDef.HomeAssistantComponent, paramDef.UnitOfMeasurement, paramDef.DeviceClass, paramDef.StateClass);
    }

    private string GetHomeAssistantDiscoveryTopic(string deviceName, string parameterName)
    {
        var deviceId = _deviceIdentifiers[deviceName];
        var (component, _, _, _) = GetParameterMetadata(parameterName);
        return $"{_options.HomeAssistant.DiscoveryPrefix}/{component}/{FormatTemplate(_options.HomeAssistant.ObjectIdentifierFormat, new() { { deviceIdKey, deviceId.ToLowerInvariant() }, { objectIdKey, ToSnakeCase(parameterName) } })}/config";
    }

    private string GetStateTopic(string deviceName, string parameterName)
    {
        var deviceId = _deviceIdentifiers[deviceName];
        return $"{_options.Topic}/{deviceId.ToLowerInvariant()}/{ToSnakeCase(parameterName)}/state";
    }

    /// <summary>
    /// Determines whether the specified parameter name is a compound parameter.
    /// </summary>
    private static bool IsCompoundParameter(string parameterName)
    {
        return RoconMqtt.Mqtt.Compound.CompoundParameterFactory.AvailableCompoundParameters.Contains(parameterName);
    }

    /// <summary>
    /// Formats a template string by replacing named placeholders with values from the provided dictionary.
    /// </summary>
    /// <param name="template">The template string with placeholders in the format {name}.</param>
    /// <param name="values">A dictionary containing the placeholder names and their corresponding values.</param>
    /// <returns>The formatted string with placeholders replaced by their values.</returns>
    private static string FormatTemplate(string template, Dictionary<string, object> values)
    {
        var result = template;
        foreach (var (key, value) in values)
        {
            result = result.Replace($"{{{key}}}", value?.ToString() ?? "");
        }
        return result;
    }

    /// <summary>
    /// Converts a PascalCase or camelCase string to snake_case.
    /// </summary>
    /// <remarks>This method is used to format parameter names for Home Assistant entity IDs and MQTT topics,
    /// converting names like "BufferTemperatureActual" to "buffer_temperature_actual" and "RoomTargetTemperature1"
    /// to "room_target_temperature_1".</remarks>
    /// <param name="input">The input string to convert. Must not be null or empty.</param>
    /// <returns>A string in snake_case format.</returns>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return string.Concat(input.Select((c, i) =>
        {
            if (i == 0)
                return char.ToLowerInvariant(c).ToString();
            
            var prevChar = input[i - 1];
            
            // Add underscore before uppercase letter if previous is not uppercase
            if (char.IsUpper(c) && !char.IsUpper(prevChar))
                return $"_{char.ToLowerInvariant(c)}";
            
            // Add underscore before digit if previous is not a digit
            if (char.IsDigit(c) && !char.IsDigit(prevChar))
                return $"_{c}";
            
            return char.ToLowerInvariant(c).ToString();
        }));
    }

    /// <summary>
    /// Converts a parameter name from PascalCase to a human-readable display name with spaces.
    /// </summary>
    /// <remarks>This method creates friendly display names for Home Assistant by inserting spaces before
    /// capital letters and digits, preserving the original casing. For example, "BufferTemperatureActual" becomes
    /// "Buffer Temperature Actual" and "RoomTargetTemperature1" becomes "Room Target Temperature 1".</remarks>
    /// <param name="parameterName">The parameter name to convert. Must not be null or empty.</param>
    /// <returns>A string containing the parameter name with spaces inserted before capital letters and digits.</returns>
    private static string FormatParameterDisplayName(string parameterName)
    {
        // Convert PascalCase to "Spaced Title Case" for display names
        // e.g., "BufferTemperatureActual" -> "Buffer Temperature Actual"
        // e.g., "RoomTargetTemperature1" -> "Room Target Temperature 1"
        return string.Concat(parameterName.Select((c, i) =>
        {
            if (i == 0)
                return c.ToString();
            
            var prevChar = parameterName[i - 1];
            
            // Add space before uppercase letter if previous is not uppercase
            if (char.IsUpper(c) && !char.IsUpper(prevChar))
                return $" {c}";
            
            // Add space before digit if previous is not a digit
            if (char.IsDigit(c) && !char.IsDigit(prevChar))
                return $" {c}";
            
            return c.ToString();
        }));
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
            
            // Translate values based on parameter type and Home Assistant component
            var publishValue = TranslateParameterValue(parameterName, result.Value);
            
            // Create a payload without the Definition for Home Assistant
            var statePayload = new MqttStatePayload(result.Name, publishValue);
            var json = JsonSerializer.Serialize(statePayload, _unIndentedJsonSerializerOptions);
            
            LogPublishingToMqtt(_logger, stateTopic, json);
            await _mqtt.PublishAsync(stateTopic, json, token, retain: true);
            
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

        // For complex objects, compare their JSON serializations to avoid reference equality issues
        var newJson = JsonSerializer.Serialize(newValue, _unIndentedJsonSerializerOptions);
        var lastJson = JsonSerializer.Serialize(lastValue, _unIndentedJsonSerializerOptions);
        return !newJson.Equals(lastJson);
    }


    /// <summary>
    /// Translates a parameter value based on its type definition and Home Assistant component.
    /// </summary>
    /// <remarks>
    /// This method provides type-specific value translations using a double switch/case approach:
    /// 1. First checks the Home Assistant component type (binary_sensor requires special handling)
    /// 2. Then checks the parameter type (Enum, Bool, etc.)
    /// 
    /// Currently supports:
    /// - binary_sensor component: Translates to "OFF"/"ON" using enumValues if defined
    /// - Enum types: Translates integer values to their string representations from enumValues
    /// - Bool types: Translates 0/1 to "OFF"/"ON"
    /// 
    /// Future translations can be added by extending the switch statements.
    /// </remarks>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The raw value to translate.</param>
    /// <returns>The translated value if a translation is defined, otherwise the original value.</returns>
    private object? TranslateParameterValue(string parameterName, object? value)
    {
        if (value == null)
            return null;

        // Skip compound parameters (they handle their own formatting)
        if (IsCompoundParameter(parameterName))
            return value;

        // Find the parameter definition
        var paramDefinition = _parameterRegistry.Parameters.Values
            .FirstOrDefault(p => p.Name == parameterName);

        if (paramDefinition == null)
            return value;

        // First switch: Check Home Assistant component type
        // binary_sensor needs special handling regardless of underlying CAN type
        switch (paramDefinition.HomeAssistantComponent)
        {
            case "binary_sensor":
                // Binary sensors are always translated to OFF/ON
                return TranslateBinaryValue(value, paramDefinition);
            
            default:
                // Second switch: Check parameter type for non-binary-sensor components
                return paramDefinition.Type switch
                {
                    ParameterType.Enum => TranslateEnumValue(value, paramDefinition),
                    ParameterType.Bool => TranslateBinaryValue(value, paramDefinition),
                    // Add future translation types here:
                    // ParameterType.TimeRange => TranslateTimeRangeValue(value, paramDefinition),
                    // ParameterType.Custom => TranslateCustomValue(value, paramDefinition),
                    _ => value // No translation for other types
                };
        }
    }

    /// <summary>
    /// Translates an enum value to its string representation.
    /// </summary>
    private static object? TranslateEnumValue(object value, ParameterDefinition paramDefinition)
    {
        if (paramDefinition.EnumValues == null || paramDefinition.EnumValues.Count == 0)
            return value;

        // Convert value to int
        int enumKey = value switch
        {
            int intValue => intValue,
            double doubleValue => (int)doubleValue,
            _ => throw new InvalidOperationException($"Cannot convert value of type {value.GetType()} to enum key")
        };

        // Lookup and return enum string, or original value if not found
        return paramDefinition.EnumValues.TryGetValue(enumKey, out var enumString) 
            ? enumString 
            : value;
    }

    /// <summary>
    /// Translates a binary value (boolean/binary_sensor) to its string representation.
    /// </summary>
    /// <remarks>
    /// Used for both Bool parameter types and Enum types that represent binary_sensor in Home Assistant.
    /// Prefers enumValues if defined, otherwise defaults to "OFF"/"ON".
    /// </remarks>
    private static object? TranslateBinaryValue(object value, ParameterDefinition paramDefinition)
    {
        // If enum values are defined, use them (e.g., custom labels like "Inactive"/"Active")
        if (paramDefinition.EnumValues != null && paramDefinition.EnumValues.Count > 0)
            return TranslateEnumValue(value, paramDefinition);

        // Default binary translation: 0 = "OFF", 1 = "ON"
        int binaryValue = value switch
        {
            int intValue => intValue,
            double doubleValue => (int)doubleValue,
            bool boolValue => boolValue ? 1 : 0,
            _ => throw new InvalidOperationException($"Cannot convert value of type {value.GetType()} to binary")
        };

        return binaryValue == 0 ? "OFF" : "ON";
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

    [LoggerMessage(EventId = 4017, Level = LogLevel.Error, Message = "Device identifier not found for {DeviceName}, device will be excluded from operations")]
    private static partial void LogDeviceIdentifierNotFound(ILogger logger, string deviceName);

    [LoggerMessage(EventId = 4018, Level = LogLevel.Error, Message = "Error querying device identifier for {DeviceName}, device will be excluded from operations")]
    private static partial void LogErrorQueryingDeviceIdentifier(ILogger logger, Exception exception, string deviceName);

    [LoggerMessage(EventId = 4019, Level = LogLevel.Error, Message = "Device identifier for {DeviceName} is invalid (null or empty), device will be excluded from operations")]
    private static partial void LogDeviceIdentifierInvalid(ILogger logger, string deviceName);

    [LoggerMessage(EventId = 4020, Level = LogLevel.Error, Message = "No valid device identifiers could be retrieved. Publisher will not query any devices.")]
    private static partial void LogNoValidDeviceIdentifiers(ILogger logger);

    [LoggerMessage(EventId = 4021, Level = LogLevel.Information, Message = "Successfully retrieved {ValidCount} of {TotalCount} device identifiers")]
    private static partial void LogDeviceIdentifiersInitialized(ILogger logger, int validCount, int totalCount);

    [LoggerMessage(EventId = 4022, Level = LogLevel.Error, Message = "Parameter '{ParameterName}' not found in registry. Ensure it is configured in can-registry.json")]
    private static partial void LogParameterNotFoundInRegistry(ILogger logger, string parameterName);

    [LoggerMessage(EventId = 4023, Level = LogLevel.Error, Message = "Parameter '{ParameterName}' is missing Home Assistant metadata field '{FieldName}' in registry configuration")]
    private static partial void LogMissingHomeAssistantMetadata(ILogger logger, string parameterName, string fieldName);

    [LoggerMessage(EventId = 4024, Level = LogLevel.Warning, Message = "Skipping Home Assistant discovery for parameter '{ParameterName}': {Reason}")]
    private static partial void LogSkippingDiscoveryForParameter(ILogger logger, string parameterName, string reason);

    [LoggerMessage(EventId = 4025, Level = LogLevel.Debug, Message = "Using synthetic metadata for compound parameter '{ParameterName}'")]
    private static partial void LogUsingCompoundParameterMetadata(ILogger logger, string parameterName);

    [LoggerMessage(EventId = 4026, Level = LogLevel.Warning, Message = "Skipping device {DeviceName} - no valid device identifier available")]
    private static partial void LogSkippingDeviceWithoutIdentifier(ILogger logger, string deviceName);

    [LoggerMessage(EventId = 4027, Level = LogLevel.Warning, Message = "Skipping Home Assistant discovery for device {DeviceName} - no valid device identifier available")]
    private static partial void LogSkippingDiscoveryForDevice(ILogger logger, string deviceName);

    #endregion
}

