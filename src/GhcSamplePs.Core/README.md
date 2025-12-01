# GhcSamplePs.Core

Business Logic Layer - UI-agnostic class library

## Purpose

This project contains the business logic, domain models, and services for the GhcSamplePs application. It is completely UI-agnostic and fully testable.

## Dependencies

- `Microsoft.Extensions.Logging.Abstractions` - For logging in services
- `Microsoft.EntityFrameworkCore.SqlServer` - For database access with SQL Server

This project should not reference the Web project or any UI-specific libraries.

## Project Structure

```
GhcSamplePs.Core/
├── Common/                      # Common utilities and result types
│   ├── ServiceResult.cs
│   └── ValidationResult.cs
├── Data/                        # Database context and configurations
│   ├── ApplicationDbContext.cs
│   └── Configurations/
│       └── PlayerConfiguration.cs
├── Extensions/                  # Extension methods
│   └── ServiceCollectionExtensions.cs
├── Migrations/                  # Database migrations
│   ├── [timestamp]_InitialCreate.cs
│   ├── [timestamp]_InitialCreate.Designer.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── Identity/                # User identity domain models
│   │   ├── ApplicationUser.cs
│   │   └── UserClaim.cs
│   └── PlayerManagement/        # Player management domain models
│       ├── DTOs/
│       │   ├── CreatePlayerDto.cs
│       │   ├── CreateTeamPlayerDto.cs
│       │   ├── PlayerDto.cs
│       │   ├── TeamPlayerDto.cs
│       │   ├── UpdatePlayerDto.cs
│       │   └── UpdateTeamPlayerDto.cs
│       ├── Player.cs
│       └── TeamPlayer.cs
├── Services/
│   ├── Interfaces/              # Service contracts
│   │   ├── IAuthenticationService.cs
│   │   ├── IAuthorizationService.cs
│   │   ├── ICurrentUserProvider.cs
│   │   ├── IPlayerService.cs
│   │   ├── ITeamPlayerService.cs
│   │   └── AuthorizationResult.cs
│   └── Implementations/         # Service implementations
│       ├── AuthenticationService.cs
│       ├── AuthorizationService.cs
│       ├── PlayerService.cs
│       └── TeamPlayerService.cs
├── Repositories/                # Repository interfaces and implementations
│   ├── Interfaces/
│   │   ├── IPlayerRepository.cs
│   │   └── ITeamPlayerRepository.cs
│   └── Implementations/
│       ├── MockPlayerRepository.cs
│       ├── EfPlayerRepository.cs
│       └── EfTeamPlayerRepository.cs
├── Validation/                  # Business validation rules
│   ├── PlayerValidator.cs
│   └── TeamPlayerValidator.cs
└── Exceptions/                  # Custom domain exceptions
    ├── AuthenticationException.cs
    ├── AuthorizationException.cs
    ├── PlayerNotFoundException.cs
    ├── PlayerValidationException.cs
    ├── RepositoryException.cs
    └── TokenValidationException.cs
```

## Responsibilities

- Business rules and validation
- Domain logic and calculations
- Data access (repositories, queries)
- Service interfaces and implementations
- Domain entities and models

## What NOT to Include

- ❌ UI components or Blazor-specific code
- ❌ HTTP context or request/response handling
- ❌ UI state management
- ❌ Any reference to GhcSamplePs.Web

## Development Guidelines

### Creating Services

1. Define interface in `Services/Interfaces/`
2. Implement interface in `Services/Implementations/`
3. Use dependency injection for dependencies
4. Make services stateless where possible
5. Use async/await for I/O operations

Example:
```
Services/
├── Interfaces/
│   └── IUserService.cs
└── Implementations/
    └── UserService.cs
```

### Business Logic Best Practices

- Validate input at service boundaries
- Use precise exception types
- Log important operations
- Return meaningful results (success/failure with errors)
- Keep services focused on single responsibility

### Testing

All public methods in this project should have corresponding unit tests in `GhcSamplePs.Core.Tests`.

## Service Registration

Services from this project are registered in `GhcSamplePs.Web/Program.cs`:

```csharp
builder.Services.AddScoped<ICurrentUserProvider, HttpContextCurrentUserProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
```

## Implemented Features

### Authentication and Authorization Services ✅

