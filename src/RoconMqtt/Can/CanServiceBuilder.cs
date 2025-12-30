using Microsoft.Extensions.DependencyInjection;

namespace RoconMqtt.Can;

public class CanServiceBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}
