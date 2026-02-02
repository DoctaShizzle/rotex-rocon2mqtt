namespace RoconMqtt.Api.Models;

/// <summary>
/// Standard error response for API endpoints
/// </summary>
public sealed class ApiErrorResponse
{
    /// <summary>
    /// Error message describing what went wrong
    /// </summary>
    public required string Error { get; init; }
}
