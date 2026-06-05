using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Persistence
{
    public static class DpendencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services;
        }
    }
}
