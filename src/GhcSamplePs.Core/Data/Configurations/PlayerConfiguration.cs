using GhcSamplePs.Core.Models.PlayerManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GhcSamplePs.Core.Data.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="Player"/> entity using Fluent API.
/// </summary>
/// <remarks>
/// <para>
/// This configuration defines:
/// - Table name and schema
/// - Primary key
/// - Required and optional properties with constraints
/// - Indexes for query optimization
/// - Audit field configurations
/// </para>
/// <example>
/// <code>
/// // This configuration is automatically applied when calling:
/// modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
/// </code>
/// </example>
/// </remarks>
public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    /// <summary>
    /// Configures the <see cref="Player"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        // Table configuration
        builder.ToTable("Players");

        // Primary key
        builder.HasKey(p => p.Id);

        // Id - auto-generated
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        // UserId - required, indexed for filtering by user
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Players_UserId");

        // Name - required, indexed for search
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Players_Name");

        // DateOfBirth - required
        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        // Index on DateOfBirth for age-based queries
        builder.HasIndex(p => p.DateOfBirth)
            .HasDatabaseName("IX_Players_DateOfBirth");

        // Composite index on UserId and Name for common query pattern
        builder.HasIndex(p => new { p.UserId, p.Name })
            .HasDatabaseName("IX_Players_UserId_Name");

        // Gender - optional
        builder.Property(p => p.Gender)
            .HasMaxLength(50);

        // PhotoUrl - optional
        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(500);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.UpdatedBy)
            .HasMaxLength(450);

        // Ignore computed property - Age is calculated, not stored
        builder.Ignore(p => p.Age);
    }
}
