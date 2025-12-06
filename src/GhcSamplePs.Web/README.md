# GhcSamplePs.Web

Blazor Server UI Layer - Progressive Web Application

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![MudBlazor](https://img.shields.io/badge/MudBlazor-8.x-594AE2)](https://mudblazor.com/)

## Purpose

This project contains the Blazor Server UI layer for the GhcSamplePs application. It provides a responsive, interactive web interface with mobile-first design and Progressive Web App (PWA) capabilities.

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 10.0 | Runtime framework |
| **Blazor Server** | 10.0 | Interactive web UI with SignalR |
| **MudBlazor** | 8.x | Material Design component library |
| **Microsoft Identity Web** | 4.1.0 | Entra ID authentication |
| **Entity Framework Core** | 10.0 | Database access via Core project |
| **Azure Identity** | 1.14.2 | Azure Managed Identity support |
| **Azure Data Protection** | 1.3.4 / 1.2.4 | Multi-instance key management |

## Project Architecture

This project follows **clean architecture** principles with strict separation of concerns:

### Dependency Direction

```
GhcSamplePs.Web → GhcSamplePs.Core
```

✅ **Web depends on Core** - UI layer references business logic  
❌ **Core never depends on Web** - Business logic remains UI-agnostic

### Layer Responsibilities

**UI Layer (This Project):**
- Blazor components and pages
- User interaction handling
- Client-side validation
- UI state management
- Display logic and formatting
- Navigation and routing

**Business Logic Layer (Core):**
- Domain models and entities
- Business rules and validation
- Data access (repositories, DbContext)
- Service interfaces and implementations
- Authentication and authorization logic

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── App.razor              # Root component
│   ├── Routes.razor           # Routing configuration
│   ├── _Imports.razor         # Global using statements
│   ├── Layout/                # Layout components
│   │   ├── MainLayout.razor   # Primary app layout
│   │   ├── NavMenu.razor      # Navigation menu
│   │   └── ...
│   └── Pages/                 # Routable page components
│       ├── Home.razor         # Dashboard/home page
│       ├── Error.razor        # Error page
│       ├── PlayerManagement/ # Player CRUD pages
│       │   ├── ManagePlayers.razor
│       │   ├── CreatePlayer.razor
│       │   ├── EditPlayer.razor
│       │   └── ...
│       └── Admin/            # Admin-only pages
│           └── ...
├── Helpers/                   # UI helper classes
│   └── AuthorizationHelper.cs
├── Services/                  # UI-specific services
│   └── HttpContextCurrentUserProvider.cs
├── wwwroot/                   # Static assets
│   ├── css/                   # Stylesheets
│   ├── js/                    # JavaScript
│   ├── manifest.json          # PWA manifest
│   ├── service-worker.js      # PWA service worker
│   └── icon-*.png             # PWA icons
├── Properties/
│   └── launchSettings.json    # Development settings
├── appsettings.json           # Configuration
├── appsettings.Development.json
├── Program.cs                 # Application startup
├── Dockerfile                 # Container image definition
└── .dockerignore             # Docker build exclusions
```

## Key Features

### 🎨 Modern UI Components

- **Material Design** - MudBlazor component library for consistent, beautiful UI
- **Responsive Layout** - Mobile-first design that adapts to all screen sizes
- **Dark Mode Support** - User-selectable theme preference
- **Accessibility** - ARIA labels and keyboard navigation support

### 🔐 Authentication & Authorization

- **Microsoft Entra ID Integration** - Enterprise-grade authentication
- **Role-Based Access Control** - Admin and User roles with policy-based authorization
- **Claims-Based Identity** - Rich user profile information from Azure AD
- **Token Refresh Handling** - Automatic token refresh with retry logic for transient failures
- **Authorization Policies**:
  - `RequireAuthenticatedUser` - Any authenticated user
  - `RequireAdminRole` - Admin role required
  - `RequireUserRole` - User or Admin role required

### 👤 Player Management

- **CRUD Operations** - Create, read, update, delete players
- **Profile Photos** - Upload and display player photos
- **Advanced Search** - Filter by name, age, gender
- **Sortable Columns** - Sort by name, age, date of birth
- **Pagination** - Efficient browsing of large player lists

### 🏆 Team Management

- **Team Assignments** - Assign players to teams for championships
- **Active/Inactive Tracking** - Manage player join/leave dates
- **Multi-Team Support** - Players can be on multiple teams simultaneously
- **Championship Context** - Track teams per championship/season

### 📊 Player Statistics

- **Game-Level Tracking** - Record statistics for each game
- **Performance Metrics**:
  - Minutes played
  - Goals scored
  - Assists made
  - Starting lineup status
  - Jersey number
- **Aggregate Calculations**:
  - Total games played
  - Total goals/assists/minutes
  - Averages per game
- **Team-Specific Views** - Filter statistics by team assignment
- **Date Range Filtering** - View statistics for specific time periods

### 📱 Progressive Web App (PWA)

- **Installable** - Add to home screen on mobile devices
- **Offline Support** - Service worker caching for offline functionality
- **App-Like Experience** - Runs in standalone mode
- **Fast Loading** - Optimized asset caching
- **Responsive Icons** - Multiple icon sizes for different devices

### 🚀 Performance Optimizations

- **Response Compression** - Brotli and Gzip compression enabled
- **Static Asset Caching** - 1-year cache headers in production
- **Async Operations** - All I/O operations use async/await
- **Efficient Queries** - Pagination and filtering on server side
- **Session Affinity** - Sticky sessions for Blazor Server SignalR

### 🏥 Health Monitoring

- **Health Check Endpoint** - `/health` for load balancer probes
- **Database Connectivity** - Verifies EF Core DbContext health
- **Anonymous Access** - Health checks don't require authentication

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server, LocalDB, or SQL Express (for database)
- Visual Studio 2022 or VS Code with C# extension
- Microsoft Entra ID tenant (for authentication)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/ricardocovo/ghc-sample-ps.git
   cd ghc-sample-ps
   ```

2. **Configure Database Connection**

   See [Database Connection Setup Guide](../../docs/Database_Connection_Setup.md) for detailed instructions.

   **Quick Start (User Secrets):**
   ```bash
   cd src/GhcSamplePs.Web
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true"
   ```

3. **Configure Entra ID Authentication**

   Update `appsettings.json` with your Entra ID app registration:
   ```json
   {
     "AzureAd": {
       "Instance": "https://login.microsoftonline.com/",
       "Domain": "your-tenant.onmicrosoft.com",
       "TenantId": "your-tenant-id",
       "ClientId": "your-client-id",
       "CallbackPath": "/signin-oidc"
     }
   }
   ```

   See [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md) for complete configuration.

4. **Run the application**
   ```bash
   cd src/GhcSamplePs.Web
   dotnet run
   ```

5. **Access the application**
   - Open browser to `https://localhost:7294` (or port shown in console)
   - Sign in with your Entra ID credentials

## Development Guidelines

### Creating New Pages

1. **Create the Razor component** in `Components/Pages/[Feature]/`
   ```razor
   @page "/mypage"
   @attribute [Authorize]
   @inject IMyService MyService
   
   <PageTitle>My Page</PageTitle>
   
   <MudContainer MaxWidth="MaxWidth.Large">
       <MudText Typo="Typo.h3">My Page</MudText>
       <!-- Component content -->
   </MudContainer>
   
   @code {
       // Component logic here
   }
   ```

2. **Add navigation link** in `Components/Layout/NavMenu.razor`

3. **Follow naming conventions**:
   - PascalCase for component names
   - Descriptive route names
   - Group related pages in folders

### Component Best Practices

✅ **DO:**
- Keep components small and focused
- Use `@inject` for dependency injection
- Call Core services for business logic
- Handle loading and error states
- Use MudBlazor components for consistency
- Make components responsive

❌ **DON'T:**
- Put business logic in components
- Access database directly from components
- Create large monolithic components
- Ignore error handling
- Mix UI and business concerns

### Service Injection Pattern

```razor
@page "/players"
@inject IPlayerService PlayerService
@inject IAuthorizationService AuthorizationService
@inject ISnackbar Snackbar

@code {
    private List<PlayerDto> players = new();
    
    protected override async Task OnInitializedAsync()
    {
        var authResult = await AuthorizationService.AuthorizeAsync("RequireUserRole");
        if (!authResult.Success)
        {
            Snackbar.Add("Access denied", Severity.Error);
            return;
        }
        
        var result = await PlayerService.GetAllPlayersAsync();
        if (result.Success)
        {
            players = result.Data!.ToList();
        }
        else
        {
            Snackbar.Add("Failed to load players", Severity.Error);
        }
    }
}
```

### Authorization in Components

Use the `AuthorizationHelper` to check permissions:

```razor
@inject IAuthorizationService AuthService

@code {
    private bool canEdit = false;
    
    protected override async Task OnInitializedAsync()
    {
        canEdit = await AuthorizationHelper.CanEditAsync(
            AuthService, 
            player.UserId);
    }
}
```

Or use declarative authorization:

```razor
@attribute [Authorize(Policy = "RequireAdminRole")]
```

### Validation Pattern

```razor
@using FluentValidation

<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <MudTextField @bind-Value="model.Name" 
                  Label="Name" 
                  Required="true"
                  For="@(() => model.Name)" />
    
    <MudButton ButtonType="ButtonType.Submit" 
               Variant="Variant.Filled" 
               Color="Color.Primary">
        Save
    </MudButton>
</EditForm>

@code {
    private async Task HandleValidSubmit()
    {
        var result = await PlayerService.CreatePlayerAsync(model);
        if (result.Success)
        {
            Snackbar.Add("Player created", Severity.Success);
            NavigationManager.NavigateTo("/players");
        }
        else
        {
            foreach (var error in result.ValidationErrors.SelectMany(e => e.Value))
            {
                Snackbar.Add(error, Severity.Error);
            }
        }
    }
}
```

## Configuration

### Development Settings

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "DetailedErrors": true
}
```

### Production Settings

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "See environment variables or Key Vault"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-tenant.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc"
  }
}
```

