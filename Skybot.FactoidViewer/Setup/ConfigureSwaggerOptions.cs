// Skybot.FactoidViewer
// Skybot.FactoidViewer / ConfigureSwaggerOptions.cs BY Kristian Schlikow
// First modified on 2023.03.15
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer.Setup

{
#region
    using Asp.Versioning.ApiExplorer;

    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    using System.Text;
#endregion

    /// <summary>
    ///     Configures the Swagger generation options.
    /// </summary>
    /// <remarks>
    ///     This allows API versioning to define a Swagger document per API version after the
    ///     <see cref="IApiVersionDescriptionProvider" /> service has been resolved from the service container.
    /// </remarks>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
        /// </summary>
        /// <param name="provider">
        ///     The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger
        ///     documents.
        /// </param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var text = new StringBuilder("Factoids API supporting OData, OpenAPI, Swashbuckle, and API versioning.");

            var info = new OpenApiInfo
            {
                Title = "Factoid API",
                Version = description.ApiVersion.ToString(),
                Contact = new OpenApiContact
                {
                    Name = "Kristian Schlikow", Email = "kristian@schlikow.de"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")
                }
            };

            if (description.IsDeprecated)
            {
                text.Append("This API version has been deprecated.");
            }

            if (description.SunsetPolicy is
                {} policy)
            {
                if (policy.Date is
                    {} when)
                {
                    text.Append("The API will be sunset on ").Append(when.Date.ToShortDateString()).Append('.');
                }

                if (policy.HasLinks)
                {
                    text.AppendLine();

                    foreach (var link in policy.Links)
                    {
                        if (link.Type != "text/html")
                        {
                            continue;
                        }

                        text.AppendLine();

                        if (link.Title.HasValue)
                        {
                            text.Append(link.Title.Value).Append(": ");
                        }

                        text.Append(link.LinkTarget.OriginalString);
                    }
                }
            }

            info.Description = text.ToString();

            return info;
        }
    }
}
