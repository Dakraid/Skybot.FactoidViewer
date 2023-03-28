// Skybot.FactoidViewer
// Skybot.FactoidViewer / FactoidsModelConfiguration.cs BY Kristian Schlikow
// First modified on 2023.03.15
// Last modified on 2023.03.23

namespace Skybot.FactoidViewer.Models

{
#region
    using Asp.Versioning;
    using Asp.Versioning.OData;

    using Microsoft.OData.ModelBuilder;
#endregion

    /// <summary>
    ///     Model Configuration for the FactoidsModel.
    ///     Implements the <see cref="IModelConfiguration" />
    /// </summary>
    /// <seealso cref="IModelConfiguration" />
    public class FactoidsModelConfiguration : IModelConfiguration
    {
        /// <summary>
        ///     Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="T:Microsoft.OData.ModelBuilder.ODataModelBuilder">builder</see> used to apply
        ///     configurations.
        /// </param>
        /// <param name="apiVersion">
        ///     The <see cref="T:Asp.Versioning.ApiVersion">API version</see> associated with the
        ///     <paramref name="builder" />.
        /// </param>
        /// <param name="routePrefix">The route prefix associated with the configuration, if any.</param>
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
        {
            switch (apiVersion.MajorVersion)
            {
            case 2:
                ConfigureV2(builder);

                break;

            default:
                ConfigureDefault(builder);

                break;
            }
        }

        /// <summary>
        ///     Sets the base entity type configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>EntityTypeConfiguration&lt;Factoid&gt;.</returns>
        private static EntityTypeConfiguration<Factoid> ConfigureBase(ODataModelBuilder builder)
        {
            var factoid = builder.EntitySet<Factoid>("Factoid").EntityType;

            factoid.HasKey(e => e.Key);

            return factoid;
        }

        /// <summary>
        ///     Sets the default entity type configuration, if not V2.
        ///     Hides specific parts of the Factoid model.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void ConfigureDefault(ODataModelBuilder builder)
        {
            var configure = ConfigureBase(builder);
            configure.Ignore(f => f.CreatedBy);
            configure.Ignore(f => f.ModifiedBy);
            configure.Ignore(f => f.LockedAt);
            configure.Ignore(f => f.LockedBy);
        }

        /// <summary>
        ///     Sets the V2 entity type configuration.
        ///     Returns all fields of the Factoid model.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void ConfigureV2(ODataModelBuilder builder) => ConfigureBase(builder);
    }
}
