# GhcSamplePs.Core

Business Logic Layer - UI-Agnostic Class Library

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4)](https://docs.microsoft.com/ef/core/)
[![Tests](https://img.shields.io/badge/tests-802%20passing-success)](../../tests/GhcSamplePs.Core.Tests/)

## Purpose

This project contains the business logic, domain models, data access layer, and services for the GhcSamplePs application. It is completely **UI-agnostic** and **fully testable**, following clean architecture principles.

## Architecture Principle

**Core is Independent**

```
❌ Core → Web (NEVER)
✅ Web → Core (ALWAYS)
```

- **Core** has no dependencies on Web or any UI framework
- **Web** references Core and calls its services
- This ensures business logic can be tested independently and reused across different UI technologies

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 10.0 | Runtime framework |
| **C#** | 14 | Programming language |
| **Entity Framework Core** | 10.0 | ORM for database access |
| **SQL Server** | Latest | Database provider |
| **Microsoft.Extensions.Logging** | 10.0 | Logging abstractions |

## Project Structure

```
GhcSamplePs.Core/
├── Common/                      # Common utilities and result types
│   ├── ServiceResult.cs         # Standard service return type
│   └── ValidationResult.cs      # Validation result container
│
├── Data/                        # Database context and configurations
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   ├── DesignTimeDbContextFactory.cs
│   └── Configurations/          # Fluent API entity configurations
│       ├── PlayerConfiguration.cs
│       ├── TeamPlayerConfiguration.cs
│       └── PlayerStatisticConfiguration.cs
│
├── Exceptions/                  # Custom domain exceptions
│   ├── AuthenticationException.cs
│   ├── AuthorizationException.cs
│   ├── PlayerNotFoundException.cs
│   ├── PlayerValidationException.cs
│   ├── RepositoryException.cs
│   └── TokenValidationException.cs
│
├── Extensions/                  # Extension methods
│   └── ServiceCollectionExtensions.cs  # DI registration helpers
│
├── Migrations/                  # EF Core database migrations
│   ├── [timestamp]_InitialCreate.cs
│   ├── [timestamp]_AddTeamPlayersTable.cs
│   ├── [timestamp]_AddPlayerStatisticsTable.cs
│   └── ApplicationDbContextModelSnapshot.cs
│
├── Models/                      # Domain entities and DTOs
│   ├── Identity/                # User identity models
│   │   ├── ApplicationUser.cs
│   │   └── UserClaim.cs
│   └── PlayerManagement/        # Player domain models
│       ├── Player.cs            # Player entity
│       ├── TeamPlayer.cs        # Team assignment entity
│       ├── PlayerStatistic.cs   # Game statistics entity
│       └── DTOs/                # Data Transfer Objects
│           ├── CreatePlayerDto.cs
│           ├── UpdatePlayerDto.cs
│           ├── PlayerDto.cs
│           ├── CreateTeamPlayerDto.cs
│           ├── UpdateTeamPlayerDto.cs
│           ├── TeamPlayerDto.cs
│           ├── CreatePlayerStatisticDto.cs
│           ├── UpdatePlayerStatisticDto.cs
│           ├── PlayerStatisticDto.cs
│           └── PlayerStatisticAggregateResult.cs
│
├── Repositories/                # Data access layer
│   ├── Interfaces/              # Repository contracts
│   │   ├── IPlayerRepository.cs
│   │   ├── ITeamPlayerRepository.cs
│   │   └── IPlayerStatisticRepository.cs
│   └── Implementations/         # Repository implementations
│       ├── EfPlayerRepository.cs
│       ├── MockPlayerRepository.cs
│       ├── EfTeamPlayerRepository.cs
│       └── EfPlayerStatisticRepository.cs
│
├── Services/                    # Business logic layer
│   ├── Interfaces/              # Service contracts
│   │   ├── IAuthenticationService.cs
│   │   ├── IAuthorizationService.cs
│   │   ├── ICurrentUserProvider.cs
│   │   ├── IPlayerService.cs
│   │   ├── ITeamPlayerService.cs
│   │   └── IPlayerStatisticService.cs
│   └── Implementations/         # Service implementations
│       ├── AuthenticationService.cs
│       ├── AuthorizationService.cs
│       ├── PlayerService.cs
│       ├── TeamPlayerService.cs
│       └── PlayerStatisticService.cs
│
└── Validation/                  # Business validation rules
    ├── PlayerValidator.cs
    ├── TeamPlayerValidator.cs
    └── PlayerStatisticValidator.cs
```

## Key Responsibilities

### ✅ What This Project Contains

- **Business Rules & Validation** - All domain logic and validation rules
- **Domain Entities** - Player, TeamPlayer, PlayerStatistic models
- **Data Access** - Repositories, DbContext, migrations
- **Service Layer** - Business logic orchestration
- **DTOs** - Data transfer objects for API contracts
- **Custom Exceptions** - Domain-specific error handling

### ❌ What This Project Should NOT Contain

- UI components or Blazor-specific code
- HTTP context or request/response handling
- UI state management
- Any reference to `GhcSamplePs.Web`

## Core Features

### 1. Player Management

**Domain Model:**
- Players with name, date of birth, gender, photo
- Audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- Automatic age calculation

**Services:**
- `IPlayerService` - CRUD operations for players
- `IPlayerRepository` - Data access abstraction

**Validation Rules:**
- Name: Required, 1-200 characters
- DateOfBirth: Required, past date, not more than 100 years ago
- Gender: Optional (Male, Female, Non-binary, Prefer not to say)
- PhotoUrl: Optional, max 500 chars, valid HTTP/HTTPS URL

### 2. Team Management

**Domain Model:**
- TeamPlayer join entity linking players to teams
- Support for multiple championships/seasons
- Active/inactive status tracking (JoinedDate, LeftDate)

**Services:**
- `ITeamPlayerService` - Manage team assignments
- `ITeamPlayerRepository` - Team assignment data access

**Business Rules:**
- TeamName: Required, max 200 characters
- ChampionshipName: Required, max 200 characters
- JoinedDate: Required, not more than 1 year in future
- LeftDate: Optional, must be after JoinedDate
- No duplicate active assignments (player + team + championship)

### 3. Player Statistics

**Domain Model:**
- Game-level performance statistics
- Tracks goals, assists, minutes played, jersey number
- Linked to specific team assignments

**Services:**
- `IPlayerStatisticService` - Manage game statistics
- `IPlayerStatisticRepository` - Statistics data access

**Features:**
- CRUD operations for individual game stats
- Aggregate calculations (totals and averages)
- Date range queries
- Team-specific statistics

**Validation Rules:**
- GameDate: Required, cannot be in future
- MinutesPlayed: 0-120 minutes
- JerseyNumber: 1-99
- Goals: ≥ 0
- Assists: ≥ 0

### 4. Authentication & Authorization

**Services:**
- `IAuthenticationService` - User identity management
- `IAuthorizationService` - Permission checking
- `ICurrentUserProvider` - Abstract access to current user

**Features:**
- Entra ID (Azure AD) integration
- Role-based authorization (Admin, User)
- Policy-based authorization
- Resource-based authorization (users can only edit their own data)
- Claims-based identity

**Authorization Policies:**
- `RequireAuthenticatedUser` - Any authenticated user
- `RequireAdminRole` - Admin role required
- `RequireUserRole` - User or Admin role

### 5. Data Access Layer

**ApplicationDbContext:**
- Entity Framework Core 10 DbContext
- Automatic audit field population (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- Fluent API entity configurations
- SQL Server provider

**Features:**
- Connection resilience (automatic retry on transient failures)
- Query splitting for better performance
- Sensitive data logging (development only)
- Detailed error messages (development only)

**Migrations:**
- `InitialCreate` - Creates Players table
- `AddTeamPlayersTable` - Creates TeamPlayers table
- `AddPlayerStatisticsTable` - Creates PlayerStatistics table

### 6. Repository Pattern

**Available Implementations:**
- `EfPlayerRepository` - Production EF Core implementation
- `MockPlayerRepository` - In-memory implementation for testing
- `EfTeamPlayerRepository` - Team assignment data access
- `EfPlayerStatisticRepository` - Statistics data access

**Features:**
- Async operations with cancellation token support
- AsNoTracking for read-only queries
- Comprehensive error handling
- Detailed logging at appropriate levels

## Service Layer Pattern

All business logic follows the same pattern:

### 1. Define Interface

```csharp
public interface IPlayerService
{
    Task<ServiceResult<IEnumerable<PlayerDto>>> GetAllPlayersAsync(
        CancellationToken cancellationToken = default);
    
    Task<ServiceResult<PlayerDto>> GetPlayerByIdAsync(
        int playerId, 
        CancellationToken cancellationToken = default);
    
    Task<ServiceResult<PlayerDto>> CreatePlayerAsync(
        CreatePlayerDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default);
    
    // ... more methods
}
```

### 2. Implement Service

```csharp
public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository repository, 
        ILogger<PlayerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ServiceResult<PlayerDto>> CreatePlayerAsync(
        CreatePlayerDto createDto,
        string currentUserId,
        CancellationToken cancellationToken)
    {
        // 1. Validate input
        var validationResult = PlayerValidator.ValidateCreatePlayer(createDto);
        if (!validationResult.IsValid)
        {
            return ServiceResult<PlayerDto>.Failure(
                validationErrors: validationResult.Errors);
        }

        // 2. Create entity
        var player = new Player
        {
            Name = createDto.Name.Trim(),
            DateOfBirth = DateTime.SpecifyKind(createDto.DateOfBirth, DateTimeKind.Utc),
            Gender = createDto.Gender?.Trim(),
            PhotoUrl = createDto.PhotoUrl?.Trim(),
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Save to database
        try
        {
            await _repository.AddAsync(player, cancellationToken);
            
            // 4. Map to DTO
            var dto = new PlayerDto
            {
                PlayerId = player.PlayerId,
                // ... map properties
            };

            return ServiceResult<PlayerDto>.Success(dto);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex, "Failed to create player");
            return ServiceResult<PlayerDto>.Failure("Failed to create player");
        }
    }
}
```

### 3. Register in DI Container

```csharp
// In Web project's Program.cs
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
```

### 4. Use in UI Layer

```csharp
@inject IPlayerService PlayerService

@code {
    private async Task CreatePlayerAsync()
    {
        var result = await PlayerService.CreatePlayerAsync(createDto, userId);
        
        if (result.Success)
        {
            Snackbar.Add("Player created successfully", Severity.Success);
            NavigationManager.NavigateTo("/players");
        }
        else if (result.ValidationErrors.Any())
        {
            foreach (var (field, errors) in result.ValidationErrors)
            {
                Snackbar.Add($"{field}: {string.Join(", ", errors)}", Severity.Error);
            }
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }
}
```

## Validation Pattern

All validation uses the `ValidationResult` pattern:

```csharp
public static class PlayerValidator
{
    public static ValidationResult ValidateCreatePlayer(CreatePlayerDto dto)
    {
        var result = new ValidationResult();

        // Name validation
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            result.AddError(nameof(dto.Name), "Name is required");
        }
        else if (dto.Name.Length > 200)
        {
            result.AddError(nameof(dto.Name), "Name must not exceed 200 characters");
        }

        // DateOfBirth validation
        if (dto.DateOfBirth == default)
        {
            result.AddError(nameof(dto.DateOfBirth), "Date of birth is required");
        }
        else if (dto.DateOfBirth >= DateTime.UtcNow.Date)
        {
            result.AddError(nameof(dto.DateOfBirth), "Date of birth must be in the past");
        }

        // ... more validation

        return result;
    }
}
```

## Database Configuration

### Connection String Setup

See [Database Connection Setup Guide](../../docs/Database_Connection_Setup.md) for detailed instructions.

**Development (User Secrets):**

```bash
cd src/GhcSamplePs.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=(localdb)\\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Production (Environment Variables):**

```bash
ConnectionStrings__DefaultConnection="Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=GhcSamplePs;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;"
```

### Service Registration

Use the extension method in `Program.cs`:

```csharp
builder.Services.AddApplicationDbContext(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
    enableSensitiveDataLogging: builder.Environment.IsDevelopment(),
    enableDetailedErrors: builder.Environment.IsDevelopment(),
    maxRetryCount: 5,
    maxRetryDelaySeconds: 30,
    commandTimeoutSeconds: 30);
```

### Database Migrations

**Add New Migration:**

```bash
dotnet ef migrations add MigrationName \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web
```

**Apply Migrations:**

```bash
dotnet ef database update \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web
```

**Generate SQL Script:**

```bash
dotnet ef migrations script \
  --project src/GhcSamplePs.Core \
  --startup-project src/GhcSamplePs.Web \
  --idempotent \
  --output migration.sql
```

**Production:**
- Migrations applied automatically on startup in development
- Use idempotent SQL scripts for production deployments

## Testing

### Test Coverage

**Total: 802 tests passing ✅**

| Test Suite | Tests | Coverage Area |
|------------|-------|---------------|
| AuthenticationServiceTests | 20 | User authentication, claims, roles |
| AuthorizationServiceTests | 17 | Permission checks, policies |
| AuthorizationScenariosTests | 17 | End-to-end authorization scenarios |
| ApplicationUserTests | 24 | User identity model |
| UserClaimTests | 18 | Claims management |
| AuthorizationResultTests | 8 | Authorization result model |
| PlayerValidatorTests | 43 | Player validation rules |
| TeamPlayerValidatorTests | 25 | Team assignment validation |
| PlayerStatisticValidatorTests | 30 | Statistics validation |
| ApplicationDbContextTests | 16 | Audit field population |
| PlayerConfigurationTests | 25 | Entity configuration |
| TeamPlayerConfigurationTests | 20 | Team player entity config |
| PlayerStatisticConfigurationTests | 18 | Statistics entity config |
| EfPlayerRepositoryTests | 32 | CRUD operations, error handling |
| EfTeamPlayerRepositoryTests | 50 | Team assignments, duplicate detection |
| EfPlayerStatisticRepositoryTests | 45 | Statistics CRUD, aggregates |
| MockPlayerRepositoryTests | 30 | In-memory implementation |
| PlayerServiceTests | 26 | Player business logic |
| TeamPlayerServiceTests | 28 | Team management logic |
| PlayerStatisticServiceTests | 37 | Statistics business logic |
| ExceptionTests | 29 | Custom exceptions |

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~PlayerServiceTests"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport
```

### Test Naming Convention

```csharp
[Fact]
public void WhenCondition_ThenExpectedBehavior()
{
    // Arrange
    var service = new PlayerService(repository, logger);
    
    // Act
    var result = service.CreatePlayer(dto);
    
    // Assert
    result.Should().BeSuccessful();
}
```

## Development Guidelines

### Creating New Features

1. **Define Domain Entity** in `Models/`
2. **Create DTOs** in `Models/[Domain]/DTOs/`
3. **Add Validation** in `Validation/`
4. **Define Repository Interface** in `Repositories/Interfaces/`
5. **Implement Repository** in `Repositories/Implementations/`
6. **Define Service Interface** in `Services/Interfaces/`
7. **Implement Service** in `Services/Implementations/`
8. **Write Unit Tests** in `GhcSamplePs.Core.Tests`
9. **Add EF Core Configuration** (if database entity)
10. **Create Migration** (if database changes)
11. **Register Services** in Web project's `Program.cs`
12. **Create UI Components** in Web project

### Code Standards

Follow `.github/instructions/csharp.instructions.md` and `.github/instructions/dotnet-architecture-good-practices.instructions.md`:

✅ **DO:**
- Use async/await for all I/O operations
- Return `ServiceResult<T>` from service methods
- Use dependency injection
- Log important operations
- Validate input at service boundaries
- Use UTC for all dates
- Throw specific exception types
- Write unit tests for all public methods

❌ **DON'T:**
- Reference UI projects or frameworks
- Use `HttpContext` or any web-specific types
- Put business logic in repositories
- Mix concerns (keep services focused)
- Ignore cancellation tokens
- Return null (use ServiceResult.Failure instead)

### ServiceResult Pattern

All service methods return `ServiceResult<T>`:

```csharp
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public Dictionary<string, List<string>> ValidationErrors { get; set; } = new();

    public static ServiceResult<T> Success(T data) 
        => new() { Success = true, Data = data };

    public static ServiceResult<T> Failure(string errorMessage) 
        => new() { Success = false, ErrorMessage = errorMessage };

    public static ServiceResult<T> Failure(
        Dictionary<string, List<string>> validationErrors) 
        => new() { Success = false, ValidationErrors = validationErrors };
}
```

**Benefits:**
- Consistent error handling
- No exceptions for business logic failures
- Rich validation error information
- Easy to test

## Exception Handling

Custom exceptions for specific failure scenarios:

| Exception | When to Use |
|-----------|-------------|
| `PlayerNotFoundException` | Player with ID not found |
| `PlayerValidationException` | Player data validation failure |
| `RepositoryException` | Database operation failure |
| `AuthenticationException` | User authentication failure |
| `AuthorizationException` | Permission denied |
| `TokenValidationException` | JWT token validation failure |

```csharp
try
{
    var player = await _repository.GetByIdAsync(playerId);
    if (player == null)
    {
        throw new PlayerNotFoundException(playerId);
    }
    return player;
}
catch (RepositoryException ex)
{
    _logger.LogError(ex, "Database error retrieving player {PlayerId}", playerId);
    throw;
}
```

## Aggregate Calculations

The `PlayerStatisticService` provides aggregate statistics:

```csharp
var result = await _statisticService.GetPlayerAggregatesAsync(playerId);

if (result.Success)
{
    var aggregates = result.Data!;
    Console.WriteLine($"Games: {aggregates.GameCount}");
    Console.WriteLine($"Total Goals: {aggregates.TotalGoals}");
    Console.WriteLine($"Total Assists: {aggregates.TotalAssists}");
    Console.WriteLine($"Total Minutes: {aggregates.TotalMinutesPlayed}");
    Console.WriteLine($"Avg Goals/Game: {aggregates.AverageGoals:F2}");
    Console.WriteLine($"Avg Assists/Game: {aggregates.AverageAssists:F2}");
    Console.WriteLine($"Avg Minutes/Game: {aggregates.AverageMinutesPlayed:F1}");
}
```

## Performance Considerations

### Repository Best Practices

- Use `AsNoTracking()` for read-only queries
- Avoid loading related entities unless needed
- Use pagination for large result sets
- Leverage indexes for frequently queried fields

### Service Layer Optimizations

- Cache frequently accessed data
- Use bulk operations when appropriate
- Avoid N+1 query problems
- Batch database updates when possible

## Additional Resources

### Documentation

- [Database Connection Setup](../../docs/Database_Connection_Setup.md)
- [Player Statistics Requirements](../../docs/playerstats-requirements.md)
- [Entra ID Integration Spec](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)

### Architecture Guidelines

- [Blazor Architecture](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)
- [DDD Best Practices](../../.github/instructions/dotnet-architecture-good-practices.instructions.md)

### Related Projects

- [GhcSamplePs.Web](../GhcSamplePs.Web/README.md) - Blazor UI layer
- [GhcSamplePs.Core.Tests](../../tests/GhcSamplePs.Core.Tests/README.md) - Unit tests

## Contributing

When making changes to Core:

1. Ensure Core remains UI-agnostic
2. Follow clean architecture principles
3. Write comprehensive unit tests
4. Use consistent naming conventions
5. Document public APIs with XML comments
6. Update relevant documentation
7. Run all tests before committing

## License

This project is part of the GhcSamplePs solution. See the main repository LICENSE file for details.

---

**Last Updated:** December 3, 2025  
**Version:** 1.0.0  
**Target Framework:** .NET 10.0  
**Test Status:** ✅ 802 tests passing
