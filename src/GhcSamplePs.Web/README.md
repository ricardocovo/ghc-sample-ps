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

## Performance Optimization

This project is optimized for mobile performance with the following features:

### Response Compression
- Brotli and Gzip compression enabled for all responses
- Optimized compression levels for production
- Support for HTTPS compression

### Caching Strategy
- Static assets cached for 1 year in production
- Disabled caching in development for faster iteration
- Proper cache-control headers set automatically

### Resource Loading
- Critical resources preloaded (MudBlazor, Blazor framework)
- Optimized script and stylesheet loading order
- Minimal render-blocking resources

### Build Configuration
- Release builds optimized for production
- Debug symbols removed for smaller binaries
- Code optimization enabled

### Performance Targets
- First Contentful Paint (FCP): < 1.5s
- Largest Contentful Paint (LCP): < 2.5s
- Time to Interactive (TTI): < 3s on 4G
- Total bundle size: < 2MB (actual: ~232KB Brotli compressed)
- Lighthouse performance score: > 90

For detailed performance information and testing procedures, see [docs/PERFORMANCE.md](../../docs/PERFORMANCE.md).

## MudBlazor Integration

This project uses MudBlazor 8.15.0 for Material Design components:

- **CSS**: `_content/MudBlazor/MudBlazor.min.css` (44KB compressed)
- **JS**: `_content/MudBlazor/MudBlazor.min.js` (16KB compressed)
- **Services**: Configured in `Program.cs` with `AddMudServices()`

MudBlazor provides:
- Material Design components
- Mobile-first responsive design
- Touch-friendly interfaces
- Built-in dark mode support
- Accessible components (WCAG 2.1 AA)

For MudBlazor documentation, visit: https://mudblazor.com/
