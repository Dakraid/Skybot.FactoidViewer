// Skybot.FactoidViewer - FactoidsContext.cs
// Created on 2023.02.17
// Last modified at 2023.02.19 13:37

#region
using Microsoft.EntityFrameworkCore;
#endregion

namespace Skybot.FactoidViewer.Models;

public class FactoidsContext : DbContext
{
    public FactoidsContext() {}

    public FactoidsContext(DbContextOptions<FactoidsContext> options) : base(options) {}

    public virtual DbSet<Factoid> Factoids { get; set; }

    public virtual DbSet<Validator> Validators { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Factoids.db");
    }

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
