# GhcSamplePs Source Code

Source code directory containing all application projects following clean architecture principles.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-success)](../.github/copilot-instructions.md)
[![Tests](https://img.shields.io/badge/tests-802%2B%20passing-success)](../tests/)

---

## Project Structure

```
src/
├── GhcSamplePs.Core/         # Business Logic Layer (UI-Agnostic)
│   ├── Models/               # Domain entities and DTOs
│   ├── Services/             # Business logic services
│   ├── Repositories/         # Data access layer
│   ├── Data/                 # EF Core DbContext
│   ├── Validation/           # Business validation rules
│   ├── Exceptions/           # Custom domain exceptions
│   ├── Extensions/           # Extension methods
│   ├── Migrations/           # EF Core migrations
│   └── README.md             # Core project documentation (750+ lines)
│
└── GhcSamplePs.Web/          # Presentation Layer (Blazor Server)
    ├── Components/           # Blazor components
    │   ├── Layout/           # Layout components
    │   ├── Pages/            # Page components
    │   └── Shared/           # Shared UI components
    ├── Services/             # UI-specific services
    ├── wwwroot/              # Static assets (CSS, JS, images)
    │   ├── manifest.json     # PWA manifest
    │   └── service-worker.js # PWA service worker
    ├── Dockerfile            # Container build definition
    ├── Program.cs            # Application startup and DI
    ├── appsettings.json      # Application configuration
    └── README.md             # Web project documentation (700+ lines)
```

---

## Architecture Overview

### Clean Architecture Principles

This solution strictly follows **clean architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────┐
│         Presentation Layer (Web)            │
│  • Blazor Components                        │
│  • User Interface                           │
│  • Display Logic                            │
└─────────────────┬───────────────────────────┘
                  │ References
                  ↓
┌─────────────────────────────────────────────┐
│       Business Logic Layer (Core)           │
│  • Services                                 │
│  • Repositories                             │
│  • Domain Models                            │
│  • Validation                               │
└─────────────────┬───────────────────────────┘
                  │ Uses
                  ↓
┌─────────────────────────────────────────────┐
│          Data Layer (EF Core)               │
│  • ApplicationDbContext                     │
│  • Entity Configurations                    │
│  • Migrations                               │
└─────────────────────────────────────────────┘
```

### Dependency Direction

**Critical Rule:** Core project is UI-agnostic and must never reference Web project.

```
✅ CORRECT:  GhcSamplePs.Web → GhcSamplePs.Core
❌ FORBIDDEN: GhcSamplePs.Core → GhcSamplePs.Web
```

This ensures:
- ✅ Business logic is testable independently
- ✅ Core can be reused across different UI technologies
- ✅ Clear separation of concerns
- ✅ Maintainable and scalable codebase

---

## Project Descriptions

### 1. GhcSamplePs.Core - Business Logic Layer

**Purpose:** Contains all business logic, domain models, services, and data access.

**Key Components:**
- **Domain Models** - Player, TeamPlayer, PlayerStatistic entities
- **Services** - Business logic orchestration (interfaces + implementations)
- **Repositories** - Data access abstraction (interfaces + implementations)
- **Validation** - Business rules and validation logic
- **DbContext** - Entity Framework Core database context
- **Migrations** - Database schema versioning

**Technology:**
- .NET 10.0 Class Library
- Entity Framework Core 10.0
- Microsoft.Extensions.Logging

**Responsibilities:**
- ✅ Business rules and validation
- ✅ Domain logic and calculations
- ✅ Data access through repositories
- ✅ Service orchestration
- ✅ Exception handling
- ❌ NO UI components
- ❌ NO Blazor-specific code
- ❌ NO HTTP context

**Documentation:** See [GhcSamplePs.Core/README.md](GhcSamplePs.Core/README.md)

---

### 2. GhcSamplePs.Web - Presentation Layer

**Purpose:** Blazor Server UI layer providing user interface and interaction.

**Key Components:**
- **Pages** - Blazor page components (routing)
- **Layout** - Application layout components
- **Components** - Reusable UI components
- **Services** - UI-specific services (state management)
- **wwwroot** - Static assets and PWA files

**Technology:**
- .NET 10.0 Blazor Server
- MudBlazor 8.x UI Components
- Microsoft Identity Web
- SignalR (Blazor Server transport)

**Responsibilities:**
- ✅ User interface rendering
- ✅ User interaction handling
- ✅ Display logic and formatting
- ✅ Client-side validation (UX)
- ✅ State management (UI state)
- ✅ Calling Core services
- ❌ NO business logic
- ❌ NO direct database access
- ❌ NO complex calculations

**Documentation:** See [GhcSamplePs.Web/README.md](GhcSamplePs.Web/README.md)

---

## Development Workflows

### Adding New Features

Follow this structured workflow when implementing new features:

#### 1. Define Requirements
- Document business requirements
- Identify affected entities and services
- Plan database changes if needed

#### 2. Core Layer (Business Logic)

```powershell
cd src/GhcSamplePs.Core
```

**a) Create Domain Model** (if needed)
```csharp
// Models/PlayerManagement/NewEntity.cs
public class NewEntity : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... properties
}
```

**b) Create Repository Interface**
```csharp
// Repositories/Interfaces/INewEntityRepository.cs
public interface INewEntityRepository
{
    Task<NewEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<NewEntity>> GetAllAsync(CancellationToken ct = default);
    Task<NewEntity> AddAsync(NewEntity entity, CancellationToken ct = default);
    Task<NewEntity> UpdateAsync(NewEntity entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
```

**c) Implement Repository**
```csharp
// Repositories/Implementations/EfNewEntityRepository.cs
public class EfNewEntityRepository : INewEntityRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EfNewEntityRepository> _logger;

    // ... implementation
}
```

**d) Create Service Interface**
```csharp
// Services/Interfaces/INewEntityService.cs
public interface INewEntityService
{
    Task<ServiceResult<NewEntityDto>> CreateAsync(CreateNewEntityDto dto, string userId);
    Task<ServiceResult<IReadOnlyList<NewEntityDto>>> GetAllAsync();
    // ... methods
}
```

**e) Implement Service**
```csharp
// Services/Implementations/NewEntityService.cs
public class NewEntityService : INewEntityService
{
    private readonly INewEntityRepository _repository;
    private readonly ILogger<NewEntityService> _logger;

    // ... implementation with business logic
}
```

**f) Add Validation** (if needed)
```csharp
// Validation/NewEntityValidator.cs
public static class NewEntityValidator
{
    public static ValidationResult Validate(NewEntity entity)
    {
        var errors = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(entity.Name))
            errors.Add(nameof(entity.Name), new() { "Name is required" });

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}
```

#### 3. Write Unit Tests

```powershell
cd ../../tests/GhcSamplePs.Core.Tests
```

Create test file following naming convention:
```csharp
// Services/NewEntityServiceTests.cs
public class NewEntityServiceTests
{
    [Fact]
    public async Task WhenCreatingEntityWithValidData_ThenEntityIsCreated()
    {
        // Arrange
        var service = CreateService();
        var dto = new CreateNewEntityDto { Name = "Test" };

        // Act
        var result = await service.CreateAsync(dto, "user-id");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    // ... more tests
}
```

#### 4. Register in Dependency Injection

```powershell
cd ../../src/GhcSamplePs.Web
```

Update `Program.cs`:
```csharp
// Register repository
builder.Services.AddScoped<INewEntityRepository, EfNewEntityRepository>();

// Register service
builder.Services.AddScoped<INewEntityService, NewEntityService>();
```

#### 5. Web Layer (UI)

**a) Create Blazor Component**
```razor
@* Components/Pages/ManageNewEntities.razor *@
@page "/new-entities"
@inject INewEntityService NewEntityService

<PageTitle>Manage New Entities</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Manage New Entities</MudText>

    @if (_entities is not null)
    {
        <MudDataGrid Items="_entities">
            <Columns>
                <PropertyColumn Property="x => x.Name" Title="Name" />
                <!-- ... columns -->
            </Columns>
        </MudDataGrid>
    }
</MudContainer>

@code {
    private List<NewEntityDto>? _entities;

    protected override async Task OnInitializedAsync()
    {
        await LoadEntitiesAsync();
    }

    private async Task LoadEntitiesAsync()
    {
        var result = await NewEntityService.GetAllAsync();
        if (result.Success && result.Data is not null)
        {
            _entities = result.Data.ToList();
        }
    }
}
```

**b) Add Navigation** (if needed)
Update `Components/Layout/NavMenu.razor`:
```razor
<MudNavLink Href="/new-entities" Icon="@Icons.Material.Filled.Category">
    New Entities
</MudNavLink>
```

#### 6. Database Migration (if schema changed)

```powershell
cd ../GhcSamplePs.Core

# Create migration
dotnet ef migrations add AddNewEntityTable --startup-project ../GhcSamplePs.Web

# Review generated migration
# Edit Migrations/[timestamp]_AddNewEntityTable.cs if needed

# Apply migration
dotnet ef database update --startup-project ../GhcSamplePs.Web
```

#### 7. Update Documentation

- Update [GhcSamplePs.Core/README.md](GhcSamplePs.Core/README.md) with new service/repository
- Update [GhcSamplePs.Web/README.md](GhcSamplePs.Web/README.md) with new page
- Update this README if project structure changed
- Update main [README.md](../README.md) if user-facing feature

#### 8. Verification Checklist

Before committing:

- [ ] All unit tests pass (`dotnet test`)
- [ ] New tests added for new functionality
- [ ] Code follows project conventions
- [ ] No business logic in UI layer
- [ ] Core remains UI-agnostic
- [ ] Documentation updated
- [ ] Migration runs successfully
- [ ] Application builds without warnings
- [ ] Manual testing completed

---

## Service Layer Pattern

All business operations follow a consistent pattern:

### 1. Interface Definition

```csharp
public interface IPlayerService
{
    // Query methods return ServiceResult<T>
    Task<ServiceResult<IReadOnlyList<PlayerDto>>> GetAllPlayersAsync(
        CancellationToken cancellationToken = default);

    Task<ServiceResult<PlayerDto>> GetPlayerByIdAsync(
        int playerId,
        CancellationToken cancellationToken = default);

    // Command methods require currentUserId for audit
    Task<ServiceResult<PlayerDto>> CreatePlayerAsync(
        CreatePlayerDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<PlayerDto>> UpdatePlayerAsync(
        int playerId,
        UpdatePlayerDto updateDto,
        string currentUserId,
        CancellationToken cancellationToken = default);
}
```

### 2. Service Implementation Structure

```csharp
public sealed class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository repository,
        ILogger<PlayerService> logger)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);

        _repository = repository;
        _logger = logger;
    }

    public async Task<ServiceResult<PlayerDto>> CreatePlayerAsync(
        CreatePlayerDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating player {Name} for user {UserId}",
            createDto.Name, currentUserId);

        try
        {
            // 1. Validate input
            var validationResult = PlayerValidator.ValidateCreate(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for player creation");
                return ServiceResult<PlayerDto>.Failure(validationResult.Errors);
            }

            // 2. Business logic
            var player = new Player
            {
                Name = createDto.Name,
                DateOfBirth = createDto.DateOfBirth,
                Gender = createDto.Gender,
                UserId = currentUserId,
                CreatedBy = currentUserId
            };

            // 3. Save to repository
            var savedPlayer = await _repository.AddAsync(player, cancellationToken);

            // 4. Map to DTO and return success
            var dto = PlayerDto.FromEntity(savedPlayer);
            _logger.LogInformation("Successfully created player with ID {PlayerId}",
                savedPlayer.Id);

            return ServiceResult<PlayerDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player for user {UserId}", currentUserId);
            return ServiceResult<PlayerDto>.Failure(
                "An error occurred while creating the player. Please try again.");
        }
    }
}
```

### 3. ServiceResult Pattern

```csharp
// Success with data
return ServiceResult<PlayerDto>.Success(playerDto);

