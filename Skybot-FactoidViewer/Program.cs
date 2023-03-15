// Skybot-FactoidViewer
// Skybot.FactoidViewer / Program.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.15



// Skybot-FactoidViewer
// Skybot.FactoidViewer / Program.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer
{
#region
    using Areas.Identity.Data;

    using AspNet.Security.OAuth.Discord;

    using Data;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.OData;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Fast.Components.FluentUI;

    using Models;

    using Setup;
#endregion

    public static class Program
    {
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

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityContext>();

            // Add controllers and enable OData with query options
            builder.Services.AddControllers().AddOData(options =>
            {
                options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count();
            });

            builder.Services.AddProblemDetails();

            // Add API versioning
            builder.Services.AddApiVersioning(options => options.AssumeDefaultVersionWhenUnspecified = true).AddOData(options =>
            {
                options.ModelBuilder.DefaultModelConfiguration = (builder, apiVersion, routePrefix) =>
                {
                    builder.EntitySet<Factoid>("Factoid");
                };

                options.AddRouteComponents();
            });

            // Add API explorer and Swagger
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ODataOperationFilter>();
            });

            // Create the web application
            var app = builder.Build();

            // If in development, enable Swagger UI
            // else enable HSTS and the exception page
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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
