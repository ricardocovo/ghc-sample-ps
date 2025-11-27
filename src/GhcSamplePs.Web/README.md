# GhcSamplePs.Web

Blazor Server UI Layer - Presentation and User Interface

## Purpose

This project contains the Blazor Server web application for the GhcSamplePs solution. It handles all UI rendering, user interactions, authentication flows, and routing. The project follows clean architecture principles, delegating business logic to the Core project.

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Application framework |
| C# | 14 | Programming language |
| Blazor Server | 10.0 | Interactive web UI framework |
| MudBlazor | 8.x | Material Design component library |
| Microsoft.Identity.Web | 4.1.0 | Azure AD authentication |
| Microsoft.Identity.Web.UI | 4.1.0 | Authentication UI components |

## Dependencies

### NuGet Packages

- **MudBlazor** - Material Design component library for Blazor
- **Microsoft.Identity.Web** - Azure AD / Entra ID authentication
- **Microsoft.Identity.Web.UI** - Sign-in/sign-out UI controllers

### Project References

- **GhcSamplePs.Core** - Business logic, domain models, and services

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── App.razor                  # Root HTML and app entry
│   ├── Routes.razor               # Routing configuration
│   ├── _Imports.razor             # Global using directives
│   ├── Layout/
│   │   ├── MainLayout.razor       # Main application layout with MudBlazor
│   │   ├── NavMenu.razor          # Navigation menu component
│   │   └── LoginDisplay.razor     # Authentication status display
│   └── Pages/
│       ├── Home.razor             # Home page with role-based content
│       ├── Weather.razor          # Weather forecast page
│       ├── Error.razor            # Error handling page
│       └── Admin/
│           └── AdminDashboard.razor  # Admin-only dashboard
├── Services/
│   └── HttpContextCurrentUserProvider.cs  # Claims provider bridge
├── Properties/
│   └── launchSettings.json        # Development launch settings
├── wwwroot/
│   └── favicon.ico                # Application favicon
├── appsettings.json               # Production configuration
├── appsettings.Development.json   # Development configuration
├── Program.cs                     # Application startup and DI configuration
└── GhcSamplePs.Web.csproj         # Project file
```

## Features

### Pages

| Route | Component | Description | Authorization |
|-------|-----------|-------------|---------------|
| `/` | Home.razor | Welcome page with role-based content | Authenticated users |
| `/weather` | Weather.razor | Weather forecast display | Authenticated users |
| `/admin` | AdminDashboard.razor | Administrative dashboard | Admin role required |
| `/error` | Error.razor | Error handling page | All users |

### Authentication

The application uses Azure Entra ID External Identities for authentication:

- **Sign-in/Sign-out** - Handled via Microsoft.Identity.Web.UI controllers
- **Claims-based identity** - User claims from Azure AD tokens
- **Role support** - Admin and User roles via app role claims

### Authorization Policies

| Policy | Description |
|--------|-------------|
| `RequireAuthenticatedUser` | Any authenticated user (fallback policy) |
| `RequireAdminRole` | User must have the Admin role |
| `RequireUserRole` | User must have User or Admin role |

### UI Framework

The application uses **MudBlazor** for Material Design components:

- **MudThemeProvider** - Light/dark theme support
- **MudLayout** - Application layout with drawer navigation
- **MudAppBar** - Top application bar
- **MudDrawer** - Collapsible side navigation
- **MudCards, MudGrid, MudButtons** - Content components

### Performance Optimizations

- **Response Compression** - Brotli and Gzip compression enabled
- **Static File Caching** - 1-year cache headers for production
- **Release Build Optimization** - Debug symbols stripped in release builds

## Configuration

### Azure AD / Entra ID Configuration

Configure Azure AD settings in `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-tenant.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

### Secrets Management

Store client secret using user secrets (development):

```bash
cd src/GhcSamplePs.Web
dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"
```

For production, use Azure Key Vault or environment variables.

## Development

### Prerequisites

- .NET 10 SDK
- Code editor (Visual Studio 2022, VS Code, or JetBrains Rider)
- Azure AD tenant configured (see [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md))

### Build and Run

```bash
# From solution root
dotnet build

# Run the web application
cd src/GhcSamplePs.Web
dotnet run
```

Access the application at: `https://localhost:5001`

### HTTPS Certificate

Trust the development certificate:

```bash
dotnet dev-certs https --trust
```

### Hot Reload

For development with hot reload:

```bash
dotnet watch run
```

## Service Registration

Services are configured in `Program.cs`:

```csharp
// Authentication with Azure AD
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));
});

// Core services
builder.Services.AddScoped<ICurrentUserProvider, HttpContextCurrentUserProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// MudBlazor
builder.Services.AddMudServices();
```

## Responsibilities

This project handles:

- ✅ Blazor component rendering and interactivity
- ✅ User interface and layout
- ✅ Routing and navigation
- ✅ Authentication UI flows (sign-in/sign-out)
- ✅ Authorization UI (protected routes, AuthorizeView)
- ✅ Calling Core services for business logic
- ✅ Response compression and static file caching

## What NOT to Include

- ❌ Business logic or calculations
- ❌ Direct database access
- ❌ Domain models (use Core models or DTOs)
- ❌ Complex validation rules (delegate to Core)

## Deployment

### Container Publishing

Build a container image:

```bash
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer
```

### Azure Container Apps

Configure environment variables for production:

```bash
AZUREAD__INSTANCE=https://login.microsoftonline.com/
AZUREAD__DOMAIN=your-tenant.onmicrosoft.com
AZUREAD__TENANTID=your-tenant-id
AZUREAD__CLIENTID=your-client-id
AZUREAD__CLIENTSECRET=@Microsoft.KeyVault(SecretUri=https://...)
```

## Related Documentation

- [Root README](../../README.md) - Solution overview
- [GhcSamplePs.Core README](../GhcSamplePs.Core/README.md) - Business logic layer
- [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md) - Authentication setup
- [Development Environment Setup](../../docs/Development_Environment_Setup.md) - Local development
- [Blazor Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md) - Architecture patterns
- [C# Guidelines](../../.github/instructions/csharp.instructions.md) - Coding standards
