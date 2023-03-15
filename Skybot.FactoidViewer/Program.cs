// Skybot.FactoidViewer
// Skybot.FactoidViewer / Program.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer

{
#region
    using Asp.Versioning;

    using AspNet.Security.OAuth.Discord;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.OData;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.Fast.Components.FluentUI;

    using Models;

    using Setup;

    using Swashbuckle.AspNetCore.SwaggerGen;
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

            // Add views and server side rendering
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            // Add UI components
            builder.Services.AddFluentUIComponents();
            builder.Services.AddDataGridEntityFrameworkAdapter();

            // Add authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthorizationEndpoint;
            }).AddCookie().AddDiscord(options =>
            {
                options.ClientId = builder.Configuration.GetSection("Authentication")["DiscordId"] ?? "";
                options.ClientSecret = builder.Configuration.GetSection("Authentication")["DiscordSecret"] ?? "";
                options.SaveTokens = true;
            });

            // Add EF database context
            builder.Services.AddDbContext<FactoidsContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add controllers and enable OData with query options
            builder.Services.AddControllers().AddOData(options =>
            {
                options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count();
            });

            builder.Services.AddProblemDetails();

            // Add API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1.0);
            }).AddOData(options =>
            {
                options.AddRouteComponents("api");
            }).AddODataApiExplorer(options =>
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

            // Create the web application
            var app = builder.Build();

            // If in development, enable Swagger UI
            // else enable HSTS and the exception page
            if (app.Environment.IsDevelopment())
            {
                app.UseODataRouteDebug();

                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    var descriptions = app.DescribeApiVersions();

                    // Build a Swagger endpoint for each discovered API version
                    foreach (var description in descriptions)
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

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
