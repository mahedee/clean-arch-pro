using Microsoft.AspNetCore.Mvc;

namespace EduTrack.Api.Models;

/// <summary>
/// RFC 7807 Problem Details implementation for standardized error responses
/// </summary>
public class ProblemDetailsResponse : ProblemDetails
{
    /// <summary>
    /// Unique identifier for tracking this specific error occurrence
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Correlation ID for request tracking across services
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validation errors (if applicable)
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Additional context information about the error
    /// </summary>
    public Dictionary<string, object>? Context { get; set; }
}