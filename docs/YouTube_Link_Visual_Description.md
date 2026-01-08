# YouTube Video Link - Visual Description

## Location
The YouTube video link appears on the **Edit Player** page, specifically within the **Stats Tab**.

## UI Layout

```
┌─────────────────────────────────────────────────────────────┐
│  Edit Player - John Doe                                      │
├─────────────────────────────────────────────────────────────┤
│  [Player]  [Teams]  [Stats] ← Active Tab                   │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  📊 Game Statistics                          [Add Statistics]│
│                                                               │
│  ╔═══════════════════════════════════════════════════════╗  │
│  ║  📺  Need help? Watch our tutorial on tracking        ║  │
│  ║      player statistics:  ▶ Watch Tutorial  ↗         ║  │
│  ╚═══════════════════════════════════════════════════════╝  │
│     ↑ YouTube Link (MudAlert Info style with icon)           │
│                                                               │
│  [Statistics content appears below...]                       │
│                                                               │
│  ┌──────────┬──────────┬──────────┬──────────┐             │
│  │  Games   │  Goals   │ Assists  │ Avg/Game │             │
│  │   Played │          │          │          │             │
│  └──────────┴──────────┴──────────┴──────────┘             │
│                                                               │
│  [Game statistics table...]                                  │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## Visual Characteristics

### The YouTube Link Alert Box:
- **Type**: MudBlazor Alert component
- **Severity**: Info (light blue/informational color)
- **Icon**: 📺 Video Library icon (MudIcon VideoLibrary)
- **Layout**: Horizontal stack with text and link
- **Positioning**: Full width, below the page header, above statistics content

### Link Button:
- **Icon**: ▶ Play Circle icon
- **Text**: "Watch Tutorial"
- **Style**: Primary color link
- **Behavior**: Opens in new tab (target="_blank")
- **Hover**: Underline effect

### Responsive Behavior:
- **Desktop**: Full-width alert with inline text and link
- **Mobile**: Stacks vertically if needed, maintains readability

## States

### When Valid YouTube URL is Configured:
```
╔═══════════════════════════════════════════════════════╗
║  📺  Need help? Watch our tutorial on tracking        ║
║      player statistics:  ▶ Watch Tutorial  ↗         ║
╚═══════════════════════════════════════════════════════╝
```

### When No Valid YouTube URL:
```
[Alert does not appear - nothing displayed]
```

## Color Scheme (MudBlazor Theme)
- **Alert Background**: Light blue (#E3F2FD or theme Info color)
- **Alert Border**: Slightly darker blue
- **Icon Color**: Primary blue
- **Link Color**: Primary theme color (typically blue)
- **Text Color**: Dark gray/black for readability

## Interaction Flow

1. **User navigates** to Edit Player page (`/players/edit/123`)
2. **User clicks** on "Stats" tab
3. **YouTube alert appears** at the top of the stats section (if valid URL configured)
4. **User clicks** "Watch Tutorial" link
5. **New browser tab opens** with the YouTube video
6. **Original page remains open** - user can continue working

## Example YouTube URLs Supported

✅ Valid:
- `https://www.youtube.com/watch?v=dQw4w9WgXcQ`
- `https://youtu.be/dQw4w9WgXcQ`
- `https://m.youtube.com/watch?v=VIDEO_ID`
- `https://www.youtube.com/embed/VIDEO_ID`

❌ Invalid (will hide the alert):
- `http://www.youtube.com/watch?v=...` (HTTP not HTTPS)
- `https://vimeo.com/...` (Not YouTube)
- Empty or null value

## Accessibility

- **Screen Readers**: Alert is properly labeled with ARIA attributes
- **Keyboard Navigation**: Link is focusable and activatable with Enter key
- **Color Contrast**: Meets WCAG AA standards for text readability
- **Icon Semantics**: Icons have proper aria-labels for screen readers

## Notes

- The YouTube link is **optional** - if no valid URL is configured, nothing displays
- The link is **non-intrusive** - appears as helpful information, not mandatory action
- The alert **does not block** any functionality - users can add statistics without watching the video
- Configuration is **centralized** in appsettings.json for easy updates
