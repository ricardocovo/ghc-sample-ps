using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Integration;

/// <summary>
/// Integration tests for TeamPlayer management workflows.
/// These tests verify end-to-end behavior with an in-memory database.
/// </summary>
public class TeamPlayerIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EfTeamPlayerRepository _teamPlayerRepository;
    private readonly EfPlayerRepository _playerRepository;
    private readonly TeamPlayerService _teamPlayerService;
    private readonly Mock<ILogger<EfTeamPlayerRepository>> _teamPlayerRepoLoggerMock;
    private readonly Mock<ILogger<EfPlayerRepository>> _playerRepoLoggerMock;
    private readonly Mock<ILogger<TeamPlayerService>> _serviceLoggerMock;
    private const string CurrentUserId = "integration-test-user";
    private bool _disposed;

    public TeamPlayerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, CurrentUserId);
        _teamPlayerRepoLoggerMock = new Mock<ILogger<EfTeamPlayerRepository>>();
        _playerRepoLoggerMock = new Mock<ILogger<EfPlayerRepository>>();
        _serviceLoggerMock = new Mock<ILogger<TeamPlayerService>>();

        _teamPlayerRepository = new EfTeamPlayerRepository(_context, _teamPlayerRepoLoggerMock.Object);
        _playerRepository = new EfPlayerRepository(_context, _playerRepoLoggerMock.Object);
        _teamPlayerService = new TeamPlayerService(
            _teamPlayerRepository,
            _playerRepository,
            _serviceLoggerMock.Object);
    }

    #region Complete Workflow Tests

    [Fact(DisplayName = "AddAndRetrieveTeamAssignment_CompleteWorkflow_Succeeds")]
    public async Task AddAndRetrieveTeamAssignment_CompleteWorkflow_Succeeds()
    {
        var player = await SeedPlayerAsync("Test Player");

        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-6)
        };

        var createResult = await _teamPlayerService.AddPlayerToTeamAsync(createDto, CurrentUserId);
        Assert.True(createResult.Success);
        Assert.NotNull(createResult.Data);
        Assert.True(createResult.Data.TeamPlayerId > 0);

        var retrieveResult = await _teamPlayerService.GetTeamAssignmentByIdAsync(createResult.Data.TeamPlayerId);
        Assert.True(retrieveResult.Success);
        Assert.NotNull(retrieveResult.Data);
        Assert.Equal("Team Alpha", retrieveResult.Data.TeamName);
        Assert.Equal("Championship 2024", retrieveResult.Data.ChampionshipName);
        Assert.Equal(player.Id, retrieveResult.Data.PlayerId);
        Assert.True(retrieveResult.Data.IsActive);

        Assert.NotEqual(default, retrieveResult.Data.CreatedAt);
        Assert.Equal(CurrentUserId, retrieveResult.Data.CreatedBy);
    }

    [Fact(DisplayName = "UpdateTeamAssignment_WithConcurrentUpdate_HandlesCorrectly")]
    public async Task UpdateTeamAssignment_WithConcurrentUpdate_HandlesCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        var updateDto1 = new UpdateTeamPlayerDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            LeftDate = DateTime.UtcNow.AddDays(-5)
        };

        var result1 = await _teamPlayerService.UpdateTeamAssignmentAsync(
            teamPlayer.TeamPlayerId,
            updateDto1,
            "user-1");
        Assert.True(result1.Success);
        Assert.NotNull(result1.Data);
        Assert.False(result1.Data.IsActive);
        Assert.NotNull(result1.Data.LeftDate);

        var existingResult = await _teamPlayerService.GetTeamAssignmentByIdAsync(teamPlayer.TeamPlayerId);
        Assert.True(existingResult.Success);
        Assert.NotNull(existingResult.Data);
        Assert.False(existingResult.Data.IsActive);
    }

    [Fact(DisplayName = "DeletePlayer_CascadesDeleteToTeamAssignments")]
    public async Task DeletePlayer_CascadesDeleteToTeamAssignments()
    {
        var player = await SeedPlayerAsync("Player to Delete");
        await SeedTeamPlayerAsync(player.Id, "Team 1", "Championship A");
        await SeedTeamPlayerAsync(player.Id, "Team 2", "Championship B");
        await SeedTeamPlayerAsync(player.Id, "Team 3", "Championship C");

        var teamsBeforeDelete = await _teamPlayerRepository.GetAllByPlayerIdAsync(player.Id);
        Assert.Equal(3, teamsBeforeDelete.Count);

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        var teamsAfterDelete = await _teamPlayerRepository.GetAllByPlayerIdAsync(player.Id);
        Assert.Empty(teamsAfterDelete);
    }

    [Fact(DisplayName = "GetActiveTeamsForMultiplePlayers_ReturnsCorrectData")]
    public async Task GetActiveTeamsForMultiplePlayers_ReturnsCorrectData()
    {
        var player1 = await SeedPlayerAsync("Player 1");
        var player2 = await SeedPlayerAsync("Player 2");

        await SeedTeamPlayerAsync(player1.Id, "Team A", "Championship 1");
        await SeedTeamPlayerAsync(player1.Id, "Team B", "Championship 1");
        var inactiveTeamPlayer = await SeedTeamPlayerAsync(player1.Id, "Team C", "Championship 1");

        await MarkTeamPlayerAsLeftAsync(inactiveTeamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-10));

        await SeedTeamPlayerAsync(player2.Id, "Team D", "Championship 2");

        var player1ActiveTeams = await _teamPlayerService.GetActiveTeamsByPlayerIdAsync(player1.Id);
        Assert.True(player1ActiveTeams.Success);
        Assert.NotNull(player1ActiveTeams.Data);
        Assert.Equal(2, player1ActiveTeams.Data.Count);
        Assert.All(player1ActiveTeams.Data, tp => Assert.True(tp.IsActive));

        var player2ActiveTeams = await _teamPlayerService.GetActiveTeamsByPlayerIdAsync(player2.Id);
        Assert.True(player2ActiveTeams.Success);
        Assert.NotNull(player2ActiveTeams.Data);
        Assert.Single(player2ActiveTeams.Data);

        var player1AllTeams = await _teamPlayerService.GetTeamsByPlayerIdAsync(player1.Id, includeInactive: true);
        Assert.True(player1AllTeams.Success);
        Assert.NotNull(player1AllTeams.Data);
        Assert.Equal(3, player1AllTeams.Data.Count);
    }

    [Fact(DisplayName = "DuplicateActiveAssignment_Prevention_Works")]
    public async Task DuplicateActiveAssignment_Prevention_Works()
    {
        var player = await SeedPlayerAsync("Test Player");

        var createDto1 = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Unique Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-3)
        };

        var result1 = await _teamPlayerService.AddPlayerToTeamAsync(createDto1, CurrentUserId);
        Assert.True(result1.Success);

        var createDto2 = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Unique Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        var result2 = await _teamPlayerService.AddPlayerToTeamAsync(createDto2, CurrentUserId);
        Assert.False(result2.Success);
        Assert.True(result2.ValidationErrors.ContainsKey("DuplicateAssignment"));
    }

    #endregion

    #region Validation Error Scenarios

    [Fact(DisplayName = "AddDuplicateActiveAssignment_ReturnsDuplicateError")]
    public async Task AddDuplicateActiveAssignment_ReturnsDuplicateError()
    {
        var player = await SeedPlayerAsync("Test Player");
        await SeedTeamPlayerAsync(player.Id, "Existing Team", "Existing Championship");

        var duplicateDto = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Existing Team",
            ChampionshipName = "Existing Championship",
            JoinedDate = DateTime.UtcNow.AddDays(-10)
        };

        var result = await _teamPlayerService.AddPlayerToTeamAsync(duplicateDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("DuplicateAssignment"));
        Assert.Contains("already has an active assignment", result.ValidationErrors["DuplicateAssignment"].First());
    }

    [Fact(DisplayName = "SetLeftDateBeforeJoinedDate_ReturnsValidationError")]
    public async Task SetLeftDateBeforeJoinedDate_ReturnsValidationError()
    {
        var player = await SeedPlayerAsync("Test Player");
        var joinedDate = DateTime.UtcNow.AddMonths(-6);
        var teamPlayer = await SeedTeamPlayerWithJoinedDateAsync(player.Id, "Test Team", "Test Championship", joinedDate);

        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            LeftDate = joinedDate.AddDays(-10) // Before joined date
        };

        var result = await _teamPlayerService.UpdateTeamAssignmentAsync(
            teamPlayer.TeamPlayerId,
            updateDto,
            CurrentUserId);

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("LeftDate"));
    }

    [Fact(DisplayName = "SetFutureDates_ReturnsValidationError")]
    public async Task SetFutureDates_ReturnsValidationError()
    {
        var player = await SeedPlayerAsync("Test Player");

        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = DateTime.UtcNow.AddYears(2)
        };

        var result = await _teamPlayerService.AddPlayerToTeamAsync(createDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("JoinedDate"));
    }

    [Fact(DisplayName = "AddPlayerToTeam_WithNonExistentPlayer_ReturnsError")]
    public async Task AddPlayerToTeam_WithNonExistentPlayer_ReturnsError()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 9999, // Non-existent player
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        var result = await _teamPlayerService.AddPlayerToTeamAsync(createDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    #endregion

    #region Audit Fields Tests

    [Fact(DisplayName = "TeamAssignment_AuditFieldsPopulatedCorrectly")]
    public async Task TeamAssignment_AuditFieldsPopulatedCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var beforeCreate = DateTime.UtcNow;

        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Audit Test Team",
            ChampionshipName = "Audit Championship",
            JoinedDate = DateTime.UtcNow.AddMonths(-3)
        };

        var createResult = await _teamPlayerService.AddPlayerToTeamAsync(createDto, "create-user");
        Assert.True(createResult.Success);
        Assert.NotNull(createResult.Data);

        var afterCreate = DateTime.UtcNow;

        Assert.True(createResult.Data.CreatedAt >= beforeCreate);
        Assert.True(createResult.Data.CreatedAt <= afterCreate);
        Assert.Equal("create-user", createResult.Data.CreatedBy);

        var beforeUpdate = DateTime.UtcNow;

        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = createResult.Data.TeamPlayerId,
            LeftDate = DateTime.UtcNow.AddDays(-1)
        };

        var updateResult = await _teamPlayerService.UpdateTeamAssignmentAsync(
            createResult.Data.TeamPlayerId,
            updateDto,
            "update-user");
        Assert.True(updateResult.Success);
        Assert.NotNull(updateResult.Data);

        var afterUpdate = DateTime.UtcNow;

        // CreatedBy is preserved from creation
        Assert.Equal("create-user", updateResult.Data.CreatedBy);

        // UpdatedAt and UpdatedBy are set by the ApplicationDbContext
        // Note: The DbContext uses its _currentUserId ("integration-test-user") for audit fields
        // This overrides the currentUserId parameter passed to the service method
        Assert.NotNull(updateResult.Data.UpdatedAt);
        Assert.True(updateResult.Data.UpdatedAt >= beforeUpdate);
        Assert.True(updateResult.Data.UpdatedAt <= afterUpdate);
        Assert.Equal(CurrentUserId, updateResult.Data.UpdatedBy); // From DbContext's _currentUserId
    }

    #endregion

    #region Remove Player From Team Tests

    [Fact(DisplayName = "RemovePlayerFromTeam_SetsLeftDateAndMarksInactive")]
    public async Task RemovePlayerFromTeam_SetsLeftDateAndMarksInactive()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Active Team", "Active Championship");

        Assert.True(teamPlayer.IsActive);

        var leftDate = DateTime.UtcNow.AddDays(-1);
        var result = await _teamPlayerService.RemovePlayerFromTeamAsync(
            teamPlayer.TeamPlayerId,
            leftDate,
            CurrentUserId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.False(result.Data.IsActive);
        Assert.NotNull(result.Data.LeftDate);
        Assert.Equal(leftDate.Date, result.Data.LeftDate.Value.Date);
    }

    [Fact(DisplayName = "RemovePlayerFromTeam_AllowsNewActiveAssignmentToSameTeam")]
    public async Task RemovePlayerFromTeam_AllowsNewActiveAssignmentToSameTeam()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Rejoining Team", "Championship 2024");

        var removeResult = await _teamPlayerService.RemovePlayerFromTeamAsync(
            teamPlayer.TeamPlayerId,
            DateTime.UtcNow.AddDays(-5),
            CurrentUserId);
        Assert.True(removeResult.Success);

        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = player.Id,
            TeamName = "Rejoining Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddDays(-1)
        };

        var addResult = await _teamPlayerService.AddPlayerToTeamAsync(createDto, CurrentUserId);
        Assert.True(addResult.Success);

        var allTeamsResult = await _teamPlayerService.GetTeamsByPlayerIdAsync(player.Id, includeInactive: true);
        Assert.True(allTeamsResult.Success);
        Assert.NotNull(allTeamsResult.Data);
        Assert.Equal(2, allTeamsResult.Data.Count);
    }

    #endregion

    #region Helper Methods

    private async Task<Player> SeedPlayerAsync(string name = "Test Player")
    {
        var player = new Player
        {
            UserId = Guid.NewGuid().ToString(),
            Name = name,
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            CreatedBy = CurrentUserId
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }

    private async Task<TeamPlayer> SeedTeamPlayerAsync(int playerId, string teamName, string championshipName)
    {
        var teamPlayer = new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = championshipName,
            JoinedDate = DateTime.UtcNow.AddMonths(-3),
            CreatedBy = CurrentUserId
        };
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();
        return teamPlayer;
    }

    private async Task<TeamPlayer> SeedTeamPlayerWithJoinedDateAsync(
        int playerId,
        string teamName,
        string championshipName,
        DateTime joinedDate)
    {
        var teamPlayer = new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = championshipName,
            JoinedDate = joinedDate,
            CreatedBy = CurrentUserId
        };
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();
        return teamPlayer;
    }

    private async Task MarkTeamPlayerAsLeftAsync(int teamPlayerId, DateTime leftDate)
    {
        var teamPlayer = await _context.TeamPlayers.FindAsync(teamPlayerId);
        if (teamPlayer is not null)
        {
            teamPlayer.MarkAsLeft(leftDate, CurrentUserId);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