### User Secrets (Development)

Store sensitive configuration locally:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
dotnet user-secrets set "AzureAd:ClientSecret" "your-client-secret"
```

### Environment Variables (Production)

Configure via Azure Container Apps or hosting environment:

- `ConnectionStrings__DefaultConnection`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `KeyVault__VaultUri`
- `Storage__BlobEndpoint`

## Service Registration

Services are registered in `Program.cs`:

```csharp
// Core services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<ITeamPlayerService, TeamPlayerService>();
builder.Services.AddScoped<IPlayerStatisticService, PlayerStatisticService>();

// Authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole("User", "Admin"));
});

// UI services
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, HttpContextCurrentUserProvider>();
```

## Deployment

### Docker Container

Build the container image:

```bash
# From repository root
docker build -t ghcsampleps-web -f src/GhcSamplePs.Web/Dockerfile .
```

Run locally:

```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  -e AzureAd__TenantId="your-tenant-id" \
  -e AzureAd__ClientId="your-client-id" \
  ghcsampleps-web
```

### Azure Container Apps

See [Infrastructure README](../../infra/README.md) for complete deployment guide.

**Quick Deploy:**

```powershell
# Deploy infrastructure
.\infra\scripts\deploy-infra.ps1 -ResourceGroupName "rg-ghcsampleps-dev"

