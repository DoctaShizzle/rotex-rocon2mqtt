using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoconMqtt.Can.Options;

namespace RoconMqtt.Can.Extensions.DependencyInjection;

public static class CanServiceExtensions
{
    private const string canSectionKey = "Can";

    public static CanServiceBuilder AddCanService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CanOptions>(configuration.GetSection(canSectionKey));
        services.AddSingleton<ICanService, CanService>();
        services.AddSingleton<ICanDecoder, CanDecoder>();
        services.AddSingleton<ICanEncoder, CanEncoder>();

        return new CanServiceBuilder(services);
    }

    public static CanServiceBuilder WithSocketCanReader(this CanServiceBuilder builder)
    {
        builder.Services.AddSingleton<ICanReader, SocketCanReader>();
        return builder;
    }
}
