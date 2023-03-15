// Skybot.FactoidViewer
// Skybot.FactoidViewer / FactoidsContext.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.15

#region
#endregion

namespace Skybot.FactoidViewer.Models

{
#region
    using Microsoft.EntityFrameworkCore;
#endregion

    public class FactoidsContext : DbContext
    {
        public FactoidsContext() {}

        public FactoidsContext(DbContextOptions<FactoidsContext> options) : base(options) {}

        public virtual DbSet<Factoid> Factoids { get; set; } = null!;

        public virtual DbSet<Validator> Validators { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factoid>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.ToTable("factoids");

                entity.Property(e => e.Key).HasColumnName("key");
                entity.Property(e => e.CreatedAt).HasColumnType("TIMESTAMP").HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.Fact).HasColumnName("fact");
                entity.Property(e => e.LockedAt).HasColumnType("TIMESTAMP").HasColumnName("locked_at");
                entity.Property(e => e.LockedBy).HasColumnName("locked_by");
                entity.Property(e => e.ModifiedAt).HasColumnType("TIMESTAMP").HasColumnName("modified_at");
                entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
                entity.Property(e => e.RequestedCount).HasColumnName("requested_count");
            });

            modelBuilder.Entity<Validator>(entity =>
            {
                entity.HasKey(e => e.ValidKey);

                entity.ToTable("validator");

                entity.Property(e => e.ValidKey).HasColumnName("validKey");
                entity.Property(e => e.ValidValue).HasColumnType("BOOL").HasColumnName("validValue");
            });
        }
    }
}