#### Service Implementations

**AuthenticationService** (`Services/Implementations/AuthenticationService.cs`):
- Retrieves and validates authenticated user information from claims
- Methods:
  - `GetCurrentUserAsync()` - Get current authenticated user as ApplicationUser
  - `GetUserClaimsAsync()` - Get all user claims as dictionary
  - `GetUserRolesAsync()` - Get user roles as list
  - `IsInRoleAsync(roleName)` - Check role membership
  - `HasClaimAsync(type, value)` - Check specific claim
- Maps standard claims (ObjectIdentifier, Email, Name, etc.) to ApplicationUser
- Depends on `ICurrentUserProvider` for HTTP context abstraction

**AuthorizationService** (`Services/Implementations/AuthorizationService.cs`):
- Performs authorization checks and determines user permissions
- Methods:
  - `AuthorizeAsync(policyName)` - Check policy requirements
  - `AuthorizeAsync(resource, policyName)` - Resource-based authorization
  - `CanAccessAsync(resourceId)` - Check resource access (admins access all, users access own)
  - `GetUserPermissionsAsync()` - Get permissions based on roles
- Built-in policies: RequireAuthenticatedUser, RequireAdminRole, RequireUserRole
- Built-in roles: Admin, User

**ICurrentUserProvider** (`Services/Interfaces/ICurrentUserProvider.cs`):
- Abstraction for accessing current user's ClaimsPrincipal
- Allows Core to remain UI-agnostic while accessing user claims
- Implemented in Web project via HttpContextAccessor

#### Authorization Policy Evaluation

The AuthorizationService evaluates policies based on user roles:

| Policy | Required Role(s) | Description |
|--------|-----------------|-------------|
| `RequireAuthenticatedUser` | None | Any authenticated user |
| `RequireAdminRole` | Admin | User must have Admin role |
| `RequireUserRole` | User or Admin | User must have User or Admin role |

#### Permission Model

Permissions are derived from roles:

| Role | Permissions |
|------|-------------|
| Admin | read, write, delete, admin.users, admin.settings |
| User | read, write |

#### Resource Access Model

- **Admin users** can access any resource
- **Regular users** can only access resources matching their user ID (case-insensitive comparison)

#### Identity Models

**ApplicationUser** (`Models/Identity/ApplicationUser.cs`):
- Represents an authenticated user with identity information from Entra ID claims
- Properties: Id, Email, DisplayName, GivenName, FamilyName, Roles, Claims, IsActive, LastLoginDate, CreatedDate
- Methods: IsInRole, HasClaim, TryGetClaim

**UserClaim** (`Models/Identity/UserClaim.cs`):
- Represents a custom claim with type, value, issuer, and optional expiration
- Methods: IsValid, Equals, GetHashCode, ToString

#### Custom Exceptions

- **AuthenticationException** - User authentication failures
- **AuthorizationException** - Permission/access denials
- **TokenValidationException** - JWT token validation failures

### Player Validation ✅

**PlayerValidator** (`Validation/PlayerValidator.cs`):
- Provides business rule validation for player data
- Validates CreatePlayerDto, UpdatePlayerDto, and Player entities
- Returns ValidationResult with field-specific error messages

#### Validation Rules

| Field | Requirement | Error Message |
|-------|-------------|---------------|
| Name | Required, 1-200 characters, not whitespace | "Name is required" or "Name must not exceed 200 characters" |
| DateOfBirth | Required, past date, not more than 100 years ago | "Date of birth is required", "Date of birth must be in the past", "Date of birth cannot be more than 100 years ago" |
| Gender | Optional, if provided must be valid option | "Gender must be Male, Female, Non-binary, or Prefer not to say" |
| PhotoUrl | Optional, max 500 chars, valid HTTP/HTTPS URL | "Photo URL must not exceed 500 characters", "Photo URL must be a valid HTTP or HTTPS URL" |

#### Valid Gender Options

- Male
- Female
- Non-binary
- Prefer not to say

(Case-insensitive comparison)

#### Methods

- `ValidateCreatePlayer(CreatePlayerDto)` - Validates player creation data
- `ValidateUpdatePlayer(UpdatePlayerDto)` - Validates player update data
- `ValidatePlayer(Player)` - Validates a Player entity

#### Usage Example

