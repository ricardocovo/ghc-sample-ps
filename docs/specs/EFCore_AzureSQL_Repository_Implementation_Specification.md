# Feature Specification: Entity Framework Core Repository Implementation with Azure SQL Database

## Executive Summary

**Feature Name**: Entity Framework Core Data Access Layer with Azure SQL Database  
**Version**: 1.0  
**Last Updated**: November 28, 2025  

### Brief Description
This specification defines the implementation of a production-ready data access layer using Entity Framework Core 10 with Azure SQL Database. It replaces the current in-memory mock repository with a persistent database solution, including DbContext configuration, entity configurations, migrations, and connection management for both development and production environments.

### Business Value
- **Data Persistence**: Ensures player and user data survives application restarts
- **Scalability**: Supports growing data volumes and concurrent users
- **Production Readiness**: Enables deployment to cloud environment with enterprise-grade database
- **Data Integrity**: Enforces referential integrity and constraints at database level
- **Audit Trail**: Maintains complete history of data changes with timestamp tracking
- **Performance**: Optimized queries and indexing for fast data retrieval

### Key Stakeholders
- **Technical Owner**: Development Team, Database Administrator
- **Business Owner**: Product Owner
- **Operations**: DevOps Team, Cloud Infrastructure Team
- **End Users**: Coaches, Parents, Administrators (indirect beneficiaries)

---

## Requirements

### Functional Requirements

#### FR1: Database Context Configuration
- Configure EF Core DbContext to manage database connections and entity mappings
- Support connection to Azure SQL Database with connection string configuration
- Implement proper lifetime management (Scoped) for DbContext
- Enable sensitive data logging for development, disable for production
- Configure retry policies for transient failures

#### FR2: Entity Configuration and Mapping
- Configure Player entity with Fluent API for precise schema control
- Configure ApplicationUser entity (if persisting user data)
- Define primary keys, foreign keys, and indexes
- Configure column types, lengths, and constraints
- Map property names to database column names
- Configure audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)

#### FR3: Database Migrations
- Create initial migration with all entity tables
- Include seed data for development and testing
- Support up and down migration paths
- Version control migration files
- Apply migrations automatically on application startup (development)
- Provide migration scripts for production deployment

#### FR4: Repository Implementation
- Implement IPlayerRepository using EF Core
- Support all CRUD operations asynchronously
- Handle database exceptions gracefully
- Implement change tracking and optimistic concurrency
- Support querying with Include/ThenInclude for related entities
- Implement efficient bulk operations when needed

#### FR5: Connection String Management
- Store connection strings securely using User Secrets (development)
- Reference Azure Key Vault for connection strings (production)
- Support multiple environments (Development, Staging, Production)
- Never commit connection strings to source control
- Use managed identity for Azure SQL authentication when possible

#### FR6: Database Schema Design
- Design tables following database normalization principles
- Implement appropriate indexes for query performance
- Add unique constraints where needed
- Configure cascade delete behaviors appropriately
- Design with future extensibility in mind (teams, statistics)

### Non-Functional Requirements

#### NFR1: Performance
- Query response time < 100ms for single entity retrieval
- List queries return results < 500ms for up to 10,000 records
- Database connection pooling enabled and optimized
- Implement query result caching where appropriate
- Use AsNoTracking for read-only queries

#### NFR2: Reliability
- Implement retry logic for transient database failures
- Handle database connection failures gracefully
- Log all database errors with context
- Support connection resiliency with circuit breaker pattern
- Maintain data consistency during concurrent operations

#### NFR3: Security
- Use parameterized queries to prevent SQL injection (built-in with EF)
- Encrypt connection strings at rest and in transit
- Use least-privilege database accounts
- Enable Azure SQL firewall rules
- Audit sensitive data access
- Never log connection strings or sensitive data

#### NFR4: Maintainability
- Follow repository pattern for clean separation
- Use Fluent API for explicit entity configuration
- Document migration purposes with clear comments
- Keep migration files small and focused
- Follow EF Core best practices and conventions

#### NFR5: Testability
- Support in-memory database provider for unit testing
- Repository implementations testable with EF Core InMemory
- Seed data available for test scenarios
- Mock DbContext for unit tests when needed
- Integration tests use test database container

#### NFR6: Scalability
- Design for horizontal scaling with multiple app instances
- Use optimistic concurrency for conflict resolution
- Implement connection pooling with appropriate pool size
- Support read replicas for query load distribution (future)
- Design schema to handle growing data volumes

### User Stories

#### US1: Persist Player Data
**As a** coach  
**I want** player data to be saved permanently in the database  
**So that** I don't lose information when the application restarts

**Acceptance Criteria:**
- Player data survives application restarts
- All CRUD operations persist to database
- Data integrity is maintained
- No data loss during normal operations

#### US2: Concurrent Data Access
**As a** system administrator  
**I want** multiple users to safely access player data simultaneously  
**So that** concurrent operations don't corrupt data

**Acceptance Criteria:**
- Multiple users can read data concurrently
- Concurrent writes handled with optimistic concurrency
- Last-write-wins strategy with proper conflict detection
- No deadlocks or race conditions

#### US3: Database Migration Management
**As a** developer  
**I want** structured database migrations  
**So that** schema changes are version-controlled and deployable

**Acceptance Criteria:**
- Migration files generated automatically
- Migrations applied consistently across environments
- Rollback capability for failed migrations
- Migration history tracked in database

#### US4: Secure Connection Management
**As a** security engineer  
**I want** database connection strings stored securely  
**So that** credentials are never exposed in code or logs

**Acceptance Criteria:**
- No connection strings in source control
- Development uses User Secrets
- Production uses Azure Key Vault
- Connection strings encrypted at rest
- No credentials in application logs

### Acceptance Criteria Summary

**Definition of Done:**
- EF Core DbContext configured and registered
- All entities mapped with Fluent API
- Initial migration created and tested
- Repository implementations using EF Core complete
- Connection string management secure
- Development and production configurations working
- Unit tests updated to use InMemory provider
- Integration tests verify database operations
- Documentation complete
- Code review approved

---

## Technical Design

### Architecture Impact

**Components/Projects Affected:**
- **GhcSamplePs.Core**: Add DbContext, entity configurations, EF repository implementations
- **GhcSamplePs.Web**: Update Program.cs for DbContext registration, add migration application logic
- **GhcSamplePs.Core.Tests**: Update tests to use EF InMemory provider
- **Infrastructure**: New Azure SQL Database resource

**New Components Required:**
- `ApplicationDbContext` class (EF Core DbContext)
- Entity configuration classes (Fluent API)
- EF-based repository implementations
- Database migrations
- Seed data logic
- Connection resiliency policies
- Migration helper classes

**Integration Points:**
- Service layer continues using IPlayerRepository interface (no changes)
- DbContext registered in DI container
- Repository implementations injected into services
- Migration applied on application startup (development)
- Azure SQL connection through connection string

