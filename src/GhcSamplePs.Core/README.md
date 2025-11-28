# GhcSamplePs.Core

Business Logic Layer - UI-agnostic class library

## Purpose

This project contains the business logic, domain models, and services for the GhcSamplePs application. It is completely UI-agnostic and fully testable.

## Dependencies

- `Microsoft.Extensions.Logging.Abstractions` - For logging in services

This project should not reference the Web project or any UI-specific libraries.

## Project Structure

```
GhcSamplePs.Core/
├── Models/
│   ├── Identity/               # User identity domain models
│   │   ├── ApplicationUser.cs
│   │   └── UserClaim.cs
│   └── PlayerManagement/       # Player domain models
│       ├── Player.cs
│       └── DTOs/
│           ├── CreatePlayerDto.cs
│           ├── PlayerDto.cs
│           └── UpdatePlayerDto.cs
├── Repositories/
│   ├── Interfaces/             # Repository contracts
│   │   └── IPlayerRepository.cs
│   └── Implementations/        # Repository implementations
│       └── MockPlayerRepository.cs
├── Services/
│   ├── Interfaces/             # Service contracts
│   │   ├── IAuthenticationService.cs
│   │   ├── IAuthorizationService.cs
│   │   ├── ICurrentUserProvider.cs
│   │   └── AuthorizationResult.cs
│   └── Implementations/        # Service implementations
│       ├── AuthenticationService.cs
│       └── AuthorizationService.cs
├── Exceptions/                 # Custom domain exceptions
│   ├── AuthenticationException.cs
│   ├── AuthorizationException.cs
│   └── TokenValidationException.cs
└── Extensions/                 # Extension methods (future)
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


### Player Repository ✅

#### IPlayerRepository Interface (`Repositories/Interfaces/IPlayerRepository.cs`)

Defines the contract for player data access operations:

- `GetAllAsync(CancellationToken)` - Gets all players
- `GetByIdAsync(int id, CancellationToken)` - Gets a player by ID
- `GetByUserIdAsync(string userId, CancellationToken)` - Gets players for a user
- `AddAsync(Player player, CancellationToken)` - Adds a new player
- `UpdateAsync(Player player, CancellationToken)` - Updates an existing player
- `DeleteAsync(int id, CancellationToken)` - Deletes a player

#### MockPlayerRepository (`Repositories/Implementations/MockPlayerRepository.cs`)

In-memory mock implementation for development and testing:

- Uses `ConcurrentDictionary<int, Player>` for thread-safe storage
- Auto-incrementing ID generation with thread-safe lock
- Pre-seeded with 10 sample players (famous athletes)
- All operations are async and support cancellation
- Case-insensitive user ID comparison for `GetByUserIdAsync`
- Preserves original audit fields (CreatedAt, CreatedBy) on updates

### Test Coverage

**Total Tests**: 262 tests, all passing ✅

- **AuthenticationServiceTests**: 20 tests
- **AuthorizationServiceTests**: 17 tests
- **AuthorizationScenariosTests**: 17 tests (comprehensive role-based scenarios)
- **ApplicationUserTests**: 24 tests
- **UserClaimTests**: 18 tests
- **AuthorizationResultTests**: 8 tests
- **Exception Tests**: 21 tests
- **MockPlayerRepositoryTests**: 30 tests

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

- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)
- [DDD Best Practices](../../.github/instructions/dotnet-architecture-good-practices.instructions.md)
- [Entra ID Specification](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
