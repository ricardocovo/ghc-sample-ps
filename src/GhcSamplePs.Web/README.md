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