// Failure with message
return ServiceResult<PlayerDto>.Failure("Player not found");

// Failure with validation errors
return ServiceResult<PlayerDto>.Failure(validationErrors);

// Using in UI
var result = await _playerService.CreatePlayerAsync(dto, userId);
if (result.Success)
{
    // Handle success
    var player = result.Data;
}
else
{
    // Handle failure
    var errorMessage = result.ErrorMessage;
    var validationErrors = result.ValidationErrors;
}
```

---

## Repository Pattern

All data access follows the repository pattern:

### Interface Definition

```csharp
public interface IPlayerRepository
{
    // Query operations
    Task<Player?> GetByIdAsync(int playerId, CancellationToken ct = default);
    Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Player>> GetByUserIdAsync(string userId, CancellationToken ct = default);

    // Command operations
    Task<Player> AddAsync(Player player, CancellationToken ct = default);
    Task<Player> UpdateAsync(Player player, CancellationToken ct = default);
    Task<bool> DeleteAsync(int playerId, CancellationToken ct = default);

    // Query helpers
    Task<bool> ExistsAsync(int playerId, CancellationToken ct = default);
}
```

### Implementation Best Practices

```csharp
public sealed class EfPlayerRepository : IPlayerRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EfPlayerRepository> _logger;

    public async Task<Player?> GetByIdAsync(
        int playerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving player with ID {PlayerId}", playerId);

        try
        {
            // Use AsNoTracking for read-only queries
            var player = await _context.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == playerId, cancellationToken);

            if (player is not null)
            {
                _logger.LogInformation("Retrieved player with ID {PlayerId}", playerId);
            }
            else
            {
                _logger.LogDebug("Player with ID {PlayerId} not found", playerId);
            }

            return player;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled for player ID {PlayerId}", playerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player with ID {PlayerId}", playerId);
            throw new RepositoryException(
                $"Failed to retrieve player with ID {playerId}.",
                nameof(GetByIdAsync),
                nameof(Player),
                playerId,
                ex);
        }
    }
}
```

**Key Patterns:**
- Use `AsNoTracking()` for read-only operations
- Handle `OperationCanceledException` separately
- Wrap exceptions in `RepositoryException`
- Log at appropriate levels (Debug, Information, Warning, Error)
- Include context in exception messages

---

## Coding Standards

### C# Conventions

Follow [.github/instructions/csharp.instructions.md](../.github/instructions/csharp.instructions.md):

- Use **C# 14** features
- Apply **PascalCase** for public members
- Use **camelCase** for private fields
- Prefix interfaces with `I`
- Use **file-scoped namespaces**
- Apply **nullable reference types**
- Add XML documentation for public APIs

### Architecture Guidelines

Follow [.github/instructions/blazor-architecture.instructions.md](../.github/instructions/blazor-architecture.instructions.md):

- Keep Core UI-agnostic
- NO UI dependencies in Core
- Services contain business logic
- Repositories handle data access only
- Components handle display logic only

### Best Practices

**DO:**
- ✅ Return `ServiceResult<T>` from services
- ✅ Use async/await throughout
- ✅ Validate at service boundaries
- ✅ Log important operations
- ✅ Handle cancellation tokens
- ✅ Write unit tests for all services

**DON'T:**
- ❌ Put business logic in repositories
- ❌ Put business logic in UI components
- ❌ Reference UI projects from Core
- ❌ Use `HttpContext` in Core
- ❌ Return null (use ServiceResult.Failure)
- ❌ Ignore cancellation tokens

---

## Testing

All Core business logic must have corresponding unit tests in [tests/GhcSamplePs.Core.Tests](../tests/GhcSamplePs.Core.Tests/).

### Test Coverage

- **Services:** 90%+ coverage
- **Repositories:** 85%+ coverage
- **Validation:** 95%+ coverage
- **Overall:** 802+ tests passing

### Running Tests

```powershell
# Run all tests
dotnet test

