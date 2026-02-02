using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoconMqtt.Can.Configuration;
using RoconMqtt.Can.Options;

namespace RoconMqtt.Can.Extensions.DependencyInjection;

public static class CanServiceExtensions
{
    private const string canSectionKey = "Can";
    private const string registryFilePathKey = "Can:RegistryFilePath";
    private const string defaultRegistryFile = "can-registry.json";

    public static CanServiceBuilder AddCanService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        //check for existance of canSectionKey in configuration
        if (!configuration.GetSection(canSectionKey).Exists())
        {
            throw new InvalidOperationException($"Configuration section '{canSectionKey}' is missing.");
        }

        services.AddOptions<CanOptions>()
            .Bind(configuration.GetSection(canSectionKey))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Configure CAN parameter registry from external file
        var registryFilePath = configuration[registryFilePathKey] ?? defaultRegistryFile;
        
        var registryConfiguration = new ConfigurationBuilder()
            .AddJsonFile(registryFilePath, optional: false, reloadOnChange: true)
            .Build();

        services.AddOptions<CanRegistryOptions>()
            .Bind(registryConfiguration)
            .ValidateOnStart();

        services.AddSingleton<ICanParameterRegistry, CanParameterRegistryService>();
        services.AddSingleton<ICanService, CanService>();
        services.AddSingleton<ICanDecoder, CanDecoder>();
        services.AddSingleton<ICanEncoder, CanEncoder>();

        // Register CAN recording service as singleton and hosted service
        services.AddSingleton<CanRecordingService>();
        services.AddHostedService(sp => sp.GetRequiredService<CanRecordingService>());

        return new CanServiceBuilder(services, configuration);
    }

    /// <summary>
    /// Adds SocketCAN bus implementation for local CAN access (Linux only).
    /// </summary>
    public static CanServiceBuilder WithSocketCanBus(this CanServiceBuilder builder)
    {
        builder.Services.AddSingleton<ICanBus, SocketCanBus>();
        return builder;
    }

    /// <summary>
    /// Adds SSH-based CAN bus implementation for remote CAN access via candump/cansend.
    /// </summary>
    public static CanServiceBuilder WithSshCanBus(this CanServiceBuilder builder)
    {
        builder.Services.AddSingleton<ICanBus, SshCanBus>();
        return builder;
    }

    /// <summary>
    /// Automatically selects CAN bus implementation based on configuration:
    /// - SSH CAN bus if Can:Ssh section is configured
    /// - SocketCAN bus otherwise (default)
    /// </summary>
    public static CanServiceBuilder WithAutoCanBus(this CanServiceBuilder builder)
    {
        builder.Services.AddSingleton<ICanBus>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CanOptions>>().Value;
            if (options.Ssh != null)
            {
                return sp.GetRequiredService<SshCanBus>();
            }
            return sp.GetRequiredService<SocketCanBus>();
        });
        
        // Register both implementations so they can be resolved
        builder.Services.AddSingleton<SocketCanBus>();
        builder.Services.AddSingleton<SshCanBus>();
        
        return builder;
    }
}
