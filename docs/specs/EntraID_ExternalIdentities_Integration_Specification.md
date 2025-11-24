# Feature Specification: Microsoft Entra ID External Identities Integration

## Executive Summary

Integrate Microsoft Entra ID External Identities (formerly Azure AD B2C) as the authentication and authorization provider for the GhcSamplePs Blazor application. This integration will enable secure user authentication for external users (customers, partners, consumers) with support for multiple identity providers, self-service registration, and enterprise-grade security.

**Business Value:**
- Enterprise-grade authentication and authorization for external users
- Support for social identity providers (Microsoft, Google, Facebook, etc.)
- Self-service user registration and profile management
- Multi-factor authentication (MFA) capabilities
- Compliance with security standards (OAuth 2.0, OpenID Connect)
- Reduced development time by leveraging Microsoft's identity platform
- Seamless integration with Azure ecosystem and Azure Container Apps
- Scalable identity management for growing user base

**Key Stakeholders:**
- End Users (external customers, partners, consumers)
- Security Team
- Development Team
- Infrastructure/DevOps Team
- Product Owner
- Compliance Officer

## Requirements

### Functional Requirements

1. **User Authentication**
   - Users can sign in using Entra ID External Identities
   - Support for social identity providers (Microsoft Account, Google, Facebook)
   - Support for local accounts with email/password
   - Single sign-on (SSO) experience across the application
   - Session management with configurable timeout
   - Sign-out functionality with proper cleanup

2. **User Registration**
   - Self-service registration flow for new users
   - Configurable registration attributes (email, name, custom attributes)
   - Email verification during registration
   - Terms of service and privacy policy acceptance
   - Profile completion workflow

3. **Authorization and Access Control**
   - Role-based access control (RBAC) using Entra ID roles
   - Policy-based authorization for fine-grained access control
   - Claim-based authorization using user attributes
   - Protect Blazor pages and components based on roles/policies
   - Protect Core business logic services with authorization checks

4. **User Profile Management**
   - Users can view and update their profile information
   - Password reset functionality for local accounts
   - Account deletion and data export (GDPR compliance)

5. **Admin Capabilities**
   - Admin role for managing users and permissions
   - Audit logging of authentication events
   - User management interface (view, disable, delete users)

### Non-Functional Requirements

1. **Security**
   - Use OAuth 2.0 and OpenID Connect protocols
   - Secure token storage and handling
   - Protection against common vulnerabilities (CSRF, XSS, token replay)
   - HTTPS enforcement for all authentication flows
   - Token expiration and refresh token handling
   - Secure configuration management (secrets in Azure Key Vault)

2. **Performance**
   - Fast authentication response (under 2 seconds)
   - Minimal impact on application startup time
   - Efficient token validation and caching
   - Optimized redirect flows

3. **Reliability**
   - Graceful handling of authentication failures
   - Retry logic for transient Entra ID connectivity issues
   - Fallback mechanisms for service unavailability
   - Clear error messages for users

4. **Usability**
   - Seamless authentication experience
   - Mobile-friendly sign-in pages
   - Clear feedback during authentication process
   - Support for browser back button during auth flows

5. **Compliance**
   - GDPR compliance for user data handling
   - SOC 2 compliance through Entra ID
   - Audit trail for authentication and authorization events
   - Data residency support through Entra ID configuration

6. **Maintainability**
   - Clear separation of authentication concerns in Core layer
   - Testable authentication and authorization logic
   - Well-documented configuration
   - Easy to update when Entra ID changes occur

### User Stories

1. **As a new user**, I want to register for an account using my Microsoft account, so I don't have to create another username and password.

2. **As a registered user**, I want to sign in quickly and securely, so I can access the application without friction.

3. **As a user**, I want to reset my password if I forget it, so I can regain access to my account without contacting support.

4. **As an administrator**, I want to assign roles to users, so I can control what features they can access.

5. **As a developer**, I want authentication logic separated from business logic, so the code is testable and maintainable.

6. **As a security officer**, I want all authentication to use industry-standard protocols, so we meet compliance requirements.

7. **As a mobile user**, I want the sign-in process to work smoothly on my smartphone, so I can authenticate on any device.

8. **As a user**, I want my session to remain active while I'm using the app, but expire when I'm inactive for security.

### Acceptance Criteria

1. Users can successfully sign in using Entra ID External Identities
2. New users can register for an account through self-service registration
3. Social identity providers (Microsoft, Google) are configured and working
4. Blazor pages are protected based on authentication and authorization policies
5. Core business logic services enforce authorization rules
6. Unauthorized access attempts are properly handled with appropriate error messages
7. User profile information is displayed correctly in the UI
8. Sign-out functionality properly clears session and redirects to login
9. All authentication flows work on mobile devices
10. Configuration is externalized and secure (no secrets in code)
11. All existing unit tests continue to pass
12. New tests validate authentication and authorization logic
13. Application works in both development and production (Azure Container Apps) environments
14. Documentation is updated with setup and configuration instructions

## Technical Design

### Architecture Impact

The integration will impact multiple layers of the application following the clean architecture pattern:

**Projects Affected:**
1. **GhcSamplePs.Web** - Primary changes for authentication UI and middleware
2. **GhcSamplePs.Core** - Authorization logic, user models, and security services
3. **GhcSamplePs.Core.Tests** - Tests for authorization logic

**New Components Needed:**
- Authentication middleware configuration in Web project
- Authorization services and policies in Core project
- User identity models in Core project
- Security-related interfaces and implementations in Core project

**Integration Points:**
- Entra ID External Identities tenant (external Azure service)
- Azure Key Vault for secrets management (optional but recommended)
- Application configuration (appsettings.json, environment variables)

