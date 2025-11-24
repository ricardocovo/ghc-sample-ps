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
- **MudBlazor** - Material Design component library (v8.15.0)

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components (Home, Counter, Weather)
├── wwwroot/             # Static files (CSS, JS, images)
├── App.razor            # Root component with MudBlazor CSS/JS references
├── Routes.razor         # Router with MudBlazor providers
├── _Imports.razor       # Global using statements including MudBlazor
├── Program.cs           # Application entry point and DI configuration
└── appsettings.json     # Configuration
```

## UI Framework

This project uses **MudBlazor** (Material Design components) for the UI instead of Bootstrap:

- **CSS Framework**: MudBlazor (Material Design)
- **Bootstrap**: Removed (no longer used)
- **Mobile-First**: Optimized for mobile devices with responsive design
- **Touch-Friendly**: Minimum 44x44 pixel touch targets

### MudBlazor Features Available

- Material Design components (buttons, cards, tables, forms)
- Built-in responsive layouts
- Dark mode support
- Dialog and Snackbar notifications
- Accessible components (WCAG 2.1 AA compliant)

## Running the Application

```powershell
cd src/GhcSamplePs.Web
dotnet run
```

Visit `https://localhost:5001` in your browser.

## UI Framework

This application uses **MudBlazor** for Material Design components:
- Responsive, mobile-first design
- Touch-friendly interactions
- Material Design styling
- Pre-built accessible components

### MudBlazor Components Used

- **MudSimpleTable** - Weather data display with horizontal scroll support
- **MudProgressCircular** - Loading indicators
- **MudThemeProvider** - Theme management
- **MudDialogProvider** - Dialog support
- **MudSnackbarProvider** - Toast notifications
- **MudPopoverProvider** - Popover support

## Development Guidelines

### Component Development

- Keep components focused and small
- Use component parameters for input: `[Parameter]`
- Use EventCallback for output events
- Avoid business logic in `.razor` files - call services instead
- Use `@inject` directive to inject services
- Use MudBlazor components instead of HTML elements where appropriate

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

### MudBlazor Component Usage

```razor
@* Use MudButton instead of HTML button *@
<MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="HandleClick">
    Click Me
</MudButton>

@* Use MudCard for content sections *@
<MudCard>
    <MudCardContent>
        <MudText Typo="Typo.h5">Card Title</MudText>
        <MudText>Card content goes here</MudText>
    </MudCardContent>
</MudCard>
```

### What NOT to Include

- ❌ Business rules or logic (belongs in Core)
- ❌ Direct data access (use services from Core)
- ❌ Complex calculations (use services from Core)
- ❌ Domain models (use DTOs/ViewModels)

## Styling and CSS

### CSS Files

- **app.css** - Global styles and mobile optimizations
  - Safe area insets for notched devices
  - Touch-friendly minimum sizes
  - Mobile-specific optimizations
  - No Bootstrap dependencies

- **Component-scoped CSS** - Use `.razor.css` files for component-specific styles
  - MainLayout.razor.css
  - NavMenu.razor.css

### Mobile-First Design

The application is optimized for mobile devices:

- **Safe Area Insets**: Support for notched devices (iPhone X+)
- **Touch Targets**: Minimum 44x44 pixels for all interactive elements
- **Responsive Text**: Minimum 16px font size on mobile
- **Smooth Scrolling**: Native smooth scroll behavior
- **Touch Optimizations**: No tap highlights, no long-press menus

### Responsive Breakpoints

MudBlazor uses these breakpoints:

| Breakpoint | Width | Device Type |
|-----------|-------|-------------|
| xs | 0-599px | Small phones |
| sm | 600-959px | Large phones |
| md | 960-1279px | Tablets |
| lg | 1280-1919px | Desktops |
| xl | 1920px+ | Large desktops |

## Configuration

Configuration is managed in:
- `appsettings.json` - General settings
- `appsettings.Development.json` - Development-specific settings
- `Program.cs` - Service registration and middleware
  - MudBlazor services registered with `builder.Services.AddMudServices()`

## Adding New Features

1. Define service interface and implementation in `GhcSamplePs.Core`
2. Register service in `Program.cs`
3. Create Blazor component in appropriate folder (use MudBlazor components)
4. Inject and use service in component
5. Test business logic in `GhcSamplePs.Core.Tests`

## Recent Changes

### CSS Migration (November 2024)
- ✅ Removed Bootstrap CSS dependencies
- ✅ Added MudBlazor CSS and JS references
- ✅ Added mobile-specific CSS optimizations
- ✅ Enhanced viewport configuration for accessibility
- ✅ Added touch-friendly minimum sizes
- ✅ Implemented safe area insets for notched devices

## See Also

- [MudBlazor Documentation](https://mudblazor.com/)
- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)

## Progressive Web App (PWA) Features

This application includes PWA capabilities for mobile installation:

### Manifest Configuration

The PWA manifest (`wwwroot/manifest.json`) includes:
- **App Name**: GhcSamplePs - Blazor Web Application
- **Short Name**: GhcSamplePs (for home screen)
- **Display Mode**: Standalone (app-like experience)
- **Theme Color**: #594AE2 (MudBlazor primary purple)
- **Background Color**: #FFFFFF (white)
- **Orientation**: Any

### App Icons

Icons are available in multiple sizes in `wwwroot/icons/`:
- 72x72, 96x96, 128x128, 144x144, 152x152 pixels
- 192x192 pixels (minimum required)
- 384x384 pixels
- 512x512 pixels (minimum required)

All icons follow Material Design guidelines and use the MudBlazor theme colors.

### Installation

**On Android (Chrome/Edge):**
1. Open the app in Chrome or Edge browser
2. Tap the menu (three dots) and select "Install app" or "Add to Home screen"
3. The app will be added to your home screen and app drawer

**On iOS (Safari):**
1. Open the app in Safari
2. Tap the Share button
3. Scroll down and tap "Add to Home Screen"
4. Tap "Add" to confirm

**On Desktop (Chrome/Edge):**
1. Open the app in Chrome or Edge
2. Look for the install icon in the address bar
3. Click it and follow the prompts to install

### Testing PWA Features

**Validate Manifest:**
- Open browser DevTools (F12)
- Go to Application tab → Manifest
- Verify all properties are loaded correctly
- Check that all icons are available

**Check Service Worker (if enabled):**
- Application tab → Service Workers
- Verify registration and status

**Lighthouse Audit:**
- Run Lighthouse audit in Chrome DevTools
- Check PWA score and recommendations

### Browser Support

- ✅ Chrome/Edge (Android): Full support
- ✅ Safari (iOS): Requires "Add to Home Screen" manually
- ✅ Chrome/Edge (Desktop): Full support
- ⚠️ Firefox: Limited PWA support (no installation prompt)

For more details, see the [MudBlazor Mobile Integration Specification](../../docs/specs/MudBlazor_Mobile_Integration_Specification.md).
