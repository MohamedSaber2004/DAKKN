using DAKKN.Application.Common.Interfaces;
using DAKKN.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuditInterceptor>();

            services.AddDbContext<DAKKNDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditInterceptor>();
                
                options.UseSqlServer(configuration.GetConnectionString("DakknConnection"),
                    b => b.MigrationsAssembly(typeof(DAKKNDbContext).Assembly.FullName))
                       .AddInterceptors(interceptor);
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<DAKKNDbContext>());

            return services;
        }
    }
}
