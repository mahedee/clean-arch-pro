using EduTrack.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EduTrack.Application.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Updated to use the correct overload of AddMediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register AutoMapper if used
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            return services;
        }
    }
}
