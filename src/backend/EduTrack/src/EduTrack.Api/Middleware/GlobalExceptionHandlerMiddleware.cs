using EduTrack.Api.Models;
using EduTrack.Application.Common.Exceptions;
using EduTrack.Domain.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace EduTrack.Api.Middleware;

/// <summary>
/// Enhanced global exception handling middleware with RFC 7807 Problem Details support,
/// correlation tracking, and structured logging
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log the exception with structured information
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = traceId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method,
            ["UserId"] = context.User?.Identity?.Name ?? "Anonymous"
        }))
        {
            _logger.LogError(exception, 
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, TraceId: {TraceId}",
                correlationId, traceId);
        }

        // Create problem details response
        var problemDetails = CreateProblemDetails(context, exception, correlationId, traceId);

        // Set response properties
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        // Configure JSON options for consistent formatting
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var jsonResponse = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }

    private ProblemDetailsResponse CreateProblemDetails(
        HttpContext context, 
        Exception exception, 
        string correlationId, 
        string traceId)
    {
        var problemDetails = new ProblemDetailsResponse
        {
            CorrelationId = correlationId,
            TraceId = traceId,
            Instance = context.Request.Path,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case EduTrack.Application.Common.Exceptions.ValidationException validationEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Title = "Validation Error";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = "One or more validation errors occurred";
                problemDetails.ErrorCode = "VALIDATION_ERROR";
                problemDetails.Errors = validationEx.Errors;
                break;

            case FluentValidation.ValidationException fluentValidationEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Title = "Validation Error";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = "One or more validation errors occurred";
                problemDetails.ErrorCode = "VALIDATION_ERROR";
                problemDetails.Errors = fluentValidationEx.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
                break;

            case NotFoundException notFoundEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                problemDetails.Title = "Resource Not Found";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Detail = notFoundEx.Message;
                problemDetails.ErrorCode = notFoundEx.ErrorCode;
                break;

            case EntityNotFoundException entityNotFoundEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                problemDetails.Title = "Entity Not Found";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Detail = entityNotFoundEx.Message;
                problemDetails.ErrorCode = entityNotFoundEx.ErrorCode;
                break;

            case DuplicateEntityException duplicateEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                problemDetails.Title = "Duplicate Resource";
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Detail = duplicateEx.Message;
                problemDetails.ErrorCode = duplicateEx.ErrorCode;
                break;

            case ForbiddenAccessException forbiddenEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                problemDetails.Title = "Forbidden Access";
                problemDetails.Status = (int)HttpStatusCode.Forbidden;
                problemDetails.Detail = forbiddenEx.Message;
                problemDetails.ErrorCode = forbiddenEx.ErrorCode;
                break;

            case BusinessRuleViolationException businessEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Title = "Business Rule Violation";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = businessEx.Message;
                problemDetails.ErrorCode = businessEx.ErrorCode;
                break;

            case DomainInvariantException domainEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Title = "Domain Rule Violation";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = domainEx.Message;
                problemDetails.ErrorCode = domainEx.ErrorCode;
                break;

            case ExternalServiceException serviceEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.3";
                problemDetails.Title = "External Service Error";
                problemDetails.Status = (int)HttpStatusCode.BadGateway;
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? serviceEx.Message 
                    : "An external service is currently unavailable";
                problemDetails.ErrorCode = serviceEx.ErrorCode;
                problemDetails.Context = new Dictionary<string, object>
                {
                    ["ServiceName"] = serviceEx.ServiceName
                };
                break;

            case ArgumentException argEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Title = "Invalid Argument";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = argEx.Message;
                problemDetails.ErrorCode = "INVALID_ARGUMENT";
                break;

            case UnauthorizedAccessException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                problemDetails.Title = "Unauthorized";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Detail = "Authentication is required to access this resource";
                problemDetails.ErrorCode = "UNAUTHORIZED";
                break;

            case TimeoutException timeoutEx:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                problemDetails.Title = "Request Timeout";
                problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
                problemDetails.Detail = "The request timed out";
                problemDetails.ErrorCode = "REQUEST_TIMEOUT";
                break;

            default:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred";
                problemDetails.ErrorCode = "INTERNAL_SERVER_ERROR";
                
                // Add stack trace in development environment
                if (_environment.IsDevelopment())
                {
                    problemDetails.Context = new Dictionary<string, object>
                    {
                        ["StackTrace"] = exception.StackTrace ?? string.Empty,
                        ["ExceptionType"] = exception.GetType().Name
                    };
                }
                break;
        }

        return problemDetails;
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        const string correlationIdHeaderName = "X-Correlation-ID";
        
        if (context.Request.Headers.TryGetValue(correlationIdHeaderName, out var correlationId))
        {
            return correlationId.FirstOrDefault() ?? Guid.NewGuid().ToString();
        }

        var newCorrelationId = Guid.NewGuid().ToString();
        context.Response.Headers.Add(correlationIdHeaderName, newCorrelationId);
        
        return newCorrelationId;
    }
}
