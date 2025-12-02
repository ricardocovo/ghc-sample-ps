using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Validation;

/// <summary>
/// Provides validation logic for player statistic data, implementing all business rules defined in the specification.
/// </summary>
/// <remarks>
/// <para>Validation Rules:</para>
/// <list type="bullet">
///   <item><description><b>TeamPlayerId:</b> Required, must be greater than 0</description></item>
///   <item><description><b>GameDate:</b> Required, valid date, not in future</description></item>
///   <item><description><b>MinutesPlayed:</b> Required, ≥ 0, ≤ 120 (soft limit)</description></item>
///   <item><description><b>JerseyNumber:</b> Required, 1-99 (reasonable range)</description></item>
///   <item><description><b>Goals:</b> Required, ≥ 0</description></item>
///   <item><description><b>Assists:</b> Required, ≥ 0</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var createDto = new CreatePlayerStatisticDto
/// {
///     TeamPlayerId = 1,
///     GameDate = new DateTime(2024, 3, 15),
///     MinutesPlayed = 90,
///     IsStarter = true,
///     JerseyNumber = 10,
///     Goals = 2,
///     Assists = 1
/// };
/// var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(createDto);
/// if (!result.IsValid)
/// {
///     foreach (var (field, messages) in result.Errors)
///     {
///         Console.WriteLine($"{field}: {string.Join(", ", messages)}");
///     }
/// }
/// </code>
/// </example>
public static class PlayerStatisticValidator
{
    /// <summary>
    /// Maximum allowed minutes played in a game (soft limit).
    /// </summary>
    public const int MaxMinutesPlayed = 120;

    /// <summary>
    /// Minimum allowed jersey number.
    /// </summary>
    public const int MinJerseyNumber = 1;

    /// <summary>
    /// Maximum allowed jersey number (reasonable range).
    /// </summary>
    public const int MaxJerseyNumber = 99;

