
using EduTrack.Api.Middleware;
using EduTrack.Application.DependencyInjection;
using EduTrack.Infrastructure.Common.Logging;
using EduTrack.Infrastructure.Data;
using EduTrack.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;

namespace EduTrack.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog early in the pipeline
            LoggingConfiguration.ConfigureSerilog(builder.Configuration, builder.Environment);
            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllers();

            // Add CORS configuration — origins are driven by config to avoid AllowAnyOrigin in production
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("X-Correlation-ID");
                });
            });

            // Add Problem Details support (RFC 7807)
            builder.Services.AddProblemDetails();

            // Add Swagger generation and configure OpenAPI (Swagger) services
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "EduTrack API", 
                    Version = "v1",
                    Description = "Educational tracking system with comprehensive error handling"
                });
                
                // Add correlation ID header to Swagger
                c.AddSecurityDefinition("CorrelationId", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "X-Correlation-ID",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Correlation ID for request tracking"
                });
            });

            // Add health checks
            builder.Services.AddHealthChecks();

            builder.Services.AddTransient<CorrelationIdMiddleware>();

            // Add application services (includes MediatR, FluentValidation, and enhanced behaviors)
            builder.Services.AddApplication();

            // Add infrastructure services (includes enhanced error handling and resilience)
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            // Ensure database is created and migrated with error handling
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    
                    logger.LogInformation("Applying database migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations completed successfully");
                    
                    // Seed the database with initial data
                    logger.LogInformation("Starting database seeding...");
                    await EduTrack.Infrastructure.Data.SeedData.DataSeeder.SeedAllDataAsync(context, logger);
                    logger.LogInformation("Database seeding completed successfully");
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database");
                    throw;
                }
            }

            // Configure the HTTP request pipeline.
            
            app.UseMiddleware<CorrelationIdMiddleware>();

            // Add CORS middleware early in the pipeline
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            // Structured HTTP request logging via Serilog.
            // Security: query strings and headers are intentionally excluded to prevent
            // tokens, passwords, or PII leaking into log files.
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    // Do NOT enrich with: QueryString, Headers, RequestBody — may contain tokens or PII
                };
            });

            // Add global exception handling middleware
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger UI in development environment
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduTrack API v1");
                    c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger
                });
            }

            // Add health check endpoints
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.UseAuthorization();

            app.MapControllers();

            try
            {
                Log.Information("Starting EduTrack API application...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "EduTrack API application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
