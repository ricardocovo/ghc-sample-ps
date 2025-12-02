using GhcSamplePs.Core.Models.PlayerManagement;
using Microsoft.EntityFrameworkCore;

namespace GhcSamplePs.Core.Data;

/// <summary>
/// Represents the Entity Framework Core database context for the application.
/// Manages entity configurations and automatic audit field population.
/// </summary>
/// <remarks>
/// <para>
/// This DbContext provides the following features:
/// - Automatic population of audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
/// - Entity configurations using Fluent API
/// - Connection retry policies for transient failures
/// </para>
/// <example>
/// <code>
/// // Register in DI container
/// builder.Services.AddDbContext&lt;ApplicationDbContext&gt;(options =>
///     options.UseSqlServer(connectionString, sqlOptions =>
///         sqlOptions.EnableRetryOnFailure(
///             maxRetryCount: 5,
///             maxRetryDelay: TimeSpan.FromSeconds(30),
///             errorNumbersToAdd: null)));
/// 
/// // Use in a service
/// public class PlayerRepository
/// {
///     private readonly ApplicationDbContext _context;
///     
///     public async Task&lt;Player&gt; GetByIdAsync(int id)
///     {
///         return await _context.Players.FindAsync(id);
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public class ApplicationDbContext : DbContext
{
    private readonly string _currentUserId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    /// <param name="currentUserId">
    /// The identifier of the current user for audit field population.
    /// Defaults to "system" if not provided.
    /// </param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string? currentUserId = null)
        : base(options)
    {
        _currentUserId = currentUserId ?? "system";
    }

    /// <summary>
    /// Gets or sets the DbSet for Player entities.
    /// </summary>
    public DbSet<Player> Players { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for TeamPlayer entities.
    /// </summary>
    public DbSet<TeamPlayer> TeamPlayers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for PlayerStatistic entities.
    /// </summary>
    public DbSet<PlayerStatistic> PlayerStatistics { get; set; } = null!;

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Automatically updates audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
    /// on entities being added or modified.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the database.
    /// </returns>
    /// <example>
    /// <code>
    /// var player = new Player
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(2010, 1, 1),
    ///     CreatedBy = "system" // Will be set automatically
    /// };
    /// 
    /// context.Players.Add(player);
    /// await context.SaveChangesAsync();
    /// // player.CreatedAt is now set to current UTC time
    /// </code>
    /// </example>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically updates audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
    /// on entities being added or modified.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Configures the model using Fluent API when the model for a derived context is being created.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Updates audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy) for entities
    /// being added or modified.
    /// </summary>
    private void UpdateAuditFields()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Player>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(Player.CreatedAt)).CurrentValue = utcNow;
                    if (string.IsNullOrWhiteSpace((string?)entry.Property(nameof(Player.CreatedBy)).CurrentValue))
                    {
                        entry.Property(nameof(Player.CreatedBy)).CurrentValue = _currentUserId;
                    }
                    break;

                case EntityState.Modified:
                    // Set UpdatedAt and UpdatedBy for modified entities
                    entry.Property(nameof(Player.UpdatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(Player.UpdatedBy)).CurrentValue = _currentUserId;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<TeamPlayer>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(TeamPlayer.CreatedAt)).CurrentValue = utcNow;
                    if (string.IsNullOrWhiteSpace((string?)entry.Property(nameof(TeamPlayer.CreatedBy)).CurrentValue))
                    {
                        entry.Property(nameof(TeamPlayer.CreatedBy)).CurrentValue = _currentUserId;
                    }
                    break;

                case EntityState.Modified:
                    // Set UpdatedAt and UpdatedBy for modified entities
                    entry.Property(nameof(TeamPlayer.UpdatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(TeamPlayer.UpdatedBy)).CurrentValue = _currentUserId;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<PlayerStatistic>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(PlayerStatistic.CreatedAt)).CurrentValue = utcNow;
                    if (string.IsNullOrWhiteSpace((string?)entry.Property(nameof(PlayerStatistic.CreatedBy)).CurrentValue))
                    {
                        entry.Property(nameof(PlayerStatistic.CreatedBy)).CurrentValue = _currentUserId;
                    }
                    break;

                case EntityState.Modified:
                    // Set UpdatedAt and UpdatedBy for modified entities
                    entry.Property(nameof(PlayerStatistic.UpdatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(PlayerStatistic.UpdatedBy)).CurrentValue = _currentUserId;
                    break;
            }
        }
    }
}
