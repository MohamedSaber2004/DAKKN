using DAKKN.Application.Common.Behaviours;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DAKKN.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            services.AddScoped<IImageValidator, ImageValidator>();

            services.AddSingleton<FileExtensionContentTypeProvider>(sp => new FileExtensionContentTypeProvider());
            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();


            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            UploadPaths.Configure(configuration);

            return services;
        }
    }
}