**Data Flow:**
1. Service calls repository method
2. Repository uses DbContext to query/modify entities
3. DbContext translates to SQL and executes against Azure SQL
4. Results returned through repository to service
5. Service returns results to UI layer

### Implementation Details

#### Data Layer - DbContext Design

**ApplicationDbContext Class**

Located in: `src/GhcSamplePs.Core/Data/ApplicationDbContext.cs`

**Purpose:** Central database context managing all entity types

**Properties:**
- DbSet\<Player\> Players: Player entities collection
- DbSet\<ApplicationUser\> Users: User entities collection (future if storing locally)

**Methods:**
- OnModelCreating(ModelBuilder): Configure entities using Fluent API
- OnConfiguring(DbContextOptionsBuilder): Configure database provider (if needed)
- SaveChangesAsync(cancellationToken): Override to add audit field updates
- SeedData(ModelBuilder): Configure seed data for development

**Responsibilities:**
- Manage database connections
- Track entity changes
- Generate SQL queries
- Execute commands against database
- Manage transactions
- Apply entity configurations

**Base Configuration:**
- Connection string from configuration
- Enable sensitive data logging (development only)
- Enable detailed errors (development only)
- Configure command timeout (30 seconds)
- Enable retry on failure with exponential backoff

**SaveChangesAsync Override Logic:**
- Iterate through tracked entities with state Added or Modified
- Update audit fields automatically:
  - For Added: Set CreatedAt to UtcNow, set CreatedBy from current user
  - For Modified: Set UpdatedAt to UtcNow, set UpdatedBy from current user
- Call base.SaveChangesAsync

#### Entity Configurations (Fluent API)

**Player Entity Configuration**

Located in: `src/GhcSamplePs.Core/Data/Configurations/PlayerConfiguration.cs`

**Configuration Approach:** Implement `IEntityTypeConfiguration<Player>`

**Schema Mapping:**

| Property | Database Column | SQL Type | Constraints | Index |
|----------|----------------|----------|-------------|-------|
| Id | Id | int | PRIMARY KEY, IDENTITY(1,1) | Clustered |
| UserId | UserId | nvarchar(450) | NOT NULL | Non-clustered index |
| Name | Name | nvarchar(200) | NOT NULL | Non-clustered index for search |
| DateOfBirth | DateOfBirth | date | NOT NULL | Non-clustered index for age queries |
| Gender | Gender | nvarchar(50) | NULL | None |
| PhotoUrl | PhotoUrl | nvarchar(500) | NULL | None |
| CreatedAt | CreatedAt | datetime2(7) | NOT NULL, DEFAULT GETUTCDATE() | None |
| CreatedBy | CreatedBy | nvarchar(450) | NOT NULL | None |
| UpdatedAt | UpdatedAt | datetime2(7) | NULL | None |
| UpdatedBy | UpdatedBy | nvarchar(450) | NULL | None |

**Fluent API Configuration:**
- Table name: "Players"
- Primary key: Id with IDENTITY
- Required fields marked with IsRequired()
- String lengths set with HasMaxLength()
- Indexes created with HasIndex()
- Property configurations for computed Age (ignored, not stored)
- Row version for optimistic concurrency (future enhancement)

**Indexes Design:**
- Primary key on Id (clustered, automatically created)
- Index on UserId for filtering players by user
- Index on Name for search/filter operations
- Index on DateOfBirth for age-based queries
- Composite index on (UserId, Name) for common query pattern

**ApplicationUser Entity Configuration**

Located in: `src/GhcSamplePs.Core/Data/Configurations/ApplicationUserConfiguration.cs`

Note: Only needed if persisting users locally (likely not needed with Entra ID)

**Decision Point:** Determine if ApplicationUser should be persisted to database or remain transient from claims. For MVP, likely **NOT** persisting users, only using claims from Entra ID.

If persisting users in future:

| Property | Database Column | SQL Type | Constraints |
|----------|----------------|----------|-------------|
| Id | Id | nvarchar(450) | PRIMARY KEY |
| Email | Email | nvarchar(256) | NOT NULL, UNIQUE |
| DisplayName | DisplayName | nvarchar(200) | NOT NULL |
| GivenName | GivenName | nvarchar(100) | NULL |
| FamilyName | FamilyName | nvarchar(100) | NULL |
| IsActive | IsActive | bit | NOT NULL, DEFAULT 1 |
| LastLoginDate | LastLoginDate | datetime2(7) | NULL |
| CreatedDate | CreatedDate | datetime2(7) | NOT NULL |

#### Database Migrations

**Initial Migration**

Migration Name: `InitialCreate`

Purpose: Create initial database schema with Players table

**Up Migration Creates:**
- Players table with all columns and constraints
- Primary key and indexes
- Default values for audit columns

**Down Migration Drops:**
- All indexes
- Players table

**Seed Data:**
- 10 sample players (same as current mock data)
- Realistic test data for development
- Only applied in Development environment

**Seed Data Players:**

| Name | DateOfBirth | Gender | UserId |
|------|-------------|--------|--------|
| Emma Rodriguez | 2014-03-15 | Female | user-001 |
| Liam Johnson | 2015-07-22 | Male | user-001 |
| Olivia Martinez | 2013-11-08 | Female | user-002 |
| Noah Williams | 2016-01-30 | Male | user-002 |
| Ava Brown | 2014-09-12 | Female | user-003 |
| Ethan Davis | 2015-04-05 | Male | user-003 |
| Sophia Garcia | 2013-06-18 | Non-binary | user-004 |
| Mason Miller | 2016-12-25 | Male | user-004 |
| Isabella Wilson | 2014-02-14 | Female | user-005 |
| Lucas Anderson | 2015-10-09 | Prefer not to say | user-005 |

**Creating Migrations:**

Commands to execute:
```
cd src/GhcSamplePs.Core
dotnet ef migrations add InitialCreate --startup-project ../GhcSamplePs.Web
```

**Applying Migrations:**

Development (automatic on startup):
- DbContext.Database.Migrate() called in Program.cs
- Only in Development environment
- Logs migration application

Production (manual script):
```
dotnet ef migrations script --startup-project src/GhcSamplePs.Web --idempotent
```
- Generate SQL script
- Review script
- Apply manually or via deployment pipeline
- Use idempotent scripts (safe to run multiple times)

#### Repository Implementation - EF Core

**EF PlayerRepository Implementation**

Located in: `src/GhcSamplePs.Core/Repositories/Implementations/EfPlayerRepository.cs`

**Class Name:** `EfPlayerRepository`

Implements: `IPlayerRepository`

**Dependencies:**
- ApplicationDbContext (injected via constructor)
- ILogger<EfPlayerRepository> (for logging)

**Method Implementations:**

**GetAllAsync:**
- Query: `_context.Players.AsNoTracking().OrderBy(p => p.Name).ToListAsync()`
- Use AsNoTracking for better read performance
- Order by Name for consistent results
- Include cancellation token support

