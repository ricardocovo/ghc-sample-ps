using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Exceptions;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Repositories.PlayerManagement;

public class EfTeamPlayerRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<EfTeamPlayerRepository>> _loggerMock;
    private readonly EfTeamPlayerRepository _repository;
    private const string CurrentUserId = "test-user-id";

    public EfTeamPlayerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, CurrentUserId);
        _loggerMock = new Mock<ILogger<EfTeamPlayerRepository>>();
        _repository = new EfTeamPlayerRepository(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Constructor Tests

    [Fact(DisplayName = "Constructor throws when context is null")]
    public void Constructor_WhenContextIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EfTeamPlayerRepository(null!, _loggerMock.Object));
    }

    [Fact(DisplayName = "Constructor throws when logger is null")]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EfTeamPlayerRepository(_context, null!));
    }

    #endregion

    #region GetAllByPlayerIdAsync Tests

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns empty list when no assignments exist")]
    public async Task GetAllByPlayerIdAsync_WhenNoAssignmentsExist_ReturnsEmptyList()
    {
        var player = await SeedPlayerAsync();

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns all assignments for player")]
    public async Task GetAllByPlayerIdAsync_WhenAssignmentsExist_ReturnsAllAssignments()
    {
        var player = await SeedPlayerAsync();
        await SeedTeamPlayersAsync(player.Id, 3);

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.Equal(3, result.Count);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns assignments ordered by JoinedDate DESC")]
    public async Task GetAllByPlayerIdAsync_WhenAssignmentsExist_ReturnsOrderedByJoinedDateDesc()
    {
        var player = await SeedPlayerAsync();
        _context.TeamPlayers.AddRange(
            CreateTeamPlayer(player.Id, "Team A", new DateTime(2024, 1, 15)),
            CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 6, 15)),
            CreateTeamPlayer(player.Id, "Team C", new DateTime(2024, 3, 15)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.Equal("Team B", result[0].TeamName);
        Assert.Equal("Team C", result[1].TeamName);
        Assert.Equal("Team A", result[2].TeamName);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync includes both active and inactive assignments")]
    public async Task GetAllByPlayerIdAsync_WhenMixedAssignments_ReturnsAll()
    {
        var player = await SeedPlayerAsync();
        var activeTeamPlayer = CreateTeamPlayer(player.Id, "Active Team", new DateTime(2024, 1, 15));
        var inactiveTeamPlayer = CreateTeamPlayer(player.Id, "Inactive Team", new DateTime(2024, 2, 15));
        _context.TeamPlayers.AddRange(activeTeamPlayer, inactiveTeamPlayer);
        await _context.SaveChangesAsync();

        // Mark one as inactive
        var toMarkInactive = await _context.TeamPlayers.FirstAsync(tp => tp.TeamName == "Inactive Team");
        toMarkInactive.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, tp => tp.IsActive);
        Assert.Contains(result, tp => !tp.IsActive);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync respects cancellation token")]
    public async Task GetAllByPlayerIdAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = await SeedPlayerAsync();
        await SeedTeamPlayersAsync(player.Id, 5);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetAllByPlayerIdAsync(player.Id, cts.Token));
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync does not return assignments for other players")]
    public async Task GetAllByPlayerIdAsync_WhenMultiplePlayers_ReturnsOnlyRequestedPlayerAssignments()
    {
        var player1 = await SeedPlayerAsync("Player 1");
        var player2 = await SeedPlayerAsync("Player 2");
        await SeedTeamPlayersAsync(player1.Id, 2);
        await SeedTeamPlayersAsync(player2.Id, 3);

        var result = await _repository.GetAllByPlayerIdAsync(player1.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, tp => Assert.Equal(player1.Id, tp.PlayerId));
    }

    #endregion

    #region GetActiveByPlayerIdAsync Tests

    [Fact(DisplayName = "GetActiveByPlayerIdAsync returns only active assignments")]
    public async Task GetActiveByPlayerIdAsync_WhenMixedAssignments_ReturnsOnlyActive()
    {
        var player = await SeedPlayerAsync();
        var activeTeamPlayer = CreateTeamPlayer(player.Id, "Active Team", new DateTime(2024, 1, 15));
        var inactiveTeamPlayer = CreateTeamPlayer(player.Id, "Inactive Team", new DateTime(2024, 2, 15));
        _context.TeamPlayers.AddRange(activeTeamPlayer, inactiveTeamPlayer);
        await _context.SaveChangesAsync();

        // Mark one as inactive
        var toMarkInactive = await _context.TeamPlayers.FirstAsync(tp => tp.TeamName == "Inactive Team");
        toMarkInactive.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");
        await _context.SaveChangesAsync();

        var result = await _repository.GetActiveByPlayerIdAsync(player.Id);

        Assert.Single(result);
        Assert.Equal("Active Team", result[0].TeamName);
        Assert.True(result[0].IsActive);
    }

    [Fact(DisplayName = "GetActiveByPlayerIdAsync returns empty list when all inactive")]
    public async Task GetActiveByPlayerIdAsync_WhenAllInactive_ReturnsEmptyList()
    {
        var player = await SeedPlayerAsync();
        var inactiveTeamPlayer = CreateTeamPlayer(player.Id, "Inactive Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(inactiveTeamPlayer);
        await _context.SaveChangesAsync();

        // Mark as inactive
        var toMarkInactive = await _context.TeamPlayers.FirstAsync();
        toMarkInactive.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");
        await _context.SaveChangesAsync();

        var result = await _repository.GetActiveByPlayerIdAsync(player.Id);

        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetActiveByPlayerIdAsync returns assignments ordered by JoinedDate DESC")]
    public async Task GetActiveByPlayerIdAsync_WhenAssignmentsExist_ReturnsOrderedByJoinedDateDesc()
    {
        var player = await SeedPlayerAsync();
        _context.TeamPlayers.AddRange(
            CreateTeamPlayer(player.Id, "Team A", new DateTime(2024, 1, 15)),
            CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 6, 15)),
            CreateTeamPlayer(player.Id, "Team C", new DateTime(2024, 3, 15)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetActiveByPlayerIdAsync(player.Id);

        Assert.Equal("Team B", result[0].TeamName);
        Assert.Equal("Team C", result[1].TeamName);
        Assert.Equal("Team A", result[2].TeamName);
    }

    [Fact(DisplayName = "GetActiveByPlayerIdAsync respects cancellation token")]
    public async Task GetActiveByPlayerIdAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = await SeedPlayerAsync();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetActiveByPlayerIdAsync(player.Id, cts.Token));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact(DisplayName = "GetByIdAsync returns assignment when it exists")]
    public async Task GetByIdAsync_WhenAssignmentExists_ReturnsAssignment()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(teamPlayer.TeamPlayerId);

        Assert.NotNull(result);
        Assert.Equal(teamPlayer.TeamPlayerId, result.TeamPlayerId);
        Assert.Equal("Test Team", result.TeamName);
    }

    [Fact(DisplayName = "GetByIdAsync returns null when assignment does not exist")]
    public async Task GetByIdAsync_WhenAssignmentDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact(DisplayName = "GetByIdAsync respects cancellation token")]
    public async Task GetByIdAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetByIdAsync(1, cts.Token));
    }

    #endregion

    #region AddAsync Tests

    [Fact(DisplayName = "AddAsync adds assignment with generated ID")]
    public async Task AddAsync_WhenValidAssignment_AddsWithGeneratedId()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "New Team", new DateTime(2024, 1, 15));

        var result = await _repository.AddAsync(teamPlayer);

        Assert.NotNull(result);
        Assert.True(result.TeamPlayerId > 0);
        Assert.Equal("New Team", result.TeamName);
    }

    [Fact(DisplayName = "AddAsync persists assignment to database")]
    public async Task AddAsync_WhenValidAssignment_PersistsToDatabase()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Persisted Team", new DateTime(2024, 1, 15));

        await _repository.AddAsync(teamPlayer);

        var savedTeamPlayer = await _context.TeamPlayers.FirstOrDefaultAsync(tp => tp.TeamName == "Persisted Team");
        Assert.NotNull(savedTeamPlayer);
        Assert.Equal(teamPlayer.TeamPlayerId, savedTeamPlayer.TeamPlayerId);
    }

    [Fact(DisplayName = "AddAsync sets CreatedAt automatically")]
    public async Task AddAsync_WhenValidAssignment_SetsCreatedAtAutomatically()
    {
        var player = await SeedPlayerAsync();
        var beforeAdd = DateTime.UtcNow;
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));

        var result = await _repository.AddAsync(teamPlayer);

        var afterAdd = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= beforeAdd);
        Assert.True(result.CreatedAt <= afterAdd);
    }

    [Fact(DisplayName = "AddAsync throws when assignment is null")]
    public async Task AddAsync_WhenAssignmentIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.AddAsync(null!));
    }

    [Fact(DisplayName = "AddAsync respects cancellation token")]
    public async Task AddAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.AddAsync(teamPlayer, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "AddAsync increments ID for each new assignment")]
    public async Task AddAsync_WhenMultipleAssignments_IncrementsId()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer1 = CreateTeamPlayer(player.Id, "Team 1", new DateTime(2024, 1, 15));
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Team 2", new DateTime(2024, 2, 15));

        var result1 = await _repository.AddAsync(teamPlayer1);
        var result2 = await _repository.AddAsync(teamPlayer2);

        Assert.True(result2.TeamPlayerId > result1.TeamPlayerId);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact(DisplayName = "UpdateAsync updates existing assignment")]
    public async Task UpdateAsync_WhenAssignmentExists_UpdatesAssignment()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Original Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        // Mark as left using the entity method
        var toUpdate = await _context.TeamPlayers.FindAsync(teamPlayer.TeamPlayerId);
        toUpdate!.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");

        var result = await _repository.UpdateAsync(toUpdate);

        Assert.NotNull(result);
        Assert.False(result.IsActive);
        Assert.Equal(new DateTime(2024, 6, 15), result.LeftDate);
    }

    [Fact(DisplayName = "UpdateAsync sets UpdatedAt automatically")]
    public async Task UpdateAsync_WhenAssignmentExists_SetsUpdatedAtAutomatically()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var toUpdate = await _context.TeamPlayers.FindAsync(teamPlayer.TeamPlayerId);
        toUpdate!.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");

        var beforeUpdate = DateTime.UtcNow;
        var result = await _repository.UpdateAsync(toUpdate);
        var afterUpdate = DateTime.UtcNow;

        Assert.NotNull(result.UpdatedAt);
        Assert.True(result.UpdatedAt >= beforeUpdate);
        Assert.True(result.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "UpdateAsync throws RepositoryException when assignment does not exist")]
    public async Task UpdateAsync_WhenAssignmentDoesNotExist_ThrowsRepositoryException()
    {
        var teamPlayer = new TeamPlayer
        {
            TeamPlayerId = 999,
            PlayerId = 1,
            TeamName = "Non-existent",
            ChampionshipName = "Test",
            JoinedDate = new DateTime(2024, 1, 1),
            CreatedBy = "test"
        };

        await Assert.ThrowsAsync<RepositoryException>(() =>
            _repository.UpdateAsync(teamPlayer));
    }

    [Fact(DisplayName = "UpdateAsync throws when assignment is null")]
    public async Task UpdateAsync_WhenAssignmentIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.UpdateAsync(null!));
    }

    [Fact(DisplayName = "UpdateAsync respects cancellation token")]
    public async Task UpdateAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var toUpdate = await _context.TeamPlayers.FindAsync(teamPlayer.TeamPlayerId);
        toUpdate!.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.UpdateAsync(toUpdate, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "UpdateAsync preserves original CreatedAt and CreatedBy")]
    public async Task UpdateAsync_WhenAssignmentExists_PreservesCreatedAtAndCreatedBy()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var originalCreatedAt = teamPlayer.CreatedAt;
        var originalCreatedBy = teamPlayer.CreatedBy;

        var toUpdate = await _context.TeamPlayers.FindAsync(teamPlayer.TeamPlayerId);
        toUpdate!.MarkAsLeft(new DateTime(2024, 6, 15), "different-user");

        var result = await _repository.UpdateAsync(toUpdate);

        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.Equal(originalCreatedBy, result.CreatedBy);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact(DisplayName = "DeleteAsync returns true when assignment exists")]
    public async Task DeleteAsync_WhenAssignmentExists_ReturnsTrue()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(teamPlayer.TeamPlayerId);

        Assert.True(result);
    }

    [Fact(DisplayName = "DeleteAsync removes assignment from database")]
    public async Task DeleteAsync_WhenAssignmentExists_RemovesFromDatabase()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();
        var teamPlayerId = teamPlayer.TeamPlayerId;

        await _repository.DeleteAsync(teamPlayerId);

        var deletedTeamPlayer = await _context.TeamPlayers.FindAsync(teamPlayerId);
        Assert.Null(deletedTeamPlayer);
    }

    [Fact(DisplayName = "DeleteAsync returns false when assignment does not exist")]
    public async Task DeleteAsync_WhenAssignmentDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "DeleteAsync respects cancellation token")]
    public async Task DeleteAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.DeleteAsync(teamPlayer.TeamPlayerId, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "DeleteAsync reduces assignment count")]
    public async Task DeleteAsync_WhenAssignmentExists_ReducesCount()
    {
        var player = await SeedPlayerAsync();
        await SeedTeamPlayersAsync(player.Id, 3);
        var teamPlayer = await _context.TeamPlayers.FirstAsync();

        await _repository.DeleteAsync(teamPlayer.TeamPlayerId);

        var remainingCount = await _context.TeamPlayers.CountAsync();
        Assert.Equal(2, remainingCount);
    }

    #endregion

    #region ExistsAsync Tests

    [Fact(DisplayName = "ExistsAsync returns true when assignment exists")]
    public async Task ExistsAsync_WhenAssignmentExists_ReturnsTrue()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(teamPlayer.TeamPlayerId);

        Assert.True(result);
    }

    [Fact(DisplayName = "ExistsAsync returns false when assignment does not exist")]
    public async Task ExistsAsync_WhenAssignmentDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "ExistsAsync respects cancellation token")]
    public async Task ExistsAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.ExistsAsync(1, cts.Token));
    }

    #endregion

    #region HasActiveDuplicateAsync Tests

    [Fact(DisplayName = "HasActiveDuplicateAsync returns true when active duplicate exists")]
    public async Task HasActiveDuplicateAsync_WhenActiveDuplicateExists_ReturnsTrue()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.HasActiveDuplicateAsync(player.Id, "Test Team", "Test Championship");

        Assert.True(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync returns false when no duplicate exists")]
    public async Task HasActiveDuplicateAsync_WhenNoDuplicateExists_ReturnsFalse()
    {
        var player = await SeedPlayerAsync();

        var result = await _repository.HasActiveDuplicateAsync(player.Id, "Non-existent Team", "Test Championship");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync ignores inactive duplicates")]
    public async Task HasActiveDuplicateAsync_WhenInactiveDuplicateExists_ReturnsFalse()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        // Mark as inactive
        var toMarkInactive = await _context.TeamPlayers.FirstAsync();
        toMarkInactive.MarkAsLeft(new DateTime(2024, 6, 15), "test-user");
        await _context.SaveChangesAsync();

        var result = await _repository.HasActiveDuplicateAsync(player.Id, "Test Team", "Test Championship");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync excludes specified ID")]
    public async Task HasActiveDuplicateAsync_WhenExcludeIdProvided_ExcludesFromCheck()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.HasActiveDuplicateAsync(
            player.Id, "Test Team", "Test Championship", teamPlayer.TeamPlayerId);

        Assert.False(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync returns true when duplicate exists and excludeId is different")]
    public async Task HasActiveDuplicateAsync_WhenDuplicateExistsAndExcludeIdDifferent_ReturnsTrue()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer1 = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Different Team", new DateTime(2024, 2, 15));
        _context.TeamPlayers.AddRange(teamPlayer1, teamPlayer2);
        await _context.SaveChangesAsync();

        var result = await _repository.HasActiveDuplicateAsync(
            player.Id, "Test Team", "Test Championship", teamPlayer2.TeamPlayerId);

        Assert.True(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync throws when teamName is null")]
    public async Task HasActiveDuplicateAsync_WhenTeamNameIsNull_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.HasActiveDuplicateAsync(1, null!, "Test Championship"));
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync throws when teamName is whitespace")]
    public async Task HasActiveDuplicateAsync_WhenTeamNameIsWhitespace_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.HasActiveDuplicateAsync(1, "   ", "Test Championship"));
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync throws when championshipName is null")]
    public async Task HasActiveDuplicateAsync_WhenChampionshipNameIsNull_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.HasActiveDuplicateAsync(1, "Test Team", null!));
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync throws when championshipName is whitespace")]
    public async Task HasActiveDuplicateAsync_WhenChampionshipNameIsWhitespace_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.HasActiveDuplicateAsync(1, "Test Team", "   "));
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync respects cancellation token")]
    public async Task HasActiveDuplicateAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.HasActiveDuplicateAsync(1, "Test Team", "Test Championship", null, cts.Token));
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync is case-insensitive for team name (matches SQL Server default)")]
    public async Task HasActiveDuplicateAsync_WhenTeamNameDifferentCase_ReturnsTrue()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        // Note: SQL Server's default collation is case-insensitive, so "Test Team" and "TEST TEAM" are considered duplicates.
        // The in-memory provider is case-sensitive by default, so this test may fail if run only with in-memory.
        // This test asserts the expected production (SQL Server) behavior: duplicate is found regardless of case.
        var result = await _repository.HasActiveDuplicateAsync(player.Id, "TEST TEAM", "Test Championship");

        Assert.True(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync differentiates by championship name")]
    public async Task HasActiveDuplicateAsync_WhenDifferentChampionship_ReturnsFalse()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.HasActiveDuplicateAsync(player.Id, "Test Team", "Different Championship");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasActiveDuplicateAsync differentiates by player ID")]
    public async Task HasActiveDuplicateAsync_WhenDifferentPlayer_ReturnsFalse()
    {
        var player1 = await SeedPlayerAsync("Player 1");
        var player2 = await SeedPlayerAsync("Player 2");
        var teamPlayer = CreateTeamPlayer(player1.Id, "Test Team", new DateTime(2024, 1, 15));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        var result = await _repository.HasActiveDuplicateAsync(player2.Id, "Test Team", "Test Championship");

        Assert.False(result);
    }

    #endregion

    #region Logging Tests

    [Fact(DisplayName = "GetAllByPlayerIdAsync logs retrieval operation")]
    public async Task GetAllByPlayerIdAsync_WhenCalled_LogsOperation()
    {
        var player = await SeedPlayerAsync();
        await SeedTeamPlayersAsync(player.Id, 3);

        await _repository.GetAllByPlayerIdAsync(player.Id);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved 3 team player assignments")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "AddAsync logs successful addition")]
    public async Task AddAsync_WhenSuccessful_LogsOperation()
    {
        var player = await SeedPlayerAsync();
        var teamPlayer = CreateTeamPlayer(player.Id, "Test Team", new DateTime(2024, 1, 15));

        await _repository.AddAsync(teamPlayer);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully added team player assignment")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static TeamPlayer CreateTeamPlayer(int playerId, string teamName, DateTime joinedDate)
    {
        return new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = "Test Championship",
            JoinedDate = joinedDate,
            CreatedBy = "test-creator"
        };
    }

    private async Task<Player> SeedPlayerAsync(string name = "Test Player")
    {
        var player = new Player
        {
            UserId = "test-user",
            Name = name,
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            CreatedBy = "test-creator"
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }

    private async Task SeedTeamPlayersAsync(int playerId, int count)
    {
        for (var i = 1; i <= count; i++)
        {
            _context.TeamPlayers.Add(CreateTeamPlayer(playerId, $"Team {i}", DateTime.UtcNow.AddDays(-i)));
        }

        await _context.SaveChangesAsync();
    }

    #endregion
}
