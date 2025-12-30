# Resilience Policies (Polly)

The RoconMqttPublisher uses **Polly** resilience policies to handle transient failures gracefully when querying CAN parameters.

## Features

### 1. Retry with Linear Backoff
- Automatically retries failed queries due to timeouts
- Configurable number of retry attempts
- Linear delay between retries (BaseRetryDelayMs × attempt number)

### 2. Circuit Breaker
- Prevents overwhelming devices that are offline or unresponsive
- Opens circuit after a threshold of consecutive failures
- Stays open for a configurable duration before testing recovery
- Three states:
  - **Closed**: Normal operation, requests flow through
  - **Open**: Circuit tripped, requests fail fast without hitting the device
  - **Half-Open**: Testing if service has recovered

## Configuration

Add the `Resilience` section to `appsettings.json`:

```json
{
  "Resilience": {
    "Enabled": true,
    "RetryCount": 2,
    "BaseRetryDelayMs": 500,
    "CircuitBreakerFailureThreshold": 3,
    "CircuitBreakerDurationSeconds": 60
  }
}
```

### Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `Enabled` | `true` | Enable/disable all resilience policies |
| `RetryCount` | `2` | Number of retry attempts for timeouts |
| `BaseRetryDelayMs` | `500` | Base delay in ms (actual delay = base × attempt) |
| `CircuitBreakerFailureThreshold` | `3` | Consecutive failures before opening circuit |
| `CircuitBreakerDurationSeconds` | `60` | Time circuit stays open (in seconds) |

## Example Scenarios

### Scenario 1: Transient Timeout
```
Attempt 1: Timeout ? Wait 500ms
Attempt 2: Timeout ? Wait 1000ms
Attempt 3: Success ?
```

### Scenario 2: Device Offline
```
Query 1: Timeout (retry exhausted) ? Failure count: 1
Query 2: Timeout (retry exhausted) ? Failure count: 2
Query 3: Timeout (retry exhausted) ? Failure count: 3
Circuit Opens ? Future queries fail fast for 60 seconds
After 60s ? Circuit Half-Opens ? Test query
If successful ? Circuit Closes
If failed ? Circuit reopens for another 60s
```

## Logging

Resilience events are logged with specific event IDs:

| Event ID | Level | Description |
|----------|-------|-------------|
| 5001 | Information | Resilience policies disabled |
| 5002 | Warning | Retrying after timeout |
| 5003 | Error | Circuit breaker opened |
| 5004 | Information | Circuit breaker closed |
| 5005 | Warning | Circuit breaker half-open (testing) |
| 4010 | Warning | Query skipped due to open circuit |

## Benefits

? **Improved Reliability**: Automatic recovery from transient failures  
? **Resource Protection**: Prevents overwhelming unresponsive devices  
? **Better Observability**: Detailed logging of failure patterns  
? **Graceful Degradation**: Fails fast when device is known to be down  
? **Self-Healing**: Automatically tests recovery and resumes operation

## Disabling Resilience

To disable all resilience policies (e.g., for debugging):

```json
{
  "Resilience": {
    "Enabled": false
  }
}
```

When disabled, queries will fail immediately on first timeout without retries.
