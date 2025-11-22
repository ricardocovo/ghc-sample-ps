# GhcSamplePs.Core

Business Logic Layer - UI-agnostic class library

## Purpose

This project contains the business logic, domain models, and services for the GhcSamplePs application. It is completely UI-agnostic and fully testable.

## Dependencies

None - This project should not reference the Web project or any UI-specific libraries.

## Project Structure

```
GhcSamplePs.Core/
├── Models/              # Domain entities and models
├── Services/
│   ├── Interfaces/      # Service contracts
│   └── Implementations/ # Service implementations
├── Repositories/
│   ├── Interfaces/      # Repository contracts
│   └── Implementations/ # Repository implementations
├── Validation/          # Business validation rules
└── Extensions/          # Extension methods
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

### Creating Repositories

1. Define interface in `Repositories/Interfaces/`
2. Implement interface in `Repositories/Implementations/`
3. Follow repository pattern for data access
4. Return domain models, not DTOs

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
builder.Services.AddScoped<IMyService, MyService>();
builder.Services.AddScoped<IMyRepository, MyRepository>();
```

## Adding New Features

1. Create domain model in `Models/` if needed
2. Create service interface in `Services/Interfaces/`
3. Implement service in `Services/Implementations/`
4. Write unit tests in `GhcSamplePs.Core.Tests`
5. Register service in Web project's `Program.cs`
6. Use service in Blazor components

## See Also

- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)
- [DDD Best Practices](../../.github/instructions/dotnet-architecture-good-practices.instructions.md)
