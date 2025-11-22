---
description: Blazor application architecture guidelines with clean separation of concerns
applyTo: '**/*.razor,**/*.cs,**/*.csproj'
---

# Blazor Application Architecture Guidelines

## Solution Structure

The solution follows a clean architecture approach with clear separation between UI and business logic:

### Required Projects

1. **[AppName].Web** - Blazor UI Layer
   - Contains Blazor components (`.razor` files)
   - Pages and layouts
   - UI-specific services and state management
   - Client-side validation and user interaction logic
   - Dependency: References `[AppName].Core`

2. **[AppName].Core** - Business Logic Layer
   - Domain models and entities
   - Business logic and services
   - Interfaces and abstractions
   - Data access logic (repositories, DbContext)
   - Validation rules
   - No UI dependencies - this project should be UI-agnostic

3. **[AppName].Core.Tests** - Unit Tests
   - Unit tests for business logic
   - Follow naming convention: `{ClassName}Tests`
   - Use xUnit, NUnit, or MSTest (consistent with solution)
   - Mock external dependencies
   - Test through public APIs only

### Optional Projects (as needed)

4. **[AppName].Shared** - Shared Models/DTOs
   - Data Transfer Objects (DTOs)
   - Shared constants and enums
   - API contracts
   - Can be referenced by both Web and Core

5. **[AppName].Infrastructure** - External Dependencies
   - Database implementations
   - External API clients
   - File system access
   - Third-party integrations

## Separation of Concerns

### Blazor UI Layer ([AppName].Web)

**Responsibilities:**
- Rendering UI components
- Handling user interactions
- Client-side routing
- UI state management
- Calling business logic services
- Displaying validation errors

**Should NOT contain:**
- Business rules or logic
- Direct data access
- Complex calculations
- Domain models (use DTOs/ViewModels)

**Example Structure:**
```
[AppName].Web/
├── Pages/
│   ├── Index.razor
│   └── Counter.razor
├── Components/
│   └── Shared/
├── Services/
│   └── UIServices/ (UI-specific only)
├── wwwroot/
├── _Imports.razor
├── App.razor
└── Program.cs
```

### Business Logic Layer ([AppName].Core)

**Responsibilities:**
- Business rules and validation
- Domain logic
- Data access (repositories, queries)
- Service interfaces and implementations
- Domain entities and models

**Should NOT contain:**
- UI components or Blazor-specific code
- HTTP context or request/response handling
- UI state management

**Example Structure:**
```
[AppName].Core/
├── Models/
│   └── Entities/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Validation/
└── Extensions/
```

## Dependency Injection Setup

In `[AppName].Web/Program.cs`:

Register Core services:
- Register business services from Core project
- Register repositories and data contexts
- Use interfaces for testability
- Configure service lifetimes appropriately (Scoped, Transient, Singleton)

Example pattern:
```
builder.Services.AddScoped<IMyService, MyService>();
builder.Services.AddScoped<IMyRepository, MyRepository>();
```

## Communication Between Layers

### Blazor Component → Business Logic

1. **Inject services** into Blazor components using `@inject` directive
2. **Call service methods** from component code-behind or event handlers
3. **Handle results** and update UI state
4. **Display errors** using validation summaries or error messages

### Business Logic → Data

1. **Use repository pattern** for data access
2. **Keep DbContext in Core** project (or Infrastructure if separated)
3. **Return domain models** from repositories
4. **Map to DTOs/ViewModels** when passing to UI if needed

## Testing Strategy

### Unit Tests ([AppName].Core.Tests)

**Focus on:**
- Business logic validation
- Service method behavior
- Repository logic (use in-memory database)
- Edge cases and error handling

**Setup:**
- Reference `[AppName].Core` project only
- Use dependency injection in tests
- Mock external dependencies (databases, APIs)
- Follow AAA pattern (Arrange, Act, Assert)

**Test Project Structure:**
```
[AppName].Core.Tests/
├── Services/
│   └── MyServiceTests.cs
├── Repositories/
│   └── MyRepositoryTests.cs
├── Models/
│   └── EntityValidationTests.cs
└── TestHelpers/
    └── TestDataBuilder.cs
```

## Best Practices

### General
- **Keep UI and business logic separate** - never mix Blazor components with business rules
- **Use interfaces** for services and repositories to enable testing and flexibility
- **Apply SOLID principles** throughout the solution
- **Minimize dependencies** - Core should not reference Web

### Blazor-Specific
- Keep components focused and small
- Use component parameters for input
- Use EventCallback for output
- Avoid business logic in `.razor` files - move to services
- Use `@code` blocks sparingly - prefer code-behind files for complex logic

### Business Logic
- Keep services stateless where possible
- Use async/await for I/O operations
- Validate input at service boundaries
- Return meaningful results (success/failure, errors)
- Log important operations and errors

### Testing
- Write tests for all public service methods
- Test business rules thoroughly
- Use descriptive test names: `WhenCondition_ThenExpectedBehavior`
- Keep tests independent and isolated
- Use test data builders for complex object setup

## Project References

```
[AppName].Web
  └─> [AppName].Core
  └─> [AppName].Shared (optional)

[AppName].Core
  └─> [AppName].Shared (optional)

[AppName].Core.Tests
  └─> [AppName].Core
  
[AppName].Infrastructure (optional)
  └─> [AppName].Core
```

## Migration Path

### Initial Setup (MVP)
1. Create `[AppName].Web` project (Blazor Server or Blazor WebAssembly)
2. Create `[AppName].Core` class library
3. Create `[AppName].Core.Tests` test project
4. Set up basic folder structure in each project
5. Configure dependency injection in `Program.cs`
6. Create initial service interfaces and implementations

### As Solution Grows
1. Extract shared models to `[AppName].Shared` if needed
2. Move infrastructure concerns to `[AppName].Infrastructure` if Core becomes too large
3. Consider API layer if building separate backend
4. Add integration tests project for end-to-end testing

## Key Reminders

- ✅ UI components call services, services contain business logic
- ✅ Core project is UI-agnostic and testable
- ✅ Use dependency injection throughout
- ✅ Write tests as you build features
- ✅ Keep concerns separated - resist the urge to mix layers
- ❌ No business logic in `.razor` files
- ❌ No UI components or Blazor code in Core project
- ❌ No direct database calls from UI components
