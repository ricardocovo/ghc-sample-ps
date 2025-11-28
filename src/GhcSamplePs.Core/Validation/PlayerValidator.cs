using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Validation;

/// <summary>
/// Validator for player data validation according to business rules.
/// </summary>
/// <remarks>
/// <para>Validation Rules:</para>
/// <list type="bullet">
/// <item>UserId: Required, cannot be empty or whitespace</item>
/// <item>Name: Required, not empty/whitespace, length 1-200 characters</item>
/// <item>DateOfBirth: Required, must be past date, not more than 100 years ago</item>
/// <item>Gender: If provided, must be one of: "Male", "Female", "Non-binary", "Prefer not to say"</item>
/// <item>PhotoUrl: If provided, max 500 characters, must be valid HTTP/HTTPS URL</item>
/// </list>
/// </remarks>
public static class PlayerValidator
{
    /// <summary>
    /// The maximum length allowed for a player's name.
    /// </summary>
    public const int MaxNameLength = 200;

    /// <summary>
    /// The maximum length allowed for a photo URL.
    /// </summary>
    public const int MaxPhotoUrlLength = 500;

    /// <summary>
    /// The maximum age allowed for a player (in years).
    /// </summary>
    public const int MaxAgeInYears = 100;

    /// <summary>
    /// The valid gender options for a player.
    /// </summary>
    public static readonly IReadOnlyList<string> ValidGenderOptions = new[]
    {
        "Male",
        "Female",
        "Non-binary",
        "Prefer not to say"
    };

    /// <summary>
    /// Validates a CreatePlayerDto against all business rules.
    /// </summary>
    /// <param name="dto">The DTO to validate.</param>
    /// <returns>A ValidationResult indicating if the data is valid.</returns>
    /// <example>
    /// <code>
    /// var dto = new CreatePlayerDto
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(2010, 5, 15)
    /// };
    /// var result = PlayerValidator.Validate(dto);
    /// if (!result.IsValid)
    /// {
    ///     foreach (var (field, errors) in result.Errors)
    ///     {
    ///         Console.WriteLine($"{field}: {string.Join(", ", errors)}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static ValidationResult Validate(CreatePlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateUserId(dto.UserId, errors);
        ValidateName(dto.Name, errors);
        ValidateDateOfBirth(dto.DateOfBirth, errors);
        ValidateGender(dto.Gender, errors);
        ValidatePhotoUrl(dto.PhotoUrl, errors);

        if (errors.Count == 0)
        {
            return ValidationResult.Valid();
        }

        return ValidationResult.Invalid(
            errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
    }

    /// <summary>
    /// Validates an UpdatePlayerDto against all business rules.
    /// </summary>
    /// <param name="dto">The DTO to validate.</param>
    /// <returns>A ValidationResult indicating if the data is valid.</returns>
    /// <example>
    /// <code>
    /// var dto = new UpdatePlayerDto
    /// {
    ///     Id = 1,
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(2010, 5, 15)
    /// };
    /// var result = PlayerValidator.Validate(dto);
    /// </code>
    /// </example>
    public static ValidationResult Validate(UpdatePlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateName(dto.Name, errors);
        ValidateDateOfBirth(dto.DateOfBirth, errors);
        ValidateGender(dto.Gender, errors);
        ValidatePhotoUrl(dto.PhotoUrl, errors);

        if (errors.Count == 0)
        {
            return ValidationResult.Valid();
        }

        return ValidationResult.Invalid(
            errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
    }

    private static void ValidateUserId(string userId, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            AddError(errors, nameof(CreatePlayerDto.UserId), "User ID is required.");
        }
    }

    private static void ValidateName(string name, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AddError(errors, nameof(CreatePlayerDto.Name), "Name is required.");
            return;
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
        {
            AddError(errors, nameof(CreatePlayerDto.Name), $"Name cannot exceed {MaxNameLength} characters.");
        }
    }

    private static void ValidateDateOfBirth(DateTime dateOfBirth, Dictionary<string, List<string>> errors)
    {
        var today = DateTime.UtcNow.Date;

        if (dateOfBirth.Date >= today)
        {
            AddError(errors, nameof(CreatePlayerDto.DateOfBirth), "Date of birth must be in the past.");
            return;
        }

        var maxPastDate = today.AddYears(-MaxAgeInYears);
        if (dateOfBirth.Date < maxPastDate)
        {
            AddError(errors, nameof(CreatePlayerDto.DateOfBirth), $"Date of birth cannot be more than {MaxAgeInYears} years ago.");
        }
    }

    private static void ValidateGender(string? gender, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(gender))
        {
            return;
        }

        var trimmedGender = gender.Trim();
        if (!ValidGenderOptions.Contains(trimmedGender, StringComparer.OrdinalIgnoreCase))
        {
            var validOptions = string.Join(", ", ValidGenderOptions);
            AddError(errors, nameof(CreatePlayerDto.Gender), $"Gender must be one of: {validOptions}.");
        }
    }

    private static void ValidatePhotoUrl(string? photoUrl, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return;
        }

        var trimmedUrl = photoUrl.Trim();

        if (trimmedUrl.Length > MaxPhotoUrlLength)
        {
            AddError(errors, nameof(CreatePlayerDto.PhotoUrl), $"Photo URL cannot exceed {MaxPhotoUrlLength} characters.");
            return;
        }

        if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            AddError(errors, nameof(CreatePlayerDto.PhotoUrl), "Photo URL must be a valid HTTP or HTTPS URL.");
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
}
