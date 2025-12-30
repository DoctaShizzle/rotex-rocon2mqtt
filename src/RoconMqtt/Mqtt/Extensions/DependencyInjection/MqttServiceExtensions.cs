using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            .ValidateOnStart();

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
