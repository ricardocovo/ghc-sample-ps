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
| `/players` | ManagePlayers.razor | Player listing and management | Authenticated users |
| `/players/create` | CreatePlayer.razor | Create new player form | Authenticated users |
| `/players/edit/{Id:int}` | EditPlayer.razor | Edit player with tabbed interface | Authenticated users |
| `/error` | Error.razor | Error handling page | All users |

### Player Management

The Player Management feature provides comprehensive player data management:

**ManagePlayers.razor** - Player listing page:
- Displays all players in a searchable table
- Search by name with real-time filtering
- Navigation to create and edit players
- MudBlazor components: MudTable, MudTextField (search), MudButton

**CreatePlayer.razor** - Player creation form:
- Form with validation for player details
- Fields: Name (required), Date of Birth (required), Gender (optional)
- Age auto-calculated from date of birth
- MudBlazor components: MudForm, MudTextField, MudDatePicker, MudSelect

**EditPlayer.razor** - Tabbed player editor:
- **Player Tab**: Edit basic player information
- **Teams Tab**: Team assignments management
- **Stats Tab**: Player game statistics management
- MudBlazor components: MudTabs, MudTabPanel, MudForm, MudTextField, MudDatePicker, MudSelect, MudDialog (delete confirmation)

### Team Management (Backend Ready)

The Team Management backend is fully implemented in the Core project:
- **TeamPlayer Entity**: Tracks player team memberships for championships
- **TeamPlayerService**: Full CRUD operations with business validation
- **TeamPlayerValidator**: Validation rules for team assignments
- **EfTeamPlayerRepository**: Database persistence with duplicate detection

The Teams tab in EditPlayer.razor displays a placeholder while the UI implementation is planned for a future release.

#### User Workflows (When UI Complete)

**Adding a Team Assignment:**
1. Navigate to Edit Player page
2. Select Teams tab
3. Click "Add Team" button
4. Enter Team Name, Championship Name, and Joined Date
5. Save - system prevents duplicate active assignments

**Editing a Team Assignment:**
1. Select the team assignment from the list
2. Modify the Left Date if player is leaving team
3. Save changes

**Marking Player as Left Team:**
1. Find the active team assignment
2. Set the Left Date (must be after Joined Date)
3. Save - player status changes from Active to Inactive

### Player Statistics (Backend Ready)

The Player Statistics backend is fully implemented in the Core project:
- **PlayerStatistic Entity**: Tracks game-level performance data for players
- **PlayerStatisticService**: Full CRUD operations with aggregate calculations
- **PlayerStatisticValidator**: Validation rules for statistics data
- **EfPlayerStatisticRepository**: Database persistence with aggregate queries

**Key Features:**
- Track individual game performance statistics
- Record goals, assists, minutes played, jersey number per game
- Mark players as starters or substitutes
- View aggregate statistics (totals and averages)

**MudBlazor Components Used:**
- MudTable - Display statistics in tabular format
- MudTextField - Input for numeric statistics
- MudDatePicker - Game date selection
- MudCheckBox - Starter/substitute toggle
- MudCard - Summary cards for aggregates
- MudDialog - Delete confirmation dialogs
- MudSelect - Team player selection

#### User Workflows (When UI Complete)

**Adding Game Statistics:**
1. Navigate to Edit Player page
2. Select Stats tab
3. Select the team context (team player assignment)
4. Click "Add Game Stats" button
5. Enter game date, minutes played, goals, assists, jersey number
6. Toggle "Started" checkbox if player started the game
7. Save - statistics are recorded for the selected team

**Editing Game Statistics:**
1. Find the game entry in the statistics table
2. Click edit button
3. Modify the values as needed
4. Save changes

**Deleting Game Statistics:**
1. Find the game entry in the statistics table
2. Click delete button
3. Confirm deletion in the dialog
4. Statistics are permanently removed

**Understanding Summary Cards:**
The summary cards display aggregate statistics:
- **Games Played**: Total number of games with recorded statistics
- **Total Goals**: Sum of all goals scored
- **Total Assists**: Sum of all assists made
- **Average Goals**: Goals per game (TotalGoals / GamesPlayed)
- **Average Assists**: Assists per game (TotalAssists / GamesPlayed)

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

### Database Configuration

The application uses Entity Framework Core with SQL Server. DbContext is configured with:

- **Retry Policy** - 5 retries with exponential backoff up to 30 seconds
- **Command Timeout** - 30 seconds for database operations
- **Query Splitting** - Automatic query splitting for complex queries with related data
- **Sensitive Data Logging** - Enabled only in Development environment
- **Detailed Errors** - Enabled only in Development environment

> 📖 **Setup Guide**: For step-by-step connection string configuration, see [Database Connection Setup Guide](../../docs/Database_Connection_Setup.md)

### Health Check Endpoint

A health check endpoint is available at `/health` for load balancers and monitoring:

- **Endpoint**: `GET /health`
- **Authorization**: Anonymous (accessible without authentication)
- **Checks**: Database connectivity via DbContext

The health check is only registered when a valid database connection string is configured.

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
- [Database Connection Setup Guide](../../docs/Database_Connection_Setup.md) - Database connection string configuration
- [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md) - Authentication setup
- [Development Environment Setup](../../docs/Development_Environment_Setup.md) - Local development
- [Blazor Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md) - Architecture patterns
- [C# Guidelines](../../.github/instructions/csharp.instructions.md) - Coding standards