    /// <summary>
    /// Validates a CreatePlayerStatisticDto against all business rules.
    /// </summary>
    /// <param name="dto">The CreatePlayerStatisticDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerStatisticDto
    /// {
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 0,
    ///     Assists = 0
    /// };
    /// var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(createDto);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateCreatePlayerStatistic(CreatePlayerStatisticDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateTeamPlayerId(dto.TeamPlayerId, errors);
        ValidateGameDate(dto.GameDate, errors);
        ValidateMinutesPlayed(dto.MinutesPlayed, errors);
        ValidateJerseyNumber(dto.JerseyNumber, errors);
        ValidateGoals(dto.Goals, errors);
        ValidateAssists(dto.Assists, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates an UpdatePlayerStatisticDto against all business rules.
    /// </summary>
    /// <param name="dto">The UpdatePlayerStatisticDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    /// <example>
    /// <code>
    /// var updateDto = new UpdatePlayerStatisticDto
    /// {
    ///     PlayerStatisticId = 1,
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 3,
    ///     Assists = 1
    /// };
    /// var result = PlayerStatisticValidator.ValidateUpdatePlayerStatistic(updateDto);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateUpdatePlayerStatistic(UpdatePlayerStatisticDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateTeamPlayerId(dto.TeamPlayerId, errors);
        ValidateGameDate(dto.GameDate, errors);
        ValidateMinutesPlayed(dto.MinutesPlayed, errors);
        ValidateJerseyNumber(dto.JerseyNumber, errors);
        ValidateGoals(dto.Goals, errors);
        ValidateAssists(dto.Assists, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates a PlayerStatistic entity against all business rules.
    /// </summary>
    /// <param name="statistic">The PlayerStatistic entity to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistic is null.</exception>
    /// <example>
    /// <code>
    /// var statistic = new PlayerStatistic
    /// {
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 2,
    ///     Assists = 1,
    ///     CreatedBy = "system"
    /// };
    /// var result = PlayerStatisticValidator.ValidatePlayerStatistic(statistic);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidatePlayerStatistic(PlayerStatistic statistic)
    {
        ArgumentNullException.ThrowIfNull(statistic);

        var errors = new Dictionary<string, List<string>>();

        ValidateTeamPlayerId(statistic.TeamPlayerId, errors);
        ValidateGameDate(statistic.GameDate, errors);
        ValidateMinutesPlayed(statistic.MinutesPlayed, errors);
        ValidateJerseyNumber(statistic.JerseyNumber, errors);
        ValidateGoals(statistic.Goals, errors);
        ValidateAssists(statistic.Assists, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates a CreatePlayerStatisticDto against all business rules. Alias for ValidateCreatePlayerStatistic.
    /// </summary>
    /// <param name="dto">The CreatePlayerStatisticDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    public static ValidationResult Validate(CreatePlayerStatisticDto dto) => ValidateCreatePlayerStatistic(dto);

    /// <summary>
    /// Validates an UpdatePlayerStatisticDto against all business rules. Alias for ValidateUpdatePlayerStatistic.
    /// </summary>
    /// <param name="dto">The UpdatePlayerStatisticDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    public static ValidationResult Validate(UpdatePlayerStatisticDto dto) => ValidateUpdatePlayerStatistic(dto);

    private static void ValidateTeamPlayerId(int teamPlayerId, Dictionary<string, List<string>> errors)
    {
        if (teamPlayerId <= 0)
        {
            AddError(errors, nameof(PlayerStatistic.TeamPlayerId), "Team player ID must be greater than 0");
        }
    }

    private static void ValidateGameDate(DateTime gameDate, Dictionary<string, List<string>> errors)
    {
        if (gameDate == default)
        {
            AddError(errors, nameof(PlayerStatistic.GameDate), "Game date is required");
            return;
        }

        if (gameDate.Date > DateTime.UtcNow.Date)
        {
            AddError(errors, nameof(PlayerStatistic.GameDate), "Game date cannot be in the future");
        }
    }

    private static void ValidateMinutesPlayed(int minutesPlayed, Dictionary<string, List<string>> errors)
    {
        if (minutesPlayed < 0)
        {
            AddError(errors, nameof(PlayerStatistic.MinutesPlayed), "Minutes played must be non-negative");
        }
        else if (minutesPlayed > MaxMinutesPlayed)
        {
            AddError(errors, nameof(PlayerStatistic.MinutesPlayed), $"Minutes played should not exceed {MaxMinutesPlayed}");
        }
    }

    private static void ValidateJerseyNumber(int jerseyNumber, Dictionary<string, List<string>> errors)
    {
        if (jerseyNumber < MinJerseyNumber)
        {
            AddError(errors, nameof(PlayerStatistic.JerseyNumber), "Jersey number must be greater than 0");
        }
        else if (jerseyNumber > MaxJerseyNumber)
        {
            AddError(errors, nameof(PlayerStatistic.JerseyNumber), $"Jersey number should not exceed {MaxJerseyNumber}");
        }
    }

    private static void ValidateGoals(int goals, Dictionary<string, List<string>> errors)
    {
        if (goals < 0)
        {
            AddError(errors, nameof(PlayerStatistic.Goals), "Goals must be non-negative");
        }
    }

    private static void ValidateAssists(int assists, Dictionary<string, List<string>> errors)
    {
        if (assists < 0)
        {
            AddError(errors, nameof(PlayerStatistic.Assists), "Assists must be non-negative");
        }
    }

    private static void AddError(Dictionary<string, List<string>> errors, string fieldName, string errorMessage)
    {
        if (!errors.TryGetValue(fieldName, out var fieldErrors))
        {
            fieldErrors = [];
            errors[fieldName] = fieldErrors;
        }

        fieldErrors.Add(errorMessage);
    }

    private static ValidationResult BuildResult(Dictionary<string, List<string>> errors)
    {
        if (errors.Count == 0)
        {
            return ValidationResult.Valid();
        }

        var errorDictionary = errors.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToArray());

        return ValidationResult.Invalid(errorDictionary);
    }
}
