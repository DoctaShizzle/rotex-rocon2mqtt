using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using RoconMqtt.Can;
using RoconMqtt.Can.Options;
using RoconMqtt.Mqtt;
using RoconMqtt.Mqtt.Options;
using Serilog;

namespace RoconMqtt;

public static class Program
{
    private static async Task Main(string[] args)
    {
        // Check if user wants to run the CAN test
        if (args.Length > 0 && args[0].Equals("test-can", StringComparison.OrdinalIgnoreCase))
        {
            await TestCanCommunication.RunAsync(args);
            return;
        }

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
            /*

            //test

            Dictionary<DeviceType, List<string>> deviceParameters = new Dictionary<DeviceType, List<string>>();

            foreach (var parameter in CanParameterRegistry.Parameters.Values)
            {
                var subsystem = CanParameterRegistry.GetSubsystemForParameter(parameter!.InfoNumber);
                if (subsystem.HasValue)
                {
                    if (!deviceParameters.ContainsKey(subsystem.Value))
                    {
                        deviceParameters[subsystem.Value] = new List<string>();
                    }
                    deviceParameters[subsystem.Value].Add(parameter.Name);
                }
            }
            */


            /*

            // Find the parameter definition by name
            var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == "cGERAETE_KENNUNG");
            //var paramDef = CanParameterRegistry.Parameters.Values.FirstOrDefault(p => p.Name == "cAUSSENTEMP");

            // Get appropriate communication profile based on parameter subsystem
            var subsystem = CanParameterRegistry.GetSubsystemForParameter(paramDef!.InfoNumber);
            var testDevice = subsystem switch
            {
                DeviceType.HeatGenerator => CanParameterRegistry.HeatGenerators[0],
                DeviceType.HeatingCircuit => CanParameterRegistry.HeatingCircuits[0],
                DeviceType.HeatingCircuitModule => CanParameterRegistry.HeatingCircuitModules[0],
                _ => CanParameterRegistry.HeatGenerators[0]
            };

            // Encode the value using Answer command format
            var outgoingFrameData = new CanEncoder(new NullLogger<CanEncoder>()).Encode(testDevice.Profile.Get, paramDef.InfoNumber, 0);

            var input = new byte[] { 0xD2, 0x1D, 0xFA, 0x01, 0x48, 0x00, 0x00 };
            var decoded = new CanDecoder(new NullLogger<CanDecoder>()).Decode(input, testDevice.Profile.Answer);
            var output = new CanEncoder(new NullLogger<CanEncoder>()).Encode(testDevice.Profile.Answer, decoded.Definition.InfoNumber, decoded.Value);
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