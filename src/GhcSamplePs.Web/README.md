# GhcSamplePs.Web

Blazor Web App - UI Layer

## Purpose

This project contains the user interface layer of the GhcSamplePs application, including:
- Blazor components (`.razor` files)
- Pages and layouts
- UI-specific services and state management
- Client-side validation and user interaction logic
- **Progressive Web App (PWA) support**

## Dependencies

- **GhcSamplePs.Core** - Business logic and services
- **MudBlazor** - Material Design component library (v8.15.0)

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
├── wwwroot/             # Static files (CSS, JS, images, PWA assets)
│   ├── manifest.json    # PWA manifest
│   ├── icon-192.png     # PWA icon (192x192)
│   └── icon-512.png     # PWA icon (512x512)
├── App.razor            # Root component (includes PWA meta tags)
├── Program.cs           # Application entry point and DI configuration
└── appsettings.json     # Configuration
```

## Progressive Web App (PWA)

The application includes PWA support to enable installation on mobile devices:

- **Manifest Link**: `App.razor` includes a link to `/manifest.json`
- **Browser Detection**: Supported browsers (Chrome, Edge, Safari 16.4+) will detect PWA capability
- **Installation**: Users can install the app via browser menu (Add to Home Screen)

### PWA Configuration

The manifest link is configured in `Components/App.razor`:
```html
<link rel="manifest" href="/manifest.json" />
```

This enables:
- Install prompt on supported browsers
- Add to home screen functionality on mobile devices
- App-like experience when launched from home screen

## Running the Application

```powershell
cd src/GhcSamplePs.Web
dotnet run
```

Visit `https://localhost:5001` in your browser.

## Progressive Web App (PWA) Support

This application is configured as a Progressive Web App, allowing users to install it on their mobile devices for an app-like experience.

### PWA Features

- ✅ **Installable**: Can be installed on Android and iOS home screens
- ✅ **Standalone Mode**: Launches without browser UI
- ✅ **App Icons**: Custom icons for home screen
- ✅ **Theme Colors**: Branded colors for mobile browsers
- ✅ **Mobile Optimized**: Responsive design for all screen sizes

### Testing PWA Installation

See the comprehensive testing guide: [PWA Testing Guide](../../docs/PWA_Testing_Guide.md)

**Quick testing steps:**

**Android (Chrome):**
1. Open app in Chrome on Android
2. Tap menu (⋮) → "Install app"
3. Launch from home screen

**iOS (Safari 16.4+):**
1. Open app in Safari on iOS
2. Tap Share (⬆) → "Add to Home Screen"
3. Launch from home screen

### PWA Configuration Files

- **manifest.json**: App metadata, display mode, icons
- **App.razor**: PWA meta tags for Android and iOS
- **Icons**: 192x192 and 512x512 PNG files

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
- `manifest.json` - PWA configuration

## Adding New Features

1. Define service interface and implementation in `GhcSamplePs.Core`
2. Register service in `Program.cs`
3. Create Blazor component in appropriate folder (use MudBlazor components)
4. Inject and use service in component
5. Test business logic in `GhcSamplePs.Core.Tests`

## Mobile Optimization

The application is optimized for mobile devices:
- Responsive layouts (works on screens from 320px to desktop)
- Touch-friendly UI elements
- Optimized viewport settings
- PWA support for installation

## See Also

- [PWA Testing Guide](../../docs/PWA_Testing_Guide.md)
- [PWA Test Results](../../docs/PWA_Test_Results.md)
- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)
- [MudBlazor Mobile Integration Spec](../../docs/specs/MudBlazor_Mobile_Integration_Specification.md)
