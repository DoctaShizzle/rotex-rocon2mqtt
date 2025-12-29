using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;
using System.Collections.Concurrent;

namespace RoconMqtt;

/// <summary>
/// Simple test program to query CAN parameters without MQTT.
/// Run with: dotnet run -- test-can
/// </summary>
public class TestCanCommunication
{
    public static async Task RunAsync(string[] args)
    {
        Console.WriteLine("=== ROCON CAN Communication Test ===\n");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Setup DI container
        var services = new ServiceCollection();
        
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Configuration
        services.Configure<CanOptions>(configuration.GetSection("Can"));
        
        // CAN Services
        services.AddSingleton<ICanReader, SocketCanReader>();
        services.AddSingleton<ICanDecoder, CanDecoder>();
        services.AddSingleton<ICanEncoder, CanEncoder>();
        services.AddSingleton<IRoconService, RoconService>();

        var serviceProvider = services.BuildServiceProvider();

        // Get configuration values
        var devices = configuration.GetSection("Mqtt:Devices").Get<List<string>>() ?? new List<string>();
        var parameters = configuration.GetSection("Mqtt:Parameters").Get<List<string>>() ?? new List<string>();
        var responseTimeoutMs = configuration.GetValue<int>("Mqtt:ResponseTimeoutMs", 1000);

        Console.WriteLine($"Devices: {string.Join(", ", devices)}");
        Console.WriteLine($"Parameters: {parameters.Count} configured");
        Console.WriteLine($"Response timeout: {responseTimeoutMs}ms\n");

        // Get services
        var roconService = serviceProvider.GetRequiredService<IRoconService>();
        var logger = serviceProvider.GetRequiredService<ILogger<TestCanCommunication>>();

        // Storage for responses
        var responses = new ConcurrentDictionary<string, DecodedParameter>();
        var responsesReceived = 0;
        var totalRequests = devices.Count * parameters.Count;

        using var cts = new CancellationTokenSource();

        // Start listening for responses in background
        var listenTask = Task.Run(async () =>
        {
            try
            {
                await roconService.ListenForResponses(async parameter =>
                {
                    var key = parameter.Name;
                    responses[key] = parameter;
                    Interlocked.Increment(ref responsesReceived);
                    
                    Console.WriteLine($"[Response {responsesReceived}/{totalRequests}] {parameter.Name} = {parameter.Value}");
                    await Task.CompletedTask;
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected when we cancel
            }
        }, cts.Token);

        // Give the listener time to start
        await Task.Delay(500);

        Console.WriteLine("\n=== Querying Parameters ===\n");

        // Query each parameter for each device
        foreach (var device in devices)
        {
            Console.WriteLine($"--- Querying device: {device} ---");
            
            foreach (var parameter in parameters)
            {
                try
                {
                    logger.LogInformation("Sending GET request for {Device}/{Parameter}", device, parameter);
                    await roconService.SendRequestAsync(device, parameter, CommandType.Get, null, cts.Token);
                    
                    // Wait for response
                    await Task.Delay(100); // Small delay between requests
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to query {Device}/{Parameter}", device, parameter);
                    Console.WriteLine($"[Error] {device}/{parameter}: {ex.Message}");
                }
            }
        }

        // Wait for remaining responses
        Console.WriteLine($"\nWaiting up to {responseTimeoutMs * 2}ms for remaining responses...");
        await Task.Delay(responseTimeoutMs * 2);

        // Stop listening
        cts.Cancel();
        try
        {
            await listenTask;
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Print summary
        Console.WriteLine("\n=== Results Summary ===\n");
        Console.WriteLine($"Total requests sent: {totalRequests}");
        Console.WriteLine($"Total responses received: {responsesReceived}");
        Console.WriteLine($"Success rate: {(responsesReceived * 100.0 / totalRequests):F1}%\n");

        // Print all results organized by device
        foreach (var device in devices)
        {
            Console.WriteLine($"Device: {device}");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine($"{"Parameter",-30} {"Value",-25} {"Type",-10}");
            Console.WriteLine(new string('-', 80));

            foreach (var parameter in parameters)
            {
                if (responses.TryGetValue(parameter, out var decoded))
                {
                    var valueStr = FormatValue(decoded.Value);
                    Console.WriteLine($"{decoded.Name,-30} {valueStr,-25} {decoded.Definition.Type,-10}");
                }
                else
                {
                    Console.WriteLine($"{parameter,-30} {"<no response>",-25} {"?",-10}");
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n=== Test Complete ===");
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            double d => $"{d:F2}",
            float f => $"{f:F2}",
            bool b => b ? "true" : "false",
            _ => value?.ToString() ?? "<null>"
        };
    }
}
