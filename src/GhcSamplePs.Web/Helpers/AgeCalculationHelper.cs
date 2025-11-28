namespace GhcSamplePs.Web.Helpers;

/// <summary>
/// Helper class for age-related calculations in the UI layer.
/// Provides consistent age calculation across all player management pages.
/// </summary>
public static class AgeCalculationHelper
{
    /// <summary>
    /// Calculates the age in years from a given date of birth.
    /// Uses UTC date for consistency with the domain model.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth to calculate age from.</param>
    /// <returns>The calculated age in years, or null if the date of birth is null.</returns>
    public static int? CalculateAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        var dob = dateOfBirth.Value;
        var age = today.Year - dob.Year;

        // If the birthday has not occurred yet this year, subtract one year
        if (dob.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    /// <summary>
    /// Formats the calculated age as a display string.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth to calculate age from.</param>
    /// <returns>A formatted string like "25 years", or empty string if date of birth is null.</returns>
    public static string FormatAge(DateTime? dateOfBirth)
    {
        var age = CalculateAge(dateOfBirth);
        return age.HasValue ? $"{age} years" : string.Empty;
    }
}
