using GhcSamplePs.Core.Models.PlayerManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GhcSamplePs.Core.Data.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="PlayerStatistic"/> entity using Fluent API.
/// </summary>
/// <remarks>
/// <para>
/// This configuration defines:
/// - Table name and schema
/// - Primary key
/// - Required properties with constraints
/// - Indexes for query optimization
/// - Foreign key relationships
/// - Audit field configurations
/// </para>
/// <example>
/// <code>
/// // This configuration is automatically applied when calling:
/// modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
/// </code>
/// </example>
/// </remarks>
public class PlayerStatisticConfiguration : IEntityTypeConfiguration<PlayerStatistic>
{
    /// <summary>
    /// Configures the <see cref="PlayerStatistic"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<PlayerStatistic> builder)
    {
        // Table configuration
        builder.ToTable("PlayerStatistics");

        // Primary key
        builder.HasKey(ps => ps.PlayerStatisticId);

        // PlayerStatisticId - auto-generated
        builder.Property(ps => ps.PlayerStatisticId)
            .ValueGeneratedOnAdd();

        // TeamPlayerId - required foreign key with index
        builder.Property(ps => ps.TeamPlayerId)
            .IsRequired();

        builder.HasIndex(ps => ps.TeamPlayerId)
            .HasDatabaseName("IX_PlayerStatistics_TeamPlayerId");

        // Foreign key relationship with TeamPlayer - cascade delete
        builder.HasOne(ps => ps.TeamPlayer)
            .WithMany()
            .HasForeignKey(ps => ps.TeamPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // GameDate - required with index
        builder.Property(ps => ps.GameDate)
            .IsRequired();

        builder.HasIndex(ps => ps.GameDate)
            .HasDatabaseName("IX_PlayerStatistics_GameDate");

        // Composite index on TeamPlayerId and GameDate for common query pattern
        builder.HasIndex(ps => new { ps.TeamPlayerId, ps.GameDate })
            .HasDatabaseName("IX_PlayerStatistics_TeamPlayerId_GameDate");

        // MinutesPlayed - required
        builder.Property(ps => ps.MinutesPlayed)
            .IsRequired();

        // IsStarter - required
        builder.Property(ps => ps.IsStarter)
            .IsRequired();

        // JerseyNumber - required
        builder.Property(ps => ps.JerseyNumber)
            .IsRequired();

        // Goals - required
        builder.Property(ps => ps.Goals)
            .IsRequired();

        // Assists - required
        builder.Property(ps => ps.Assists)
            .IsRequired();

        // Audit fields
        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        builder.Property(ps => ps.CreatedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(ps => ps.UpdatedAt);

        builder.Property(ps => ps.UpdatedBy)
            .HasMaxLength(450);
    }
}
