using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace EduTrack.Infrastructure.Common.Logging;

/// <summary>
/// Configuration for structured logging with Serilog
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configure Serilog for the application
    /// </summary>
    public static void ConfigureSerilog(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ApplicationName", "EduTrack")
            .Enrich.WithProperty("Environment", environment.EnvironmentName);

        // Console logging configuration based on environment
        if (environment.IsDevelopment())
        {
            loggerConfiguration
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        }
        else
        {
            loggerConfiguration
                .WriteTo.Console(new CompactJsonFormatter())
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning);
        }

        // File logging for all environments.
        // Extension is .log (universal standard recognised by log shippers and rotation tools).
        // Content is CLEF (Compact Log Event Format / newline-delimited JSON) from CompactJsonFormatter.
        // Security: query strings, request bodies, passwords, emails and phone numbers must
        // NEVER appear in log messages — log entity IDs and status enums only.
        loggerConfiguration
            .WriteTo.File(
                new CompactJsonFormatter(),
                path: "logs/edutrack-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                fileSizeLimitBytes: 50 * 1024 * 1024, // 50MB
                restrictedToMinimumLevel: LogEventLevel.Information);

        Log.Logger = loggerConfiguration.CreateLogger();
    }
}