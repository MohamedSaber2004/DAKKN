using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Repositories.Interfaces.Base;
using DAKKN.Infrastructure.Options;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DAKKN.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddOptions<B2Options>()
                .Bind(configuration.GetSection("B2"));

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.RegisterRepositories();

            if (env.IsDevelopment())
            {
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
            }
            else
            {
                services.AddScoped<IFileStorageService, B2FileStorageService>();
            }

            return services;
        }

        private static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            var repositoryAssembly = typeof(UnitOfWork).Assembly;

            var implementations = repositoryAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && t.Name.EndsWith("Repository"));

            foreach (var implementation in implementations)
            {
                var interfaces = implementation.GetInterfaces();
                var primaryInterface = interfaces.FirstOrDefault(i => i.Name == "I" + implementation.Name);

                if (primaryInterface != null)
                {
                    services.AddScoped(implementation);

                    services.AddScoped(primaryInterface, sp => sp.GetRequiredService(implementation));

                    foreach (var @interface in interfaces)
                    {
                        if (@interface.IsGenericType &&
                            (@interface.GetGenericTypeDefinition() == typeof(IGenericRepository<,>) ||
                             @interface.GetGenericTypeDefinition() == typeof(IGenericRepository<>)))
                        {
                            services.AddScoped(@interface, sp => sp.GetRequiredService(implementation));
                        }
                    }
                }
            }

            return services;
        }
    }
}
