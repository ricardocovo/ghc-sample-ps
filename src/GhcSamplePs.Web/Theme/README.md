# Theme Configuration

This folder contains the custom MudBlazor theme configuration for the GhcSamplePs application.

## AppTheme.cs

The `AppTheme` class defines a comprehensive Material Design theme with custom brand colors, layout settings, and responsive design considerations.

### Theme Components

#### 1. Color Palette (Light Mode)

| Color Type | Hex Value | Usage |
|-----------|-----------|-------|
| Primary | #1976d2 (Blue 700) | Primary actions, buttons, links, app bar |
| Secondary | #424242 (Grey 800) | Secondary elements, subtle accents |
| Tertiary | #00acc1 (Cyan 600) | Additional accent color |
| Success | #4caf50 (Green 500) | Success messages, positive actions |
| Warning | #ff9800 (Orange 500) | Warnings, cautionary states |
| Error | #f44336 (Red 500) | Errors, destructive actions |
| Info | #2196f3 (Blue 500) | Informational messages |

**Background Colors:**
- Background: #f5f5f5 (Light Grey 100)
- Surface: #ffffff (White)
- Drawer Background: #ffffff (White)

**Text Colors:**
- Primary Text: rgba(0,0,0, 0.87) - High emphasis
- Secondary Text: rgba(0,0,0, 0.54) - Medium emphasis
- Disabled Text: rgba(0,0,0, 0.38) - Low emphasis

All text colors follow WCAG AA contrast ratio guidelines for accessibility.

#### 2. Color Palette (Dark Mode)

Dark mode uses lighter variants of the primary colors for better visibility:
- Primary: #64b5f6 (Blue 300)
- Background: #121212 (True Dark - OLED optimized)
- Surface: #1e1e1e (Dark Grey)
- Text colors adjusted for dark backgrounds

#### 3. Layout Properties

| Property | Value | Purpose |
|----------|-------|---------|
| DefaultBorderRadius | 4px | Rounded corners on components |
| DrawerWidthLeft | 260px | Navigation drawer width |
| DrawerWidthRight | 260px | Right drawer width (if used) |
| AppbarHeight | 64px | Top app bar height (mobile-friendly) |

#### 4. Typography

**Font Family**: Roboto, Helvetica, Arial, sans-serif
- Roboto is the official Material Design font
- Loaded via Google Fonts CDN in App.razor
- Fallbacks to Helvetica, Arial for compatibility

**Font Sizing**: Uses MudBlazor's default typography scale with responsive sizing for mobile devices.

#### 5. Responsive Breakpoints

The theme uses MudBlazor's built-in responsive breakpoints:

| Breakpoint | Width Range | Device Type |
|-----------|-------------|-------------|
| xs | 0-599px | Small phones (iPhone SE, etc.) |
| sm | 600-959px | Large phones (iPhone 13, most Android) |
| md | 960-1279px | Tablets (iPad, Android tablets) |
| lg | 1280-1919px | Small to medium desktops |
| xl | 1920px+ | Large desktops and 4K displays |

## Customization Guide

### Changing Brand Colors

To update the primary brand color:

```csharp
PaletteLight = new PaletteLight
{
    Primary = "#your-color-here",
    PrimaryContrastText = "#ffffff", // Adjust if needed for contrast
    AppbarBackground = "#your-color-here", // Usually same as Primary
    // ...
}
```

### Adding Custom Colors

You can extend the theme by adding custom color properties and using them in components:

```csharp
// In AppTheme.cs
PaletteLight = new PaletteLight
{
    // ... existing colors ...
    Tertiary = "#your-custom-color",
    TertiaryContrastText = "#ffffff"
}
```

### Adjusting Layout Spacing

Modify the `LayoutProperties` section:

```csharp
LayoutProperties = new LayoutProperties
{
    DefaultBorderRadius = "8px", // More rounded
    DrawerWidthLeft = "280px", // Wider drawer
    AppbarHeight = "72px" // Taller app bar
}
```

## Testing the Theme

After making changes:

1. Build the application:
   ```bash
   dotnet build
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Test theme across all pages:
   - Home page
   - Counter page
   - Weather page
   - Any custom pages

4. Test on multiple screen sizes using browser DevTools

## Design Principles

The theme follows Material Design 3 principles:
- **Hierarchy**: Clear visual hierarchy through color and typography
- **Consistency**: Uniform application of colors and spacing
- **Accessibility**: WCAG AA compliant contrast ratios
- **Responsive**: Mobile-first design with appropriate breakpoints
- **Touch-Friendly**: Adequate spacing for touch targets (minimum 44x44px)

## References

- [MudBlazor Documentation](https://mudblazor.com/)
- [Material Design Guidelines](https://material.io/design)
- [MudBlazor Theme Configuration](https://mudblazor.com/features/themes)
- [WCAG Contrast Guidelines](https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html)