```csharp
var dto = new CreatePlayerDto
{
    UserId = "user-123",
    Name = "John Doe",
    DateOfBirth = new DateTime(1990, 6, 15),
    Gender = "Male"
};

var result = PlayerValidator.ValidateCreatePlayer(dto);
if (!result.IsValid)
{
    foreach (var (field, messages) in result.Errors)
    {
        Console.WriteLine($"{field}: {string.Join(", ", messages)}");
    }
}
```

### Entity Framework Core Database Context ✅

**ApplicationDbContext** (`Data/ApplicationDbContext.cs`):
- Entity Framework Core 10 DbContext for managing database connections and entity tracking
- Automatic audit field population (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- Entity configurations using Fluent API

#### Features

- **Automatic Audit Fields**: SaveChanges/SaveChangesAsync automatically sets:
  - `CreatedAt` and `CreatedBy` for new entities
  - `UpdatedAt` and `UpdatedBy` for modified entities
  
- **Entity Configurations** (Fluent API):
  - `PlayerConfiguration` - Configures Player entity with proper constraints and indexes

#### Player Entity Configuration

| Property | Type | Constraints |
|----------|------|-------------|
| Id | int | Primary key, auto-generated |
| UserId | string | Required, max 450 chars, indexed |
| Name | string | Required, max 200 chars, indexed |
| DateOfBirth | DateTime | Required, indexed |
| Gender | string | Optional, max 50 chars |
| PhotoUrl | string | Optional, max 500 chars |
| CreatedAt | DateTime | Required |
| CreatedBy | string | Required, max 450 chars |
| UpdatedAt | DateTime? | Optional |
| UpdatedBy | string? | Optional, max 450 chars |
| Age | int | Ignored (computed property) |

#### Indexes

- `IX_Players_UserId` - For filtering by user
- `IX_Players_Name` - For search operations
- `IX_Players_DateOfBirth` - For age-based queries

#### TeamPlayer Entity Configuration

| Property | Type | Constraints |
|----------|------|-------------|
| TeamPlayerId | int | Primary key, auto-generated |
| PlayerId | int | Required, FK → Players(Id), indexed, cascade delete |
| TeamName | string | Required, max 200 chars, indexed |
| ChampionshipName | string | Required, max 200 chars |
| JoinedDate | DateTime | Required |
| LeftDate | DateTime? | Optional (null = active) |
| CreatedAt | DateTime | Required |
| CreatedBy | string | Required, max 450 chars |
| UpdatedAt | DateTime? | Optional |
| UpdatedBy | string? | Optional, max 450 chars |
| IsActive | bool | Computed property (LeftDate is null) |

#### TeamPlayer Indexes

- `IX_TeamPlayers_PlayerId` - For retrieving player's teams
- `IX_TeamPlayers_TeamName` - For team-based queries
- `IX_TeamPlayers_IsActive` - For filtering active/inactive assignments (on LeftDate column)
- `IX_TeamPlayers_PlayerId_IsActive` - Composite index for active player teams
- `IX_TeamPlayers_PlayerId_TeamName_ChampionshipName` - For duplicate detection

#### TeamPlayer Entity Methods

| Method | Description |
|--------|-------------|
| `Validate()` | Validates entity against all business rules |
| `MarkAsLeft(leftDate, userId)` | Sets LeftDate and updates audit fields |
| `UpdateLastModified(userId)` | Updates audit fields without changing LeftDate |

#### Registration

Use the extension method in `Program.cs`:

```csharp
// SQL Server configuration with retry policies
builder.Services.AddApplicationDbContext(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
    enableSensitiveDataLogging: builder.Environment.IsDevelopment(),
    enableDetailedErrors: builder.Environment.IsDevelopment(),
    maxRetryCount: 5,
    maxRetryDelaySeconds: 30,
    commandTimeoutSeconds: 30);
```

#### ServiceCollectionExtensions Configuration Options

| Parameter | Default | Description |
|-----------|---------|-------------|
| `connectionString` | Required | SQL Server connection string |
| `enableSensitiveDataLogging` | false | Log query parameter values (dev only) |
| `enableDetailedErrors` | false | Include detailed error information (dev only) |
| `maxRetryCount` | 5 | Maximum retry attempts for transient failures |
| `maxRetryDelaySeconds` | 30 | Maximum delay between retries (seconds) |
| `commandTimeoutSeconds` | 30 | Database command timeout (seconds) |

Additional configuration applied automatically:
- **EnableRetryOnFailure** - Exponential backoff retry policy for transient failures
- **UseQuerySplittingBehavior** - SplitQuery for better performance with related data

#### Connection String Configuration

> 📖 **Complete Guide**: For step-by-step setup instructions, see [Database Connection Setup Guide](../../docs/Database_Connection_Setup.md)

**Development** - Use User Secrets (recommended):
```bash
cd src/GhcSamplePs.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Production**: Configure via environment variables or Azure Key Vault.

### Database Migrations ✅

**InitialCreate** (`Migrations/[timestamp]_InitialCreate.cs`):
- Creates the `Players` table with all columns and constraints
- Creates indexes for UserId, Name, and DateOfBirth

#### Running Migrations

**Development**: Migrations are applied automatically on application startup when a connection string is configured.

**Manual Commands**:
```bash
# Add a new migration
dotnet ef migrations add MigrationName --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web

# Apply migrations to database
dotnet ef database update --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web

# Generate idempotent SQL script for production
dotnet ef migrations script --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web --idempotent --output migration.sql
```

#### Migration Scripts

Pre-generated idempotent migration scripts are available in `docs/migrations/`:
- `InitialCreate.sql` - Creates Players table and indexes

These scripts are safe to run multiple times and can be used for production deployments.

### Player Repository ✅

Two repository implementations are available for player data access:

#### EfPlayerRepository (Production)

**EfPlayerRepository** (`Repositories/Implementations/EfPlayerRepository.cs`):
- Entity Framework Core implementation of IPlayerRepository
- Full database persistence with SQL Server
- Optimized read operations using AsNoTracking
- Comprehensive error handling and logging

**Features:**
- **CRUD Operations**: GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, ExistsAsync
- **Performance**: AsNoTracking for read-only queries
- **Error Handling**: Catches and translates DbUpdateException, DbUpdateConcurrencyException
- **Logging**: Detailed operation logging at appropriate levels
- **Cancellation Support**: All methods support CancellationToken

**Exception Types:**
- `PlayerNotFoundException` - Player with specified ID not found
- `RepositoryException` - Database operation failures with context information

**Usage Example:**
```csharp
// Register in DI (production configuration)
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();

// Use in a service
public class PlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IPlayerRepository repository, ILogger<PlayerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Player?> GetPlayerAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex, "Failed to retrieve player {PlayerId}", id);
            throw;
        }
    }
}
```

#### MockPlayerRepository (Development/Testing)

**MockPlayerRepository** (`Repositories/Implementations/MockPlayerRepository.cs`):
- In-memory implementation for development and testing
- Pre-seeded with 10 sample players
- Thread-safe using ConcurrentDictionary

**Use Cases:**
- Local development without database
- Unit testing services without EF Core dependency
- Demos and prototyping

### TeamPlayer Repository ✅

**EfTeamPlayerRepository** (`Repositories/Implementations/EfTeamPlayerRepository.cs`):
- Entity Framework Core implementation of ITeamPlayerRepository
- Manages player team assignments for championships
- Full CRUD operations with player-specific queries
- Active duplicate detection to prevent conflicting team memberships

**Features:**
- **CRUD Operations**: GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, ExistsAsync
- **Player-Specific Queries**: GetAllByPlayerIdAsync, GetActiveByPlayerIdAsync (ordered by JoinedDate DESC)
- **Duplicate Detection**: HasActiveDuplicateAsync checks for existing active team assignments
- **Performance**: AsNoTracking for all read-only queries
- **Error Handling**: RepositoryException with operation context
- **Logging**: Detailed operation logging at appropriate levels
- **Cancellation Support**: All methods support CancellationToken

**Key Methods:**

| Method | Description |
|--------|-------------|
| `GetAllByPlayerIdAsync(playerId)` | Get all team assignments for a player (active and inactive) |
| `GetActiveByPlayerIdAsync(playerId)` | Get only active team assignments (where LeftDate is null) |
| `HasActiveDuplicateAsync(playerId, teamName, championshipName, excludeId)` | Check if player already has an active assignment to the same team/championship |

**Usage Example:**
```csharp
// Register in DI
builder.Services.AddScoped<ITeamPlayerRepository, EfTeamPlayerRepository>();

