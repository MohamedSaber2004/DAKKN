using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Services;
using DAKKN.Application;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Infrastructure;
using DAKKN.Persistence;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using Serilog;
using System.Reflection;
using System.Threading.RateLimiting;

namespace DAKKN.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = builder.Environment;

            builder.Configuration.Sources.Clear();
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            JsonLocalizationProvider.Initialize(env.ContentRootPath);

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null)
                    builder.Configuration.AddUserSecrets(appAssembly, optional: true);
            }

            builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();

            Log.Information("DAKKN App is starting up at {Time}", DateTime.Now);

            builder.Host.UseSerilog();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/auth/access-denied";
            });

            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.AddLocalization();

            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy => 
                {
                    policy.Expire(TimeSpan.FromMinutes(10));
                    policy.VaryByValue((context, ct) => 
                    {
                        var culture = context.Request.Cookies[".AspNetCore.Culture"] ?? "default";
                        return ValueTask.FromResult(new KeyValuePair<string, string>("culture", culture));
                    });
                });
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default. Clear that restriction to allow all proxies.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CQRS", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddMvc();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
                options.MaxModelValidationErrors = 50;
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                var globalSettings = builder.Configuration.GetSection("RateLimiting:Global");
                var globalPermitLimit = globalSettings.GetValue<int>("PermitLimit");
                var globalWindowSeconds = globalSettings.GetValue<int>("WindowSeconds");
                
                // Fallback to sane defaults if not configured
                if (globalPermitLimit <= 0) globalPermitLimit = 100;
                if (globalWindowSeconds <= 0) globalWindowSeconds = 60;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "global",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = globalPermitLimit,
                            QueueLimit = globalSettings.GetValue("QueueLimit", 0),
                            Window = TimeSpan.FromSeconds(globalWindowSeconds)
                        }));

                var authSettings = builder.Configuration.GetSection("RateLimiting:Auth");
                var authPermitLimit = authSettings.GetValue<int>("PermitLimit");
                var authWindowSeconds = authSettings.GetValue<int>("WindowSeconds");

                if (authPermitLimit <= 0) authPermitLimit = 10;
                if (authWindowSeconds <= 0) authWindowSeconds = 60;

                options.AddFixedWindowLimiter(policyName: "auth", options =>
                {
                    options.PermitLimit = authPermitLimit;
                    options.Window = TimeSpan.FromSeconds(authWindowSeconds);
                    options.QueueLimit = authSettings.GetValue("QueueLimit", 0);
                    options.AutoReplenishment = true;
                });
            });

            var app = builder.Build();

            // Seed Data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                    await DAKKNDbContextSeed.SeedAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during database seeding.");
                }
            }

            app.UseForwardedHeaders();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts(hsts => hsts
                    .MaxAge(days: 365)
                    .IncludeSubdomains()
                    .Preload()
                );
            }

            app.UseHttpsRedirection();

            app.UseXContentTypeOptions();
            app.UseXfo(xfo => xfo.Deny());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseReferrerPolicy(opts => opts.NoReferrer());

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new CustomFileProvider(app.Environment.WebRootPath),
                RequestPath = "/files"
            });

            app.UseNoCacheHttpHeaders();

            if (app.Environment.IsDevelopment())
            {
                app.UseCsp(opts => opts
                    .ScriptSources(s => s
                        .Self()
                        .CustomSources("https://cdn.tailwindcss.com")
                        .UnsafeInline()
                        .UnsafeEval()
                    )
                    .StyleSources(s => s
                        .Self()
                        .CustomSources(
                            "https://fonts.googleapis.com",
                            "https://fonts.gstatic.com",
                            "https://cdn.tailwindcss.com"  
                        )
                        .UnsafeInline()
                    )
                    .FontSources(s => s
                        .Self()
                        .CustomSources(
                            "https://fonts.googleapis.com",
                            "https://fonts.gstatic.com"
                        )
                    )
                    .ImageSources(s => s
                        .Self()
                        .CustomSources(
                            "https://lh3.googleusercontent.com",
                            "https://ui-avatars.com",
                            "data:",
                            "blob:"
                        )
                    )
                    .ConnectSources(s => s
                        .Self()
                        .CustomSources(
                            "https://cdn.tailwindcss.com",
                            "localhost:*",
                            "http://localhost:*",
                            "https://localhost:*",
                            "ws://localhost:*",
                            "wss://localhost:*"
                        )
                    )
                    .FrameSources(s => s.None())
                    .ObjectSources(s => s.None())
                );
            }
            else
            {
                app.UseCsp(opts => opts
                    .ScriptSources(s => s
                        .Self()
                        .CustomSources("https://cdn.tailwindcss.com")
                        .UnsafeInline()
                        .UnsafeEval()
                    )
                    .StyleSources(s => s
                        .Self()
                        .CustomSources(
                            "https://fonts.googleapis.com",
                            "https://fonts.gstatic.com",
                            "https://cdn.tailwindcss.com" 
                        )
                        .UnsafeInline()
                    )
                    .FontSources(s => s
                        .Self()
                        .CustomSources(
                            "https://fonts.googleapis.com",
                            "https://fonts.gstatic.com"
                        )
                    )
                    .ImageSources(s => s
                        .Self()
                        .CustomSources(
                            "https://lh3.googleusercontent.com",
                            "https://ui-avatars.com",
                            "data:"
                        )
                    )
                    .ConnectSources(s => s
                        .Self()
                        .CustomSources("https://cdn.tailwindcss.com")
                    )
                    .FrameSources(s => s.None())
                    .ObjectSources(s => s.None())
                );
            }

            var supportedCultures = new[] { "en", "ar" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[1]) // "ar"
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            // Prioritize Cookie and QueryString providers
            localizationOptions.RequestCultureProviders.Clear();
            localizationOptions.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

            app.UseRequestLocalization(localizationOptions);

            app.UseRouting();

            app.UseOutputCache();

            app.UseCors("CQRS");

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}