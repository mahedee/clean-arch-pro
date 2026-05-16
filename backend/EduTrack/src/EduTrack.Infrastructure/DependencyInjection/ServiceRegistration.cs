using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Infrastructure.Data;
using EduTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EduTrack.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add HTTP Context Accessor for correlation ID tracking
            services.AddHttpContextAccessor();

            // Configure structured logging with Serilog
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

            // Configure Entity Framework with basic retry
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        // Enable retry on failure with exponential backoff
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                        
                        // Set command timeout
                        npgsqlOptions.CommandTimeout(30);
                    })
                    .EnableSensitiveDataLogging(false)
                    .EnableDetailedErrors();
            });

            // Register repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add health checks
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>("database", tags: new[] { "db", "ready" });

            return services;
        }
    }
}
