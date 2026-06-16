using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Infrastructure.Services;
using DAKKN.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DAKKN.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.RegisterRepositories();

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGoogleAuth, GoogleAuth>();

            var identityOptionsConfig = new IdentityModel();
            configuration.Bind("IdentityOptions", identityOptionsConfig);

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireNonAlphanumeric = identityOptionsConfig.RequireNonAlphanumeric;
                options.Password.RequiredLength = identityOptionsConfig.RequiredLength;
                options.Password.RequireDigit = identityOptionsConfig.RequiredDigit;
                options.Password.RequireLowercase = identityOptionsConfig.RequireLowercase;
                options.Password.RequiredUniqueChars = identityOptionsConfig.RequiredUniqueChars;
                options.Password.RequireUppercase = identityOptionsConfig.RequireUppercase;
                options.Lockout.MaxFailedAccessAttempts = identityOptionsConfig.MaxFailedAttempts;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(identityOptionsConfig.LockoutTimeSpanInDays);
                options.SignIn.RequireConfirmedEmail = identityOptionsConfig.RequireConfirmedEmail;
                options.User.AllowedUserNameCharacters = identityOptionsConfig.AllowedUserNameCharacters;
                options.User.RequireUniqueEmail = identityOptionsConfig.RequireUniqueEmail;
            })
            .AddEntityFrameworkStores<DAKKNDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<DAKKN.Infrastructure.Identity.ApplicationUserClaimsPrincipalFactory>();


            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<GoogleAuthSettings>(configuration.GetSection("GoogleAuthSettings"));
            var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() ?? new JwtSettings();
            var secretKey = jwtSettings.Secret;

            var tokenValidationParameters = new TokenValidationParameters

            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = false,
                ValidAudience = jwtSettings.Audience,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            services.AddSingleton(tokenValidationParameters);

            var googleSection = configuration.GetSection("GoogleAuthSettings");

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = googleSection["WebClientId"];
                options.ClientSecret = googleSection["WebClientSecret"];
                options.CallbackPath = "/signin-google";
                options.SaveTokens = true;
                options.AccessType = "offline";
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        var isApiRequest = context.HttpContext.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ?? false;

                        if (isApiRequest)
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var localizedMessage = JsonLocalizationProvider.GetLocalizedString(LocalizationKeys.ExceptionMessages.Unauthorized.Value);
                            var result = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                succeeded = false,
                                message = localizedMessage,
                                errors = new Dictionary<string, string[]>(),
                                code = 401
                            });

                            return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    }
                };
            });

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
