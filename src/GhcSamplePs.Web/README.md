# GhcSamplePs.Web

Blazor Web App - UI Layer

## Purpose

This project contains the user interface layer of the GhcSamplePs application, including:
- Blazor components (`.razor` files)
- Pages and layouts
- UI-specific services and state management
- Client-side validation and user interaction logic
- **Progressive Web App (PWA) support**
- **Authentication with Azure Entra ID**
- **Role-Based Access Control (RBAC)**

## Dependencies

- **GhcSamplePs.Core** - Business logic and services
- **MudBlazor** - Material Design component library (v8.15.0)
- **Microsoft.Identity.Web** - Microsoft Identity platform integration (v4.1.0)
- **Microsoft.Identity.Web.UI** - Pre-built authentication UI components (v4.1.0)

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── Layout/             # Layout components
│   │   ├── MainLayout.razor     # Main layout with navigation drawer
│   │   └── LoginDisplay.razor   # Authentication UI
│   └── Pages/              # Page components
│       ├── Home.razor
│       ├── Weather.razor   # Protected page
│       ├── Admin/
│       │   └── AdminDashboard.razor  # Admin-only dashboard
│       └── Account/
│           └── AccessDenied.razor
├── Services/               # Web-specific services
│   └── HttpContextCurrentUserProvider.cs
├── wwwroot/                # Static files
│   ├── manifest.json       # PWA manifest
│   ├── icon-192.png        # PWA icon (192x192)
│   └── icon-512.png        # PWA icon (512x512)
├── App.razor               # Root component
├── Program.cs              # Application entry point and DI configuration
└── appsettings.json        # Configuration
```

## Authentication and Authorization ✅

### Azure Entra ID Integration

This application uses **Azure Entra ID External Identities** for authentication and authorization.

#### Implementation Status: ✅ Complete

- ✅ Microsoft.Identity.Web packages installed (v4.1.0)
- ✅ Authentication middleware configured in Program.cs
- ✅ OpenID Connect authentication scheme configured
- ✅ Authorization policies defined (RequireAuthenticatedUser, RequireAdminRole, RequireUserRole)
- ✅ Cascading authentication state enabled for Blazor
- ✅ LoginDisplay component created for sign-in/sign-out
- ✅ Protected page implemented with [Authorize] attribute
- ✅ AccessDenied page created
- ✅ HttpContextCurrentUserProvider bridges Core services with HttpContext
- ✅ Admin Dashboard with role-based protection
- ✅ Navigation drawer with AuthorizeView conditional rendering

#### Authorization Features

- **Role-Based Access Control (RBAC)**: Users are assigned roles (Admin, User)
- **Policy-Based Authorization**: Custom policies for fine-grained control
- **Defense in Depth**: Authorization enforced at both UI and Core layers
- **Conditional UI Rendering**: AuthorizeView components show/hide based on roles

#### Authorization Policies

| Policy | Description | Required Roles |
|--------|-------------|----------------|
| `RequireAuthenticatedUser` | User must be logged in | Any authenticated user |
| `RequireAdminRole` | User must have Admin role | Admin |
| `RequireUserRole` | User must have User or Admin role | User, Admin |

#### Protected Pages

| Page | Route | Protection | Description |
|------|-------|------------|-------------|
| Home | `/` | Authenticated | Main landing page |
| Weather | `/weather` | Authenticated | Weather forecasts |
| Admin Dashboard | `/admin` | Admin Role | Admin-only dashboard |
| Access Denied | `/Account/AccessDenied` | None | Unauthorized access page |

#### Navigation with AuthorizeView

The main layout includes a navigation drawer with conditional rendering based on user roles:

```razor
<MudNavMenu>
    <MudNavLink Href="/" Icon="@Icons.Material.Filled.Home">Home</MudNavLink>
    <AuthorizeView Policy="RequireAdminRole">
        <Authorized>
            <MudNavGroup Title="Admin" Icon="@Icons.Material.Filled.AdminPanelSettings">
                <MudNavLink Href="/admin" Icon="@Icons.Material.Filled.Dashboard">Dashboard</MudNavLink>
            </MudNavGroup>
        </Authorized>
    </AuthorizeView>
</MudNavMenu>
```

Admin navigation items are only visible to users with the Admin role.

#### Configuration

Authentication configuration in `appsettings.json`:

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

**Security Note**: Client Secret must be stored in:
- **Development**: User Secrets (`dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"`)
- **Production**: Azure Key Vault (referenced via environment variables)

#### Service Registration

Services are registered in `Program.cs`:

```csharp
// Authentication
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
```

#### Creating Protected Pages

```razor
@page "/protected"
@attribute [Authorize]

<h1>This page requires authentication</h1>
```

#### Role-Based Protection

```razor
@page "/admin"
@attribute [Authorize(Policy = "RequireAdminRole")]

<h1>This page requires Admin role</h1>
```

#### Policy-Based Protection

```razor
@page "/users"
@attribute [Authorize(Policy = "RequireUserRole")]

<h1>This page requires User or Admin role</h1>
```

#### Conditional Content

```razor
<AuthorizeView>
    <Authorized>
        <p>Welcome, @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>Please sign in.</p>
    </NotAuthorized>
</AuthorizeView>

<AuthorizeView Policy="RequireAdminRole">
    <Authorized>
        <MudButton Href="/admin">Admin Dashboard</MudButton>
    </Authorized>
</AuthorizeView>
```

## Running the Application

```powershell
cd src/GhcSamplePs.Web
dotnet run
```

Visit `https://localhost:5001` in your browser.

## Progressive Web App (PWA) Support

This application is configured as a Progressive Web App, allowing users to install it on their mobile devices.

### PWA Features

- ✅ **Installable**: Can be installed on Android and iOS home screens
- ✅ **Standalone Mode**: Launches without browser UI
- ✅ **App Icons**: Custom icons for home screen
- ✅ **Theme Colors**: Branded colors for mobile browsers
- ✅ **Mobile Optimized**: Responsive design for all screen sizes

See the comprehensive testing guide: [PWA Testing Guide](../../docs/PWA_Testing_Guide.md)

## Development Guidelines

### Component Development

- Keep components focused and small
- Use component parameters for input: `[Parameter]`
- Use EventCallback for output events
- Avoid business logic in `.razor` files - call services instead
- Use `@inject` directive to inject services
- Use MudBlazor components instead of HTML elements where appropriate

### What NOT to Include

- ❌ Business rules or logic (belongs in Core)
- ❌ Direct data access (use services from Core)
- ❌ Complex calculations (use services from Core)
- ❌ Domain models (use DTOs/ViewModels)

## Configuration

Configuration is managed in:
- `appsettings.json` - General settings including the `AzureAd` configuration section
- `appsettings.Development.json` - Development-specific settings
- `Program.cs` - Service registration and middleware
- `manifest.json` - PWA configuration

## See Also

- [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md)
- [Development Environment Setup](../../docs/Development_Environment_Setup.md)
- [PWA Testing Guide](../../docs/PWA_Testing_Guide.md)
- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [Entra ID Specification](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
