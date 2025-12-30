using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RoconMqtt.Mqtt.Options;

namespace RoconMqtt.Mqtt.Resilience;

/// <summary>
/// Factory for creating Polly resilience pipelines for CAN parameter queries.
/// </summary>
public partial class ResiliencePipelineFactory(IOptions<ResilienceOptions> options, ILogger<ResiliencePipelineFactory> logger)
{
    private readonly ResilienceOptions _options = options.Value;
    private readonly ILogger<ResiliencePipelineFactory> _logger = logger;

    /// <summary>
    /// Creates a resilience pipeline for querying CAN parameters.
    /// Includes retry with exponential backoff and circuit breaker.
    /// </summary>
    public ResiliencePipeline CreateQueryPipeline()
    {
        if (!_options.Enabled)
        {
            LogResiliencePoliciesDisabled(_logger);
            return ResiliencePipeline.Empty;
        }

        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
                MaxRetryAttempts = _options.RetryCount,
                Delay = TimeSpan.FromMilliseconds(_options.BaseRetryDelayMs),
                BackoffType = DelayBackoffType.Linear,
                OnRetry = args =>
                {
                    LogRetryingAfterTimeout(_logger, args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
                FailureRatio = 1.0, // Trip after consecutive failures
                MinimumThroughput = _options.CircuitBreakerFailureThreshold,
                BreakDuration = TimeSpan.FromSeconds(_options.CircuitBreakerDurationSeconds),
                OnOpened = args =>
                {
                    LogCircuitBreakerOpened(_logger, args.BreakDuration.TotalSeconds);
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    LogCircuitBreakerClosed(_logger);
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    LogCircuitBreakerHalfOpen(_logger);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    [LoggerMessage(EventId = 5001, Level = LogLevel.Information, Message = "Resilience policies are disabled")]
    private static partial void LogResiliencePoliciesDisabled(ILogger logger);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Warning, Message = "Retrying after timeout (attempt {AttemptNumber}, delay {DelayMs}ms)")]
    private static partial void LogRetryingAfterTimeout(ILogger logger, int attemptNumber, double delayMs);

    [LoggerMessage(EventId = 5003, Level = LogLevel.Error, Message = "Circuit breaker opened, will retry after {BreakDurationSeconds}s")]
    private static partial void LogCircuitBreakerOpened(ILogger logger, double breakDurationSeconds);

    [LoggerMessage(EventId = 5004, Level = LogLevel.Information, Message = "Circuit breaker closed, resuming normal operation")]
    private static partial void LogCircuitBreakerClosed(ILogger logger);

    [LoggerMessage(EventId = 5005, Level = LogLevel.Warning, Message = "Circuit breaker half-open, testing if service has recovered")]
    private static partial void LogCircuitBreakerHalfOpen(ILogger logger);
}
