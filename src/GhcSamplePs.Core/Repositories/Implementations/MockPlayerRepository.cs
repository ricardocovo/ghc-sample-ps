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
    public Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<Player>>(_players.Values.ToList());
    }

    /// <inheritdoc />
    public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _players.TryGetValue(id, out var player);
        return Task.FromResult(player);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Player>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        cancellationToken.ThrowIfCancellationRequested();

        var players = _players.Values
            .Where(p => string.Equals(p.UserId, userId, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IEnumerable<Player>>(players);
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
    public Task<Player?> UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_players.TryGetValue(player.Id, out var existingPlayer))
        {
            return Task.FromResult<Player?>(null);
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

        if (_players.TryUpdate(player.Id, updatedPlayer, existingPlayer))
        {
            return Task.FromResult<Player?>(updatedPlayer);
        }

        return Task.FromResult<Player?>(null);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_players.TryRemove(id, out _));
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
                Name = "Michael Jordan",
                DateOfBirth = new DateTime(1963, 2, 17),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/mjordan.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-001",
                Name = "LeBron James",
                DateOfBirth = new DateTime(1984, 12, 30),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/ljames.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-002",
                Name = "Serena Williams",
                DateOfBirth = new DateTime(1981, 9, 26),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/swilliams.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-002",
                Name = "Simone Biles",
                DateOfBirth = new DateTime(1997, 3, 14),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/sbiles.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-003",
                Name = "Lionel Messi",
                DateOfBirth = new DateTime(1987, 6, 24),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/lmessi.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-003",
                Name = "Cristiano Ronaldo",
                DateOfBirth = new DateTime(1985, 2, 5),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/cronaldo.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-004",
                Name = "Usain Bolt",
                DateOfBirth = new DateTime(1986, 8, 21),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/ubolt.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-004",
                Name = "Katie Ledecky",
                DateOfBirth = new DateTime(1997, 3, 17),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/kledecky.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-005",
                Name = "Roger Federer",
                DateOfBirth = new DateTime(1981, 8, 8),
                Gender = "Male",
                PhotoUrl = "https://example.com/photos/rfederer.jpg",
                CreatedBy = "system"
            },
            new Player
            {
                Id = GetNextId(),
                UserId = "user-005",
                Name = "Naomi Osaka",
                DateOfBirth = new DateTime(1997, 10, 16),
                Gender = "Female",
                PhotoUrl = "https://example.com/photos/nosaka.jpg",
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
