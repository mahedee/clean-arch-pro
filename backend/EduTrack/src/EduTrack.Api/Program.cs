
using EduTrack.Api.Middleware;
using EduTrack.Application.DependencyInjection;
using EduTrack.Infrastructure.DependencyInjection;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace EduTrack.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Add CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add Swagger generation and configure OpenAPI (Swagger) services
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduTrack API", Version = "v1" });
            });

            // Add application services (includes MediatR, FluentValidation, and ValidationBehavior)
            builder.Services.AddApplication();

            // Add infrastructure services
            builder.Services.AddInfrastructure(builder.Configuration);



            var app = builder.Build();

            // Ensure database is created and migrated
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate(); // Apply migrations for PostgreSQL
            }

            // Configure the HTTP request pipeline.
            
            // Add CORS middleware early in the pipeline
            app.UseCors("AllowAll");
            
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
