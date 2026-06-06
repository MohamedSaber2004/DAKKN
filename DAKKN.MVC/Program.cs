using Asp.Versioning;
using DAKKN.Application;
using DAKKN.Application.Localization;
using DAKKN.Infrastructure;
using DAKKN.Persistence;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Serilog;
using System.Reflection;

namespace DAKKN.MVC
{
    public class Program
    {
        public static void Main(string[] args)
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


            builder.Services.AddApplication();
            builder.Services.AddPersistence();
            builder.Services.AddInfrastructure();

            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.AddLocalization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
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

            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                    options.AccessDeniedPath = "/auth/login";
                });

            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            var app = builder.Build();

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

            app.UseCors("AllowAll");

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