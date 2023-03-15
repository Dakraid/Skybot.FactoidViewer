// Skybot.FactoidViewer
// Skybot.FactoidViewer / FactoidsModelConfiguration.cs BY Kristian Schlikow
// First modified on 2023.03.15
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer.Models

{
#region
    using Asp.Versioning;
    using Asp.Versioning.OData;

    using Microsoft.OData.ModelBuilder;
#endregion

    public class FactoidsModelConfiguration : IModelConfiguration
    {
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
        {
            switch (apiVersion.MajorVersion)
            {
            case 2:
                ConfigureV2(builder);

                break;

            default:
                ConfigureV1(builder);

                break;
            }
        }

        private void ConfigureV1(ODataModelBuilder builder)
        {
            var configure = ConfigureCurrent(builder);
            configure.Ignore(f => f.CreatedBy);
            configure.Ignore(f => f.ModifiedBy);
            configure.Ignore(f => f.LockedAt);
            configure.Ignore(f => f.LockedBy);
        }

        private void ConfigureV2(ODataModelBuilder builder) => ConfigureCurrent(builder);

        private EntityTypeConfiguration<Factoid> ConfigureCurrent(ODataModelBuilder builder)
        {
            var factoid = builder.EntitySet<Factoid>("Factoid").EntityType;

            factoid.HasKey(e => e.Key);

            factoid.Property(e => e.Key);
            factoid.Property(e => e.CreatedAt);
            factoid.Property(e => e.CreatedBy);
            factoid.Property(e => e.Fact);
            factoid.Property(e => e.LockedAt);
            factoid.Property(e => e.LockedBy);
            factoid.Property(e => e.ModifiedAt);
            factoid.Property(e => e.ModifiedBy);
            factoid.Property(e => e.RequestedCount);

            return factoid;
        }
    }
}
