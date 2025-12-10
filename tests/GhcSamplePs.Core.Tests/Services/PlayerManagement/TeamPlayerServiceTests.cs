using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services.PlayerManagement;

public class TeamPlayerServiceTests
{
    private readonly Mock<ITeamPlayerRepository> _mockTeamPlayerRepository;
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly Mock<ILogger<TeamPlayerService>> _mockLogger;
    private readonly TeamPlayerService _service;

    public TeamPlayerServiceTests()
    {
        _mockTeamPlayerRepository = new Mock<ITeamPlayerRepository>();
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockLogger = new Mock<ILogger<TeamPlayerService>>();
        _service = new TeamPlayerService(
            _mockTeamPlayerRepository.Object,
            _mockPlayerRepository.Object,
            _mockLogger.Object);
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when teamPlayerRepository is null")]
    public void Constructor_WhenTeamPlayerRepositoryNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TeamPlayerService(null!, _mockPlayerRepository.Object, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when playerRepository is null")]
    public void Constructor_WhenPlayerRepositoryNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TeamPlayerService(_mockTeamPlayerRepository.Object, null!, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when logger is null")]
    public void Constructor_WhenLoggerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TeamPlayerService(_mockTeamPlayerRepository.Object, _mockPlayerRepository.Object, null!));
    }

    [Fact(DisplayName = "GetTeamsByPlayerIdAsync with teams returns all teams")]
    public async Task GetTeamsByPlayerIdAsync_WhenTeamsExist_ReturnsAllTeams()
    {
        var playerId = 1;
        var teamPlayers = new List<TeamPlayer>
        {
            TestTeamPlayerFactory.CreateCustomTeamPlayer("Team A", "Championship 2024", DateTime.UtcNow.AddMonths(-6), playerId),
            TestTeamPlayerFactory.CreateInactiveTeamPlayer(DateTime.UtcNow.AddMonths(-12), DateTime.UtcNow.AddMonths(-6))
        };
        teamPlayers[0].TeamPlayerId = 1;
        teamPlayers[1].TeamPlayerId = 2;

        _mockTeamPlayerRepository.Setup(r => r.GetAllByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamPlayers);

        var result = await _service.GetTeamsByPlayerIdAsync(playerId, includeInactive: true);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact(DisplayName = "GetTeamsByPlayerIdAsync with no teams returns empty list")]
    public async Task GetTeamsByPlayerIdAsync_WhenNoTeams_ReturnsEmptyList()
    {
        var playerId = 999;
        _mockTeamPlayerRepository.Setup(r => r.GetAllByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamPlayer>());

        var result = await _service.GetTeamsByPlayerIdAsync(playerId, includeInactive: true);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact(DisplayName = "GetActiveTeamsByPlayerIdAsync returns only active teams")]
    public async Task GetActiveTeamsByPlayerIdAsync_WhenCalled_ReturnsOnlyActiveTeams()
    {
        var playerId = 1;
        var activeTeamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        activeTeamPlayer.TeamPlayerId = 1;

        _mockTeamPlayerRepository.Setup(r => r.GetActiveByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamPlayer> { activeTeamPlayer });

        var result = await _service.GetActiveTeamsByPlayerIdAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.True(result.Data.First().IsActive);
    }

    [Fact(DisplayName = "GetActiveTeamsByPlayerIdAsync excludes inactive teams")]
    public async Task GetActiveTeamsByPlayerIdAsync_WhenCalled_ExcludesInactiveTeams()
    {
        var playerId = 1;
        _mockTeamPlayerRepository.Setup(r => r.GetActiveByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamPlayer>());

        var result = await _service.GetActiveTeamsByPlayerIdAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact(DisplayName = "GetTeamAssignmentByIdAsync when exists returns team assignment")]
    public async Task GetTeamAssignmentByIdAsync_WhenExists_ReturnsTeamAssignment()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamPlayer);