// Use in a service
public class TeamPlayerService
{
    private readonly ITeamPlayerRepository _repository;

    public async Task<ServiceResult> AssignPlayerToTeamAsync(
        int playerId, 
        string teamName, 
        string championshipName,
        CancellationToken cancellationToken)
    {
        // Check for existing active assignment
        if (await _repository.HasActiveDuplicateAsync(
            playerId, teamName, championshipName, cancellationToken: cancellationToken))
        {
            return ServiceResult.Failure("Player is already active on this team in this championship");
        }

        var teamPlayer = new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = championshipName,
            JoinedDate = DateTime.UtcNow,
            CreatedBy = "system"
        };

        await _repository.AddAsync(teamPlayer, cancellationToken);
        return ServiceResult.Success();
    }

    public async Task<IReadOnlyList<TeamPlayer>> GetActiveTeamsAsync(
        int playerId, 
        CancellationToken cancellationToken)
    {
        return await _repository.GetActiveByPlayerIdAsync(playerId, cancellationToken);
    }
}
```

**Duplicate Detection Logic:**
- Checks PlayerId + TeamName + ChampionshipName combination
- Only considers active assignments (LeftDate is null)
- Supports excludeId parameter for update scenarios
- Case-sensitive matching (SQL Server default collation may differ)

### TeamPlayer Service ✅

**TeamPlayerService** (`Services/Implementations/TeamPlayerService.cs`):
- Service layer for team player management operations
- Coordinates between presentation and data layers
- Applies business rules and validation
- Manages audit fields (CreatedBy, UpdatedBy, timestamps UTC)

**Features:**
- **Team Assignments**: Manage player team memberships for championships
- **Business Rules**: Validates data and prevents duplicate active assignments
- **Audit Trail**: Automatic population of audit fields
- **ServiceResult Pattern**: Consistent success/failure handling

**Methods:**

| Method | Description |
|--------|-------------|
| `GetTeamsByPlayerIdAsync(playerId, includeInactive)` | Get all team assignments for a player |
| `GetActiveTeamsByPlayerIdAsync(playerId)` | Get only active team assignments |
| `GetTeamAssignmentByIdAsync(teamPlayerId)` | Get a single team assignment by ID |
| `AddPlayerToTeamAsync(createDto, currentUserId)` | Add a player to a team |
| `UpdateTeamAssignmentAsync(teamPlayerId, updateDto, currentUserId)` | Update a team assignment |
| `RemovePlayerFromTeamAsync(teamPlayerId, leftDate, currentUserId)` | Remove a player from a team |
| `ValidateTeamAssignmentAsync(createDto)` | Validate team assignment data |

**Business Rules:**
- TeamName: Required, trimmed, max 200 characters
- ChampionshipName: Required, trimmed, max 200 characters
- JoinedDate: Required, not more than 1 year in the future
- LeftDate: Optional, must be after JoinedDate, not in the future
- No duplicate active assignments (same player + team + championship)
- All dates stored in UTC

**Usage Example:**
```csharp
// Register in DI
builder.Services.AddScoped<ITeamPlayerService, TeamPlayerService>();

