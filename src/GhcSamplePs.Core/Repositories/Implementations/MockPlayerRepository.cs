using System.Collections.Concurrent;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Interfaces;

namespace GhcSamplePs.Core.Repositories.Implementations;

/// <summary>
/// In-memory mock implementation of IPlayerRepository for development and testing.
/// Uses ConcurrentDictionary for thread-safe operations.
/// </summary>
public sealed class MockPlayerRepository : IPlayerRepository
{
    private readonly ConcurrentDictionary<int, Player> _players = new();
    private int _nextId = 1;
    private readonly object _idLock = new();

    /// <summary>
    /// Initializes a new instance of the MockPlayerRepository class with seed data.
    /// </summary>
    public MockPlayerRepository()
    {
        SeedData();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Player>>(_players.Values.ToList());
    }

    /// <inheritdoc />
    public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _players.TryGetValue(id, out var player);
        return Task.FromResult(player);
    }

    /// <inheritdoc />
    public Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);
        cancellationToken.ThrowIfCancellationRequested();

        var newPlayer = new Player
        {
            Id = GetNextId(),
            UserId = player.UserId,
            Name = player.Name,
            DateOfBirth = player.DateOfBirth,
            Gender = player.Gender,
            PhotoUrl = player.PhotoUrl,
            CreatedAt = player.CreatedAt,
            CreatedBy = player.CreatedBy
        };

        _players.TryAdd(newPlayer.Id, newPlayer);
        return Task.FromResult(newPlayer);
    }

    /// <inheritdoc />
    public Task<Player> UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_players.TryGetValue(player.Id, out var existingPlayer))
        {
            throw new InvalidOperationException($"Player with ID {player.Id} not found.");
        }

        var updatedPlayer = new Player
        {
            Id = player.Id,
            UserId = player.UserId,
            Name = player.Name,
            DateOfBirth = player.DateOfBirth,
            Gender = player.Gender,
            PhotoUrl = player.PhotoUrl,
            CreatedAt = existingPlayer.CreatedAt,
            CreatedBy = existingPlayer.CreatedBy
        };

        updatedPlayer.UpdateLastModified(player.UpdatedBy ?? "system");

        if (!_players.TryUpdate(player.Id, updatedPlayer, existingPlayer))
        {
            throw new InvalidOperationException($"Failed to update player with ID {player.Id}. Concurrent modification detected.");
        }

        return Task.FromResult(updatedPlayer);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_players.TryRemove(id, out _));
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_players.ContainsKey(id));
    }

    /// <summary>
    /// Seeds the repository with sample player data.
    /// </summary>
    private void SeedData()
    {
        var samplePlayers = new[]
        {
            new Player
            {
                Id = GetNextId(),
                UserId = "user-001",
                Name = "Emma Rodriguez",
                DateOfBirth = new DateTime(2014, 3, 15),
                Gender = "Female",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-001",
                Name = "Liam Johnson",
                DateOfBirth = new DateTime(2015, 7, 22),
                Gender = "Male",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-002",
                Name = "Olivia Martinez",
                DateOfBirth = new DateTime(2013, 11, 8),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/olivia.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-002",
                Name = "Noah Williams",
                DateOfBirth = new DateTime(2016, 1, 30),
                Gender = "Male",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-003",
                Name = "Ava Brown",
                DateOfBirth = new DateTime(2014, 9, 12),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/ava.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-003",
                Name = "Ethan Davis",
                DateOfBirth = new DateTime(2015, 4, 5),
                Gender = "Male",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-004",
                Name = "Sophia Garcia",
                DateOfBirth = new DateTime(2013, 6, 18),
                Gender = "Non-binary",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-004",
                Name = "Mason Miller",
                DateOfBirth = new DateTime(2016, 12, 25),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/mason.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-005",
                Name = "Isabella Wilson",
                DateOfBirth = new DateTime(2014, 2, 14),
                Gender = "Female",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-005",
                Name = "Lucas Anderson",
                DateOfBirth = new DateTime(2015, 10, 9),
                Gender = "Prefer not to say",
                CreatedBy = "system"
            }
        };

        foreach (var player in samplePlayers)
        {
            _players.TryAdd(player.Id, player);
        }
    }

    /// <summary>
    /// Gets the next available ID in a thread-safe manner.
    /// </summary>
    /// <returns>The next available ID.</returns>
    private int GetNextId()
    {
        lock (_idLock)
        {
            return _nextId++;
        }
    }
}
