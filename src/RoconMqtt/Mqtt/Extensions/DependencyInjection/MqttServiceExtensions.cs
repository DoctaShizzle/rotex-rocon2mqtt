using Microsoft.Extensions.Options;
using RoconMqtt.Can.Configuration;
using RoconMqtt.Mqtt.Options;
using RoconMqtt.Mqtt.Resilience;

namespace RoconMqtt.Mqtt.Extensions.DependencyInjection;

public static class MqttServiceExtensions
{
    /// <summary>
    /// Adds MQTT services and configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMqttService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<MqttOptions>()
            .Bind(configuration.GetSection("Mqtt"))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure<IOptions<CanRegistryOptions>>((mqttOptions, registryOptions) =>
            {
                // If MQTT options don't specify devices/parameters, use the ones from the CAN registry
                var registry = registryOptions.Value;
                
                if (mqttOptions.Devices.Count == 0 && registry.Devices.Count > 0)
                {
                    mqttOptions.Devices = registry.Devices;
                }
                
                if (mqttOptions.Parameters.Count == 0 && registry.MqttParameters.Count > 0)
                {
                    mqttOptions.Parameters = registry.MqttParameters;
                }
                
                if (mqttOptions.CompoundParameters.Count == 0 && registry.MqttCompoundParameters.Count > 0)
                {
                    mqttOptions.CompoundParameters = registry.MqttCompoundParameters;
                }
            });

        services.AddOptions<ResilienceOptions>()
            .Bind(configuration.GetSection("Resilience"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ResiliencePipelineFactory>();
        services.AddSingleton<IMqttService, MqttService>();
        services.AddSingleton<RoconMqttPublisher>();
        services.AddHostedService<RoconMqttPublisher>();

        return services;
    }
}