# Build and push image
.\infra\scripts\build-push-image.ps1 -RegistryName "acrghcsamplepsdev"

# Update container app
az containerapp update `
    --name ghcsampleps-dev-app `
    --resource-group rg-ghcsampleps-dev `
    --image acrghcsamplepsdev.azurecr.io/ghcsampleps-web:latest
```

### Health Checks

The application exposes a health endpoint for monitoring:

**Endpoint:** `GET /health`

**Response (Healthy):**
```
HTTP/1.1 200 OK
Healthy
```

**Response (Unhealthy):**
```
HTTP/1.1 503 Service Unavailable
Unhealthy
```

Health checks verify:
- Application is running
- Database connectivity (via EF Core DbContext)

## Testing

### Running the Application

```bash
# Development mode (with hot reload)
cd src/GhcSamplePs.Web
dotnet watch run

# Production mode
dotnet run --configuration Release
```

### Manual Testing Checklist

- [ ] Sign in with Entra ID
- [ ] Navigate all menu items
- [ ] Create a new player
- [ ] Edit an existing player
- [ ] Upload player photo
- [ ] Search and filter players
- [ ] Assign player to team
- [ ] Add game statistics
- [ ] View statistics aggregates
- [ ] Test responsive layout (mobile, tablet, desktop)
- [ ] Test dark mode toggle
- [ ] Verify authorization (admin vs user)
- [ ] Test error handling

### Browser Compatibility

Tested and supported on:
- Chrome 120+
- Firefox 120+
- Safari 17+
- Edge 120+

## Troubleshooting

### Application Won't Start

**Error:** `Unable to bind to https://localhost:7294`

**Solution:** Port is already in use. Change port in `launchSettings.json` or stop conflicting process.

---

**Error:** `SQL connection error`

**Solution:** Verify connection string is configured. See [Database Setup Guide](../../docs/Database_Connection_Setup.md).

### Authentication Fails

**Error:** `AADSTS50011: The reply URL specified in the request does not match`

