
using EduTrack.Application.DependencyInjection;
using EduTrack.Infrastructure.DependencyInjection;
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

            // Add Swagger generation and configure OpenAPI (Swagger) services
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduTrack API", Version = "v1" });
            });

            // Add application services
            builder.Services.AddApplication();

            // Add infrastructure services
            builder.Services.AddInfrastructure(builder.Configuration);



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger UI in development environment
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduTrack API v1");
                    c.RoutePrefix = string.Empty; // To serve Swagger UI at the root (e.g., http://localhost:5000)
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