**GetByIdAsync:**
- Query: `_context.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken)`
- Returns null if not found
- AsNoTracking since read-only

**AddAsync:**
- Add entity: `_context.Players.Add(player)`
- Save changes: `await _context.SaveChangesAsync(cancellationToken)`
- Return added entity with generated Id
- Audit fields set automatically by SaveChangesAsync override
- Log successful creation

**UpdateAsync:**
- Retrieve existing: `await _context.Players.FindAsync(new object[] { player.Id }, cancellationToken)`
- Throw PlayerNotFoundException if not found
- Update properties manually or use Entry.CurrentValues.SetValues
- Save changes: `await _context.SaveChangesAsync(cancellationToken)`
- Audit fields updated automatically
- Handle concurrency exceptions
- Log successful update

**DeleteAsync:**
- Retrieve existing: `await _context.Players.FindAsync(new object[] { id }, cancellationToken)`
- If found, remove: `_context.Players.Remove(player)`
- Save changes: `await _context.SaveChangesAsync(cancellationToken)`
- Return true if deleted, false if not found
- Log deletion

**ExistsAsync:**
- Query: `await _context.Players.AnyAsync(p => p.Id == id, cancellationToken)`
- Efficient existence check without loading entity

**Error Handling:**
- Catch DbUpdateException for constraint violations
- Catch DbUpdateConcurrencyException for concurrent updates
- Catch SqlException for database connection issues
- Log all exceptions with context
- Rethrow as domain-specific exceptions

**Transaction Handling:**
- SaveChanges automatically uses transaction
- For multi-step operations, use explicit transaction:
  - Begin transaction
  - Execute operations
  - Commit on success
  - Rollback on exception

#### Connection String Management

**Connection String Structure:**

Standard Azure SQL connection string:
```
Server=tcp:your-server.database.windows.net,1433;
Initial Catalog=ghcsampleps-db;
Persist Security Info=False;
User ID=your-username;
Password=your-password;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

With Managed Identity (recommended for production):
```
Server=tcp:your-server.database.windows.net,1433;
Initial Catalog=ghcsampleps-db;
Authentication=Active Directory Managed Identity;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

**Development Environment:**

Storage: User Secrets

Set secret command:
```powershell
cd src\GhcSamplePs.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true"
```

Or for Azure SQL:
```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=ghcsampleps-dev;User ID=devuser;Password=DevPassword123!;Encrypt=True;"
```

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Connection string from user secrets"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**Production Environment:**

Storage: Azure Key Vault

**appsettings.json (production):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/DbConnectionString/)"
  }
}
```

Azure Container Apps Configuration:
- Add Key Vault reference as environment variable
- Configure managed identity for Key Vault access
- Connection string retrieved at runtime

**Staging Environment:**

Use separate database instance:
- Server: same as production
- Database: ghcsampleps-staging
- Connection string in Key Vault
- Configuration similar to production

#### DbContext Registration and Configuration

**Program.cs Changes**

Located in: `src/GhcSamplePs.Web/Program.cs`

**Add DbContext Registration:**

```csharp
// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register ApplicationDbContext with SQL Server provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Enable retry on transient failures
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        
        // Set command timeout
        sqlOptions.CommandTimeout(30);
        
        // Enable query splitting for related data
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
    
    // Enable sensitive data logging in development only
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

**Update Repository Registration:**

Replace:
```csharp
builder.Services.AddSingleton<IPlayerRepository, MockPlayerRepository>();
```

With:
```csharp
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
```

**Apply Migrations on Startup (Development):**

```csharp
// After app.Build(), before app.Run()
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Apply pending migrations
    await dbContext.Database.MigrateAsync();
    
    app.Logger.LogInformation("Database migrations applied successfully");
}
```

**Health Checks (Optional but Recommended):**

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

// Map health check endpoint
app.MapHealthChecks("/health");
```

### Code Conventions to Follow

**From Project Guidelines:**

**EF Core Specific Conventions:**
- DbContext class name ends with "DbContext"
- Configuration classes in `Data/Configurations/` folder
- One configuration class per entity
- Implement `IEntityTypeConfiguration<TEntity>`
- Use Fluent API, not data annotations for entity configuration
- Migration names use PascalCase descriptive names
- Repository implementations prefixed with "Ef" (EfPlayerRepository)

**Naming:**
- DbContext: ApplicationDbContext
- Configurations: PlayerConfiguration, ApplicationUserConfiguration
- Repository: EfPlayerRepository
- Migration: InitialCreate, AddPlayerIndexes, etc.

**File Organization:**
```
Core/
  Data/
    ApplicationDbContext.cs
    Configurations/
      PlayerConfiguration.cs
      ApplicationUserConfiguration.cs
  Repositories/
    Interfaces/
      IPlayerRepository.cs (existing, no changes)
    Implementations/
      MockPlayerRepository.cs (keep for testing)
      EfPlayerRepository.cs (new)
  Migrations/
    [timestamp]_InitialCreate.cs
    [timestamp]_InitialCreate.Designer.cs
    ApplicationDbContextModelSnapshot.cs
```

**Testing Conventions:**
- Use EF InMemory provider for unit tests
- Create new DbContext instance per test
- Use unique database names for each test
- Dispose DbContext after each test
- Test naming: `MethodName_Condition_ExpectedResult`

### Dependencies

**NuGet Packages Required for GhcSamplePs.Core:**

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

**NuGet Packages Required for GhcSamplePs.Core.Tests:**

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.0" />
```

**Azure Resources Required:**
- Azure SQL Server instance
- Azure SQL Database (Basic or Standard tier for development)
- Azure Key Vault (for connection string storage)
- Managed Identity for Container App (for Key Vault access)

**Configuration Files:**
- appsettings.json (no connection string, only Key Vault reference)
- appsettings.Development.json (logging configuration)
- User Secrets (local connection string)

### Security Considerations

**SQL Injection Prevention:**
- EF Core uses parameterized queries automatically
- Never concatenate SQL strings
- Never use FromSqlRaw with string interpolation
- Use FromSqlInterpolated or parameters for raw SQL if needed

**Connection String Security:**
- Never commit connection strings to source control
- Use User Secrets for development
- Use Azure Key Vault for production
- Encrypt connection strings at rest
- Use managed identity when possible (eliminates password)

**Database Access Control:**
- Use least-privilege database accounts
- Application account has only necessary permissions: SELECT, INSERT, UPDATE, DELETE on specific tables
- No DDL permissions on application account
- Separate admin account for migrations
- Use Azure SQL firewall rules to limit access

**Data Protection:**
- Encrypt data in transit (Encrypt=True in connection string)
- Enable Azure SQL Transparent Data Encryption (TDE)
- Audit sensitive data access
- Mask sensitive fields in logs (no connection strings logged)
- Regular security updates for EF Core packages

**Authentication:**
- Prefer Azure AD authentication over SQL authentication
- Use managed identity in production
- Rotate secrets regularly if using SQL authentication
- Strong passwords for database accounts

