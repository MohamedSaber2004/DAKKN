using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Services;
using DAKKN.Application;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Infrastructure;
using DAKKN.MVC.Services;
using DAKKN.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Localization;
using Serilog;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
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

            // Preemptively configure thread pool for high concurrency
            ThreadPool.SetMinThreads(200, 200);

            builder.Host.UseSerilog();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".DAKKN.GuestCart";
            });

            builder.Services.AddScoped<IGuestCartStorage, SessionCartStorage>();

            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

            // Response Compression — serves Brotli/Gzip-compressed responses to clients that support them.
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/json",
                    "application/javascript",
                    "text/css",
                    "text/html",
                    "text/plain",
                    "image/svg+xml"
                });
            });
            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
                options.Level = System.IO.Compression.CompressionLevel.Fastest);
            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
                options.Level = System.IO.Compression.CompressionLevel.SmallestSize);

            // Health Checks — monitors DB connectivity and overall liveness.
            var connectionString = builder.Configuration.GetConnectionString("DakknConnection") ?? string.Empty;
            builder.Services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: connectionString,
                    name: "sqlserver",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "ready" });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "JWT_OR_COOKIE";
                options.DefaultAuthenticateScheme = "JWT_OR_COOKIE";
                options.DefaultChallengeScheme = "JWT_OR_COOKIE";
            })
            .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }
                    return IdentityConstants.ApplicationScheme;
                };
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/auth/access-denied";
            });

            builder.Services.AddSingleton<System.Text.Encodings.Web.HtmlEncoder>(
                System.Text.Encodings.Web.HtmlEncoder.Create(System.Text.Unicode.UnicodeRanges.All));

            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.AddLocalization();

            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy => 
                {
                    policy.Expire(TimeSpan.FromMinutes(10));
                    policy.VaryByValue((context, ct) => 
                    {
                        var cultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var culture = cultureFeature?.RequestCulture.Culture.Name ?? "default";
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

            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CQRS", policy =>
                {
                    if (allowedOrigins.Length > 0)
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    else
                        policy.SetIsOriginAllowed(_ => false);
                });
            });


            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddMvc();

            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = ".DAKKN.Antiforgery";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build()));
                options.MaxModelValidationErrors = 50;
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

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

                if (authPermitLimit <= 0) authPermitLimit = 5;
                if (authWindowSeconds <= 0) authWindowSeconds = 120;

                options.AddFixedWindowLimiter(policyName: "auth", options =>
                {
                    options.PermitLimit = authPermitLimit;
                    options.Window = TimeSpan.FromSeconds(authWindowSeconds);
                    options.QueueLimit = authSettings.GetValue("QueueLimit", 0);
                    options.AutoReplenishment = true;
                });
            });

            var app = builder.Build();

            // Seed Data — only run full seeds in Development / Test environments.
            // In Production/Live only essential data (roles + admin account) is seeded.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                    await DAKKNDbContextSeed.SeedAsync(userManager, roleManager);

                    if (env.IsDevelopment() || env.EnvironmentName == "Test")
                    {
                        var context = services.GetRequiredService<DAKKNDbContext>();
                        await DAKKNDbContextSeed.SeedGovernoratesAsync(context);
                        await DAKKNDbContextSeed.SeedCategoriesAndProductsAsync(context);
                        await DAKKNDbContextSeed.SeedSupportDataAsync(context);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during database seeding.");
                }
            }

            app.UseForwardedHeaders();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseXContentTypeOptions();
            app.UseXfo(xfo => xfo.Deny());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseReferrerPolicy(opts => opts.NoReferrer());

            // Permissions-Policy: restrict access to sensitive browser APIs
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Permissions-Policy"] =
                    "camera=(), microphone=(), geolocation=(), payment=(), usb=(), interest-cohort=()";
                await next();
            });

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
                        .CustomSources("https://cdn.tailwindcss.com", "https://cdn.jsdelivr.net", "https://cdn.jsdelivr.net/npm/", "https://accounts.google.com")
                        .UnsafeInline()
                        .UnsafeEval()
                    )
                    .StyleSources(s => s
                        .Self()
                        .CustomSources(
                            "https://fonts.googleapis.com",
                            "https://fonts.gstatic.com",
                            "https://cdn.tailwindcss.com",
                            "https://accounts.google.com"
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
                            "https://flagcdn.com",
                            "https://dakkn.runasp.net",
                            "https://picsum.photos",
                            "https://fastly.picsum.photos",
                            "https://placehold.co",
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
                            "wss://localhost:*",
                            "https://accounts.google.com",
                            "https://cdn.jsdelivr.net"
                        )
                    )
                    .FrameSources(s => s.CustomSources("https://accounts.google.com"))
                    .ObjectSources(s => s.None())
                );
            }
            else
            {
                app.UseCsp(opts => opts
                    .ScriptSources(s => s
                        .Self()
                        .CustomSources("https://cdn.tailwindcss.com", "https://cdn.jsdelivr.net", "https://cdn.jsdelivr.net/npm/", "https://accounts.google.com")
                        .UnsafeInline()
                        .UnsafeEval()
                    )
                    .StyleSources(s => s
                        .Self()
                        .CustomSources(
                            "https://fonts.googleapis.com",
                            "https://fonts.gstatic.com",
                            "https://cdn.tailwindcss.com",
                            "https://accounts.google.com"
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
                            "https://flagcdn.com",
                            "https://dakkn.runasp.net",
                            "https://picsum.photos",
                            "https://fastly.picsum.photos",
                            "https://placehold.co",
                            "data:",
                            "blob:"
                        )
                    )
                    .ConnectSources(s => s
                        .Self()
                        .CustomSources("https://cdn.tailwindcss.com", "https://accounts.google.com", "https://cdn.jsdelivr.net")
                    )
                    .FrameSources(s => s.CustomSources("https://accounts.google.com"))
                    .ObjectSources(s => s.None())
                );
            }

            var supportedCultures = new[] { "ar", "en" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);


            // Priority: 1. QueryString (one-off override), 2. DB User Preference (for authenticated users),
            // 3. Legacy Cookie (for guest/landing), 4. Browser Accept-Language header.
            // This ensures changing language on landing page doesn't affect authenticated admin/customer pages.
            localizationOptions.RequestCultureProviders.Clear();
            localizationOptions.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Add(new DAKKN.MVC.Localization.UserPreferenceRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());

            app.UseRouting();

            app.UseCors("CQRS");

            app.UseSession();

            app.UseRateLimiter();

            app.UseAuthentication();

            app.UseRequestLocalization(localizationOptions);

            app.UseOutputCache();

            app.UseAuthorization();

            app.MapControllers();

            // Health check endpoints
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            duration = e.Value.Duration.TotalMilliseconds
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            }).AllowAnonymous();

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false, // only liveness — no dependency checks
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { status = report.Status.ToString() }));
                }
            }).AllowAnonymous();

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            duration = e.Value.Duration.TotalMilliseconds
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            }).AllowAnonymous();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                Log.Fatal(ex, "AppDomain unhandled exception: {Message}", ex?.Message);
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Log.Fatal(args.Exception, "Unobserved task exception: {Message}", args.Exception.Message);
                args.SetObserved();
            };

            app.Run();
        }
    }
}