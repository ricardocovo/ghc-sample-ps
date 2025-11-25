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
│       ├── Home.razor           # Landing page with role-specific content
│       ├── Weather.razor        # Protected page
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
- ✅ Home page with AuthorizeView role-specific content examples

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
| Home | `/` | Authenticated | Main landing page with role-specific content |
| Weather | `/weather` | Authenticated | Weather forecasts |
| Admin Dashboard | `/admin` | Admin Role | Admin-only dashboard |
| Access Denied | `/Account/AccessDenied` | None | Unauthorized access page |

### AuthorizeView Patterns

The application uses `AuthorizeView` components to conditionally render UI based on authentication state and user roles. This follows the "defense in depth" principle - UI hides unauthorized features, but backend policies enforce access control.

#### Pattern 1: Navigation with Role-Based Visibility

The main layout includes a navigation drawer with conditional rendering based on user roles:

```razor
<MudNavMenu>
    <MudNavLink Href="/" Icon="@Icons.Material.Filled.Home">Home</MudNavLink>
    <MudNavLink Href="/weather" Icon="@Icons.Material.Filled.WbSunny">Weather</MudNavLink>
    <AuthorizeView Policy="RequireAdminRole">
        <Authorized>
            <MudDivider Class="my-2" />
            <MudNavGroup Title="Admin" Icon="@Icons.Material.Filled.AdminPanelSettings">
                <MudNavLink Href="/admin" Icon="@Icons.Material.Filled.Dashboard">Dashboard</MudNavLink>
            </MudNavGroup>
        </Authorized>
    </AuthorizeView>
</MudNavMenu>
```

Admin navigation items are only visible to users with the Admin role.

#### Pattern 2: Authenticated vs Anonymous Content

Show different content based on whether the user is authenticated:

```razor
<AuthorizeView>
    <Authorized>
        <MudPaper Elevation="2" Class="pa-4 mb-4">
            <MudText Typo="Typo.h6">Welcome, @context.User.Identity?.Name!</MudText>
            <MudText>You are signed in and can access authenticated content.</MudText>
        </MudPaper>
    </Authorized>
    <NotAuthorized>
        <MudPaper Elevation="2" Class="pa-4 mb-4">
            <MudText Typo="Typo.h6">Welcome, Guest!</MudText>
            <MudText>Please sign in to access personalized content.</MudText>
        </MudPaper>
    </NotAuthorized>
</AuthorizeView>
```

#### Pattern 3: Role-Specific Content (Admin Only)

Show content only to users with a specific role:

```razor
<AuthorizeView Policy="RequireAdminRole">
    <Authorized>
        <MudAlert Severity="Severity.Info" Class="mb-4">
            <strong>Admin Access:</strong> You have administrative privileges.
            Visit the <MudLink Href="/admin">Admin Dashboard</MudLink> to manage the application.
        </MudAlert>
    </Authorized>
</AuthorizeView>
```

This pattern is used when you want to show content only to specific roles without showing any fallback content to other users.

#### Pattern 4: User Dashboard (Multiple Roles)

Show content to users with specific roles (using policies that allow multiple roles):

```razor
<AuthorizeView Policy="RequireUserRole">
    <Authorized>
        <MudPaper Elevation="2" Class="pa-4">
            <MudText Typo="Typo.h6">Your Dashboard</MudText>
            <!-- Dashboard content visible to User and Admin roles -->
        </MudPaper>
    </Authorized>
</AuthorizeView>
```

The `RequireUserRole` policy requires either the User or Admin role, allowing content to be visible to all registered users.

#### Pattern 5: Login/Logout Display

The `LoginDisplay` component shows sign-in/sign-out options based on authentication state:

```razor
<AuthorizeView>
    <Authorized>
        <MudMenu Icon="@Icons.Material.Filled.AccountCircle" Color="Color.Inherit">
            <MudMenuItem Disabled="true">
                <MudText>@context.User.Identity?.Name</MudText>
            </MudMenuItem>
            <MudMenuItem OnClick="SignOut">Sign out</MudMenuItem>
        </MudMenu>
    </Authorized>
    <NotAuthorized>
        <MudButton OnClick="SignIn">Sign in</MudButton>
    </NotAuthorized>
</AuthorizeView>
```

### Defense in Depth

The application implements a "defense in depth" strategy:

1. **UI Layer**: `AuthorizeView` hides unauthorized UI elements
2. **Page Protection**: `[Authorize]` attribute prevents page access
3. **Core Layer**: `IAuthorizationService` verifies permissions in business logic

This ensures that even if a user bypasses the UI, the backend will still enforce access control.

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
