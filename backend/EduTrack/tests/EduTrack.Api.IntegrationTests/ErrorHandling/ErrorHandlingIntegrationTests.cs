using EduTrack.Api.Models;
using EduTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace EduTrack.Api.IntegrationTests.ErrorHandling;

public class ErrorHandlingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ErrorHandlingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetStudent_WhenStudentNotFound_ShouldReturnNotFoundWithProblemDetails()
    {
        // Arrange
        var nonExistentStudentId = 999;

        // Act
        var response = await _client.GetAsync($"/api/students/{nonExistentStudentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Entity Not Found", problemDetails.Title);
        Assert.Equal(404, problemDetails.Status);
        Assert.NotEmpty(problemDetails.CorrelationId);
        Assert.NotEmpty(problemDetails.TraceId);
        Assert.True(problemDetails.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateStudent_WithInvalidData_ShouldReturnBadRequestWithValidationErrors()
    {
        // Arrange
        var invalidStudent = new
        {
            Name = "", // Invalid: empty name
            Email = "invalid-email", // Invalid: not a valid email format
            Age = -5 // Invalid: negative age
        };

        var json = JsonSerializer.Serialize(invalidStudent);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/students", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Validation Error", problemDetails.Title);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal("VALIDATION_ERROR", problemDetails.ErrorCode);
        Assert.NotNull(problemDetails.Errors);
        Assert.NotEmpty(problemDetails.CorrelationId);
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }

    [Fact]
    public async Task HealthCheckReady_ShouldReturnHealthyWithDatabaseCheck()
    {
        // Act
        var response = await _client.GetAsync("/health/ready");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }

    [Fact]
    public async Task AnyRequest_ShouldIncludeCorrelationIdInResponse()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        _client.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.True(response.Headers.Contains("X-Correlation-ID"));
        var responseCorrelationId = response.Headers.GetValues("X-Correlation-ID").FirstOrDefault();
        Assert.Equal(correlationId, responseCorrelationId);
    }

    [Fact]
    public async Task AnyRequest_WithoutCorrelationId_ShouldGenerateOne()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.True(response.Headers.Contains("X-Correlation-ID"));
        var correlationId = response.Headers.GetValues("X-Correlation-ID").FirstOrDefault();
        Assert.NotNull(correlationId);
        Assert.True(Guid.TryParse(correlationId, out _));
    }

    [Theory]
    [InlineData("/api/nonexistent")]
    [InlineData("/api/students/abc")] // Invalid ID format
    public async Task NonExistentEndpoint_ShouldReturnNotFoundWithProblemDetails(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        // Should still include correlation ID for tracking
        Assert.True(response.Headers.Contains("X-Correlation-ID"));
    }

    [Fact]
    public async Task DatabaseConnectionFailure_ShouldReturnInternalServerErrorWithProblemDetails()
    {
        // This test would require mocking the database connection to fail
        // For demonstration purposes, we'll test with a malformed request that causes internal processing
        
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Configure a database that will fail
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a bad connection string to simulate database failure
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql("Host=nonexistent;Database=test;Username=test;Password=test");
                });
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/api/students");

        // Assert - This would typically return 500 due to database connection failure
        // The exact assertion depends on how the application handles database connection failures
        Assert.True(response.StatusCode == HttpStatusCode.InternalServerError || 
                   response.StatusCode == HttpStatusCode.ServiceUnavailable);
        
        if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
        {
            var content = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Assert.NotNull(problemDetails);
            Assert.NotEmpty(problemDetails.CorrelationId);
            Assert.NotEmpty(problemDetails.TraceId);
        }
    }
}