### Error Handling

**Expected Exceptions:**

**DbUpdateException:**
- Thrown when: Constraint violations, data integrity issues
- Causes: Duplicate keys, foreign key violations, check constraints
- Handled by: Repository layer catches and translates to domain exception
- User sees: Friendly error message like "Player name already exists"
- Logged: Warning level with constraint details

**DbUpdateConcurrencyException:**
- Thrown when: Concurrent update conflict detected
- Causes: Record modified by another user between read and save
- Handled by: Repository catches, optionally retries or returns conflict result
- User sees: "Data was modified by another user, please refresh"
- Logged: Warning level with entity details

**SqlException:**
- Thrown when: Database connection or server errors
- Causes: Connection timeout, server unavailable, network issues
- Handled by: Retry policy handles transient errors, critical errors bubble up
- User sees: "Database temporarily unavailable, please try again"
- Logged: Error level with full exception details

**InvalidOperationException:**
- Thrown when: DbContext misuse, entity not tracked
- Causes: Programming errors
- Handled by: Development-time issue, should not occur in production
- User sees: Generic error message
- Logged: Error level

**Error Messages:**

**User-Facing:**
- Connection errors: "Unable to connect to database. Please try again."
- Constraint violation: "Unable to save: [specific constraint violated]"
- Concurrency conflict: "The data has been modified. Please refresh and try again."
- Timeout: "Operation took too long. Please try again."
- General error: "An error occurred while saving data."

**Logging:**
- Log all database exceptions
- Include operation attempted (Add, Update, Delete, Query)
- Include entity type and ID
- Include user context
- Mask sensitive data
- Include full exception stack trace at Error level

**Retry Logic:**
- Transient errors automatically retried up to 5 times
- Exponential backoff between retries (2s, 4s, 8s, 16s, 30s)
- Retried errors: Connection timeout, deadlock, transient network
- Not retried: Constraint violations, authentication failures
- Log each retry attempt

---

## Testing Strategy

### Unit Test Requirements

**Test Project**: `tests/GhcSamplePs.Core.Tests/Repositories/`

**Coverage Target**: 90% for repository implementations

**Test Categories:**

**1. EfPlayerRepository Tests**

File: `Repositories/EfPlayerRepositoryTests.cs`

Approach: Use EF Core InMemory provider for testing

Test scenarios:
- GetAllAsync_WhenPlayersExist_ReturnsAllPlayers
- GetAllAsync_WhenDatabaseEmpty_ReturnsEmptyList
- GetByIdAsync_WhenPlayerExists_ReturnsPlayer
- GetByIdAsync_WhenPlayerNotFound_ReturnsNull
- AddAsync_WithValidPlayer_AddsToDatabase
- AddAsync_SetsCreatedAtAndCreatedBy
- AddAsync_GeneratesId
- UpdateAsync_WithValidPlayer_UpdatesInDatabase
- UpdateAsync_SetsUpdatedAtAndUpdatedBy
- UpdateAsync_WhenPlayerNotFound_ThrowsException
- DeleteAsync_WhenPlayerExists_RemovesFromDatabase
- DeleteAsync_WhenPlayerNotFound_ReturnsFalse
- ExistsAsync_WhenPlayerExists_ReturnsTrue
- ExistsAsync_WhenPlayerNotFound_ReturnsFalse

**2. ApplicationDbContext Tests**

File: `Data/ApplicationDbContextTests.cs`

Test scenarios:
- SaveChangesAsync_WhenAddingEntity_SetsCreatedAtAndCreatedBy
- SaveChangesAsync_WhenUpdatingEntity_SetsUpdatedAtAndUpdatedBy
- SaveChangesAsync_PreservesCreatedAtWhenUpdating
- Configuration_PlayerEntity_HasCorrectIndexes
- Configuration_PlayerEntity_EnforcesMaxLengths
- Configuration_PlayerEntity_RequiredFieldsEnforced
- SeedData_InDevelopment_ContainsSamplePlayers

**3. Player Entity Persistence Tests**

File: `Models/PlayerManagement/PlayerPersistenceTests.cs`

Test scenarios:
- Player_SavedToDatabase_CanBeRetrieved
- Player_WithAllFields_PersistsCorrectly
- Player_WithOptionalFieldsNull_PersistsCorrectly
- Player_NameIndex_ImprovesQueryPerformance
- Player_ConcurrentUpdate_DetectsConflict

**Test Data Setup:**

Helper class: `TestHelpers/DbContextFactory.cs`

Provides:
- CreateInMemoryDbContext(): Returns ApplicationDbContext with InMemory provider
- SeedTestPlayers(context): Adds test player data
- GetTestConnectionString(): Returns test database connection string (for integration tests)

**Testing Approach:**

Each test:
1. Arrange: Create in-memory DbContext
2. Act: Execute repository method
3. Assert: Verify expected outcome
4. Dispose: Clean up DbContext

**Example Test Structure:**

```
Test method structure:
- Create unique in-memory database
- Seed with necessary data
- Create repository with DbContext
- Execute operation
- Query database directly to verify
- Assert expected results
```

### Integration Test Requirements

**Purpose:** Verify EF Core works correctly with actual SQL Server

**Approach:** Use Docker container with SQL Server for tests

**Test Project**: `tests/GhcSamplePs.Integration.Tests/` (new project to create)

**Test Scenarios:**
- Full CRUD cycle with real database
- Migration application
- Connection resiliency
- Transaction rollback
- Concurrent access scenarios
- Performance benchmarks

**Setup:**
- Use Testcontainers library to spin up SQL Server container
- Apply migrations before tests
- Clean up database after tests
- Run as separate test suite (not in unit test run)

**Note:** Integration tests are optional for Phase 1, recommended for Phase 2

### Migration Testing

**Test Approach:**

1. **Up Migration Test:**
   - Start with empty database
   - Apply migration
   - Verify tables created
   - Verify indexes created
   - Verify seed data inserted (if applicable)

2. **Down Migration Test:**
   - Apply migration
   - Rollback migration
   - Verify database returns to previous state
   - Verify no orphaned objects

3. **Idempotent Script Test:**
   - Apply migration
   - Run idempotent script again
   - Verify no errors
   - Verify no duplicate data

**Testing Commands:**

```powershell
# Test migration up
dotnet ef database update --startup-project src\GhcSamplePs.Web

# Test migration down
dotnet ef database update 0 --startup-project src\GhcSamplePs.Web

# Generate and review idempotent script
dotnet ef migrations script --idempotent --startup-project src\GhcSamplePs.Web
```

### Code Coverage Expectations

**Minimum Coverage Targets:**
- EfPlayerRepository: 95%
- ApplicationDbContext: 85%
- Entity Configurations: 100% (simple configuration code)
- Overall Data Layer: 90%

**Excluded from Coverage:**
- Migration files (auto-generated)
- DbContext constructors
- OnConfiguring methods (configuration only)

---

## Implementation Phases