**Data Flow Changes:**
1. User initiates sign-in → Redirected to Entra ID login page
2. User authenticates with Entra ID → Receives ID token and access token
3. Application validates token → Establishes authenticated session
4. Subsequent requests include authentication context → Authorization checks applied
5. Protected Blazor components/pages check authentication state → Render or redirect
6. Core services validate authorization → Execute business logic or throw exception

### Implementation Details

#### Data Layer

**User Identity Models:**

Create new models in `src/GhcSamplePs.Core/Models/Identity/`:

**ApplicationUser Properties:**
- Id: Unique identifier (GUID) from Entra ID (subject claim)
- Email: User email address (string, from email claim)
- DisplayName: User's display name (string, from name claim)
- GivenName: First name (string, from given_name claim)
- FamilyName: Last name (string, from family_name claim)
- Roles: Collection of role names assigned to user (List<string>)
- Claims: Collection of custom claims (Dictionary<string, string>)
- IsActive: Boolean indicating if user account is active
- LastLoginDate: Timestamp of last successful login
- CreatedDate: Timestamp of account creation

**No database changes required initially** - User identity is managed by Entra ID. Application stores only session-related information in memory. Future enhancement could add local user profiles database if needed.

**Repository Pattern (Optional Future Enhancement):**

If local user data storage is needed later, create repositories in `src/GhcSamplePs.Core/Repositories/`:

- Interface: `IUserProfileRepository` with methods: GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync
- Implementation would use Entity Framework Core with Azure SQL Server
- Database schema would include UserProfiles table with foreign key to Entra ID user ID

#### Business Logic Layer

**Service Contracts:**

Create new interfaces in `src/GhcSamplePs.Core/Services/Interfaces/`:

**IAuthenticationService Methods:**
- GetCurrentUserAsync(): Retrieves current authenticated user's information
- GetUserClaimsAsync(): Returns all claims for current user
- GetUserRolesAsync(): Returns list of roles assigned to current user
- IsInRoleAsync(roleName): Checks if current user has specific role
- HasClaimAsync(claimType, claimValue): Checks if user has specific claim

**IAuthorizationService Methods:**
- AuthorizeAsync(policy): Checks if current user meets policy requirements
- AuthorizeAsync(resource, policy): Checks authorization for specific resource
- CanAccessAsync(resource): Determines if user can access given resource
- GetUserPermissionsAsync(): Returns list of permissions for current user

**Business Rules:**

Authentication and authorization rules to implement in Core:

1. **Authentication Rules:**
   - User must have valid Entra ID token to access protected resources
   - Token must not be expired
   - User account must be active (not disabled in Entra ID)
   - Session timeout after 60 minutes of inactivity (configurable)

2. **Authorization Rules:**
   - Admin role required for administrative functions
   - User role required for standard application features
   - Specific policies for sensitive operations (e.g., data deletion requires Admin role)
   - Resource-based authorization for user-owned data

3. **Validation Requirements:**
   - Validate all tokens using Entra ID public keys
   - Validate issuer matches configured Entra ID tenant
   - Validate audience matches application client ID
   - Reject tokens with missing or invalid claims

**Domain Logic Patterns:**

Follow existing service patterns in Core project:
- Reference pattern: `src/GhcSamplePs.Core/Services/Implementations/` (existing service implementations)
- Use dependency injection for all services
- Services should be stateless and thread-safe
- Use async/await for all operations
- Return Result<T> or custom response objects (not domain exceptions for business logic failures)

#### API/Interface Layer

While this is a Blazor Server application (not a REST API), there are internal interfaces for authentication:

**Authentication Endpoints (Handled by Framework):**

These endpoints are provided by ASP.NET Core authentication middleware:

| Endpoint | Purpose | Handler |
|----------|---------|---------|
| /signin-oidc | OpenID Connect callback after authentication | OIDC middleware |
| /signout-callback-oidc | OpenID Connect callback after sign-out | OIDC middleware |
| /MicrosoftIdentity/Account/SignIn | Initiates sign-in flow | Microsoft Identity middleware |
| /MicrosoftIdentity/Account/SignOut | Initiates sign-out flow | Microsoft Identity middleware |

**Authorization Policies:**

Define policies in `Program.cs` that can be referenced throughout the application:

**Policy Definitions (in natural language):**

- **RequireAuthenticatedUser**: User must be authenticated (logged in)
- **RequireAdminRole**: User must be authenticated AND have "Admin" role
- **RequireUserRole**: User must be authenticated AND have "User" or "Admin" role
- **RequireEmailVerified**: User must have verified email claim set to true
- **RequireProfileComplete**: User must have completed profile setup (custom claim)

**Configuration Requirements:**

Configuration settings to add to `appsettings.json`:

**AzureAd Section Structure:**
- Instance: Entra ID authority URL (e.g., "https://login.microsoftonline.com/")
- Domain: Tenant domain name (e.g., "yourtenant.onmicrosoft.com")
- TenantId: Entra ID tenant identifier (GUID)
- ClientId: Application (client) ID from Entra ID app registration (GUID)
- ClientSecret: Client secret from app registration (store in Azure Key Vault for production)
- CallbackPath: OAuth callback path (e.g., "/signin-oidc")
- SignedOutCallbackPath: Sign-out callback path (e.g., "/signout-callback-oidc")

**Additional Configuration:**
- Cookie settings: SlidingExpiration, ExpireTimeSpan, Cookie.SecurePolicy
- Token validation: ValidateIssuer, ValidateAudience, SaveTokens
- Claims mapping: Map Entra ID claims to application claims

#### UI/Presentation Layer

**Authentication UI Components:**

Create/modify components in `src/GhcSamplePs.Web/Components/`:

**LoginDisplay Component:**
- Location: `Components/Layout/LoginDisplay.razor`
- Purpose: Display user name and sign-out button when authenticated, sign-in link when not
- Shows user's display name or email
- Provides sign-out button that calls authentication service
- Shows in main layout header/navbar