// Use in a component or controller
public class TeamManagementComponent
{
    private readonly ITeamPlayerService _teamPlayerService;

    public TeamManagementComponent(ITeamPlayerService teamPlayerService)
    {
        _teamPlayerService = teamPlayerService;
    }

    public async Task AssignPlayerToTeamAsync()
    {
        var createDto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow
        };

        var result = await _teamPlayerService.AddPlayerToTeamAsync(createDto, "admin-user");
        
        if (result.Success)
        {
            Console.WriteLine($"Created assignment with ID: {result.Data!.TeamPlayerId}");
        }
        else if (result.ValidationErrors.Any())
        {
            foreach (var (field, errors) in result.ValidationErrors)
            {
                Console.WriteLine($"{field}: {string.Join(", ", errors)}");
            }
        }
    }
}
```

### TeamPlayer Validation ✅

**TeamPlayerValidator** (`Validation/TeamPlayerValidator.cs`):
- Provides business rule validation for team player data
- Validates CreateTeamPlayerDto, UpdateTeamPlayerDto, and TeamPlayer entities
- Returns ValidationResult with field-specific error messages

#### Validation Rules

| Field | Requirement | Error Message |
|-------|-------------|---------------|
| TeamName | Required, 1-200 characters, not whitespace | "Team name is required" or "Team name must not exceed 200 characters" |
| ChampionshipName | Required, 1-200 characters, not whitespace | "Championship name is required" or "Championship name must not exceed 200 characters" |
| JoinedDate | Required, not more than 1 year in future | "Joined date is required" or "Joined date cannot be more than 1 year in the future" |
| LeftDate | Optional, must be after JoinedDate, not in future | "Left date must be after the joined date" or "Left date cannot be in the future" |

#### Methods

- `ValidateCreateTeamPlayer(CreateTeamPlayerDto)` - Validates team player creation data
- `ValidateUpdateTeamPlayer(UpdateTeamPlayerDto, joinedDate)` - Validates team player update data
- `ValidateTeamPlayer(TeamPlayer)` - Validates a TeamPlayer entity

#### Usage Example

```csharp
var dto = new CreateTeamPlayerDto
{
    PlayerId = 1,
    TeamName = "Team Alpha",
    ChampionshipName = "Championship 2024",
    JoinedDate = DateTime.UtcNow.AddMonths(-1)
};