### Phase 1: Core EF Implementation (This Specification)

**Week 1: Foundation Setup**

**Days 1-2: NuGet Packages and DbContext**
- Add EF Core NuGet packages to Core project
- Create ApplicationDbContext class
- Configure DbContext with SQL Server provider
- Implement SaveChangesAsync override for audit fields
- Write DbContext unit tests

**Days 3-4: Entity Configuration**
- Create PlayerConfiguration with Fluent API
- Define all constraints, indexes, and column mappings
- Configure table names and relationships
- Write configuration unit tests

**Day 5: Initial Migration**
- Generate InitialCreate migration
- Review generated migration code
- Add seed data to migration
- Test migration locally with LocalDB

**Week 2: Repository Implementation**

**Days 1-2: EF Repository**
- Implement EfPlayerRepository
- Implement all IPlayerRepository methods
- Add error handling and logging
- Handle concurrency conflicts

**Days 3-4: Repository Testing**
- Write comprehensive unit tests for repository
- Test all CRUD operations
- Test error scenarios
- Test concurrent access patterns
- Achieve 90%+ coverage

**Day 5: Integration and Testing**
- Update Program.cs registration
- Test with real LocalDB
- Verify migration application
- Test all user workflows

**Week 3: Connection Management and Documentation**

**Days 1-2: Connection String Management**
- Configure User Secrets for development
- Document connection string setup
- Test connection to Azure SQL (if available)
- Configure retry policies

**Days 3-4: Update Existing Tests**
- Update service tests if needed
- Ensure all tests still pass
- Test switching between Mock and EF repositories
- Performance testing

**Day 5: Documentation and Review**
- Complete documentation
- Update README files
- Code review
- Final testing

**Timeline**: 3 weeks (15 working days)

**Success Criteria:**
- EF Core DbContext configured and working
- All repository operations using database
- Migration created and tested
- Unit tests passing with 90%+ coverage
- Connection strings managed securely
- Documentation complete
- Code review approved

### Phase 2: Production Deployment (Future)

**Scope**: Deploy to Azure SQL Database in production

**Deliverables:**
1. Provision Azure SQL Server and Database
2. Configure firewall rules
3. Set up Key Vault for connection string
4. Configure managed identity
5. Apply migrations to production database
6. Deploy application to Azure Container Apps
7. Monitor and verify

**Prerequisites:**
- Phase 1 complete and tested
- Azure infrastructure ready
- Connection strings in Key Vault

**Timeline**: 1 week

### Phase 3: Performance Optimization (Future)

**Scope**: Optimize database performance for production load

**Deliverables:**
1. Query optimization and tuning
2. Index optimization based on query patterns
3. Implement query result caching
4. Add database performance monitoring
5. Optimize connection pooling
6. Implement read replicas (if needed)

**Prerequisites:**
- Phase 2 complete
- Production load data available
- Performance monitoring in place

**Timeline**: 1-2 weeks

### Phase 4: Advanced Features (Future)

**Scope**: Add advanced EF Core features

**Deliverables:**
1. Soft delete functionality
2. Audit log tables
3. Complex queries with projections
4. Optimistic concurrency UI handling
5. Bulk operations
6. Database health monitoring

**Prerequisites:**
- Phase 3 complete
- Business requirements defined

**Timeline**: 2 weeks

---

## Migration and Deployment Considerations

### Database Setup

**Azure SQL Database Provisioning:**

**Resource Configuration:**
- Resource Group: rg-ghcsampleps
- SQL Server Name: sql-ghcsampleps-[env]
- Database Name: ghcsampleps-db
- Pricing Tier: Basic (5 DTUs) for development, Standard S0 (10 DTUs) for production
- Backup: Automated with 7-day retention
- Geo-redundancy: Disabled for development, enabled for production

**Firewall Rules:**
- Allow Azure services
- Development: Add developer IP addresses
- Production: Container Apps subnet only

**Authentication:**
- SQL Authentication for development
- Azure AD authentication for production (with managed identity)

**Creating Database via Azure CLI:**

```bash
# Create SQL Server
az sql server create \
  --name sql-ghcsampleps-dev \
  --resource-group rg-ghcsampleps \
  --location eastus \
  --admin-user sqladmin \
  --admin-password [SecurePassword123!]

# Create Database
az sql db create \
  --resource-group rg-ghcsampleps \
  --server sql-ghcsampleps-dev \
  --name ghcsampleps-db \
  --service-objective Basic

# Configure firewall
az sql server firewall-rule create \
  --resource-group rg-ghcsampleps \
  --server sql-ghcsampleps-dev \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### Migration Deployment Strategy

**Development Environment:**
- Migrations applied automatically on application startup
- Database.Migrate() called in Program.cs
- Safe for development, not recommended for production

**Staging Environment:**
- Manual migration script execution
- Review script before applying
- Backup database before migration
- Apply during maintenance window

**Production Environment:**
- Use idempotent migration scripts
- Review and test scripts in staging first
- Create database backup before migration
- Apply via deployment pipeline or manual execution
- Monitor application during and after migration
- Have rollback plan ready

**Migration Script Generation:**

```powershell
# Generate idempotent script for all migrations
dotnet ef migrations script --idempotent --startup-project src\GhcSamplePs.Web --output migrations.sql

# Generate script for specific migration range
dotnet ef migrations script PreviousMigration TargetMigration --idempotent --startup-project src\GhcSamplePs.Web --output delta-migration.sql
```

**Applying Migration Script:**

```bash
# Using sqlcmd
sqlcmd -S tcp:sql-ghcsampleps-prod.database.windows.net,1433 \
  -d ghcsampleps-db \
  -U sqladmin \
  -P [password] \
  -i migrations.sql