**AuthorizeView Component Usage:**
- Use built-in Blazor `<AuthorizeView>` component to conditionally render content
- Show different content for authenticated vs. anonymous users
- Show role-specific content using `<AuthorizeView Roles="Admin">`
- Show policy-specific content using `<AuthorizeView Policy="PolicyName">`

**Protected Pages:**
- Add `@attribute [Authorize]` directive to pages requiring authentication
- Add `@attribute [Authorize(Roles = "Admin")]` for role-specific pages
- Add `@attribute [Authorize(Policy = "PolicyName")]` for policy-based pages
- Examples: Home page (public), Weather page (authenticated), Admin page (Admin role)

**User Interactions:**

**Sign-In Flow:**
1. Unauthenticated user clicks "Sign In" link
2. Application redirects to Entra ID External Identities login page
3. User selects identity provider (Microsoft, Google, local account)
4. User authenticates with selected provider
5. Entra ID redirects back to application with authentication token
6. Application validates token and establishes session
7. User redirected to originally requested page or home page

**Sign-Out Flow:**
1. Authenticated user clicks "Sign Out" button
2. Application initiates sign-out with Entra ID
3. Entra ID clears its session
4. Application clears local session cookies
5. User redirected to home page or custom sign-out confirmation page

**Error Handling Scenarios:**
- Authentication failed: Show friendly error message, log details, offer retry
- Token expired: Automatically refresh if refresh token available, otherwise redirect to login
- Insufficient permissions: Show "Access Denied" page with explanation
- Entra ID unavailable: Show maintenance message, retry automatically

**UI Patterns to Follow:**

Reference existing Blazor component patterns:
- Examine `src/GhcSamplePs.Web/Components/Pages/*.razor` for component structure
- Follow layout patterns in `src/GhcSamplePs.Web/Components/Layout/`
- Use dependency injection with `@inject` directive
- Use `@code` blocks for component logic or code-behind files for complex components

### Code Conventions to Follow

**From `.github/instructions/csharp.instructions.md`:**
- Use C# 14 features (latest version)
- Use PascalCase for classes, methods, public members
- Use camelCase for private fields and local variables
- Prefix interfaces with "I" (e.g., IAuthenticationService)
- Use file-scoped namespace declarations
- Use pattern matching and switch expressions
- Use `nameof` instead of string literals
- Add XML documentation comments for public APIs
- Always use `is null` or `is not null` for null checks
- Use async/await for all I/O operations

**From `.github/instructions/blazor-architecture.instructions.md`:**
- Keep UI code in GhcSamplePs.Web project
- Keep business logic in GhcSamplePs.Core project
- Core must never reference Web project
- Use dependency injection throughout
- Register services in `Program.cs`
- Inject services into components using `@inject`
- Keep components focused and small
- Avoid business logic in `.razor` files

**From `.github/instructions/dotnet-architecture-good-practices.instructions.md`:**
- Follow DDD principles for domain modeling
- Follow SOLID principles throughout
- Use ubiquitous language in code
- Encapsulate business rules in domain services
- Use dependency injection for all dependencies
- Write comprehensive tests for business logic
- Test naming convention: `MethodName_Condition_ExpectedResult`
- Minimum 85% code coverage for Core layer

**Naming Conventions for This Feature:**

Files to create in Core:
- `Models/Identity/ApplicationUser.cs`
- `Models/Identity/UserClaim.cs`
- `Services/Interfaces/IAuthenticationService.cs`
- `Services/Interfaces/IAuthorizationService.cs`
- `Services/Implementations/AuthenticationService.cs`
- `Services/Implementations/AuthorizationService.cs`

Files to create in Web:
- `Components/Layout/LoginDisplay.razor`
- `Components/Pages/Account/AccessDenied.razor`

Files to modify:
- `src/GhcSamplePs.Web/Program.cs` - Add authentication/authorization configuration
- `src/GhcSamplePs.Web/Components/Layout/MainLayout.razor` - Add LoginDisplay component
- `src/GhcSamplePs.Web/appsettings.json` - Add AzureAd configuration section
- `src/GhcSamplePs.Web/appsettings.Development.json` - Add development-specific config
- `src/GhcSamplePs.Web/_Imports.razor` - Add authorization-related using statements

### Dependencies

**NuGet Packages Required:**

For `GhcSamplePs.Web` project:
- **Microsoft.AspNetCore.Authentication.OpenIdConnect** (version 10.0.0 or latest)
  - Purpose: OpenID Connect authentication middleware
- **Microsoft.Identity.Web** (version 3.0.0 or latest)
  - Purpose: Simplified integration with Microsoft identity platform
- **Microsoft.Identity.Web.UI** (version 3.0.0 or latest)
  - Purpose: Pre-built UI components for account management

For `GhcSamplePs.Core` project:
- **Microsoft.AspNetCore.Authorization** (version 10.0.0 or latest)
  - Purpose: Authorization abstractions and policies
  - Note: May already be included transitively

For `GhcSamplePs.Core.Tests` project:
- **Microsoft.AspNetCore.Authentication.JwtBearer** (version 10.0.0 or latest)
  - Purpose: Testing JWT token validation
- **Moq** (version 4.20.0 or latest) - if not already present
  - Purpose: Mocking authentication services in tests

**External Service Dependencies:**

1. **Microsoft Entra ID External Identities Tenant:**
   - Purpose: Identity provider and user management
   - Configuration: Create tenant in Azure Portal
   - Setup: Configure user flows, identity providers, app registration

2. **Azure Key Vault (Recommended for Production):**
   - Purpose: Secure storage of client secrets and sensitive configuration
   - Configuration: Create Key Vault in Azure, add secrets
   - Integration: Use Azure.Identity library for secret access

3. **Azure Container Apps (Deployment Target):**
   - Purpose: Hosting environment
   - Configuration: Environment variables for production settings
   - Authentication: Use managed identity for Key Vault access

**Configuration Prerequisites:**

