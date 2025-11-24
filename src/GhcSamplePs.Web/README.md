# GhcSamplePs.Web

Blazor Web App - UI Layer

## Purpose

This project contains the user interface layer of the GhcSamplePs application, including:
- Blazor components (`.razor` files)
- Pages and layouts
- UI-specific services and state management
- Client-side validation and user interaction logic
- Progressive Web App (PWA) configuration

## Dependencies

- **GhcSamplePs.Core** - Business logic and services

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
├── wwwroot/             # Static files (CSS, JS, images)
├── App.razor            # Root component (includes PWA manifest link)
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
- [MudBlazor Mobile Integration Spec](../../docs/specs/MudBlazor_Mobile_Integration_Specification.md)
