using FastEndpoints;
using FastEndpoints.Swagger;
using RoconMqtt.Can.Extensions.DependencyInjection;
using RoconMqtt.Mqtt.Extensions.DependencyInjection;
using Serilog;

namespace RoconMqtt;

public static class Program
{
    private static async Task Main(string[] args)
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
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Host.UseSerilog();

            // Configure CAN service
            builder.Services.AddCanService(builder.Configuration)
                //.WithSocketCanBus();
                .WithSshCanBus();
                //.WithAutoCanBus();

            // Configure MQTT services
            builder.Services.AddMqttService(builder.Configuration);

            // Configure FastEndpoints
            builder.Services.AddFastEndpoints();

            // Configure Swagger/OpenAPI
            builder.Services.SwaggerDocument(o =>
            {
                o.DocumentSettings = s =>
                {
                    s.Title = "Rocon MQTT API";
                    s.Version = "v1";
                    s.Description = "REST API for Rotex Rocon G1 heating controller communication over CAN bus";
                };
                
                o.RemoveEmptyRequestSchema = true;
            });

            var app = builder.Build();

            // Configure middleware
            app.UseSerilogRequestLogging();

            app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
            });

            // Enable Swagger UI
            app.UseSwaggerGen();

            // Redirect root to Swagger UI (after UseSwaggerGen so it's not included in OpenAPI)
            app.MapGet("/", () => Results.Redirect("/swagger", permanent: false))
                .ExcludeFromDescription();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}