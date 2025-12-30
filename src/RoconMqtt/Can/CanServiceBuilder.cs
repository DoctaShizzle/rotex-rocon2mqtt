using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RoconMqtt.Can;

public class CanServiceBuilder(IServiceCollection services, IConfiguration configuration)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
    public IConfiguration Configuration { get; } = configuration ?? throw new ArgumentNullException(nameof(configuration));
}
