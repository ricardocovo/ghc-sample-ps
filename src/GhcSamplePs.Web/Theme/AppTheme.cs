using MudBlazor;

namespace GhcSamplePs.Web.Theme;

/// <summary>
/// Custom MudBlazor theme configuration for the application.
/// Provides a professional Material Design theme with custom brand colors and layout settings.
/// </summary>
public static class AppTheme
{
    /// <summary>
    /// Gets the custom theme configuration with Material Design 3 inspired colors.
    /// 
    /// Color Palette:
    /// - Primary: Blue 700 (#1976d2) - Brand color for primary actions (buttons, links, app bar)
    /// - Secondary: Grey 800 (#424242) - Accent color for secondary elements
    /// - Tertiary: Cyan 600 (#00acc1) - Additional accent color
    /// - Success: Green 500 (#4caf50) - Success states and positive actions
    /// - Warning: Orange 500 (#ff9800) - Warning states and cautionary actions
    /// - Error: Red 500 (#f44336) - Error states and destructive actions
    /// - Info: Blue 500 (#2196f3) - Informational messages
    /// 
    /// Typography:
    /// - Font Family: Roboto (Google's Material Design font)
    /// - Responsive font sizes for mobile-first design
    /// - Proper line heights for readability
    /// 
    /// Layout Properties:
    /// - Border Radius: 4px - Material Design default rounded corners
    /// - App Bar Height: 64px - Standard height, mobile-friendly
    /// - Drawer Width: 260px - Comfortable navigation width
    /// 
    /// Responsive Breakpoints (MudBlazor defaults):
    /// - xs: 0-599px (small phones)
    /// - sm: 600-959px (large phones)
    /// - md: 960-1279px (tablets)
    /// - lg: 1280-1919px (desktops)
    /// - xl: 1920px+ (large desktops)
    /// </summary>
    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            // Primary brand color - Material Design Blue 700
            Primary = "#1976d2",
            PrimaryContrastText = "#ffffff",
            
            // Secondary accent color - Material Design Grey 800
            Secondary = "#424242",
            SecondaryContrastText = "#ffffff",
            
            // Tertiary color for additional accents - Material Design Cyan 600
            Tertiary = "#00acc1",
            TertiaryContrastText = "#ffffff",
            
            // App bar configuration
            AppbarBackground = "#1976d2",
            AppbarText = "#ffffff",
            
            // Background colors - Light grey for modern look
            Background = "#f5f5f5",
            BackgroundGray = "#f0f0f0",
            Surface = "#ffffff",
            
            // Drawer configuration for navigation
            DrawerBackground = "#ffffff",
            DrawerText = "rgba(0,0,0, 0.87)",
            DrawerIcon = "rgba(0,0,0, 0.54)",
            
            // Text colors with proper contrast ratios (WCAG AA compliant)
            TextPrimary = "rgba(0,0,0, 0.87)",
            TextSecondary = "rgba(0,0,0, 0.54)",
            TextDisabled = "rgba(0,0,0, 0.38)",
            
            // Action colors for interactive elements
            ActionDefault = "rgba(0,0,0, 0.54)",
            ActionDisabled = "rgba(0,0,0, 0.26)",
            ActionDisabledBackground = "rgba(0,0,0, 0.12)",
            
            // Divider colors for visual separation
            Divider = "rgba(0,0,0, 0.12)",
            DividerLight = "rgba(0,0,0, 0.06)",
            
            // Semantic colors for user feedback
            Info = "#2196f3",
            InfoContrastText = "#ffffff",
            Success = "#4caf50",
            SuccessContrastText = "#ffffff",
            Warning = "#ff9800",
            WarningContrastText = "#000000",
            Error = "#f44336",
            ErrorContrastText = "#ffffff",
            Dark = "#424242",
            DarkContrastText = "#ffffff"
        },
        
        PaletteDark = new PaletteDark
        {
            // Primary brand color for dark mode - Lighter blue for better visibility
            Primary = "#64b5f6",
            PrimaryContrastText = "#000000",
            
            // Secondary accent color - Light grey for dark mode
            Secondary = "#e0e0e0",
            SecondaryContrastText = "#000000",
            
            // Tertiary color for additional accents in dark mode
            Tertiary = "#4dd0e1",
            TertiaryContrastText = "#000000",
            
            // App bar configuration for dark mode
            AppbarBackground = "#1e1e1e",
            AppbarText = "rgba(255,255,255, 0.87)",
            
            // Background colors for dark mode - True dark for OLED displays
            Background = "#121212",
            BackgroundGray = "#1a1a1a",
            Surface = "#1e1e1e",
            
            // Drawer configuration for dark mode
            DrawerBackground = "#1e1e1e",
            DrawerText = "rgba(255,255,255, 0.87)",
            DrawerIcon = "rgba(255,255,255, 0.70)",
            
            // Text colors with proper contrast for dark mode
            TextPrimary = "rgba(255,255,255, 0.87)",
            TextSecondary = "rgba(255,255,255, 0.70)",
            TextDisabled = "rgba(255,255,255, 0.38)",
            
            // Action colors for dark mode
            ActionDefault = "rgba(255,255,255, 0.70)",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            
            // Divider colors for dark mode
            Divider = "rgba(255,255,255, 0.12)",
            DividerLight = "rgba(255,255,255, 0.06)",
            
            // Semantic colors (lighter variants for dark mode visibility)
            Info = "#64b5f6",
            InfoContrastText = "#000000",
            Success = "#81c784",
            SuccessContrastText = "#000000",
            Warning = "#ffb74d",
            WarningContrastText = "#000000",
            Error = "#e57373",
            ErrorContrastText = "#000000",
            Dark = "#e0e0e0",
            DarkContrastText = "#000000"
        },
        
        LayoutProperties = new LayoutProperties
        {
            // Material Design default border radius for rounded corners
            DefaultBorderRadius = "4px",
            
            // Drawer widths for responsive navigation
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "260px",
            
            // Mobile-friendly app bar height (follows Material Design guidelines)
            AppbarHeight = "64px"
        }
    };
}