var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);
if (!result.IsValid)
{
    foreach (var (field, messages) in result.Errors)
    {
        Console.WriteLine($"{field}: {string.Join(", ", messages)}");
    }
}
```

### Custom Exceptions

#### RepositoryException

**RepositoryException** (`Exceptions/RepositoryException.cs`):
- Exception for database operation failures
- Provides detailed context about the failed operation

**Properties:**
- `Operation` - Name of the failed operation (e.g., "GetByIdAsync")
- `EntityType` - Type of entity involved (e.g., "Player")
- `EntityId` - ID of the entity, if applicable

**Usage Example:**
```csharp
try
{
    await repository.UpdateAsync(player);
}
catch (RepositoryException ex) when (ex.Operation == "UpdateAsync")
{
    logger.LogError(ex, "Update failed for {EntityType} with ID {EntityId}",
        ex.EntityType, ex.EntityId);
    // Handle specific update failure
}
```

### Test Coverage

**Total Tests**: 576 tests, all passing ✅

- **AuthenticationServiceTests**: 20 tests
- **AuthorizationServiceTests**: 17 tests
- **AuthorizationScenariosTests**: 17 tests (comprehensive role-based scenarios)
- **ApplicationUserTests**: 24 tests
- **UserClaimTests**: 18 tests
- **AuthorizationResultTests**: 8 tests
- **Exception Tests**: 29 tests (includes RepositoryException)
- **PlayerValidatorTests**: 43 tests
- **ApplicationDbContextTests**: 16 tests (audit field population)
- **PlayerConfigurationTests**: 25 tests (entity configuration)
- **EfPlayerRepositoryTests**: 32 tests (CRUD operations, error handling)
- **EfTeamPlayerRepositoryTests**: 50 tests (CRUD operations, duplicate detection, logging)
- **MockPlayerRepositoryTests**: Various tests
- **PlayerServiceTests**: 26 tests (service operations, validation, error handling)
- **TeamPlayerServiceTests**: 28 tests (service operations, business rules, validation)

#### Authorization Scenario Tests

The `AuthorizationScenariosTests` class provides comprehensive coverage for authorization scenarios:

- Anonymous user denied access to all policies
- Regular user can access authenticated and user policies
- Regular user denied access to admin policy
- Admin user can access all standard policies
- Admin has full permissions including admin-specific
- Regular user has limited permissions
- Admin can access any resource
- Regular user can only access own resources
- Inactive user treated as authenticated
- User without roles fails role-based policies
- Resource-based authorization respects admin role
- Resource-based authorization denies regular user for admin policy
- User ID case insensitivity for resource access
- Custom policy falls back to role check
- Authorization failure includes meaningful reason

## See Also

- [Database Connection Setup Guide](../../docs/Database_Connection_Setup.md) - Connection string configuration for development and production
- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)
- [DDD Best Practices](../../.github/instructions/dotnet-architecture-good-practices.instructions.md)
- [Entra ID Specification](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
