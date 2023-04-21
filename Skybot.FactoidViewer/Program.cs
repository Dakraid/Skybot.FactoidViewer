// Skybot.FactoidViewer
// Skybot.FactoidViewer / Program.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.23

namespace Skybot.FactoidViewer

{
#region
    using Asp.Versioning;


    using Data;

    using Microsoft.AspNetCore.OData;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    using Syncfusion.Blazor;

    using Setup;

    using Swashbuckle.AspNetCore.SwaggerGen;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.AspNetCore.DataProtection;
    using System;
    using Skybot.FactoidViewer.Shared;
    using System.IO.Compression;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Components.Authorization;
    using Skybot.FactoidViewer.Areas.Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;

    #endregion
    /// <summary>
    ///     Class Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddJsonFile("config/appsettings.Development.json", optional: true, reloadOnChange: true);

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SynfusionLicenseKey"));

            // Add EF database context
            var factoidContextConnectionString = builder.Configuration.GetConnectionString("FactoidContextConnection") ?? throw new InvalidOperationException("Connection string 'FactoidContextConnection' not found.");
            builder.Services.AddDbContext<FactoidsContext>(options => options.UseSqlite(factoidContextConnectionString));

            var identityContextConnectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");
            builder.Services.AddDbContext<ApplicationIdentityContext>(options => options.UseSqlServer(identityContextConnectionString));

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationIdentityContext>();

            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            // Add views and server side rendering
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMvc()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
                .AddRazorPagesOptions(options =>
                {
                    if (string.Equals(builder.Configuration["SkipAntiForgery"], "DoSkip", StringComparison.OrdinalIgnoreCase)) {
                        options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                    }
                });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            builder.Services.AddDataProtection().SetApplicationName(ApplicationCookie.Application.Name);

            builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://facts.netrve.net", "https://facts.netrve.net")));

            builder.Services.AddAntiforgery((options) =>
            {
                options.Cookie.Name = ApplicationCookie.Antiforgery.Name;
                options.Cookie.Path = "/";
                options.Cookie.Domain = builder.Configuration["CookieDomain"];
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.FormFieldName = "_anti-forgery";
                options.HeaderName = "x-anti-forgery";
            });

            builder.Services.ConfigureApplicationCookie((options) =>
            {
                options.Cookie.Name = ApplicationCookie.Application.Name;
                options.Cookie.Path = "/";
                options.Cookie.Domain = builder.Configuration["CookieDomain"];
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            builder.Services.AddResponseCaching();

            builder.Services.Configure<GzipCompressionProviderOptions>((p) => p.Level = CompressionLevel.Fastest);
            builder.Services.Configure<BrotliCompressionProviderOptions>((p) => p.Level = CompressionLevel.Fastest);

            builder.Services.AddResponseCaching();
            builder.Services.AddResponseCompression((options) =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            });

            // Add UI components
            builder.Services.AddHttpClient();
            builder.Services.AddSyncfusionBlazor();

            // Add authentication
            /*.AddDiscord(options =>
            {
                options.ClientId = builder.Configuration.GetSection("Authentication")["DiscordId"] ?? "";
                options.ClientSecret = builder.Configuration.GetSection("Authentication")["DiscordSecret"] ?? "";
                options.SaveTokens = true;
            });
            */

            // Add controllers and enable OData with query options
            builder.Services.AddControllers().AddOData(options => options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count());

            // Add API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1.0);
            }).AddOData(options => options.AddRouteComponents("api")).AddODataApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Add API explorer and Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen(options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                // integrate xml comments
                options.IncludeXmlComments(filePath);
            });

            builder.Services.AddProblemDetails();

            // Create the web application
            var app = builder.Build();

            app.UseForwardedHeaders();

            // If in development, enable Swagger UI
            // else enable HSTS and the exception page
            if (app.Environment.IsDevelopment())
            {
                app.UseODataRouteDebug();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // Build a Swagger endpoint for each discovered API version
                    foreach (var description in app.DescribeApiVersions())
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        var name = description.GroupName.ToUpperInvariant();
                        options.SwaggerEndpoint(url, name);
                    }
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.SameAsRequest
            });

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.MapGet("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
            app.MapPost("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));

            app.MapGet("/Identity/Account/ResendEmailConfirmation", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
            app.MapPost("/Identity/Account/ResendEmailConfirmation", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));

            app.MapGet("/Identity/Account/ForgotPassword", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
            app.MapPost("/Identity/Account/ForgotPassword", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));

            app.Run();
        }
    }
}
