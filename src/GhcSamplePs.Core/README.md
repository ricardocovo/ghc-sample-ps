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

## Planned Features

### Authentication and Authorization Services

The Core project will contain authentication and authorization logic (business rules and validation), while the Web project will handle authentication middleware and UI components.

#### Planned Domain Models

Future authentication-related models will be added to `Models/Identity/`:

- **ApplicationUser**: User identity information extracted from Entra ID claims
  - Id, Email, DisplayName, Roles, Claims, IsActive, LastLoginDate
- **UserClaim**: Custom claim information
- **Role definitions**: Admin, User role constants

#### Planned Services

Future authentication services will be added:

**IAuthenticationService** (`Services/Interfaces/IAuthenticationService.cs`):
- GetCurrentUserAsync() - Retrieve current authenticated user
- GetUserClaimsAsync() - Get user claims
- GetUserRolesAsync() - Get user roles
- IsInRoleAsync(roleName) - Check role membership

**IAuthorizationService** (`Services/Interfaces/IAuthorizationService.cs`):
- AuthorizeAsync(policy) - Check policy requirements
- CanAccessAsync(resource) - Resource-based authorization
- GetUserPermissionsAsync() - Get user permissions

#### Implementation Status

**Status**: 🔄 Not Yet Implemented

The authentication and authorization services will be implemented in a future development phase after the Azure infrastructure is set up.

**Prerequisites:**
- Azure Entra ID infrastructure must be configured
- See: [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md)

**Next Steps:**
1. Azure infrastructure setup (current epic)
2. Add authentication models to Core
3. Implement authentication/authorization services
4. Write comprehensive unit tests
5. Integrate with Web project authentication middleware

See specification: [Entra ID External Identities Integration](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
