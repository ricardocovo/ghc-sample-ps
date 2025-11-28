using System.Collections.Concurrent;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Interfaces;

namespace GhcSamplePs.Core.Repositories.Implementations;

/// <summary>
/// In-memory mock implementation of the player repository for development and testing.
/// Uses thread-safe ConcurrentDictionary for data storage.
/// </summary>
/// <remarks>
/// <para>This implementation stores data in memory and will lose all data when the application restarts.</para>
/// <para>Pre-seeds with sample data for testing purposes (10 diverse players).</para>
/// </remarks>
public sealed class MockPlayerRepository : IPlayerRepository
{
    private readonly ConcurrentDictionary<int, Player> _players;
    private int _nextId;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockPlayerRepository"/> class with pre-seeded sample data.
    /// </summary>
    public MockPlayerRepository()
    {
        _players = new ConcurrentDictionary<int, Player>();
        SeedData();
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var players = _players.Values.ToList();
        return Task.FromResult<IReadOnlyList<Player>>(players);
    }

    /// <inheritdoc/>
    public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _players.TryGetValue(id, out var player);
        return Task.FromResult(player);
    }

    /// <inheritdoc/>
    public Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);

        var id = Interlocked.Increment(ref _nextId);
        player.Id = id;

        if (!_players.TryAdd(id, player))
        {
            throw new InvalidOperationException($"Failed to add player with ID {id}.");
        }

        return Task.FromResult(player);
    }

    /// <inheritdoc/>
    public Task<Player> UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (!_players.ContainsKey(player.Id))
        {
            throw new InvalidOperationException($"Player with ID {player.Id} does not exist.");
        }

        _players[player.Id] = player;
        return Task.FromResult(player);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_players.TryRemove(id, out _));
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_players.ContainsKey(id));
    }

    /// <summary>
    /// Seeds the repository with sample player data for testing.
    /// </summary>
    private void SeedData()
    {
        var now = DateTime.UtcNow;
        var samplePlayers = new List<Player>
        {
            new()
            {
                UserId = "system",
                Name = "Emma Rodriguez",
                DateOfBirth = new DateTime(2014, 3, 15),
                Gender = "Female",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-60)
            },
            new()
            {
                UserId = "system",
                Name = "Liam Johnson",
                DateOfBirth = new DateTime(2015, 7, 22),
                Gender = "Male",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-55)
            },
            new()
            {
                UserId = "system",
                Name = "Olivia Martinez",
                DateOfBirth = new DateTime(2013, 11, 8),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/olivia.jpg",
                CreatedBy = "system",
                CreatedAt = now.AddDays(-50)
            },
            new()
            {
                UserId = "system",
                Name = "Noah Williams",
                DateOfBirth = new DateTime(2016, 1, 30),
                Gender = "Male",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-45)
            },
            new()
            {
                UserId = "system",
                Name = "Ava Brown",
                DateOfBirth = new DateTime(2014, 9, 12),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/ava.jpg",
                CreatedBy = "system",
                CreatedAt = now.AddDays(-40)
            },
            new()
            {
                UserId = "system",
                Name = "Ethan Davis",
                DateOfBirth = new DateTime(2015, 4, 5),
                Gender = "Male",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-35)
            },
            new()
            {
                UserId = "system",
                Name = "Sophia Garcia",
                DateOfBirth = new DateTime(2013, 6, 18),
                Gender = "Non-binary",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-30)
            },
            new()
            {
                UserId = "system",
                Name = "Mason Miller",
                DateOfBirth = new DateTime(2016, 12, 25),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/mason.jpg",
                CreatedBy = "system",
                CreatedAt = now.AddDays(-25)
            },
            new()
            {
                UserId = "system",
                Name = "Isabella Wilson",
                DateOfBirth = new DateTime(2014, 2, 14),
                Gender = "Female",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-20)
            },
            new()
            {
                UserId = "system",
                Name = "Lucas Anderson",
                DateOfBirth = new DateTime(2015, 10, 9),
                Gender = "Prefer not to say",
                PhotoUrl = null,
                CreatedBy = "system",
                CreatedAt = now.AddDays(-15)
            }
        };

        foreach (var player in samplePlayers)
        {
            var id = Interlocked.Increment(ref _nextId);
            player.Id = id;
            _players.TryAdd(id, player);
        }
    }
}
