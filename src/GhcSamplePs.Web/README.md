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
- `manifest.json` - PWA configuration

## Adding New Features

1. Define service interface and implementation in `GhcSamplePs.Core`
2. Register service in `Program.cs`
3. Create Blazor component in appropriate folder
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
