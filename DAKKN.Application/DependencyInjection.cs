using DAKKN.Application.Interfaces;
using DAKKN.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, MockProductService>();
            services.AddScoped<IDashboardService, MockDashboardService>();

            return services;
        }
    }
}
