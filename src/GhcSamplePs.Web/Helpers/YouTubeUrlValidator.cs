namespace GhcSamplePs.Web.Helpers;

/// <summary>
/// Helper class for validating YouTube URLs in the UI layer.
/// Ensures only valid YouTube video links are displayed in the application.
/// </summary>
public static class YouTubeUrlValidator
{
    private static readonly string[] ValidYouTubeDomains =
    [
        "youtube.com",
        "www.youtube.com",
        "m.youtube.com",
        "youtu.be"
    ];

    /// <summary>
    /// Validates if the provided URL is a valid YouTube video URL.
    /// Supports formats:
    /// - https://www.youtube.com/watch?v=VIDEO_ID
    /// - https://youtu.be/VIDEO_ID
    /// - https://m.youtube.com/watch?v=VIDEO_ID
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is a valid YouTube video URL, false otherwise.</returns>
    public static bool IsValidYouTubeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Must use HTTPS
        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        // Check if domain is a valid YouTube domain
        var host = uri.Host.ToLowerInvariant();
        if (!ValidYouTubeDomains.Any(domain => host == domain))
        {
            return false;
        }

        // For youtu.be format, just check that there's a path
        if (host == "youtu.be")
        {
            return !string.IsNullOrWhiteSpace(uri.AbsolutePath) && uri.AbsolutePath.Length > 1;
        }

        // For youtube.com format, check for watch?v= parameter or /embed/ path
        if (uri.AbsolutePath.StartsWith("/watch", StringComparison.OrdinalIgnoreCase))
        {
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return !string.IsNullOrWhiteSpace(query["v"]);
        }

        if (uri.AbsolutePath.StartsWith("/embed/", StringComparison.OrdinalIgnoreCase))
        {
            return uri.AbsolutePath.Length > "/embed/".Length;
        }

        return false;
    }

    /// <summary>
    /// Gets a validated YouTube URL, returning null if the URL is invalid.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>The original URL if valid, null otherwise.</returns>
    public static string? GetValidatedYouTubeUrl(string? url)
    {
        return IsValidYouTubeUrl(url) ? url : null;
    }
}