        var result = await _service.GetTeamAssignmentByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(teamPlayer.TeamPlayerId, result.Data.TeamPlayerId);
        Assert.Equal(teamPlayer.TeamName, result.Data.TeamName);
    }

    [Fact(DisplayName = "GetTeamAssignmentByIdAsync when not found returns fail result")]
    public async Task GetTeamAssignmentByIdAsync_WhenNotFound_ReturnsFailResult()
    {
        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamPlayer?)null);

        var result = await _service.GetTeamAssignmentByIdAsync(999);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync with valid data creates successfully")]
    public async Task AddPlayerToTeamAsync_WithValidData_CreatesSuccessfully()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "New Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        _mockPlayerRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTeamPlayerRepository.Setup(r => r.HasActiveDuplicateAsync(
                1, "New Team", "Championship 2024", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockTeamPlayerRepository.Setup(r => r.AddAsync(It.IsAny<TeamPlayer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamPlayer tp, CancellationToken _) =>
            {
                tp.TeamPlayerId = 1;
                return tp;
            });

        var result = await _service.AddPlayerToTeamAsync(createDto, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("New Team", result.Data.TeamName);
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync with duplicate active assignment returns validation error")]
    public async Task AddPlayerToTeamAsync_WithDuplicateActive_ReturnsValidationError()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Existing Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        _mockPlayerRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTeamPlayerRepository.Setup(r => r.HasActiveDuplicateAsync(
                1, "Existing Team", "Championship 2024", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.AddPlayerToTeamAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("DuplicateAssignment"));
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync with invalid TeamName returns validation error")]
    public async Task AddPlayerToTeamAsync_WithInvalidTeamName_ReturnsValidationError()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        var result = await _service.AddPlayerToTeamAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("TeamName"));
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync with invalid ChampionshipName returns validation error")]
    public async Task AddPlayerToTeamAsync_WithInvalidChampionshipName_ReturnsValidationError()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Valid Team",
            ChampionshipName = "",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        var result = await _service.AddPlayerToTeamAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("ChampionshipName"));
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync with future JoinedDate returns validation error")]
    public async Task AddPlayerToTeamAsync_WithFutureJoinedDate_ReturnsValidationError()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Valid Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddYears(2) // More than 1 year in future
        };

        var result = await _service.AddPlayerToTeamAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("JoinedDate"));
    }

    [Fact(DisplayName = "UpdateTeamAssignmentAsync with valid data updates successfully")]
    public async Task UpdateTeamAssignmentAsync_WithValidData_UpdatesSuccessfully()
    {
        var existingTeamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = existingTeamPlayer.TeamName,
            ChampionshipName = existingTeamPlayer.ChampionshipName,
            JoinedDate = existingTeamPlayer.JoinedDate,
            LeftDate = DateTime.UtcNow.AddDays(-1)
        };

        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeamPlayer);
        _mockTeamPlayerRepository.Setup(r => r.UpdateAsync(It.IsAny<TeamPlayer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamPlayer tp, CancellationToken _) => tp);

        var result = await _service.UpdateTeamAssignmentAsync(1, updateDto, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.LeftDate);
    }

    [Fact(DisplayName = "UpdateTeamAssignmentAsync with invalid LeftDate returns error")]
    public async Task UpdateTeamAssignmentAsync_WithInvalidLeftDate_ReturnsError()
    {
        var existingTeamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = existingTeamPlayer.TeamName,
            ChampionshipName = existingTeamPlayer.ChampionshipName,
            JoinedDate = existingTeamPlayer.JoinedDate,
            LeftDate = existingTeamPlayer.JoinedDate.AddDays(-1) // Before JoinedDate
        };

        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeamPlayer);

        var result = await _service.UpdateTeamAssignmentAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("LeftDate"));
    }

    [Fact(DisplayName = "UpdateTeamAssignmentAsync when not found returns fail result")]
    public async Task UpdateTeamAssignmentAsync_WhenNotFound_ReturnsFailResult()
    {
        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 999,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = DateTime.UtcNow.AddDays(-30),
            LeftDate = DateTime.UtcNow.AddDays(-1)
        };

        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamPlayer?)null);

        var result = await _service.UpdateTeamAssignmentAsync(999, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "RemovePlayerFromTeamAsync sets LeftDate and marks inactive")]
    public async Task RemovePlayerFromTeamAsync_WhenCalled_SetsLeftDateAndMarksInactive()
    {
        var existingTeamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var leftDate = DateTime.UtcNow.AddDays(-1);

        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeamPlayer);
        _mockTeamPlayerRepository.Setup(r => r.UpdateAsync(It.IsAny<TeamPlayer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamPlayer tp, CancellationToken _) => tp);

        var result = await _service.RemovePlayerFromTeamAsync(1, leftDate, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.LeftDate);
        Assert.False(result.Data.IsActive);
    }

    [Fact(DisplayName = "RemovePlayerFromTeamAsync when not found returns fail result")]
    public async Task RemovePlayerFromTeamAsync_WhenNotFound_ReturnsFailResult()
    {
        _mockTeamPlayerRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamPlayer?)null);

        var result = await _service.RemovePlayerFromTeamAsync(999, DateTime.UtcNow, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "ValidateTeamAssignmentAsync with valid data returns success")]
    public async Task ValidateTeamAssignmentAsync_WithValidData_ReturnsSuccess()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Valid Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        var result = await _service.ValidateTeamAssignmentAsync(createDto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync throws when createDto is null")]
    public async Task AddPlayerToTeamAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.AddPlayerToTeamAsync(null!, "admin-user"));
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync throws when currentUserId is null or empty")]
    public async Task AddPlayerToTeamAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddPlayerToTeamAsync(createDto, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddPlayerToTeamAsync(createDto, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddPlayerToTeamAsync(createDto, "   "));
    }

    [Fact(DisplayName = "UpdateTeamAssignmentAsync throws when updateDto is null")]
    public async Task UpdateTeamAssignmentAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateTeamAssignmentAsync(1, null!, "admin-user"));
    }

    [Fact(DisplayName = "UpdateTeamAssignmentAsync throws when currentUserId is null or empty")]
    public async Task UpdateTeamAssignmentAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = DateTime.UtcNow.AddDays(-30),
            LeftDate = DateTime.UtcNow.AddDays(-1)
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateTeamAssignmentAsync(1, updateDto, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateTeamAssignmentAsync(1, updateDto, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateTeamAssignmentAsync(1, updateDto, "   "));
    }

    [Fact(DisplayName = "RemovePlayerFromTeamAsync throws when currentUserId is null or empty")]
    public async Task RemovePlayerFromTeamAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RemovePlayerFromTeamAsync(1, DateTime.UtcNow, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RemovePlayerFromTeamAsync(1, DateTime.UtcNow, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RemovePlayerFromTeamAsync(1, DateTime.UtcNow, "   "));
    }

    [Fact(DisplayName = "ValidateTeamAssignmentAsync throws when createDto is null")]
    public async Task ValidateTeamAssignmentAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ValidateTeamAssignmentAsync(null!));
    }

    [Fact(DisplayName = "UpdateTeamAssignmentAsync returns failure when ID mismatch")]
    public async Task UpdateTeamAssignmentAsync_WithIdMismatch_ReturnsFailure()
    {
        var updateDto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 2,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = DateTime.UtcNow.AddDays(-30),
            LeftDate = DateTime.UtcNow.AddDays(-1)
        };

        var result = await _service.UpdateTeamAssignmentAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("mismatch", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "AddPlayerToTeamAsync returns failure when player not found")]
    public async Task AddPlayerToTeamAsync_WhenPlayerNotFound_ReturnsFailure()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 999,
            TeamName = "Valid Team",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.AddMonths(-1)
        };

        _mockPlayerRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.AddPlayerToTeamAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }
}
