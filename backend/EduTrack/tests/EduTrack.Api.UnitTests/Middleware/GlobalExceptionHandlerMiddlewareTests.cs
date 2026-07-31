using EduTrack.Api.Middleware;
using EduTrack.Api.Models;
using EduTrack.Application.Common.Exceptions;
using EduTrack.Domain.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace EduTrack.Api.UnitTests.Middleware;

public class GlobalExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly GlobalExceptionHandlerMiddleware _middleware;

    public GlobalExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new InvalidOperationException("Test exception"),
            _loggerMock.Object,
            _environmentMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionThrown_ShouldReturnBadRequestWithProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => 
            {
                var failures = new[]
                {
                    new FluentValidation.Results.ValidationFailure("Email", "Email is required"),
                    new FluentValidation.Results.ValidationFailure("Name", "Name is required")
                };
                throw new FluentValidation.ValidationException(failures);
            },
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Validation Error", problemDetails.Title);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.Equal("VALIDATION_ERROR", problemDetails.ErrorCode);
        Assert.NotNull(problemDetails.Errors);
        Assert.Contains("Email", problemDetails.Errors.Keys);
        Assert.Contains("Name", problemDetails.Errors.Keys);
    }

    [Fact]
    public async Task InvokeAsync_WhenEntityNotFoundExceptionThrown_ShouldReturnNotFoundWithProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new EntityNotFoundException("Student", 123),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Entity Not Found", problemDetails.Title);
        Assert.Equal("Student with id '123' was not found", problemDetails.Detail);
        Assert.Equal("ENTITY_NOT_FOUND", problemDetails.ErrorCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenBusinessRuleViolationExceptionThrown_ShouldReturnBadRequestWithProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new BusinessRuleViolationException("Student cannot be enrolled in more than 5 courses"),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Business Rule Violation", problemDetails.Title);
        Assert.Equal("Student cannot be enrolled in more than 5 courses", problemDetails.Detail);
        Assert.Equal("BUSINESS_RULE_VIOLATION", problemDetails.ErrorCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledExceptionInDevelopment_ShouldIncludeStackTrace()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _environmentMock.Setup(x => x.IsDevelopment()).Returns(true);
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new InvalidOperationException("Unexpected error"),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Equal("Unexpected error", problemDetails.Detail);
        Assert.NotNull(problemDetails.Context);
        Assert.True(problemDetails.Context.ContainsKey("StackTrace"));
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledExceptionInProduction_ShouldNotIncludeStackTrace()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _environmentMock.Setup(x => x.IsDevelopment()).Returns(false);
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new InvalidOperationException("Unexpected error"),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Equal("An unexpected error occurred", problemDetails.Detail);
        Assert.Null(problemDetails.Context);
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetCorrelationIdHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new ArgumentException("Test argument exception"),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(context.Response.Headers.ContainsKey("X-Correlation-ID"));
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.NotEmpty(problemDetails.CorrelationId);
        Assert.NotEmpty(problemDetails.TraceId);
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogExceptionWithStructuredData()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/students";
        context.Request.Method = "POST";
        
        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new ArgumentException("Test exception"),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception occurred")),
                It.IsAny<ArgumentException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldSanitizeUserControlledScopeValuesBeforeLogging()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/students%0d%0aforged";
        context.Request.Method = "POST\r\nFORGED";
        context.Request.Headers["X-Correlation-ID"] = "corr-123\r\nFORGED";
        context.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "alice\r\nFORGED")],
                authenticationType: "Test"));

        object? capturedScope = null;
        var scopeDisposable = Mock.Of<IDisposable>();

        _loggerMock
            .Setup(x => x.BeginScope(It.IsAny<It.IsAnyType>()))
            .Callback(new InvocationAction(invocation => capturedScope = invocation.Arguments[0]))
            .Returns(scopeDisposable);

        var middleware = new GlobalExceptionHandlerMiddleware(
            async (context) => throw new ArgumentException("Test exception"),
            _loggerMock.Object,
            _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var scopeValues = Assert.IsAssignableFrom<IReadOnlyCollection<KeyValuePair<string, object>>>(capturedScope);
        var scopeDictionary = scopeValues.ToDictionary(pair => pair.Key, pair => pair.Value?.ToString());

        Assert.Equal("corr-123FORGED", scopeDictionary["CorrelationId"]);
        Assert.Equal("/api/students%0d%0aforged", scopeDictionary["RequestPath"]);
        Assert.Equal("POSTFORGED", scopeDictionary["RequestMethod"]);
        Assert.Equal("aliceFORGED", scopeDictionary["UserId"]);
    }
}