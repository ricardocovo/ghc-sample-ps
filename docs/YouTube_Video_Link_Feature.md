# YouTube Video Link on Statistics Page

## Overview

A YouTube video link has been added to the **Statistics tab** on the **Edit Player page** (`/players/edit/{id}`). This link provides users with tutorial content about tracking player statistics.

## Features

### YouTube URL Validation

The implementation includes robust validation to ensure only valid YouTube video URLs are displayed:

**Supported YouTube URL Formats:**
- `https://www.youtube.com/watch?v=VIDEO_ID`
- `https://youtu.be/VIDEO_ID`
- `https://m.youtube.com/watch?v=VIDEO_ID`
- `https://www.youtube.com/embed/VIDEO_ID`

**Validation Rules:**
- ✅ Must use HTTPS protocol (HTTP is rejected)
- ✅ Must be from valid YouTube domains: youtube.com, www.youtube.com, m.youtube.com, or youtu.be
- ✅ Must include a valid video ID
- ❌ Non-YouTube URLs are rejected
- ❌ Empty or null values are rejected

### User Interface

The YouTube link appears as an informational alert with:
- 📺 Video library icon
- "Need help?" message with tutorial description
- Play button icon next to the "Watch Tutorial" link
- Opens in a new browser tab when clicked
- Only displays if a valid YouTube URL is configured

## Configuration

### Setting the YouTube Video URL

The YouTube video URL is configured in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "StatisticsHelp": {
    "YouTubeVideoUrl": "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
  }
}
```

**To change the video:**
1. Open `src/GhcSamplePs.Web/appsettings.json` (for production)
2. Update the `StatisticsHelp:YouTubeVideoUrl` value with your YouTube video URL
3. Ensure the URL follows one of the supported formats listed above
4. For development, also update `src/GhcSamplePs.Web/appsettings.Development.json`
5. Restart the application for changes to take effect

**Example YouTube URLs:**
```json
"YouTubeVideoUrl": "https://www.youtube.com/watch?v=VIDEO_ID_HERE"
"YouTubeVideoUrl": "https://youtu.be/VIDEO_ID_HERE"
```

### Removing the YouTube Link

To hide the YouTube link entirely:
1. Set the `YouTubeVideoUrl` to an empty string or remove the configuration
2. The link will not display if the URL is invalid or empty

## Implementation Details

### Files Modified/Created

1. **`src/GhcSamplePs.Web/Helpers/YouTubeUrlValidator.cs`** (NEW)
   - Static helper class for YouTube URL validation
   - `IsValidYouTubeUrl(string? url)` - Validates if a URL is a valid YouTube video link
   - `GetValidatedYouTubeUrl(string? url)` - Returns validated URL or null

2. **`src/GhcSamplePs.Web/Components/Pages/PlayerManagement/EditPlayer.razor`** (MODIFIED)
   - Added IConfiguration injection to read app settings
   - Added `_youTubeVideoUrl` field to store validated URL
   - Added initialization logic in `OnInitializedAsync()` to validate URL from configuration
   - Added conditional YouTube link display in the Stats tab

3. **`src/GhcSamplePs.Web/appsettings.json`** (MODIFIED)
   - Added `StatisticsHelp` configuration section

4. **`src/GhcSamplePs.Web/appsettings.Development.json`** (MODIFIED)
   - Added `StatisticsHelp` configuration section

### Validation Logic

```csharp
// Example usage
var configuredUrl = Configuration["StatisticsHelp:YouTubeVideoUrl"];
var validatedUrl = YouTubeUrlValidator.GetValidatedYouTubeUrl(configuredUrl);

if (validatedUrl != null)
{
    // Display the YouTube link
}
else
{
    // Hide the link or show a different message
}
```

### Architecture Compliance

The implementation follows the project's clean architecture principles:
- **UI Layer**: YouTube link display and user interaction (EditPlayer.razor)
- **Helper Class**: URL validation logic (YouTubeUrlValidator in Web/Helpers)
- **Configuration**: Externalized configuration using appsettings.json
- **Validation**: Only YouTube URLs are accepted, non-YouTube URLs are rejected

## Testing

### Manual Testing

1. Navigate to any player's edit page
2. Click on the "Stats" tab
3. Verify the YouTube link appears at the top of the statistics section
4. Click the link to ensure it opens the video in a new tab

### Validation Testing

To test the validation:

**Valid URLs** (should display the link):
- `https://www.youtube.com/watch?v=dQw4w9WgXcQ`
- `https://youtu.be/dQw4w9WgXcQ`

**Invalid URLs** (should hide the link):
- `http://www.youtube.com/watch?v=dQw4w9WgXcQ` (HTTP not HTTPS)
- `https://vimeo.com/123456` (Not YouTube)
- Empty string or null

### Automated Testing

All existing unit tests (891 tests) continue to pass with this implementation.

## Security Considerations

- **HTTPS Only**: Only HTTPS YouTube URLs are accepted for security
- **Domain Validation**: Only official YouTube domains are whitelisted
- **No XSS Risk**: The URL is validated before rendering in the UI
- **External Links**: Opens in a new tab to prevent navigation away from the app

## Future Enhancements

Potential improvements for future iterations:
- Admin interface to change the YouTube URL without editing configuration files
- Multiple video links for different topics
- Localization support for different languages
- Video embed option instead of external link
- Analytics tracking for video link clicks

## Support

For issues or questions about the YouTube link feature:
- Check that the configured URL is a valid YouTube video URL
- Verify the URL uses HTTPS protocol
- Ensure the video ID is present in the URL
- Check browser console for any validation errors