Before implementation, the following must be set up in Azure:

1. Create Entra ID External Identities tenant
2. Register application in Entra ID
3. Configure redirect URIs (e.g., https://localhost:5001/signin-oidc for development)
4. Create app roles in app registration (Admin, User)
5. Configure identity providers (Microsoft, Google, etc.)
6. Create user flows for sign-up and sign-in
7. Obtain client ID, tenant ID, and client secret

### Security Considerations

**Authentication Security:**
- All authentication uses OAuth 2.0 and OpenID Connect standards
- Tokens are validated using Entra ID public keys (JWKS endpoint)
- HTTPS required for all authentication endpoints (enforced)
- Anti-forgery tokens used for state parameter in OAuth flow
- Tokens stored securely in encrypted cookies with HttpOnly and Secure flags
- Short token lifetimes with automatic refresh capability

**Authorization Security:**
- Authorization checks performed on every protected resource access
- Authorization logic centralized in Core services for consistency
- Defense in depth: UI hides unauthorized features, but Core enforces rules
- Sensitive operations require explicit authorization checks
- Authorization decisions logged for audit trail

**Data Validation:**
- All tokens validated for signature, expiration, issuer, and audience
- User claims validated before use in business logic
- Input validation for all user-provided data
- Protection against injection attacks (SQL, XSS, CSRF)

**Sensitive Data Handling:**
- Client secrets never stored in source code
- Production secrets stored in Azure Key Vault
- Development secrets in user secrets or environment variables
- Tokens not logged or exposed in error messages
- User PII handled according to GDPR requirements

**Token Management:**
- Access tokens have short lifespan (1 hour typical)
- Refresh tokens used to obtain new access tokens without re-authentication
- Token revocation capability through Entra ID
- Expired tokens automatically rejected
- Token replay attacks prevented through nonce and state parameters

### Error Handling

**Expected Exceptions:**

Define custom exceptions in `src/GhcSamplePs.Core/Exceptions/`:

**AuthenticationException:**
- Thrown when: User authentication fails
- Message: "Authentication failed. Please try signing in again."
- Handling: Log error, redirect to login page

**AuthorizationException:**
- Thrown when: User lacks required permissions
- Message: "You do not have permission to access this resource."
- Handling: Log attempt, show access denied page

**TokenValidationException:**
- Thrown when: Token validation fails
- Message: "Invalid or expired authentication token."
- Handling: Log error, clear session, redirect to login

**EntraIdConnectionException:**
- Thrown when: Cannot connect to Entra ID services
- Message: "Authentication service temporarily unavailable. Please try again."
- Handling: Log error, show maintenance page, retry with exponential backoff

**Error Messages:**

User-facing error messages should be:
- Clear and non-technical
- Actionable when possible (e.g., "Click here to sign in again")
- Localized if application supports multiple languages
- Never expose sensitive details (token contents, internal error details)

Developer-facing logs should include:
- Full exception details
- Request correlation ID
- User identifier (if available)
- Timestamp
- Stack trace for debugging

**Logging Requirements:**

Implement structured logging in all authentication/authorization code:

**Log Events to Capture:**
- Authentication attempts (success and failure)
- Authorization checks (granted and denied)
- Token validation results
- Configuration errors
- External service communication failures
- Session lifecycle events (created, expired, terminated)

**Log Levels:**
- **Information**: Successful authentication, successful authorization
- **Warning**: Failed authentication attempts, permission denied
- **Error**: Token validation failures, Entra ID connection errors
- **Critical**: Configuration errors preventing authentication

**Logging Implementation:**
- Use ILogger<T> injected into services
- Include correlation IDs for request tracing
- Avoid logging sensitive data (tokens, passwords, PII)
- Use structured logging with properties (not string concatenation)
- Reference logging pattern in `src/GhcSamplePs.Core/Services/Implementations/` (existing services)

## Testing Strategy

### Unit Test Requirements

**Authentication Service Tests:**

Create test file: `tests/GhcSamplePs.Core.Tests/Services/AuthenticationServiceTests.cs`

Tests to implement (following naming convention):

1. `GetCurrentUserAsync_WhenUserAuthenticated_ReturnsUserInformation`
   - Setup: Mock authenticated user context
   - Verify: Returns ApplicationUser with correct claims

2. `GetCurrentUserAsync_WhenUserNotAuthenticated_ReturnsNull`
   - Setup: Mock unauthenticated context
   - Verify: Returns null

3. `GetUserRolesAsync_WhenUserHasRoles_ReturnsRoleList`
   - Setup: Mock user with Admin and User roles
   - Verify: Returns list containing both roles

4. `IsInRoleAsync_WhenUserInRole_ReturnsTrue`
   - Setup: Mock user with Admin role
   - Verify: IsInRole("Admin") returns true

5. `IsInRoleAsync_WhenUserNotInRole_ReturnsFalse`
   - Setup: Mock user without Admin role
   - Verify: IsInRole("Admin") returns false

**Authorization Service Tests:**

Create test file: `tests/GhcSamplePs.Core.Tests/Services/AuthorizationServiceTests.cs`

Tests to implement:

1. `AuthorizeAsync_WhenUserMeetsPolicy_ReturnsSuccess`
   - Setup: Mock user meeting policy requirements
   - Verify: Authorization succeeds

2. `AuthorizeAsync_WhenUserDoesNotMeetPolicy_ReturnsFailure`
   - Setup: Mock user not meeting policy requirements
   - Verify: Authorization fails with appropriate reason

3. `CanAccessAsync_WhenUserOwnsResource_ReturnsTrue`
   - Setup: Mock user accessing their own resource
   - Verify: Access granted

4. `CanAccessAsync_WhenUserDoesNotOwnResource_ReturnsFalse`
   - Setup: Mock user accessing another user's resource
   - Verify: Access denied

**Test Infrastructure:**

Use xUnit framework (already configured in solution):
- Create test fixtures for common setup
- Use Moq for mocking HTTP context and authentication services
- Create test helpers for generating test users and claims
- Implement custom assertions for authorization results

**Mock Setup Patterns:**

For mocking authentication:
- Mock IHttpContextAccessor to provide test ClaimsPrincipal
- Mock IAuthorizationService for policy evaluation
- Create helper methods to build test ClaimsPrincipal with specific roles/claims
- Use InMemory approach, not actual Entra ID connections

### Integration Test Scenarios

While unit tests focus on Core logic, integration tests validate end-to-end authentication flows:

**Potential Integration Test Scenarios:**
(These would be implemented if integration test infrastructure is added to solution)

1. **Complete Authentication Flow:**
   - Scenario: Unauthenticated user accesses protected page
   - Expected: Redirected to login, after auth, returned to original page

2. **Authorization Enforcement:**
   - Scenario: User without Admin role tries to access admin page
   - Expected: Access denied page displayed

3. **Token Refresh:**
   - Scenario: Access token expires during active session
   - Expected: Token automatically refreshed without user interaction

4. **Sign-Out:**
   - Scenario: User signs out
   - Expected: Session cleared, user redirected, cannot access protected pages

**Note:** Integration tests require actual Entra ID tenant or mock server. Implementation is optional for MVP.

### Test Data Needed

**Test User Identities:**

Create test helper class to generate mock users:

**Test User Profiles:**

1. **Admin User:**
   - Email: admin@test.com
   - Display Name: "Test Admin"
   - Roles: ["Admin", "User"]
   - Claims: { email_verified: "true", profile_complete: "true" }

2. **Regular User:**
   - Email: user@test.com
   - Display Name: "Test User"
   - Roles: ["User"]
   - Claims: { email_verified: "true", profile_complete: "true" }

3. **Unauthenticated User:**
   - No claims or roles
   - Used to test anonymous access scenarios

4. **Incomplete Profile User:**
   - Email: newuser@test.com
   - Display Name: "New User"
   - Roles: ["User"]
   - Claims: { email_verified: "true", profile_complete: "false" }

**Test Claims:**

Common claim types to use in tests:
- "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" (user ID)
- "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" (email)
- "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" (display name)
- "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" (roles)

### Code Coverage Expectations

Following DDD guidelines requirement of 85% minimum coverage for Core layer:

**Coverage Targets:**
- **Core.Services**: 90%+ coverage (business logic must be well-tested)
- **Core.Models**: 80%+ coverage (test model validation and behavior)
- **Web.Components**: Optional (UI components harder to unit test, consider integration tests)

**Coverage Tools:**
- Use dotnet-coverage tool (already mentioned in README)
- Generate Cobertura format reports
- Integrate with CI/CD pipeline to enforce coverage requirements

**What to Test:**
- All public methods in authentication and authorization services
- All authorization policy logic
- All custom claim transformations
- Error handling paths
- Edge cases (null values, empty collections, expired tokens)

**What Not to Test:**
- Framework code (ASP.NET Core authentication middleware)
- Entra ID itself (external service)
- Simple property getters/setters without logic

## Implementation Phases

### Phase 1: MVP (Minimum Viable Product)

**Goal:** Basic authentication working end-to-end with Entra ID

**Core Features:**
1. Configure Entra ID External Identities tenant and app registration
2. Add NuGet packages to projects
3. Configure authentication in Program.cs
4. Implement basic authentication service in Core
5. Create LoginDisplay component in Web
6. Add Authorize attribute to one protected page (e.g., Weather page)
7. Test sign-in and sign-out flows
8. Add basic unit tests for authentication service
9. Update configuration files (appsettings.json)
10. Document setup instructions in README

**Success Criteria:**
- User can sign in with Microsoft account
- Protected page requires authentication
- User can sign out successfully
- Basic tests pass
- Application runs locally

**Estimated Effort:** 2-3 days

### Phase 2: Authorization and Roles

**Goal:** Implement role-based access control

**Features:**
1. Define authorization policies in Program.cs
2. Configure Entra ID app roles
3. Implement authorization service in Core
4. Add role-based page protection (Admin page)
5. Create AccessDenied page
6. Add AuthorizeView components for conditional rendering
7. Add comprehensive unit tests for authorization
8. Test with users in different roles

**Success Criteria:**
- Admin role enforced on admin pages
- Non-admin users see access denied
- UI hides features based on roles
- All authorization tests pass

**Estimated Effort:** 2-3 days

### Phase 3: Production Readiness

**Goal:** Prepare for production deployment

**Features:**
1. Configure Azure Key Vault for secrets
2. Add environment-specific configuration
3. Implement comprehensive error handling
4. Add structured logging throughout
5. Configure session management and timeouts
6. Implement token refresh handling
7. Add security headers and HTTPS enforcement
8. Performance testing and optimization
9. Security testing (penetration testing checklist)
10. Complete documentation

**Success Criteria:**
- Application deployable to Azure Container Apps
- Secrets managed securely
- Comprehensive error handling
- Production-grade logging
- Security review passed
- Documentation complete

**Estimated Effort:** 3-4 days

### Phase 4: Enhanced Features (Optional)

**Goal:** Advanced authentication and user management capabilities

**Features:**
1. Social identity providers (Google, Facebook)
2. Multi-factor authentication (MFA) support
3. User profile management UI
4. Password reset functionality
5. Admin user management interface
6. Custom claims and policies
7. Email verification enforcement
8. Profile completion workflow
9. Audit log viewer
10. Advanced testing (integration tests)

**Success Criteria:**
- Multiple identity providers working
- MFA enforced for sensitive operations
- Users can manage their profiles
- Admin can manage users
- Comprehensive audit trail

**Estimated Effort:** 5-7 days

## Migration/Deployment Considerations

### Development Environment Setup

**Prerequisites:**
1. Azure subscription with Entra ID External Identities tenant
2. .NET 10 SDK installed
3. Visual Studio 2022, VS Code, or Rider
4. Azure CLI installed (for Key Vault access)

**Configuration Steps:**

1. **Create Entra ID Resources:**
   - Create External Identities tenant in Azure Portal
   - Register application and note Client ID and Tenant ID
   - Generate client secret and store securely
   - Configure redirect URIs: https://localhost:5001/signin-oidc
   - Create app roles: Admin, User

2. **Configure Development Environment:**
   - Add configuration to appsettings.Development.json
   - Use user secrets for client secret: `dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"`
   - Test connection to Entra ID

3. **Initial Testing:**
   - Run application locally
   - Test sign-in flow with test user
   - Verify role assignments work

### Database Migrations

**Phase 1 (MVP): No database changes required**
- User identity managed entirely by Entra ID
- Application stores no local user data
- Session state in memory or distributed cache

**Phase 2+ (Optional Enhancement):**
If local user profile storage is implemented:

1. Create Entity Framework migration for UserProfiles table
2. Include fields: Id, EntraIdUserId, CustomAttributes, LastSyncDate
3. Apply migration: `dotnet ef database update`
4. Seed with test data for development

**Migration Strategy:**
- Use code-first migrations with Entity Framework Core
- Store connection string in Azure Key Vault for production
- Run migrations automatically on startup (development) or manually (production)
- Include rollback scripts for safety

### Configuration Changes

**Development Configuration** (appsettings.Development.json):
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "yourtenant.onmicrosoft.com",
    "TenantId": "your-tenant-id-guid",
    "ClientId": "your-client-id-guid",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

**Production Configuration** (environment variables in Azure Container Apps):
- AZUREAD__INSTANCE
- AZUREAD__DOMAIN
- AZUREAD__TENANTID
- AZUREAD__CLIENTID
- AZUREAD__CLIENTSECRET (from Key Vault reference)

**Key Vault Integration:**
- Store production client secret in Azure Key Vault
- Use managed identity in Azure Container Apps to access Key Vault
- Reference secret in configuration: `@Microsoft.KeyVault(SecretUri=https://...)`

### Deployment Steps

**Deploying to Azure Container Apps:**

1. **Pre-Deployment:**
   - Update Entra ID app registration with production redirect URIs
   - Configure production Azure Key Vault with secrets
   - Set up managed identity for Container App
   - Grant Key Vault access to managed identity

2. **Build and Deploy:**
   - Build container image: `dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer`
   - Push image to Azure Container Registry
   - Deploy to Azure Container Apps
   - Set environment variables in Container Apps configuration

3. **Post-Deployment Verification:**
   - Verify application health endpoint
   - Test authentication flow in production
   - Verify Key Vault secrets are accessible
   - Check logs for any authentication errors
   - Test with production Entra ID users

4. **Monitoring Setup:**
   - Configure Application Insights
   - Set up alerts for authentication failures
   - Monitor token validation errors
   - Track user sign-in metrics

### Rollback Strategy

**If Issues Occur Post-Deployment:**

1. **Immediate Actions:**
   - Roll back to previous container image version
   - Disable new features using feature flags (if implemented)
   - Restore previous configuration

2. **Rollback Steps:**
   - Deploy previous working container image
   - Restore previous environment variables
   - Verify authentication working with previous version
   - Investigate issues in non-production environment

3. **Prevention:**
   - Always test thoroughly in staging environment first
   - Use blue-green deployment strategy for zero downtime
   - Keep previous container images available for quick rollback
   - Document configuration changes for easy restoration

### Zero-Downtime Deployment

**Strategy:**
- Use Azure Container Apps revision management
- Deploy new revision alongside old
- Gradually shift traffic to new revision
- Monitor authentication success rate
- Roll back if error rate increases

## Success Metrics

### Functional Metrics

1. **Authentication Success Rate:**
   - Target: >99% successful authentication attempts
   - Measure: Successful sign-ins / Total sign-in attempts

2. **Authorization Accuracy:**
   - Target: 100% correct authorization decisions
   - Measure: No unauthorized access to protected resources

3. **User Adoption:**
   - Target: 100% of users authenticate through Entra ID
   - Measure: Active users using Entra ID authentication

### Performance Metrics

1. **Authentication Response Time:**
   - Target: <2 seconds for sign-in flow completion
   - Measure: Time from clicking "Sign In" to returning to application

2. **Authorization Check Performance:**
   - Target: <50ms for authorization policy evaluation
   - Measure: Time to complete authorization check in Core services

3. **Token Validation Speed:**
   - Target: <100ms for token validation
   - Measure: Time to validate JWT token signature and claims

4. **Application Startup Time:**
   - Target: No more than 500ms increase from baseline
   - Measure: Time from app start to ready for requests

### Reliability Metrics

1. **Authentication Availability:**
   - Target: 99.9% uptime
   - Measure: Successful authentication attempts / Total attempts

2. **Error Rate:**
   - Target: <0.1% authentication errors (excluding user errors)
   - Measure: System authentication errors / Total authentication attempts

3. **Token Refresh Success:**
   - Target: >98% successful token refreshes
   - Measure: Successful refreshes / Token refresh attempts

### Security Metrics

1. **Failed Authentication Attempts:**
   - Target: <5% of total attempts (indicates good security)
   - Measure: Track and alert on unusual patterns
   - Monitor for brute force attempts

2. **Unauthorized Access Attempts:**
   - Target: 0 successful unauthorized access
   - Measure: Attempts blocked by authorization system
   - Alert on multiple attempts from same user

3. **Token Validation Failures:**
   - Target: <0.5% of validation attempts
   - Measure: Invalid tokens rejected / Total tokens validated
   - Investigate patterns indicating attack attempts

### User Acceptance Metrics

1. **User Satisfaction:**
   - Target: >4.0/5.0 rating on authentication experience
   - Measure: User surveys or feedback

2. **Support Tickets:**
   - Target: <2% users require support for authentication
   - Measure: Auth-related support tickets / Total users

3. **Time to First Successful Sign-In:**
   - Target: <5 minutes for new users
   - Measure: From account creation to first successful sign-in

### Monitoring and Alerting

**Implement Alerts For:**
- Authentication success rate drops below 95%
- Authorization errors spike above threshold
- Entra ID connectivity failures
- Unusual patterns in failed sign-in attempts
- Token validation error rate increases
- Application unable to reach Entra ID metadata endpoint

**Dashboard Metrics:**
- Real-time authentication success/failure counts
- Authorization decisions (granted/denied) per minute
- Average authentication response time
- Active authenticated sessions count
- Token refresh success rate
- Top authentication error types

## Risks and Mitigations

### Technical Risks

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Entra ID service outage | High | Low | Implement graceful degradation, cache authentication state, display maintenance message, have support channel |
| Token validation failures due to key rotation | High | Medium | Implement automatic key refresh from JWKS endpoint, test key rotation scenarios, monitor token validation errors |
| Configuration errors in production | High | Medium | Use Infrastructure as Code, validate configuration in staging, automated deployment tests, configuration review checklist |
| Performance degradation with authentication checks | Medium | Medium | Implement efficient caching of authorization decisions, optimize policy evaluation, performance test under load |
| Dependency on Microsoft Identity packages | Medium | Low | Stay on LTS versions, monitor breaking changes, have update strategy, abstract authentication behind interfaces |
| Security vulnerability in authentication flow | High | Low | Security code review, penetration testing, follow OWASP guidelines, regular security audits, keep libraries updated |

### Business Risks

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Users unable to authenticate (business continuity) | High | Low | Entra ID SLA 99.99%, have support escalation process, communication plan for outages |
| User resistance to new authentication method | Medium | Medium | Clear communication, training materials, support documentation, gradual rollout |
| Compliance violation if not implemented correctly | High | Low | Compliance review before production, follow security best practices, documentation for audits |
| Increased operational complexity | Medium | Medium | Comprehensive documentation, training for ops team, monitoring and alerting, runbook for common issues |
| Additional Azure costs for Entra ID | Low | High | Free tier for basic features, monitor usage, project costs, optimize configuration |

### Security Risks

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Token theft or replay attacks | High | Low | Short token lifetimes, HTTPS only, secure cookie settings, anti-replay measures built into OAuth |
| Compromised client secret | High | Low | Rotate secrets regularly, store in Key Vault, monitor for unauthorized API calls, certificate-based auth alternative |
| Authorization bypass vulnerabilities | High | Low | Defense in depth, authorization in UI and Core, security code review, comprehensive testing |
| Session hijacking | Medium | Low | Secure cookie settings, session timeout, IP validation, suspicious activity monitoring |
| Insufficient logging for security incidents | Medium | Medium | Comprehensive audit logging, log retention policy, security monitoring tools, regular log reviews |

### Implementation Risks

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Development timeline underestimated | Medium | Medium | Phased implementation approach, MVP first, buffer time in estimates, regular progress reviews |
| Breaking existing functionality | Medium | Low | Comprehensive testing, preserve existing tests, regression testing, staged rollout |
| Incomplete testing | Medium | Medium | Test checklist, automated tests, code coverage requirements, security testing |
| Knowledge gap in Entra ID | Medium | Medium | Team training, Microsoft documentation review, proof of concept phase, expert consultation if needed |
| Integration issues with existing architecture | Medium | Low | Follow clean architecture principles, incremental integration, interface abstraction |

## Open Questions

### Technical Decisions

- [ ] **Question 1:** Should we implement custom user profile storage in Azure SQL, or rely entirely on Entra ID for user attributes?
  - **Context:** MVP doesn't require it, but Phase 4 features might benefit from local storage
  - **Recommendation:** Start without database, add if needed in Phase 4
  - **Decision Needed By:** Before Phase 2 completion

- [ ] **Question 2:** Should we use certificate-based authentication instead of client secret for production?
  - **Context:** Certificates are more secure but more complex to manage
  - **Recommendation:** Use client secret in Key Vault for MVP, consider certificates for enhancement
  - **Decision Needed By:** Before production deployment

- [ ] **Question 3:** What should the session timeout be?
  - **Context:** Balance between security and user convenience
  - **Options:** 30 minutes, 60 minutes, 8 hours
  - **Recommendation:** 60 minutes with sliding expiration (configurable)
  - **Decision Needed By:** Phase 3 implementation

- [ ] **Question 4:** Should we implement distributed session state for multi-instance deployments?
  - **Context:** Azure Container Apps can scale to multiple instances
  - **Options:** Redis cache, Azure SQL, or sticky sessions
  - **Recommendation:** Start with sticky sessions (Azure Container Apps default), add Redis if needed
  - **Decision Needed By:** Before production deployment

### Business Decisions

- [ ] **Question 5:** Which social identity providers should be enabled?
  - **Options:** Microsoft, Google, Facebook, GitHub, LinkedIn, Twitter
  - **Recommendation:** Microsoft and Google for Phase 4
  - **Decision Needed By:** Before Phase 4

- [ ] **Question 6:** Should we require email verification for all users?
  - **Context:** Trade-off between security and user friction
  - **Recommendation:** Yes, require email verification
  - **Decision Needed By:** Before Phase 2

- [ ] **Question 7:** Should we implement MFA for all users or just admins?
  - **Options:** All users, only admins, optional for users
  - **Recommendation:** Optional for users, required for admins
  - **Decision Needed By:** Before Phase 4

- [ ] **Question 8:** What should the password policy be for local accounts?
  - **Context:** Entra ID has default policies that can be customized
  - **Recommendation:** Use Entra ID defaults initially (8+ chars, complexity requirements)
  - **Decision Needed By:** During Entra ID configuration

### Compliance and Security

- [ ] **Question 9:** Do we need to support custom claims or attributes from external systems?
  - **Context:** Some organizations integrate with their own identity systems
  - **Recommendation:** Not needed for MVP, evaluate in Phase 4
  - **Decision Needed By:** Before Phase 4

- [ ] **Question 10:** What audit logging retention period is required?
  - **Context:** Compliance requirements may dictate retention
  - **Recommendation:** Consult with compliance team, default to 90 days
  - **Decision Needed By:** Before production deployment

- [ ] **Question 11:** Should we implement IP-based restrictions or geo-blocking?
  - **Context:** Additional security layer but may block legitimate users
  - **Recommendation:** Not needed initially, monitor and add if required
  - **Decision Needed By:** Phase 3 if needed

### Infrastructure

- [ ] **Question 12:** Should we use a dedicated Entra ID tenant or share with other applications?
  - **Context:** Isolation vs. management complexity
  - **Recommendation:** Dedicated External Identities tenant for this application
  - **Decision Needed By:** Before initial setup

- [ ] **Question 13:** What Azure region should host the Entra ID tenant?
  - **Context:** Data residency and latency considerations
  - **Recommendation:** Same region as Azure Container Apps deployment
  - **Decision Needed By:** During Entra ID tenant creation

## Appendix

### Related Documentation

**Microsoft Documentation:**
- [Entra ID External Identities Overview](https://learn.microsoft.com/en-us/entra/external-id/)
- [Secure ASP.NET Core Blazor authentication](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)
- [Microsoft.Identity.Web Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.web)
- [Azure AD B2C User Flows](https://learn.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview)

**Project Documentation:**
- Architecture guidelines: `.github/instructions/blazor-architecture.instructions.md`
- C# standards: `.github/instructions/csharp.instructions.md`
- DDD and .NET best practices: `.github/instructions/dotnet-architecture-good-practices.instructions.md`
- Project README: `README.md`

**Azure Documentation:**
- [Deploy to Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/)
- [Azure Key Vault Secrets](https://learn.microsoft.com/en-us/azure/key-vault/secrets/)
- [Managed Identity for Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/managed-identity)

### References

**Existing Features to Reference:**

While the application is currently minimal, follow these patterns when they exist:
- Service registration pattern: `src/GhcSamplePs.Web/Program.cs`
- Blazor component structure: `src/GhcSamplePs.Web/Components/`
- Core service patterns: `src/GhcSamplePs.Core/Services/` (to be established)
- Test structure: `tests/GhcSamplePs.Core.Tests/`

**Similar Patterns in Other Specs:**
- Configuration management: Reference UpgradeToDotNet10_Specification.md approach
- UI component patterns: Reference MudBlazor_Mobile_Integration_Specification.md approach

**External Examples:**
- Microsoft Identity Web samples repository (GitHub)
- Blazor authentication samples (ASP.NET Core documentation)
- Azure AD B2C samples (Microsoft Learn)

### Glossary

**Terms Used in This Specification:**

- **Entra ID**: Microsoft's identity and access management service (formerly Azure Active Directory)
- **External Identities**: Entra ID feature for authenticating external users (customers, partners)
- **OAuth 2.0**: Industry-standard authorization protocol
- **OpenID Connect**: Authentication layer built on OAuth 2.0
- **JWT**: JSON Web Token, used for secure token transmission
- **Claims**: Pieces of information about a user (name, email, roles)
- **Principal**: Authenticated user identity in .NET
- **Policy**: Set of requirements for authorization decision
- **JWKS**: JSON Web Key Set, public keys for token validation
- **Client ID**: Unique identifier for application in Entra ID
- **Client Secret**: Password for application to authenticate with Entra ID
- **Tenant**: Instance of Entra ID for an organization
- **Redirect URI**: URL where Entra ID sends users after authentication
- **Callback Path**: Application path handling authentication callback
- **Managed Identity**: Azure feature for secure credential management

### Setup Checklist

**Pre-Implementation Checklist:**

- [ ] Azure subscription available
- [ ] Permissions to create Entra ID resources
- [ ] Development environment set up (.NET 10 SDK installed)
- [ ] Team trained on Entra ID concepts
- [ ] Security requirements documented
- [ ] Compliance requirements understood
- [ ] Stakeholder approval obtained

**Entra ID Configuration Checklist:**

- [ ] Create Entra ID External Identities tenant
- [ ] Register application in Entra ID
- [ ] Configure redirect URIs for all environments
- [ ] Create app roles (Admin, User)
- [ ] Configure identity providers (Microsoft, Google)
- [ ] Create user flows for sign-up and sign-in
- [ ] Create test users for development
- [ ] Generate and securely store client secret
- [ ] Document configuration for team

**Development Environment Checklist:**

- [ ] Clone repository
- [ ] Configure user secrets with client secret
- [ ] Update appsettings.Development.json
- [ ] Test connection to Entra ID
- [ ] Create test users in Entra ID
- [ ] Verify development environment works

**Production Deployment Checklist:**

- [ ] Create Azure Key Vault
- [ ] Store production client secret in Key Vault
- [ ] Configure production redirect URIs in Entra ID
- [ ] Set up managed identity for Container App
- [ ] Grant Key Vault access to managed identity
- [ ] Configure environment variables in Container Apps
- [ ] Test authentication in staging environment
- [ ] Perform security review
- [ ] Update documentation
- [ ] Deploy to production
- [ ] Verify production authentication
- [ ] Set up monitoring and alerts
- [ ] Train support team

---

**Document Version:** 1.0  
**Last Updated:** 2025-11-24  
**Status:** Draft - Pending Review and Approval  
**Author:** Feature Specification Architect Agent  
**Reviewers:** Development Team, Security Team, Product Owner
