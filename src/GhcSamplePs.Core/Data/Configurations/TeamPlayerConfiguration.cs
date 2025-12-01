using GhcSamplePs.Core.Models.PlayerManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GhcSamplePs.Core.Data.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="TeamPlayer"/> entity using Fluent API.
/// </summary>
/// <remarks>
/// <para>
/// This configuration defines:
/// - Table name and schema
/// - Primary key
/// - Required and optional properties with constraints
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
public class TeamPlayerConfiguration : IEntityTypeConfiguration<TeamPlayer>
{
    /// <summary>
    /// Configures the <see cref="TeamPlayer"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TeamPlayer> builder)
    {
        // Table configuration
        builder.ToTable("TeamPlayers");

        // Primary key
        builder.HasKey(tp => tp.TeamPlayerId);

        // TeamPlayerId - auto-generated
        builder.Property(tp => tp.TeamPlayerId)
            .ValueGeneratedOnAdd();

        // PlayerId - required foreign key with index
        builder.Property(tp => tp.PlayerId)
            .IsRequired();

        builder.HasIndex(tp => tp.PlayerId)
            .HasDatabaseName("IX_TeamPlayers_PlayerId");

        // Foreign key relationship with Player - cascade delete
        builder.HasOne(tp => tp.Player)
            .WithMany()
            .HasForeignKey(tp => tp.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // TeamName - required, indexed for search
        builder.Property(tp => tp.TeamName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(tp => tp.TeamName)
            .HasDatabaseName("IX_TeamPlayers_TeamName");

        // ChampionshipName - required
        builder.Property(tp => tp.ChampionshipName)
            .IsRequired()
            .HasMaxLength(200);

        // JoinedDate - required
        builder.Property(tp => tp.JoinedDate)
            .IsRequired();

        // LeftDate - optional
        builder.Property(tp => tp.LeftDate);

        // Ignore computed property - IsActive is calculated, not stored
        builder.Ignore(tp => tp.IsActive);

        // Index on IsActive - computed column index using LeftDate
        // Since IsActive is computed, we create an index on LeftDate for filtering active players
        builder.HasIndex(tp => tp.LeftDate)
            .HasDatabaseName("IX_TeamPlayers_IsActive");

        // Composite index on PlayerId and IsActive (using LeftDate)
        builder.HasIndex(tp => new { tp.PlayerId, tp.LeftDate })
            .HasDatabaseName("IX_TeamPlayers_PlayerId_IsActive");

        // Composite index on PlayerId, TeamName, and ChampionshipName
        builder.HasIndex(tp => new { tp.PlayerId, tp.TeamName, tp.ChampionshipName })
            .HasDatabaseName("IX_TeamPlayers_PlayerId_TeamName_ChampionshipName");

        // Audit fields
        builder.Property(tp => tp.CreatedAt)
            .IsRequired();

        builder.Property(tp => tp.CreatedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(tp => tp.UpdatedAt);

        builder.Property(tp => tp.UpdatedBy)
            .HasMaxLength(450);
    }
}