# Or via Azure Data Studio GUI
```

### Configuration Changes

**Development Configuration:**

User Secrets to set:
```
ConnectionStrings:DefaultConnection = Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;
```

Or for Azure SQL:
```
ConnectionStrings:DefaultConnection = Server=tcp:sql-ghcsampleps-dev.database.windows.net,1433;Initial Catalog=ghcsampleps-db;User ID=sqladmin;Password=[password];Encrypt=True;
```

**Production Configuration:**

Azure Container Apps Environment Variables:
```
CONNECTIONSTRINGS__DEFAULTCONNECTION = @Microsoft.KeyVault(SecretUri=https://kv-ghcsampleps.vault.azure.net/secrets/DbConnectionString/)
```

Key Vault Secret Value:
```
Server=tcp:sql-ghcsampleps-prod.database.windows.net,1433;Initial Catalog=ghcsampleps-db;Authentication=Active Directory Managed Identity;Encrypt=True;
```

**Program.cs Changes:**

Replace mock repository registration:
```csharp
// OLD
builder.Services.AddSingleton<IPlayerRepository, MockPlayerRepository>();

// NEW
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
```

Add DbContext registration (before service registrations):
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(30);
    });
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

### Deployment Steps

**Phase 1 - Local Development:**

1. Install EF Core tools globally
2. Add NuGet packages to projects
3. Create DbContext and configurations
4. Generate initial migration
5. Configure User Secrets
6. Apply migration locally
7. Test all CRUD operations
8. Run unit tests
9. Verify data persistence

**Phase 2 - Azure SQL Setup:**

1. Provision Azure SQL Server and Database
2. Configure firewall rules
3. Store connection string in Key Vault
4. Configure managed identity for Container App
5. Grant database permissions to managed identity
6. Update Container App configuration
7. Deploy application
8. Apply migrations
9. Verify application connectivity

**Phase 3 - Production Deployment:**

1. Backup current production database (if exists)
2. Generate migration script
3. Review script in staging
4. Schedule maintenance window
5. Apply migration to production
6. Deploy updated application
7. Smoke test critical functionality
8. Monitor application logs and performance
9. Verify data integrity

### Rollback Strategy

**If Migration Fails:**

1. Stop application
2. Restore database from backup
3. Verify backup integrity
4. Restart application with previous version
5. Investigate migration failure
6. Fix issues and test in staging
7. Plan new deployment

**If Application Fails After Migration:**

1. Check application logs for specific errors
2. Verify connection string configuration
3. Check firewall rules and network connectivity
4. Verify managed identity permissions
5. If unrecoverable:
   - Deploy previous application version
   - Consider rolling back migration if needed
   - Restore from backup as last resort

**Database Backup Before Migration:**

```bash
# Create backup before migration
az sql db export \
  --resource-group rg-ghcsampleps \
  --server sql-ghcsampleps-prod \
  --name ghcsampleps-db \
  --admin-user sqladmin \
  --admin-password [password] \
  --storage-key-type SharedAccessKey \
  --storage-key [key] \
  --storage-uri https://storageaccount.blob.core.windows.net/backups/pre-migration-backup.bacpac
```

**Rolling Back Migration:**

```powershell
# Rollback to previous migration
dotnet ef database update PreviousMigrationName --startup-project src\GhcSamplePs.Web

# Or rollback all migrations
dotnet ef database update 0 --startup-project src\GhcSamplePs.Web
```

### Smoke Tests Post-Deployment

**Critical Functionality to Verify:**

1. Application starts successfully
2. Database connection established
3. Health check endpoint returns healthy
4. Player list page loads
5. Create new player succeeds
6. Edit player succeeds
7. Player data persists after app restart
8. Search/filter functionality works
9. No errors in application logs
10. Performance meets benchmarks

**Automated Smoke Test Script:**

```powershell
# Test health endpoint
$health = Invoke-RestMethod -Uri "https://ghcsampleps.azurecontainerapps.io/health"
if ($health.status -ne "Healthy") {
    Write-Error "Health check failed"
    exit 1
}

# Test player list endpoint (requires authentication)
# Add authentication and test API endpoints
```

---

## Success Metrics

### Performance Benchmarks

**Database Operation Performance:**

| Operation | Target | Measurement |
|-----------|--------|-------------|
| Single player query | < 50ms | P95 latency |
| Player list query (100 records) | < 200ms | P95 latency |
| Player list query (1000 records) | < 500ms | P95 latency |
| Create player | < 100ms | P95 latency |
| Update player | < 100ms | P95 latency |
| Delete player | < 100ms | P95 latency |

**Connection Pool Metrics:**

| Metric | Target | Measurement |
|--------|--------|-------------|
| Pool size | 100 connections | Max pool size |
| Connection timeout | < 5s | Average time to get connection |
| Connection reuse | > 90% | Percentage of reused connections |

**Measurement Tools:**
- Application Insights for latency tracking
- SQL Server DMVs for query performance
- EF Core logging for query analysis
- Custom performance logging in repository

### Reliability Metrics

**Database Availability:**
- Target: 99.9% uptime
- Measurement: Azure SQL metrics
- Includes: Successful connections / Total connection attempts

**Transaction Success Rate:**
- Target: > 99.5%
- Measurement: Successful SaveChanges / Total attempts
- Excludes: Business validation failures

**Retry Success Rate:**
- Target: > 95% of retried operations succeed
- Measurement: Successful retries / Total retry attempts

**Data Integrity:**
- Zero data corruption incidents
- Zero constraint violation incidents in production
- All migrations applied successfully

### Migration Success Criteria

**Migration Application:**
- [ ] Migration script generated without errors
- [ ] Script reviewed and approved
- [ ] Applied successfully in development
- [ ] Applied successfully in staging
- [ ] Applied successfully in production
- [ ] All tables created with correct schema
- [ ] All indexes created
- [ ] Seed data inserted correctly (development only)
- [ ] Application connects and operates correctly
- [ ] No data loss during migration

**Post-Migration Validation:**
- [ ] All existing functionality works
- [ ] Performance meets benchmarks
- [ ] No errors in application logs
- [ ] Database backup created and verified
- [ ] Rollback plan documented and tested

### Quality Metrics

**Code Quality:**
- Unit test coverage: ≥ 90% for data layer
- All unit tests passing
- No critical or high severity bugs
- Code review approved by 2+ reviewers
- Performance tests passing

**Documentation Quality:**
- DbContext documented
- Entity configurations documented
- Migration process documented
- Connection string setup documented
- Troubleshooting guide complete

**Security:**
- No connection strings in source control
- Connection strings encrypted in Key Vault
- Database access using least privilege
- SQL injection risk mitigated (via EF parameterization)
- Security review completed

---

## Risks and Mitigations

### Technical Risks

**Risk 1: Migration Failures in Production**
- **Impact**: High (data loss, application downtime)
- **Probability**: Medium (complex migration, large data volume)
- **Mitigation**:
  - Test migrations extensively in development and staging
  - Create database backup before production migration
  - Use idempotent scripts
  - Apply during low-traffic maintenance window
  - Have rollback plan ready and tested
  - Monitor closely during and after migration

**Risk 2: Performance Degradation**
- **Impact**: High (poor user experience, timeouts)
- **Probability**: Medium (inefficient queries, missing indexes)
- **Mitigation**:
  - Design proper indexes from the start
  - Use AsNoTracking for read-only queries
  - Implement query result caching
  - Monitor query performance with Application Insights
  - Load test before production deployment
  - Have query optimization plan ready

**Risk 3: Connection Pool Exhaustion**
- **Impact**: High (application cannot connect to database)
- **Probability**: Low (proper configuration, connection management)
- **Mitigation**:
  - Configure appropriate pool size
  - Ensure DbContext is disposed properly (using Scoped lifetime)
  - Monitor connection pool metrics
  - Implement connection timeout and retry policies
  - Alert on high connection counts
  - Have scaling plan ready

**Risk 4: Concurrency Conflicts**
- **Impact**: Medium (data conflicts, user frustration)
- **Probability**: Low (low concurrent update volume initially)
- **Mitigation**:
  - Implement optimistic concurrency with row version
  - Handle DbUpdateConcurrencyException gracefully
  - Provide clear error messages to users
  - Log concurrency conflicts for monitoring
  - Implement conflict resolution UI (future)

**Risk 5: Data Migration Issues**
- **Impact**: High (data loss or corruption)
- **Probability**: Low (starting with empty database or mock data)
- **Mitigation**:
  - For initial deployment, database is empty (no existing data)
  - If migrating from mock data, export and import carefully
  - Validate data integrity after migration
  - Have data backup and restoration procedures

### Business Risks

**Risk 1: Extended Downtime During Migration**
- **Impact**: Medium (user access interruption)
- **Probability**: Low (simple initial migration)
- **Mitigation**:
  - Schedule migration during low-usage period
  - Communicate maintenance window to users
  - Have quick rollback option available
  - Target migration completion in < 30 minutes

**Risk 2: Increased Azure Costs**
- **Impact**: Medium (budget overrun)
- **Probability**: Low (Basic tier is inexpensive)
- **Mitigation**:
  - Start with Basic tier (~ $5/month)
  - Monitor database usage and costs
  - Scale up only when needed
  - Set budget alerts in Azure
  - Review costs monthly

**Risk 3: Data Privacy and Compliance**
- **Impact**: High (legal/regulatory issues)
- **Probability**: Low (handling youth player data)
- **Mitigation**:
  - Store only necessary data
  - Encrypt data at rest (TDE enabled)
  - Encrypt data in transit (TLS)
  - Implement audit logging
  - Review LGPD compliance requirements
  - Have data retention and deletion policies

### Mitigation Strategies Summary

**Proactive Measures:**
1. Comprehensive testing in development and staging
2. Database backup before production changes
3. Idempotent migration scripts
4. Connection retry policies
5. Performance monitoring from day one
6. Code reviews for database code
7. Security reviews for connection management

**Reactive Measures:**
1. Rollback procedures documented and tested
2. Database restore procedures ready
3. Performance optimization playbook
4. Incident response procedures
5. Escalation contacts identified
6. Backup recovery time tested

**Monitoring and Alerts:**
1. Database performance monitoring
2. Connection pool monitoring
3. Error rate alerts
4. Performance degradation alerts
5. Cost monitoring and alerts
6. Security audit log monitoring

---

## Open Questions

### Technical Decisions

- [ ] **Q1**: Should we use LocalDB or SQL Server Express for local development, or connect to Azure SQL Dev database?
  - **Context**: LocalDB simpler setup, Azure SQL closer to production
  - **Decision needed by**: Week 1, Day 1
  - **Stakeholders**: Development Team, Tech Lead
  - **Recommendation**: Start with LocalDB for ease, optionally use Azure SQL for integration testing

- [ ] **Q2**: Should we implement soft delete (IsDeleted flag) from the start?
  - **Context**: Allows data recovery, adds complexity
  - **Decision needed by**: Week 1, Day 2
  - **Stakeholders**: Product Owner, Tech Lead
  - **Recommendation**: No for Phase 1, add in future phase if needed

- [ ] **Q3**: Should we use row version (timestamp) for optimistic concurrency from the start?
  - **Context**: Prevents lost updates, adds complexity
  - **Decision needed by**: Week 1, Day 3
  - **Stakeholders**: Tech Lead, Developers
  - **Recommendation**: Yes, add to Player entity configuration (low overhead, high value)

- [ ] **Q4**: Should we implement a separate audit log table?
  - **Context**: Complete audit trail, additional complexity
  - **Decision needed by**: Week 1, Day 4
  - **Stakeholders**: Product Owner, Compliance Team
  - **Recommendation**: No for Phase 1, audit fields on entity sufficient initially

- [ ] **Q5**: Should ApplicationUser be persisted to database or remain transient from claims?
  - **Context**: Entra ID is source of truth for users
  - **Decision needed by**: Week 1, Day 2
  - **Stakeholders**: Architect, Tech Lead
  - **Recommendation**: Keep transient for Phase 1, persist only if needed for application-specific user data

### Infrastructure Decisions

- [ ] **Q6**: What Azure SQL pricing tier should we use for production?
  - **Context**: Basic ($5/mo) sufficient initially, Standard may be needed for performance
  - **Decision needed by**: Before production deployment
  - **Stakeholders**: Product Owner, DevOps, Finance
  - **Recommendation**: Start with Basic, monitor performance, scale up if needed

- [ ] **Q7**: Should we enable geo-replication for production database?
  - **Context**: Disaster recovery, additional cost
  - **Decision needed by**: Before production deployment
  - **Stakeholders**: Business Owner, DevOps
  - **Recommendation**: Not needed initially for small user base

- [ ] **Q8**: Should we use SQL Authentication or Managed Identity for production?
  - **Context**: Managed Identity more secure but requires setup
  - **Decision needed by**: Before production deployment
  - **Stakeholders**: Security Team, DevOps
  - **Recommendation**: Use Managed Identity for production (best practice)

### Development Process

- [ ] **Q9**: Should migrations be applied automatically on startup in production?
  - **Context**: Automatic is convenient, manual is safer
  - **Decision needed by**: Week 2, Day 5
  - **Stakeholders**: Tech Lead, DevOps
  - **Recommendation**: No, manual migration application in production

- [ ] **Q10**: Should we keep MockPlayerRepository for testing purposes?
  - **Context**: Useful for fast tests, adds maintenance burden
  - **Decision needed by**: Week 2, Day 3
  - **Stakeholders**: Development Team
  - **Recommendation**: Yes, keep for unit tests, use InMemory provider for repository tests

- [ ] **Q11**: Should we create integration tests with real database?
  - **Context**: Better test coverage, slower execution, more setup
  - **Decision needed by**: Week 2, Day 5
  - **Stakeholders**: QA, Development Team
  - **Recommendation**: Optional for Phase 1, recommended for Phase 2

### Data Questions

- [ ] **Q12**: What should be the database retention policy for audit data?
  - **Context**: Storage costs, compliance requirements
  - **Decision needed by**: Before production deployment
  - **Stakeholders**: Product Owner, Compliance, DevOps
  - **Recommendation**: No deletion for Phase 1, define policy as data grows

- [ ] **Q13**: Should we implement database backups beyond Azure SQL automatic backups?
  - **Context**: Azure SQL provides 7-day retention, additional backups add cost
  - **Decision needed by**: Before production deployment
  - **Stakeholders**: DevOps, Business Owner
  - **Recommendation**: Azure automatic backups sufficient initially

---

## Appendix

### Related Documentation

**Project Documentation:**
- Main README: `c:\playground\ghc-sample-ps\README.md`
- Core README: `c:\playground\ghc-sample-ps\src\GhcSamplePs.Core\README.md`
- Web README: `c:\playground\ghc-sample-ps\src\GhcSamplePs.Web\README.md`
- Player Management Spec: `c:\playground\ghc-sample-ps\docs\specs\ManagePlayers_Feature_Specification.md`

**Architecture Guidelines:**
- C# Guidelines: `c:\playground\ghc-sample-ps\.github\instructions\csharp.instructions.md`
- Blazor Architecture: `c:\playground\ghc-sample-ps\.github\instructions\blazor-architecture.instructions.md`
- DDD Guidelines: `c:\playground\ghc-sample-ps\.github\instructions\dotnet-architecture-good-practices.instructions.md`

**Existing Code References:**
- Current Repository Interface: `src/GhcSamplePs.Core/Repositories/Interfaces/IPlayerRepository.cs`
- Current Mock Repository: `src/GhcSamplePs.Core/Repositories/Implementations/MockPlayerRepository.cs`
- Player Entity: `src/GhcSamplePs.Core/Models/PlayerManagement/Player.cs`

### EF Core Resources

**Official Documentation:**
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [EF Core Configuration](https://learn.microsoft.com/en-us/ef/core/modeling/)
- [EF Core Best Practices](https://learn.microsoft.com/en-us/ef/core/performance/)
- [Azure SQL Database Documentation](https://learn.microsoft.com/en-us/azure/azure-sql/database/)

**Code Samples:**
- [EF Core Getting Started](https://learn.microsoft.com/en-us/ef/core/get-started/)
- [EF Core with Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/data/)

### Azure SQL Resources

**Azure SQL Documentation:**
- [Azure SQL Database Overview](https://learn.microsoft.com/en-us/azure/azure-sql/database/sql-database-paas-overview)
- [Connection Strings](https://learn.microsoft.com/en-us/azure/azure-sql/database/connect-query-content-reference-guide)
- [Managed Identity](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-configure)
- [Security Best Practices](https://learn.microsoft.com/en-us/azure/azure-sql/database/security-best-practice)
- [Performance Tuning](https://learn.microsoft.com/en-us/azure/azure-sql/database/monitor-tune-overview)

### Command Reference

**EF Core CLI Commands:**

```powershell
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Update EF Core tools
dotnet tool update --global dotnet-ef

# Add migration
dotnet ef migrations add MigrationName --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# Remove last migration
dotnet ef migrations remove --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# List migrations
dotnet ef migrations list --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# Update database
dotnet ef database update --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# Update to specific migration
dotnet ef database update MigrationName --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# Rollback all migrations
dotnet ef database update 0 --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# Generate SQL script
dotnet ef migrations script --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core --idempotent --output migration.sql

# Drop database (development only)
dotnet ef database drop --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core --force
```

**Azure CLI Commands:**

```bash
# Create SQL Server
az sql server create --name [server-name] --resource-group [rg-name] --location [location] --admin-user [username] --admin-password [password]

# Create Database
az sql db create --resource-group [rg-name] --server [server-name] --name [db-name] --service-objective Basic

# List databases
az sql db list --resource-group [rg-name] --server [server-name]

# Show database
az sql db show --resource-group [rg-name] --server [server-name] --name [db-name]

# Create firewall rule
az sql server firewall-rule create --resource-group [rg-name] --server [server-name] --name [rule-name] --start-ip-address [ip] --end-ip-address [ip]

# List firewall rules
az sql server firewall-rule list --resource-group [rg-name] --server [server-name]

# Get connection string
az sql db show-connection-string --client ado.net --server [server-name] --name [db-name]
```

### Glossary

**Domain Terms:**
- **Entity**: Domain object with unique identity (Player, ApplicationUser)
- **Aggregate**: Cluster of entities treated as single unit for data changes
- **Repository**: Abstraction for data access operations
- **DbContext**: EF Core context managing database connections and entity tracking

**EF Core Terms:**
- **Migration**: Version-controlled schema change
- **Fluent API**: Code-based entity configuration approach
- **Change Tracker**: Tracks entity state changes (Added, Modified, Deleted)
- **DbSet**: Collection of entities in DbContext
- **AsNoTracking**: Query mode without change tracking (read-only)
- **Seed Data**: Initial data inserted during migration

**Database Terms:**
- **Primary Key**: Unique identifier for table row
- **Foreign Key**: Reference to primary key in another table
- **Index**: Database structure improving query performance
- **Constraint**: Rule enforcing data integrity
- **Transaction**: Unit of work executed atomically
- **Connection Pool**: Reusable database connections
- **Optimistic Concurrency**: Conflict detection using row version
- **TDE**: Transparent Data Encryption

**Azure Terms:**
- **DTU**: Database Transaction Unit (performance measure)
- **Managed Identity**: Azure AD identity for Azure resources
- **Key Vault**: Secure storage for secrets and keys
- **Firewall Rule**: Network access control for SQL Server
- **Geo-Replication**: Database replication across regions

### Connection String Examples

**LocalDB (Development):**
```
Server=(localdb)\mssqllocaldb;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true
```

**Azure SQL with SQL Authentication:**
```
Server=tcp:sql-ghcsampleps.database.windows.net,1433;Initial Catalog=ghcsampleps-db;Persist Security Info=False;User ID=sqladmin;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**Azure SQL with Managed Identity:**
```
Server=tcp:sql-ghcsampleps.database.windows.net,1433;Initial Catalog=ghcsampleps-db;Authentication=Active Directory Managed Identity;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**SQL Server Express:**
```
Server=localhost\SQLEXPRESS;Database=GhcSamplePs;Trusted_Connection=True;MultipleActiveResultSets=true
```

### Troubleshooting Guide

**Issue: Migration fails with "table already exists"**
- Cause: Migration applied previously or table created manually
- Solution: Drop database and reapply migrations, or remove conflicting migration

**Issue: Cannot connect to database**
- Cause: Connection string incorrect, firewall blocking, credentials invalid
- Solution: Verify connection string, check firewall rules, test credentials

**Issue: "The instance of entity type cannot be tracked"**
- Cause: Adding entity that already has ID or is already tracked
- Solution: Use Update instead of Add, or attach entity with appropriate state

**Issue: Slow query performance**
- Cause: Missing indexes, N+1 query problem, not using AsNoTracking
- Solution: Add indexes, use Include for related data, use AsNoTracking for reads

**Issue: Connection pool exhausted**
- Cause: DbContext not disposed, too many concurrent operations
- Solution: Ensure Scoped lifetime for DbContext, increase pool size, investigate leaks

**Issue: Concurrency exception**
- Cause: Two users modifying same record simultaneously
- Solution: Handle DbUpdateConcurrencyException, refresh and retry, or show conflict to user

### Change Log

| Date | Version | Change Description | Author |
|------|---------|-------------------|--------|
| 2025-11-28 | 1.0 | Initial specification created | Software Architect Agent |

---

## Document Control

**Document Owner**: Development Team  
**Review Frequency**: Weekly during implementation  
**Next Review Date**: Start of Week 2  
**Approval Status**: Draft (pending stakeholder review)  

**Distribution List:**
- Development Team
- Database Administrator
- DevOps Team
- Tech Lead
- Product Owner

---

**End of Specification**