# Run Core tests only
dotnet test tests/GhcSamplePs.Core.Tests/

# Run with verbose output
dotnet test --verbosity normal

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

See [tests/GhcSamplePs.Core.Tests/README.md](../tests/GhcSamplePs.Core.Tests/README.md) for detailed testing documentation.

---

## Build & Run

### Build Solution

```powershell
# From solution root
dotnet build

# Build specific project
dotnet build src/GhcSamplePs.Core/GhcSamplePs.Core.csproj
dotnet build src/GhcSamplePs.Web/GhcSamplePs.Web.csproj
```

### Run Application

```powershell
# From Web project directory
cd src/GhcSamplePs.Web
dotnet run

# Or use watch for auto-reload
dotnet watch run
```

### Clean Build

```powershell
dotnet clean
dotnet build
```

---

## Documentation

### Project Documentation

- **[GhcSamplePs.Core/README.md](GhcSamplePs.Core/README.md)** - Business logic layer (750+ lines)
- **[GhcSamplePs.Web/README.md](GhcSamplePs.Web/README.md)** - Presentation layer (700+ lines)

### Architecture Guidelines

- [.github/copilot-instructions.md](../.github/copilot-instructions.md) - Overall architecture
- [.github/instructions/blazor-architecture.instructions.md](../.github/instructions/blazor-architecture.instructions.md) - Blazor patterns
- [.github/instructions/csharp.instructions.md](../.github/instructions/csharp.instructions.md) - C# standards
- [.github/instructions/dotnet-architecture-good-practices.instructions.md](../.github/instructions/dotnet-architecture-good-practices.instructions.md) - DDD principles

### Main Documentation

- **[../README.md](../README.md)** - Complete project overview (855+ lines)

---

## Contributing

When making changes to source code:

1. Follow clean architecture principles
2. Keep Core UI-agnostic
3. Write comprehensive unit tests
4. Follow coding standards
5. Document public APIs
6. Update relevant README files
7. Ensure all tests pass before committing

---

## License

This project is part of the GhcSamplePs solution. See the main repository [LICENSE](../LICENSE) file for details.

---

**Last Updated:** December 29, 2025
**Version:** 1.0.1
**Target Framework:** .NET 10.0
**Projects:** 2 (Core, Web)
**Test Status:** ✅ 802+ tests passing
