namespace RoconMqtt.Mqtt.Options;

/// <summary>
/// Configuration options for resilience policies (Polly).
/// </summary>
public class ResilienceOptions
{
    /// <summary>
    /// Number of retry attempts for transient failures (e.g., timeouts).
    /// Default is 3.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Base delay in milliseconds for exponential backoff between retries.
    /// Actual delay = BaseRetryDelayMs * attempt.
    /// Default is 500ms.
    /// </summary>
    public int BaseRetryDelayMs { get; set; } = 500;

    /// <summary>
    /// Number of consecutive failures allowed before opening the circuit breaker.
    /// Default is 3.
    /// </summary>
    public int CircuitBreakerFailureThreshold { get; set; } = 3;

    /// <summary>
    /// Duration in seconds to keep the circuit breaker open after it trips.
    /// Default is 60 seconds (1 minute).
    /// </summary>
    public int CircuitBreakerDurationSeconds { get; set; } = 60;

    /// <summary>
    /// Enable or disable resilience policies.
    /// Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