**Solution:** Update redirect URIs in Entra ID app registration:
- Add: `https://localhost:7294/signin-oidc`
- Add: `https://localhost:7294/signout-callback-oidc`

See [Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md).

### Blazor Circuit Disconnected

**Symptoms:** "Attempting to reconnect to the server" message

**Causes:**
- Network interruption
- Server restart
- SignalR timeout

**Solutions:**
- Refresh the page
- Check network connectivity
- Verify server is running
- Review SignalR configuration in production

### Database Migration Errors

**Error:** `Pending model changes detected`

**Solution:** Apply pending migrations:
```bash
cd src/GhcSamplePs.Web
dotnet ef database update --project ../GhcSamplePs.Core
```

## Performance Considerations

### Blazor Server Characteristics

- **Server-Side Rendering** - All UI interactions processed on server
- **SignalR Connection** - Maintains persistent WebSocket connection
- **Stateful** - UI state maintained on server per user
- **Session Affinity Required** - Same user must route to same server instance

### Optimization Strategies

✅ **Enabled:**
- Response compression (Brotli, Gzip)
- Static asset caching (1-year in production)
- Async operations throughout
- Pagination for large datasets
- Efficient EF Core queries (AsNoTracking for reads)

📊 **Monitoring:**
- Application Insights integration
- Health check endpoint
- Structured logging
- Performance metrics

## Security

### Authentication

- Microsoft Entra ID (Azure AD) integration
- OAuth 2.0 / OpenID Connect
- JWT token validation
- Session-based authentication

### Token Refresh Handling

The application implements automatic token refresh with resilience features:

- **Automatic Token Refresh** - Tokens are saved and automatically refreshed on expiration
- **Retry Policy** - Exponential backoff with jitter for transient failures
  - 3 retry attempts
  - Delays: ~2s, ~4s, ~8s with random jitter (0-1s)
  - Handles HTTP 5xx errors and network timeouts
- **Event Logging** - All token events are logged for monitoring
  - Token validation success
  - Authentication failures (with exception type classification)
  - Remote authentication failures
- **Graceful Failure Handling** - Users redirected to sign-in on unrecoverable errors
- **Backchannel Timeout** - 30-second timeout for token operations

### Authorization

- Role-based access control (Admin, User)
- Policy-based authorization
- Resource-based authorization
- Claims-based identity

### Data Protection

- Azure Key Vault integration for secrets
- Multi-instance key management (Azure Blob + Key Vault)
- HTTPS enforced
- TLS 1.2 minimum
- Secure cookie settings

### Best Practices Implemented

✅ HTTPS-only communication  
✅ Anti-forgery token validation  
✅ CORS configured appropriately  
✅ Content Security Policy headers  
✅ Secure authentication cookies  
✅ Input validation on all forms  
✅ Output encoding to prevent XSS  
✅ SQL injection prevention (parameterized queries)

## Additional Resources

### Documentation

- [Database Connection Setup](../../docs/Database_Connection_Setup.md) - Connection string configuration
- [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md) - Authentication configuration
- [Player Statistics User Guide](../../docs/Player_Statistics_User_Guide.md) - Statistics feature guide
- [Team Management User Guide](../../docs/Team_Management_User_Guide.md) - Team feature guide
- [PWA Implementation Summary](../../docs/PWA_Implementation_Summary.md) - Progressive Web App details

### Architecture Guidelines

- [Blazor Architecture](../../.github/instructions/blazor-architecture.instructions.md) - Architecture patterns
- [C# Guidelines](../../.github/instructions/csharp.instructions.md) - C# coding standards
- [DDD Best Practices](../../.github/instructions/dotnet-architecture-good-practices.instructions.md) - Architecture principles

### Infrastructure

- [Infrastructure README](../../infra/README.md) - Azure deployment guide
- [High-Level Architecture](../../docs/infra/high-level.md) - Architecture decisions
- [Implementation Plan](../../docs/infra/implementation-plan.md) - Deployment specifications

## Contributing

When making changes to the UI:

1. Follow clean architecture principles - no business logic in components
2. Use MudBlazor components for consistency
3. Implement responsive design
4. Add proper error handling
5. Test on multiple screen sizes
6. Update relevant documentation
7. Use conventional commit messages

## License

This project is part of the GhcSamplePs solution. See the main repository LICENSE file for details.

---

**Last Updated:** December 3, 2025  
**Version:** 1.0.0  
**Framework:** .NET 10.0  
**Build Status:** ✅ All 802 tests passing
