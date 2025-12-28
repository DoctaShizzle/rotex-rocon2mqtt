using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using RoconMqtt.Can;
using RoconMqtt.Mqtt;
using RoconMqtt.Mqtt.Options;
using Serilog;

namespace RoconMqtt;

public static class Program
{
    private static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .Build()
            )
            .CreateLogger();

        try
        {
            //test

            /*
            var input = new byte[] { 0xD2, 0x1D, 0xFA, 0x0A, 0x06, 0x01, 0xE0 };

            var decoded = new CanDecoder(new NullLogger<CanDecoder>()).Decode(input);
            var output = new CanEncoder().Encode(decoded.Definition.InfoNumber, decoded.Value);

            */

            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<ICanReader, SocketCanReader>();
                    services.AddSingleton<ICanDecoder, CanDecoder>();
                    services.AddSingleton<ICanEncoder, CanEncoder>();
                    services.AddSingleton<IRoconService, RoconService>();
                    services.Configure<MqttOptions>(ctx.Configuration.GetSection("Mqtt"));
                    services.Configure<CanOptions>(ctx.Configuration.GetSection("Can"));
                    services.AddSingleton<IMqttService, MqttService>();
                    services.AddSingleton<RoconMqttPublisher>();
                    services.AddHostedService<RoconMqttPublisher>();
                })
                .Build()
                .Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}