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
- **MudBlazor** - Material Design component library for responsive, mobile-friendly UI

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
- Use MudBlazor components for consistent UI

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

### Using MudBlazor Components

```razor
@* Loading indicator *@
<MudProgressCircular Color="Color.Primary" Indeterminate="true" />

@* Simple data table *@
<MudSimpleTable>
    <thead>
        <tr>
            <th>Column 1</th>
            <th>Column 2</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in items)
        {
            <tr>
                <td>@item.Property1</td>
                <td>@item.Property2</td>
            </tr>
        }
    </tbody>
</MudSimpleTable>
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
- `Program.cs` - Service registration and middleware (includes MudBlazor services)

## Adding New Features

1. Define service interface and implementation in `GhcSamplePs.Core`
2. Register service in `Program.cs`
3. Create Blazor component in appropriate folder
4. Use MudBlazor components for UI elements
5. Inject and use service in component
6. Test business logic in `GhcSamplePs.Core.Tests`

## Mobile Optimization

The application is optimized for mobile devices:
- Responsive layouts using MudBlazor's Material Design system
- Touch-friendly interactive elements
- Horizontal scrolling for wide tables on narrow screens
- Mobile-first approach to component design

## See Also

- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)
- [MudBlazor Documentation](https://mudblazor.com/)
- [MudBlazor Mobile Integration Spec](../../docs/specs/MudBlazor_Mobile_Integration_Specification.md)
