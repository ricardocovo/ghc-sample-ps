# GhcSamplePs.Core

Business Logic Layer - UI-agnostic class library

## Purpose

This project contains the business logic, domain models, and services for the GhcSamplePs application. It is completely UI-agnostic and fully testable.

## Dependencies

None - This project should not reference the Web project or any UI-specific libraries.

## Project Structure

```
GhcSamplePs.Core/
├── Models/
│   └── Identity/            # User identity domain models
│       ├── ApplicationUser.cs
│       └── UserClaim.cs
├── Services/
│   └── Interfaces/          # Service contracts
│       ├── IAuthenticationService.cs
│       ├── IAuthorizationService.cs
│       └── AuthorizationResult.cs
├── Exceptions/              # Custom domain exceptions
│   ├── AuthenticationException.cs
│   ├── AuthorizationException.cs
│   └── TokenValidationException.cs
└── Extensions/              # Extension methods (future)
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

## Implemented Features

### Authentication and Authorization Domain Models ✅

The Core project now contains authentication and authorization domain models and service interfaces. These provide the business logic layer for user identity, claims, and security context.

#### Identity Models

**ApplicationUser** (`Models/Identity/ApplicationUser.cs`):
- Represents an authenticated user with identity information from Entra ID claims
- Properties: Id, Email, DisplayName, GivenName, FamilyName, Roles, Claims, IsActive, LastLoginDate, CreatedDate
- Methods:
  - `IsInRole(roleName)` - Check if user has specific role
  - `HasClaim(type, value)` - Check if user has specific claim
  - `TryGetClaim(type, out value)` - Get claim value if exists
- Immutable with read-only collections
- No database persistence (user identity managed by Entra ID)

**UserClaim** (`Models/Identity/UserClaim.cs`):
- Represents a custom claim with type, value, issuer, and optional expiration
- Properties: Type, Value, Issuer, IssuedAt, ExpiresAt
- Methods:
  - `IsValid()` - Check if claim has not expired
  - `Equals()` - Compare claims based on type and value
  - `GetHashCode()` - Hash based on type and value
  - `ToString()` - Format as "Type: Value"
- Supports claim validation and lifecycle management

#### Service Interfaces

**IAuthenticationService** (`Services/Interfaces/IAuthenticationService.cs`):
- Retrieves and validates authenticated user information
- Methods:
  - `GetCurrentUserAsync()` - Get current authenticated user
  - `GetUserClaimsAsync()` - Get all user claims
  - `GetUserRolesAsync()` - Get user roles
  - `IsInRoleAsync(roleName)` - Check role membership
  - `HasClaimAsync(type, value)` - Check specific claim
- All methods are async and accept CancellationToken
- Implementation will be in future development phase

**IAuthorizationService** (`Services/Interfaces/IAuthorizationService.cs`):
- Performs authorization checks and determines user permissions
- Methods:
  - `AuthorizeAsync(policyName)` - Check policy requirements
  - `AuthorizeAsync(resource, policyName)` - Resource-based authorization
  - `CanAccessAsync(resourceId)` - Check resource access
  - `GetUserPermissionsAsync()` - Get all user permissions
- Returns `AuthorizationResult` for authorization decisions
- Implementation will be in future development phase

**AuthorizationResult** (`Services/Interfaces/IAuthorizationService.cs`):
- Result type for authorization checks
- Properties: Succeeded, FailureReason, MissingPermissions
- Factory methods:
  - `Success()` - Create successful result
  - `Failure(reason, missingPermissions)` - Create failure result
- Immutable with read-only collections

#### Custom Exceptions

**AuthenticationException** (`Exceptions/AuthenticationException.cs`):
- Thrown when user authentication fails
- Default message: "Authentication failed. Please try signing in again."
- Supports custom messages and inner exceptions

**AuthorizationException** (`Exceptions/AuthorizationException.cs`):
- Thrown when user lacks required permissions
- Default message: "You do not have permission to access this resource."
- Properties: Resource, RequiredPermission
- Provides context about access denial

**TokenValidationException** (`Exceptions/TokenValidationException.cs`):
- Derived from AuthenticationException
- Thrown when token validation fails
- Default message: "Invalid or expired authentication token."
- Used for JWT token validation failures

#### Test Support

**Test Helpers** (in `GhcSamplePs.Core.Tests/TestHelpers/`):

**TestUserFactory**:
- `CreateAdminUser()` - Admin user with Admin and User roles
- `CreateRegularUser()` - Standard user with User role
- `CreateIncompleteProfileUser()` - User with incomplete profile
- `CreateInactiveUser()` - Inactive user account
- `CreateCustomUser()` - Fully customizable test user

**TestClaimFactory**:
- `CreateEmailVerifiedClaim(verified)` - Email verification claim
- `CreateProfileCompleteClaim(complete)` - Profile completion claim
- `CreateExpiringClaim(type, value, minutes)` - Claim with expiration
- `CreateExpiredClaim(type, value)` - Already expired claim
- `CreateCustomClaim()` - Fully customizable test claim
- `CreateStandardClaims()` - Collection of standard claims

#### Test Coverage

All models, exceptions, and service interfaces have comprehensive unit tests:
- **ApplicationUserTests**: 24 tests covering all methods and properties
- **UserClaimTests**: 18 tests covering validation, equality, and lifecycle
- **AuthenticationExceptionTests**: 5 tests for exception behavior
- **AuthorizationExceptionTests**: 10 tests including property validation
- **TokenValidationExceptionTests**: 6 tests for inheritance and behavior
- **AuthorizationResultTests**: 8 tests for result creation and validation

**Total Test Coverage**: 71 tests, all passing ✅

### Implementation Status

**Status**: ✅ Domain Models and Interfaces Complete

The authentication and authorization domain layer is complete with:
- ✅ Identity models (ApplicationUser, UserClaim)
- ✅ Custom exceptions (Authentication, Authorization, TokenValidation)
- ✅ Service interfaces (IAuthenticationService, IAuthorizationService)
- ✅ Authorization result pattern
- ✅ Test helpers for unit testing
- ✅ Comprehensive unit tests (85%+ coverage)

**Next Steps**:
1. Implement IAuthenticationService and IAuthorizationService in `Services/Implementations/`
2. Integrate with Web project authentication middleware
3. Connect to Entra ID for claims extraction and validation
4. Add authorization policies in Web project
5. Create UI components for authentication flows

**Prerequisites**:
- Azure Entra ID infrastructure must be configured (separate epic)
- See: [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md)

See specification: [Entra ID External Identities Integration](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
