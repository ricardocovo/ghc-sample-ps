# GhcSamplePs.Web

Blazor Web App - UI Layer

## Purpose

This project contains the user interface layer of the GhcSamplePs application, including:
- Blazor components (`.razor` files)
- Pages and layouts
- UI-specific services and state management
- Client-side validation and user interaction logic

## Dependencies

- **GhcSamplePs.Core** - Business logic and services

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
├── wwwroot/             # Static files (CSS, JS, images)
├── App.razor            # Root component
├── Program.cs           # Application entry point and DI configuration
└── appsettings.json     # Configuration
```

## Running the Application

```powershell
cd src/GhcSamplePs.Web
dotnet run
```

Visit `https://localhost:5001` in your browser.

## Development Guidelines

### Component Development

- Keep components focused and small
- Use component parameters for input: `[Parameter]`
- Use EventCallback for output events
- Avoid business logic in `.razor` files - call services instead
- Use `@inject` directive to inject services

### Service Usage

```razor
@inject IMyService MyService

@code {
    private async Task HandleClick()
    {
        var result = await MyService.DoSomethingAsync();
        // Update UI state
    }
}
```

### What NOT to Include

- ❌ Business rules or logic (belongs in Core)
- ❌ Direct data access (use services from Core)
- ❌ Complex calculations (use services from Core)
- ❌ Domain models (use DTOs/ViewModels)

## Configuration

Configuration is managed in:
- `appsettings.json` - General settings
- `appsettings.Development.json` - Development-specific settings
- `Program.cs` - Service registration and middleware

## Adding New Features

1. Define service interface and implementation in `GhcSamplePs.Core`
2. Register service in `Program.cs`
3. Create Blazor component in appropriate folder
4. Inject and use service in component
5. Test business logic in `GhcSamplePs.Core.Tests`

## See Also

- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)

## Theme Configuration

### Custom MudBlazor Theme

The application uses a custom Material Design theme configured through MudBlazor. The theme is defined in `Theme/AppTheme.cs` and provides a consistent, professional appearance across all components.

#### Color Palette

**Light Mode:**
- **Primary**: Blue 700 (#1976d2) - Used for primary actions, buttons, app bar
- **Secondary**: Grey 800 (#424242) - Accent color for secondary elements
- **Tertiary**: Cyan 600 (#00acc1) - Additional accent color
- **Success**: Green 500 (#4caf50) - Success states and positive feedback
- **Warning**: Orange 500 (#ff9800) - Warning states and caution
- **Error**: Red 500 (#f44336) - Error states and destructive actions
- **Info**: Blue 500 (#2196f3) - Informational messages

**Dark Mode:**
- Lighter variants of all colors for better visibility on dark backgrounds
- True dark background (#121212) optimized for OLED displays
- Proper contrast ratios maintaining WCAG AA compliance

#### Typography

- **Font Family**: Roboto (Google's Material Design font)
- Loaded from Google Fonts CDN
- Responsive font sizes optimized for mobile-first design
- Proper line heights and letter spacing for readability

#### Layout Properties

- **Border Radius**: 4px - Material Design default for rounded corners
- **App Bar Height**: 64px - Standard height, mobile-friendly
- **Drawer Width**: 260px - Comfortable navigation width
- **Spacing**: Uses MudBlazor's built-in spacing system

#### Responsive Breakpoints

The theme uses MudBlazor's default responsive breakpoints:
- **xs**: 0-599px (small phones)
- **sm**: 600-959px (large phones)
- **md**: 960-1279px (tablets)
- **lg**: 1280-1919px (desktops)
- **xl**: 1920px+ (large desktops)

### Customizing the Theme

To modify the theme colors or layout:

1. Edit `Theme/AppTheme.cs`
2. Update color values in `PaletteLight` and/or `PaletteDark`
3. Modify `LayoutProperties` for spacing and sizing
4. Rebuild the application: `dotnet build`

### Theme Application

The theme is applied in `App.razor` through the `MudThemeProvider` component:

```razor
<MudThemeProvider Theme="@_theme" />
```

All MudBlazor components automatically use the configured theme colors and styles.
