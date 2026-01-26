using System.Text.Json;

namespace RoconMqtt.Tests.Helpers;

/// <summary>
/// Helper class for testing JSON serialization with source-generated JsonSerializerContext.
/// This is especially important for ARM/Linux deployments where reflection-based serialization is not available.
/// </summary>
public static class JsonSerializationTestHelper
{
    /// <summary>
    /// Gets JsonSerializerOptions configured to use ApiJsonContext (source-generated serialization).
    /// This mirrors the configuration used in production.
    /// </summary>
    public static JsonSerializerOptions GetApiJsonOptions()
    {
        return new JsonSerializerOptions
        {
            TypeInfoResolverChain = { ApiJsonContext.Default }
        };
    }

    /// <summary>
    /// Tests that a type can be serialized and deserialized without reflection.
    /// Returns the deserialized object for further assertions.
    /// </summary>
    /// <typeparam name="T">The type to test</typeparam>
    /// <param name="value">The value to serialize</param>
    /// <returns>The deserialized value</returns>
    public static T AssertCanRoundTrip<T>(T value)
    {
        var options = GetApiJsonOptions();
        var json = JsonSerializer.Serialize(value, options);
        var deserialized = JsonSerializer.Deserialize<T>(json, options);
        
        if (deserialized is null)
        {
            throw new InvalidOperationException($"Deserialization of {typeof(T).Name} returned null");
        }

        return deserialized;
    }

    /// <summary>
    /// Verifies that the specified type is NOT registered in ApiJsonContext.
    /// This should throw NotSupportedException when trying to serialize.
    /// </summary>
    /// <typeparam name="T">The type that should not be serializable</typeparam>
    /// <param name="value">A sample value of that type</param>
    /// <returns>True if serialization fails as expected</returns>
    public static bool AssertTypeNotRegistered<T>(T value)
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = ApiJsonContext.Default
        };

        try
        {
            JsonSerializer.Serialize(value, options);
            return false; // Serialization succeeded when it should have failed
        }
        catch (NotSupportedException)
        {
            return true; // Expected: type is not in ApiJsonContext
        }
    }

    /// <summary>
    /// Extracts a double value from an object that might be a JsonElement.
    /// This is useful when testing properties of type 'object' that contain numeric values.
    /// </summary>
    public static double GetDoubleValue(object? value)
    {
        return value switch
        {
            double d => d,
            int i => i,
            System.Text.Json.JsonElement je => je.GetDouble(),
            _ => throw new ArgumentException($"Cannot convert {value?.GetType().Name ?? "null"} to double")
        };
    }

    /// <summary>
    /// Extracts a string value from an object that might be a JsonElement.
    /// </summary>
    public static string? GetStringValue(object? value)
    {
        return value switch
        {
            string s => s,
            System.Text.Json.JsonElement je => je.GetString(),
            null => null,
            _ => value.ToString()
        };
    }

    /// <summary>
    /// Extracts an int value from an object that might be a JsonElement.
    /// </summary>
    public static int GetInt32Value(object? value)
    {
        return value switch
        {
            int i => i,
            System.Text.Json.JsonElement je => je.GetInt32(),
            _ => throw new ArgumentException($"Cannot convert {value?.GetType().Name ?? "null"} to int")
        };
    }

    /// <summary>
    /// Extracts a bool value from an object that might be a JsonElement.
    /// </summary>
    public static bool GetBooleanValue(object? value)
    {
        return value switch
        {
            bool b => b,
            System.Text.Json.JsonElement je => je.GetBoolean(),
            _ => throw new ArgumentException($"Cannot convert {value?.GetType().Name ?? "null"} to bool")
        };
    }
}
