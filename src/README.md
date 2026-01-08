# GhcSamplePs Source Code

Source code directory containing all application projects following clean architecture principles.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-success)](../.github/copilot-instructions.md)
[![Tests](https://img.shields.io/badge/tests-891%20passing-success)](../tests/)

---

## 📋 Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Project Architecture](#project-architecture)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Development Workflows](#development-workflows)
- [Coding Standards](#coding-standards)
- [Testing](#testing)
- [Contributing](#contributing)

---

## Overview

This directory contains the core source code for GhcSamplePs, a Blazor Server application following clean architecture principles. The solution is organized into two main projects:

- **GhcSamplePs.Core** - Business logic layer (UI-agnostic)
- **GhcSamplePs.Web** - Presentation layer (Blazor UI)

### Key Architectural Principle

```
✅ CORRECT:  GhcSamplePs.Web → GhcSamplePs.Core
❌ FORBIDDEN: GhcSamplePs.Core → GhcSamplePs.Web
```

The Core project is completely **UI-agnostic** and must never reference the Web project. This ensures business logic remains testable, reusable, and independent of any UI framework.

---

## Getting Started

### Prerequisites

Before working with the source code, ensure you have:

- **.NET 10 SDK** (version 10.0 or later)
- **IDE** - Visual Studio 2022 (17.12+), VS Code with C# extension, or Rider
- **SQL Server** - LocalDB, SQL Express, or Azure SQL Database
- **Git** - For version control

> 💡 **For complete setup instructions**, see the [main README prerequisites](../README.md#prerequisites) and [Development Environment Setup](../docs/Development_Environment_Setup.md).

### Quick Start

#### 1. Opening the Solution

```powershell
# Navigate to the src directory
cd src

# Open in Visual Studio
start GhcSamplePs.sln

# Or open in VS Code
code .
```

#### 2. Building the Solution

```powershell
# Build entire solution
dotnet build

# Build specific projects
dotnet build GhcSamplePs.Core/GhcSamplePs.Core.csproj
dotnet build GhcSamplePs.Web/GhcSamplePs.Web.csproj

# Clean before building
dotnet clean
dotnet build
```

#### 3. Database Setup

```powershell
# Apply migrations (creates/updates database)
dotnet ef database update \
  --project GhcSamplePs.Core \
  --startup-project GhcSamplePs.Web

# Or use the configured task from workspace root
dotnet ef database update --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web
```

> 📖 **For detailed database setup**, see [Database Connection Setup](../docs/Database_Connection_Setup.md).

#### 4. Running the Application

```powershell
# Run from Web project directory
cd GhcSamplePs.Web
dotnet run

# Or watch for changes (hot reload)
dotnet watch run

# Application runs at: https://localhost:7001
```

#### 5. Running Tests

```powershell
# From src directory or repository root
dotnet test

# With detailed output
dotnet test --verbosity normal

# With code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Next Steps

After setting up your development environment:

1. 📚 Review [Development Workflows](#development-workflows) for adding features
2. 📖 Read [Coding Standards](#coding-standards) for conventions
3. 🧪 Explore [Testing](#testing) for test execution strategies
4. 📘 Check project-specific READMEs:
   - [GhcSamplePs.Core/README.md](GhcSamplePs.Core/README.md) - Business logic documentation
   - [GhcSamplePs.Web/README.md](GhcSamplePs.Web/README.md) - UI documentation

---

## Project Structure

```
src/
├── GhcSamplePs.Core/         # Business Logic Layer (UI-Agnostic)
│   ├── Common/               # ServiceResult, ValidationResult
│   ├── Data/                 # EF Core DbContext & configurations
│   ├── Exceptions/           # Custom domain exceptions
│   ├── Extensions/           # Extension methods
│   ├── Migrations/           # EF Core database migrations
│   ├── Models/               # Domain entities (Player, Team, etc.)
│   ├── Repositories/         # Data access interfaces & implementations
│   ├── Services/             # Business logic services
│   │   ├── Interfaces/       # Service abstractions
│   │   └── Implementations/  # Service implementations
│   ├── Validation/           # Business validation rules
│   └── README.md             # Core project documentation (750+ lines)
│
└── GhcSamplePs.Web/          # Presentation Layer (Blazor Server)
    ├── Components/           # Blazor components
    │   ├── Layout/           # MainLayout, NavMenu
    │   ├── Pages/            # Routable pages (@page)
    │   └── Shared/           # Reusable UI components
    ├── Services/             # UI-specific services (state management)
    ├── wwwroot/              # Static assets
    │   ├── css/              # Stylesheets
    │   ├── js/               # JavaScript files
    │   ├── manifest.json     # PWA manifest
    │   └── service-worker.js # PWA service worker
    ├── Dockerfile            # Multi-stage container build
    ├── Program.cs            # Application startup & DI configuration
    ├── appsettings.json      # Application configuration
    └── README.md             # Web project documentation (700+ lines)
```

---

## Development Workflows

### Service Layer Pattern

All business logic is implemented using the **Service Layer Pattern**:

1. **Define Interface** in `Core/Services/Interfaces/`
2. **Implement Service** in `Core/Services/Implementations/`
3. **Register Service** in `Web/Program.cs` with dependency injection
4. **Inject Service** into Blazor components using `@inject`

### Adding New Features - Step-by-Step Guide

Follow this structured workflow when implementing new features:

#### Step 1: Define Requirements

- Document business requirements and user stories
- Identify affected entities, services, and components
- Plan database schema changes if needed
- Create feature specification document (see `docs/specs/`)

#### Step 2: Create Domain Model (if needed)

```csharp
// GhcSamplePs.Core/Models/NewEntity.cs
public class NewEntity : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Add properties as needed
}
```

#### Step 3: Create Repository Layer

**Interface:**
```csharp
// GhcSamplePs.Core/Repositories/Interfaces/INewEntityRepository.cs
public interface INewEntityRepository
{
    Task<NewEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<NewEntity>> GetAllAsync(CancellationToken ct = default);
    Task<NewEntity> AddAsync(NewEntity entity, CancellationToken ct = default);
    Task<NewEntity> UpdateAsync(NewEntity entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
```

**Implementation:**
```csharp
// GhcSamplePs.Core/Repositories/Implementations/EfNewEntityRepository.cs
public class EfNewEntityRepository : INewEntityRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EfNewEntityRepository> _logger;

    public EfNewEntityRepository(
        ApplicationDbContext context,
        ILogger<EfNewEntityRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NewEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.NewEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }
    // ... implement other methods
}
```

#### Step 4: Create Service Layer

**Interface:**
```csharp
// GhcSamplePs.Core/Services/Interfaces/INewEntityService.cs
public interface INewEntityService
{
    Task<ServiceResult<NewEntityDto>> CreateAsync(CreateNewEntityDto dto, string userId);
    Task<ServiceResult<IReadOnlyList<NewEntityDto>>> GetAllAsync();
    Task<ServiceResult<NewEntityDto>> GetByIdAsync(int id);
    Task<ServiceResult<NewEntityDto>> UpdateAsync(int id, UpdateNewEntityDto dto, string userId);
    Task<ServiceResult> DeleteAsync(int id, string userId);
}
```

**Implementation:**
```csharp
// GhcSamplePs.Core/Services/Implementations/NewEntityService.cs
public class NewEntityService : INewEntityService
{
    private readonly INewEntityRepository _repository;
    private readonly ILogger<NewEntityService> _logger;

    public NewEntityService(
        INewEntityRepository repository,
        ILogger<NewEntityService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ServiceResult<NewEntityDto>> CreateAsync(
        CreateNewEntityDto dto,
        string userId)
    {
        try
        {
            // 1. Validate input
            var validation = NewEntityValidator.Validate(dto);
            if (!validation.IsValid)
                return ServiceResult<NewEntityDto>.Failure(validation.Errors);

            // 2. Business logic
            var entity = new NewEntity
            {
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Persist
            var created = await _repository.AddAsync(entity);

            // 4. Return result
            return ServiceResult<NewEntityDto>.Success(
                MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            return ServiceResult<NewEntityDto>.Failure(
                "An error occurred while creating the entity.");
        }
    }
    // ... implement other methods
}
```

#### Step 5: Add Validation

```csharp
// GhcSamplePs.Core/Validation/NewEntityValidator.cs
public static class NewEntityValidator
{
    public static ValidationResult Validate(CreateNewEntityDto dto)
    {
        var errors = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(nameof(dto.Name), new() { "Name is required" });

        if (dto.Name?.Length > 100)
            errors.Add(nameof(dto.Name), new() { "Name must not exceed 100 characters" });

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}
```

#### Step 6: Register Services in DI Container

```csharp
// GhcSamplePs.Web/Program.cs
builder.Services.AddScoped<INewEntityRepository, EfNewEntityRepository>();
builder.Services.AddScoped<INewEntityService, NewEntityService>();
```

#### Step 7: Create Database Migration

```powershell
# From repository root
dotnet ef migrations add AddNewEntityTable \
    --project src/GhcSamplePs.Core \
    --startup-project src/GhcSamplePs.Web

# Apply migration
dotnet ef database update \
    --project src/GhcSamplePs.Core \
    --startup-project src/GhcSamplePs.Web
```

#### Step 8: Write Unit Tests

```csharp
// tests/GhcSamplePs.Core.Tests/Services/NewEntityServiceTests.cs
public class NewEntityServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_ThenReturnsSuccess()
    {
        // Arrange
        var mockRepo = new Mock<INewEntityRepository>();
        var mockLogger = new Mock<ILogger<NewEntityService>>();
        var service = new NewEntityService(mockRepo.Object, mockLogger.Object);

        var dto = new CreateNewEntityDto { Name = "Test Entity" };

        // Act
        var result = await service.CreateAsync(dto, "user123");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<NewEntity>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenInvalidDto_ThenReturnsFailure()
    {
        // Arrange
        var service = CreateService();
        var dto = new CreateNewEntityDto { Name = "" }; // Invalid

        // Act
        var result = await service.CreateAsync(dto, "user123");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Name is required", result.ErrorMessage);
    }
}
```

#### Step 9: Create Blazor UI Components

**Page Component:**
```razor
@* GhcSamplePs.Web/Components/Pages/NewEntityManagement.razor *@
@page "/new-entities"
@inject INewEntityService NewEntityService
@inject ISnackbar Snackbar

<PageTitle>Entity Management</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Entity Management</MudText>

    <MudButton Color="Color.Primary"
               Variant="Variant.Filled"
               OnClick="OpenCreateDialog">
        <MudIcon Icon="@Icons.Material.Filled.Add" /> Add Entity
    </MudButton>

    @if (_entities is null)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else
    {
        <MudDataGrid Items="_entities" Class="mt-4">
            <Columns>
                <PropertyColumn Property="x => x.Name" Title="Name" />
                <PropertyColumn Property="x => x.CreatedAt" Title="Created" />
                <TemplateColumn Title="Actions">
                    <CellTemplate>
                        <MudIconButton Icon="@Icons.Material.Filled.Edit"
                                       OnClick="() => EditEntity(context.Item)" />
                        <MudIconButton Icon="@Icons.Material.Filled.Delete"
                                       OnClick="() => DeleteEntity(context.Item.Id)" />
                    </CellTemplate>
                </TemplateColumn>
            </Columns>
        </MudDataGrid>
    }
</MudContainer>

@code {
    private IReadOnlyList<NewEntityDto>? _entities;

    protected override async Task OnInitializedAsync()
    {
        await LoadEntitiesAsync();
    }

    private async Task LoadEntitiesAsync()
    {
        var result = await NewEntityService.GetAllAsync();
        if (result.IsSuccess)
            _entities = result.Data;
        else
            Snackbar.Add(result.ErrorMessage, Severity.Error);
    }

    private void OpenCreateDialog()
    {
        // Open MudDialog for create
    }

    private void EditEntity(NewEntityDto entity)
    {
        // Open MudDialog for edit
    }

    private async Task DeleteEntity(int id)
    {
        var result = await NewEntityService.DeleteAsync(id, "current-user");
        if (result.IsSuccess)
        {
            Snackbar.Add("Entity deleted successfully", Severity.Success);
            await LoadEntitiesAsync();
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }
}
```

#### Step 10: Update Documentation

- Update this README if architecture changes
- Update project-specific READMEs (Core/Web)
- Document new features in `docs/` directory
- Add user guides if needed

### Repository Pattern

All data access uses the **Repository Pattern** for abstraction:

```
Service Layer → Repository Interface → Repository Implementation → DbContext
```

**Benefits:**
- ✅ Testable services (mock repositories)
- ✅ Consistent data access patterns
- ✅ Easy to swap implementations
- ✅ Clear separation of concerns

### Testing Workflow

```powershell
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~NewEntityServiceTests"

# Run in watch mode (auto-run on changes)
dotnet watch test --project tests/GhcSamplePs.Core.Tests
```

---

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

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

## Quick Links

- 🏠 [Main README](../README.md) - Project overview
- 🏗️ [Infrastructure](../infra/README.md) - Azure deployment
- 🧪 [Tests](../tests/README.md) - Test documentation
- 📚 [Documentation](../docs/) - User & developer guides
- 🎯 [Specifications](../docs/specs/) - Feature specs

---

**Need Help?**

- Review project documentation in `docs/` directory
- Check `.github/instructions/` for coding guidelines
- See project-specific READMEs in `Core/` and `Web/`
- Refer to [copilot-instructions.md](../.github/copilot-instructions.md) for architecture overview

---

**Last Updated:** January 7, 2026
**Version:** 1.1.0
**Target Framework:** .NET 10.0
**Projects:** 2 (Core, Web)
**Test Status:** ✅ 891 tests passing
