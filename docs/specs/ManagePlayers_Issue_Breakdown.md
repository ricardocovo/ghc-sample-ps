# Player Management - GitHub Issue Breakdown

This document contains the complete issue breakdown for the Player Management feature based on the specification at `docs/specs/ManagePlayers_Feature_Specification.md`.

## Summary

- **Total Features**: 4 parent Feature issues
- **Total Sub-Issues**: 16 implementation tasks
- **Estimated Total Time**: 2-3 weeks (~80-100 hours)

### Feature Distribution
- Foundation/Infrastructure: 1 Feature (Core data layer and common components)
- Feature Implementation: 1 Feature (Business logic and services)  
- UI/UX: 1 Feature (All Blazor pages and components)
- Integration/Quality: 1 Feature (Testing, DI setup, documentation)

### Implementation Order

1. **Feature 1**: Core Data Layer (Foundation - no dependencies)
2. **Feature 2**: Business Logic Services (depends on Feature 1)
3. **Feature 3**: Blazor UI Pages (depends on Feature 2)
4. **Feature 4**: Testing and Integration (depends on Features 1, 2, and 3)

---

## Feature 1: Player Management - Core Data Layer

### Parent Issue

**Title**: `[Feature] Player Management - Core Data Layer`

**Labels**: `Feature`, `enhancement`, `data-layer`, `core`

**Body**:

```markdown
## Feature Overview

Implement the foundational data layer components for the Player Management system. This Feature establishes the core data structures, repository patterns, and validation logic that all other features will build upon.

## Specification Reference

`docs/specs/ManagePlayers_Feature_Specification.md` - Sections: Technical Design > Implementation Details > Data Layer

## Feature Scope

This Feature includes:
- **Domain Models**: Player entity with properties, computed properties (Age), and behavior methods
- **Data Transfer Objects**: PlayerDto, CreatePlayerDto, UpdatePlayerDto for layer communication
- **Common Types**: ServiceResult<T> wrapper and ValidationResult for standardized responses
- **Validation Logic**: PlayerValidator with comprehensive field validation rules
- **Repository Pattern**: IPlayerRepository interface and MockPlayerRepository implementation
- **Seed Data**: 10 diverse sample players for testing

## Feature Success Criteria

- [ ] All sub-issues completed
- [ ] Player entity correctly calculates age from DateOfBirth
- [ ] All validation rules enforced (name, DOB, gender, URL)
- [ ] Mock repository properly stores and retrieves players
- [ ] Thread-safe operations in mock repository
- [ ] All code follows project standards
- [ ] Ready for business logic layer integration

## Sub-Issues

This Feature is broken down into the following implementation tasks:

### Phase 1: Domain Models
- [ ] #[number] - [Core/Models] Implement Player entity and DTOs

### Phase 2: Common Infrastructure
- [ ] #[number] - [Core/Common] Implement ServiceResult and ValidationResult types

### Phase 3: Validation
- [ ] #[number] - [Core/Validation] Implement PlayerValidator

### Phase 4: Data Access
- [ ] #[number] - [Core/Repository] Implement IPlayerRepository and MockPlayerRepository

## Implementation Timeline

Estimated: 2 days (16 hours)
- Phase 1: 4 hours
- Phase 2: 3 hours
- Phase 3: 3 hours
- Phase 4: 6 hours

## Dependencies

- **Requires**: None (this is the foundation Feature)
- **Enables**: Feature 2 (Business Logic Services), Feature 4 (Testing)

## Architecture Impact

**Projects Affected:**
- `GhcSamplePs.Core` - New folders and files for player management

**New Folders:**
- `src/GhcSamplePs.Core/Models/PlayerManagement/`
- `src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/`
- `src/GhcSamplePs.Core/Repositories/Interfaces/`
- `src/GhcSamplePs.Core/Repositories/Implementations/`
- `src/GhcSamplePs.Core/Validation/`
- `src/GhcSamplePs.Core/Common/`

**Patterns to Follow:**
- See `src/GhcSamplePs.Core/Models/Identity/ApplicationUser.cs` for entity pattern
- Follow `.github/instructions/csharp.instructions.md` for coding standards
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md` for DDD patterns

## Acceptance Criteria

- [ ] Player entity implements all properties from specification
- [ ] Age calculation is accurate (handles edge cases like birthday not yet passed)
- [ ] All DTOs properly structured for their use cases
- [ ] ServiceResult supports success, failure, and validation failure scenarios
- [ ] ValidationResult provides field-specific error messages
- [ ] PlayerValidator enforces all business rules
- [ ] MockPlayerRepository implements all CRUD operations
- [ ] Seed data includes 10 diverse players (ages 6-16, mixed genders)
- [ ] Code is UI-agnostic (no Blazor dependencies in Core)
```

---

### Sub-Issue 1.1: Player Entity and DTOs

**Title**: `[Core/Models] Implement Player entity and DTOs`

**Labels**: `enhancement`, `data-layer`, `core`

**Body**:

```markdown
## Overview

Create the Player domain entity model with all properties, computed properties (Age calculation), and behavior methods. Also create the Data Transfer Objects for player operations.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Core Data Layer

## Acceptance Criteria

- [ ] Player entity created with all required properties
- [ ] Age property correctly calculated from DateOfBirth
- [ ] Validate() method checks all business rules
- [ ] UpdateLastModified() method updates audit fields
- [ ] PlayerDto created for display purposes
- [ ] CreatePlayerDto created for creation operations
- [ ] UpdatePlayerDto created for update operations
- [ ] All entities have XML documentation
- [ ] Unit tests cover age calculation edge cases

## Technical Details

### Scope

**Files to create:**
- `src/GhcSamplePs.Core/Models/PlayerManagement/Player.cs`
- `src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/PlayerDto.cs`
- `src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/CreatePlayerDto.cs`
- `src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/UpdatePlayerDto.cs`

### Implementation Guidance

**What to build:**

**Player Entity Properties:**
- Id: integer, auto-generated unique identifier
- Name: string, max 200 characters, required
- DateOfBirth: DateTime, required, must be past date
- Gender: string, max 50 characters, nullable (Male, Female, Non-binary, Prefer not to say)
- PhotoUrl: string, max 500 characters, nullable, valid URL format
- CreatedAt: DateTime, UTC timestamp
- CreatedBy: string, required
- UpdatedAt: DateTime, nullable, UTC
- UpdatedBy: string, nullable

**Computed Properties:**
- Age: int, read-only, calculated from DateOfBirth

**Behavior Methods:**
- CalculateAge(): Returns current age based on DateOfBirth and today's date
- Validate(): Returns boolean indicating if entity is valid
- UpdateLastModified(userId): Updates UpdatedAt and UpdatedBy fields

**Patterns to follow:**
- See `src/GhcSamplePs.Core/Models/Identity/ApplicationUser.cs` for entity pattern
- Use `required` keyword for required properties
- Use `init` accessors where appropriate
- Use file-scoped namespaces

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md`

**Key requirements:**
1. Age calculation must handle birthday not yet passed this year
2. Age calculation must handle leap year birthdays (Feb 29)
3. DateOfBirth must be validated as past date
4. All string properties must be trimmed

### Dependencies

- No dependencies

## Testing Requirements

- [ ] Unit tests for age calculation (birthday passed, not passed, same day)
- [ ] Unit tests for Validate() method
- [ ] Unit tests for UpdateLastModified() method
- [ ] Edge case tests (leap year, very old dates, etc.)

## Documentation Requirements

- [ ] XML comments on all public properties and methods
- [ ] Summary documentation on class

## Definition of Done

- [ ] All properties implemented per specification
- [ ] Age calculation accurate for all scenarios
- [ ] Validation logic working correctly
- [ ] DTOs properly structured
- [ ] XML documentation complete
- [ ] Tests added to test project

## Estimated Effort

Medium: 4 hours
```

---

### Sub-Issue 1.2: ServiceResult and ValidationResult

**Title**: `[Core/Common] Implement ServiceResult and ValidationResult types`

**Labels**: `enhancement`, `core`, `infrastructure`

**Body**:

```markdown
## Overview

Create standardized result wrapper types for service operations, providing consistent success/failure responses with validation error support.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Core Data Layer

## Acceptance Criteria

- [ ] ServiceResult<T> wrapper class created
- [ ] ServiceResult supports success scenarios with data
- [ ] ServiceResult supports failure scenarios with error messages
- [ ] ServiceResult supports validation failure with field-specific errors
- [ ] ValidationResult created with IsValid and Errors properties
- [ ] Factory methods for common scenarios (Ok, Fail, ValidationFailed)
- [ ] XML documentation complete

## Technical Details

### Scope

**Files to create:**
- `src/GhcSamplePs.Core/Common/ServiceResult.cs`
- `src/GhcSamplePs.Core/Common/ValidationResult.cs`

### Implementation Guidance

**What to build:**

**ServiceResult<T> Properties:**
- Success: boolean indicating operation success
- Data: Generic type T containing result data (nullable)
- ErrorMessages: List<string> for general error messages
- ValidationErrors: Dictionary<string, List<string>> for field-specific errors
- StatusCode: Optional int for future API use

**Factory Methods:**
- ServiceResult.Ok<T>(T data): Create success result with data
- ServiceResult.Fail<T>(string message): Create failure result with error message
- ServiceResult.Fail<T>(IEnumerable<string> messages): Create failure with multiple errors
- ServiceResult.ValidationFailed<T>(Dictionary<string, List<string>> errors): Create validation failure

**ValidationResult Properties:**
- IsValid: boolean indicating validation passed
- Errors: Dictionary<string, List<string>> (field name → error messages)
- ErrorMessages: List<string> (all errors flattened)

**Factory Methods:**
- ValidationResult.Success(): Create successful validation
- ValidationResult.Failure(string field, string message): Add single error
- ValidationResult.Failure(Dictionary<string, List<string>> errors): Add multiple errors

**Patterns to follow:**
- Use records for immutability where appropriate
- Consider using generic constraints
- Use collection expressions for empty collections

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md`

### Dependencies

- No dependencies

## Testing Requirements

- [ ] Unit tests for all factory methods
- [ ] Unit tests for property accessors
- [ ] Tests for edge cases (null data, empty errors)

## Documentation Requirements

- [ ] XML comments on all public members
- [ ] Example usage in class documentation

## Definition of Done

- [ ] ServiceResult<T> fully implemented
- [ ] ValidationResult fully implemented
- [ ] All factory methods working
- [ ] XML documentation complete
- [ ] Tests passing

## Estimated Effort

Small: 3 hours
```

---

### Sub-Issue 1.3: PlayerValidator

**Title**: `[Core/Validation] Implement PlayerValidator`

**Labels**: `enhancement`, `core`, `validation`

**Body**:

```markdown
## Overview

Create the validation logic for player data, implementing all business rules defined in the specification.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Core Data Layer

## Acceptance Criteria

- [ ] PlayerValidator class created
- [ ] Name validation: required, 1-200 characters, not whitespace
- [ ] DateOfBirth validation: required, past date, not more than 100 years ago
- [ ] Gender validation: if provided, must be valid option
- [ ] PhotoUrl validation: if provided, max 500 chars, valid HTTP/HTTPS URL
- [ ] Returns ValidationResult with field-specific errors
- [ ] Supports validating CreatePlayerDto and UpdatePlayerDto

## Technical Details

### Scope

**Files to create:**
- `src/GhcSamplePs.Core/Validation/PlayerValidator.cs`

### Implementation Guidance

**What to build:**

**Validation Rules:**

1. **Name Validation:**
   - Required (not null or empty)
   - Not whitespace only
   - Length 1-200 characters after trimming
   - Error message: "Name is required" or "Name must not exceed 200 characters"

2. **DateOfBirth Validation:**
   - Required
   - Must be in the past (not today or future)
   - Must not be more than 100 years ago
   - Error messages: "Date of birth is required", "Date of birth must be in the past", "Date of birth cannot be more than 100 years ago"

3. **Gender Validation (optional):**
   - If provided, must be one of: "Male", "Female", "Non-binary", "Prefer not to say"
   - Case-insensitive comparison
   - Error message: "Gender must be Male, Female, Non-binary, or Prefer not to say"

4. **PhotoUrl Validation (optional):**
   - If provided, max 500 characters
   - Must be valid HTTP or HTTPS URL
   - Error messages: "Photo URL must not exceed 500 characters", "Photo URL must be a valid HTTP or HTTPS URL"

**Methods:**
- ValidateCreatePlayer(CreatePlayerDto): Returns ValidationResult
- ValidateUpdatePlayer(UpdatePlayerDto): Returns ValidationResult
- ValidatePlayer(Player): Returns ValidationResult (for entity validation)

**Patterns to follow:**
- Use static methods or instance methods (consistent with project patterns)
- Return ValidationResult with all errors found (don't stop at first error)
- Use Uri.TryCreate for URL validation

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md`

### Dependencies

- Depends on: Sub-Issues 1.1 (DTOs) and 1.2 (ValidationResult)

## Testing Requirements

- [ ] Test valid player passes all validation
- [ ] Test empty name returns error
- [ ] Test name too long returns error
- [ ] Test future DOB returns error
- [ ] Test DOB too old returns error
- [ ] Test invalid gender returns error
- [ ] Test invalid URL returns error
- [ ] Test valid optional fields (null) pass
- [ ] Test multiple errors returned together

## Documentation Requirements

- [ ] XML comments on class and methods
- [ ] Document valid gender options

## Definition of Done

- [ ] All validation rules implemented
- [ ] ValidationResult returned with field-specific errors
- [ ] Comprehensive tests passing
- [ ] XML documentation complete

## Estimated Effort

Small: 3 hours
```

---

### Sub-Issue 1.4: Repository Pattern

**Title**: `[Core/Repository] Implement IPlayerRepository and MockPlayerRepository`

**Labels**: `enhancement`, `core`, `data-layer`, `repository`

**Body**:

```markdown
## Overview

Create the repository interface and mock implementation for player data access. The mock implementation uses in-memory storage and includes seed data.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Core Data Layer

## Acceptance Criteria

- [ ] IPlayerRepository interface created with all CRUD methods
- [ ] MockPlayerRepository implements IPlayerRepository
- [ ] In-memory storage using ConcurrentDictionary
- [ ] Auto-incrementing ID generation
- [ ] Thread-safe operations
- [ ] Pre-seeded with 10 sample players
- [ ] All operations are async

## Technical Details

### Scope

**Files to create:**
- `src/GhcSamplePs.Core/Repositories/Interfaces/IPlayerRepository.cs`
- `src/GhcSamplePs.Core/Repositories/Implementations/MockPlayerRepository.cs`

### Implementation Guidance

**What to build:**

**IPlayerRepository Methods:**
- GetAllAsync(CancellationToken): Task<IEnumerable<Player>>
- GetByIdAsync(int id, CancellationToken): Task<Player?>
- AddAsync(Player player, CancellationToken): Task<Player>
- UpdateAsync(Player player, CancellationToken): Task<Player>
- DeleteAsync(int id, CancellationToken): Task<bool>
- ExistsAsync(int id, CancellationToken): Task<bool>

**MockPlayerRepository Implementation:**
- Use ConcurrentDictionary<int, Player> for storage
- Use Interlocked.Increment for thread-safe ID generation
- Initialize with seed data in constructor
- Return deep copies from read operations (prevent external modification)
- Simulate async with Task.FromResult or small Task.Delay

**Seed Data (10 players):**

| Name | Date of Birth | Gender | PhotoUrl |
|------|---------------|--------|----------|
| Emma Rodriguez | 2014-03-15 | Female | null |
| Liam Johnson | 2015-07-22 | Male | null |
| Olivia Martinez | 2013-11-08 | Female | /photos/olivia.jpg |
| Noah Williams | 2016-01-30 | Male | null |
| Ava Brown | 2014-09-12 | Female | /photos/ava.jpg |
| Ethan Davis | 2015-04-05 | Male | null |
| Sophia Garcia | 2013-06-18 | Non-binary | null |
| Mason Miller | 2016-12-25 | Male | /photos/mason.jpg |
| Isabella Wilson | 2014-02-14 | Female | null |
| Lucas Anderson | 2015-10-09 | Prefer not to say | null |

**Patterns to follow:**
- See existing repository patterns in project if any
- Use file-scoped namespaces
- Return Task-wrapped results for async interface

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md`

### Dependencies

- Depends on: Sub-Issue 1.1 (Player entity)

## Testing Requirements

- [ ] Test GetAllAsync returns seeded players
- [ ] Test GetByIdAsync with valid ID returns player
- [ ] Test GetByIdAsync with invalid ID returns null
- [ ] Test AddAsync creates player with new ID
- [ ] Test AddAsync increments ID correctly
- [ ] Test UpdateAsync modifies existing player
- [ ] Test UpdateAsync with non-existent ID throws
- [ ] Test DeleteAsync removes player
- [ ] Test ExistsAsync returns correct boolean
- [ ] Test thread safety (parallel operations)

## Documentation Requirements

- [ ] XML comments on interface methods
- [ ] Document thread-safety guarantees

## Definition of Done

- [ ] Interface defines all required operations
- [ ] Mock implementation fully functional
- [ ] Seed data populated
- [ ] Thread-safe operations verified
- [ ] Tests passing

## Estimated Effort

Medium: 6 hours
```

---

## Feature 2: Player Management - Business Logic Services

### Parent Issue

**Title**: `[Feature] Player Management - Business Logic Services`

**Labels**: `Feature`, `enhancement`, `business-logic`, `core`

**Body**:

```markdown
## Feature Overview

Implement the player service with complete business logic, including CRUD operations, validation orchestration, age calculation, audit field management, and proper error handling with logging.

## Specification Reference

`docs/specs/ManagePlayers_Feature_Specification.md` - Sections: Technical Design > Implementation Details > Business Logic Layer

## Feature Scope

This Feature includes:
- **Service Interface**: IPlayerService with all player management operations
- **Service Implementation**: PlayerService with business logic
- **Custom Exceptions**: PlayerNotFoundException, PlayerValidationException
- **Logging Integration**: Structured logging for all operations
- **DTO Mapping**: Convert between entities and DTOs

## Feature Success Criteria

- [ ] All sub-issues completed
- [ ] Service orchestrates validation correctly
- [ ] Audit fields populated automatically
- [ ] Proper error handling with custom exceptions
- [ ] Logging captures all operations
- [ ] All code follows project standards

## Sub-Issues

### Phase 1: Interface Definition
- [ ] #[number] - [Core/Services] Implement IPlayerService interface

### Phase 2: Core Implementation
- [ ] #[number] - [Core/Services] Implement PlayerService with CRUD operations

### Phase 3: Exception Handling
- [ ] #[number] - [Core/Exceptions] Implement custom exceptions

### Phase 4: Logging
- [ ] #[number] - [Core/Services] Add logging to PlayerService

## Implementation Timeline

Estimated: 1.5 days (12 hours)
- Phase 1: 2 hours
- Phase 2: 6 hours
- Phase 3: 2 hours
- Phase 4: 2 hours

## Dependencies

- **Requires**: Feature 1 (Core Data Layer)
- **Enables**: Feature 3 (UI), Feature 4 (Testing)

## Acceptance Criteria

- [ ] IPlayerService defines all required operations
- [ ] PlayerService implements all operations correctly
- [ ] Validation runs before create/update operations
- [ ] CreatedBy/UpdatedBy populated from parameter
- [ ] CreatedAt/UpdatedAt populated with UTC timestamps
- [ ] Age calculated correctly in returned DTOs
- [ ] Custom exceptions thrown for error scenarios
- [ ] All operations logged appropriately
```

---

### Sub-Issue 2.1: IPlayerService Interface

**Title**: `[Core/Services] Implement IPlayerService interface`

**Labels**: `enhancement`, `core`, `business-logic`

**Body**:

```markdown
## Overview

Define the service interface for player management operations with proper method signatures, XML documentation, and ServiceResult return types.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Business Logic Services

## Acceptance Criteria

- [ ] IPlayerService interface created
- [ ] All CRUD methods defined
- [ ] Async methods with CancellationToken support
- [ ] Returns ServiceResult<T> for all operations
- [ ] XML documentation complete

## Technical Details

### Scope

**File to create:**
- `src/GhcSamplePs.Core/Services/Interfaces/IPlayerService.cs`

### Implementation Guidance

**What to build:**

**Methods:**
- GetAllPlayersAsync(CancellationToken): Task<ServiceResult<IEnumerable<PlayerDto>>>
- GetPlayerByIdAsync(int id, CancellationToken): Task<ServiceResult<PlayerDto>>
- CreatePlayerAsync(CreatePlayerDto dto, string currentUserId, CancellationToken): Task<ServiceResult<PlayerDto>>
- UpdatePlayerAsync(int id, UpdatePlayerDto dto, string currentUserId, CancellationToken): Task<ServiceResult<PlayerDto>>
- DeletePlayerAsync(int id, CancellationToken): Task<ServiceResult<bool>> (for future use)
- ValidatePlayerAsync(CreatePlayerDto dto, CancellationToken): Task<ValidationResult>
- ValidatePlayerAsync(UpdatePlayerDto dto, CancellationToken): Task<ValidationResult>

**Patterns to follow:**
- See `src/GhcSamplePs.Core/Services/Interfaces/IAuthenticationService.cs` for interface pattern
- Use consistent XML documentation format
- Include param and returns documentation

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md`

### Dependencies

- Depends on: Feature 1 (DTOs, ServiceResult)

## Definition of Done

- [ ] Interface defines all operations
- [ ] Consistent signatures
- [ ] XML documentation complete
- [ ] Follows project patterns

## Estimated Effort

Small: 2 hours
```

---

### Sub-Issue 2.2: PlayerService Implementation

**Title**: `[Core/Services] Implement PlayerService with CRUD operations`

**Labels**: `enhancement`, `core`, `business-logic`

**Body**:

```markdown
## Overview

Implement the PlayerService with all business logic for player management, including CRUD operations, validation orchestration, DTO mapping, and audit field management.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Business Logic Services

## Acceptance Criteria

- [ ] PlayerService implements IPlayerService
- [ ] Injects IPlayerRepository and PlayerValidator
- [ ] GetAllPlayersAsync returns all players with age calculated
- [ ] GetPlayerByIdAsync returns player or failure result
- [ ] CreatePlayerAsync validates, creates, and sets audit fields
- [ ] UpdatePlayerAsync validates, updates, and sets audit fields
- [ ] DeletePlayerAsync marks for deletion (soft delete for future)
- [ ] DTO to Entity and Entity to DTO mapping
- [ ] All dates stored as UTC

## Technical Details

### Scope

**File to create:**
- `src/GhcSamplePs.Core/Services/Implementations/PlayerService.cs`

### Implementation Guidance

**What to build:**

**Constructor Dependencies:**
- IPlayerRepository repository
- PlayerValidator validator
- ILogger<PlayerService> logger (added in sub-issue 2.4)

**Business Logic:**

1. **GetAllPlayersAsync:**
   - Call repository.GetAllAsync()
   - Map each Player entity to PlayerDto (including calculated Age)
   - Return ServiceResult.Ok with list

2. **GetPlayerByIdAsync:**
   - Call repository.GetByIdAsync()
   - If null, return ServiceResult.Fail("Player not found")
   - Map to PlayerDto and return ServiceResult.Ok

3. **CreatePlayerAsync:**
   - Validate CreatePlayerDto
   - If invalid, return ServiceResult.ValidationFailed
   - Create Player entity from DTO
   - Set CreatedAt = DateTime.UtcNow
   - Set CreatedBy = currentUserId
   - Call repository.AddAsync()
   - Map result to PlayerDto
   - Return ServiceResult.Ok

4. **UpdatePlayerAsync:**
   - Check if player exists
   - If not, return ServiceResult.Fail
   - Validate UpdatePlayerDto
   - If invalid, return ServiceResult.ValidationFailed
   - Update entity properties from DTO
   - Set UpdatedAt = DateTime.UtcNow
   - Set UpdatedBy = currentUserId
   - Call repository.UpdateAsync()
   - Map result to PlayerDto
   - Return ServiceResult.Ok

**Mapping Logic:**
- PlayerDto should include calculated Age property
- Mapping should handle nullable properties correctly

**Patterns to follow:**
- Use pattern matching for null checks
- Use expression-bodied members where appropriate

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`
- Follow `.github/instructions/dotnet-architecture-good-practices.instructions.md`

### Dependencies

- Depends on: Sub-Issue 2.1 (interface), Feature 1 (all components)

## Testing Requirements

Tests will be implemented in Feature 4

## Definition of Done

- [ ] All methods implemented correctly
- [ ] Validation integrated
- [ ] Audit fields managed
- [ ] DTO mapping correct
- [ ] Ready for testing

## Estimated Effort

Medium: 6 hours
```

---

### Sub-Issue 2.3: Custom Exceptions

**Title**: `[Core/Exceptions] Implement custom exceptions`

**Labels**: `enhancement`, `core`, `exceptions`

**Body**:

```markdown
## Overview

Create custom exception types for player management error scenarios.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Business Logic Services

## Acceptance Criteria

- [ ] PlayerNotFoundException created
- [ ] PlayerValidationException created
- [ ] RepositoryException created (generic)
- [ ] All inherit from appropriate base exceptions
- [ ] XML documentation complete

## Technical Details

### Scope

**Files to create:**
- `src/GhcSamplePs.Core/Exceptions/PlayerNotFoundException.cs`
- `src/GhcSamplePs.Core/Exceptions/PlayerValidationException.cs`
- `src/GhcSamplePs.Core/Exceptions/RepositoryException.cs`

### Implementation Guidance

**What to build:**

**PlayerNotFoundException:**
- Inherits from Exception
- Constructor: PlayerNotFoundException(int playerId)
- Message: "Player with ID {playerId} was not found."
- Property: int PlayerId

**PlayerValidationException:**
- Inherits from Exception
- Constructor: PlayerValidationException(ValidationResult validationResult)
- Message: "Player validation failed."
- Property: ValidationResult ValidationResult

**RepositoryException:**
- Inherits from Exception
- Constructor: RepositoryException(string message, Exception innerException)
- Standard exception constructors

**Patterns to follow:**
- See existing exceptions in `src/GhcSamplePs.Core/Exceptions/` if any
- Follow standard exception patterns

### Dependencies

- Depends on: Sub-Issue 1.2 (ValidationResult)

## Definition of Done

- [ ] All exceptions implemented
- [ ] XML documentation complete
- [ ] Standard exception patterns followed

## Estimated Effort

Small: 2 hours
```

---

### Sub-Issue 2.4: PlayerService Logging

**Title**: `[Core/Services] Add logging to PlayerService`

**Labels**: `enhancement`, `core`, `logging`

**Body**:

```markdown
## Overview

Add structured logging to PlayerService for all operations with appropriate log levels.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Business Logic Services

## Acceptance Criteria

- [ ] ILogger<PlayerService> injected
- [ ] Information level for successful operations
- [ ] Warning level for not found scenarios
- [ ] Warning level for validation failures
- [ ] Error level for exceptions
- [ ] Structured logging with named parameters
- [ ] No sensitive data logged (mask PII)

## Technical Details

### Scope

**File to modify:**
- `src/GhcSamplePs.Core/Services/Implementations/PlayerService.cs`

### Implementation Guidance

**What to build:**

**Log Messages:**

1. **GetAllPlayersAsync:**
   - Information: "Retrieved {Count} players"

2. **GetPlayerByIdAsync:**
   - Information (success): "Retrieved player with ID {PlayerId}"
   - Warning (not found): "Player with ID {PlayerId} not found"

3. **CreatePlayerAsync:**
   - Information (success): "Created player with ID {PlayerId}"
   - Warning (validation): "Validation failed for player creation: {ErrorCount} errors"

4. **UpdatePlayerAsync:**
   - Information (success): "Updated player with ID {PlayerId}"
   - Warning (not found): "Cannot update - Player with ID {PlayerId} not found"
   - Warning (validation): "Validation failed for player update: {ErrorCount} errors"

**Patterns to follow:**
- Use structured logging with named placeholders
- Use LoggerMessage.Define for high-performance scenarios (optional)
- Never log sensitive data (DOB is PII - reference by ID only)

**Architecture guidelines:**
- Follow `.github/instructions/csharp.instructions.md`

### Dependencies

- Depends on: Sub-Issue 2.2 (PlayerService implementation)

## Definition of Done

- [ ] Logger injected
- [ ] All operations logged
- [ ] Appropriate log levels used
- [ ] No PII in logs

## Estimated Effort

Small: 2 hours
```

---

## Feature 3: Player Management - Blazor UI Pages

### Parent Issue

**Title**: `[Feature] Player Management - Blazor UI Pages`

**Labels**: `Feature`, `enhancement`, `ui`, `blazor`

**Body**:

```markdown
## Feature Overview

Implement all three Blazor pages for player management: the list page, create form, and edit form with tabs. All pages follow MudBlazor design patterns and are responsive.

## Specification Reference

`docs/specs/ManagePlayers_Feature_Specification.md` - Sections: Technical Design > UI/Presentation Layer

## Feature Scope

This Feature includes:
- **ManagePlayers Page**: List all players with search, sort, and navigation
- **CreatePlayer Page**: Form to create new players with validation
- **EditPlayer Page**: Tabbed form to edit existing players

## Feature Success Criteria

- [ ] All three pages implemented and functional
- [ ] MudBlazor components used consistently
- [ ] Client and server-side validation working
- [ ] Navigation between pages working
- [ ] Loading states and error handling
- [ ] Responsive design for mobile/tablet/desktop

## Sub-Issues

### Phase 1: List Page
- [ ] #[number] - [Web/UI] Implement ManagePlayers list page

### Phase 2: Create Page
- [ ] #[number] - [Web/UI] Implement CreatePlayer form page

### Phase 3: Edit Page
- [ ] #[number] - [Web/UI] Implement EditPlayer page with tabs

## Implementation Timeline

Estimated: 2 days (16 hours)
- Phase 1: 5 hours
- Phase 2: 5 hours
- Phase 3: 6 hours

## Dependencies

- **Requires**: Feature 2 (Business Logic Services)
- **Enables**: Feature 4 (Integration)

## Acceptance Criteria

- [ ] ManagePlayers shows all players with name and age
- [ ] Search/filter works correctly
- [ ] Table is sortable
- [ ] CreatePlayer form validates all fields
- [ ] EditPlayer loads and saves player data
- [ ] EditPlayer shows placeholder tabs for Teams/Stats
- [ ] Navigation flows work (create → edit, edit → list)
- [ ] Error states handled gracefully
- [ ] Empty state shown when no players
```

---

### Sub-Issue 3.1: ManagePlayers List Page

**Title**: `[Web/UI] Implement ManagePlayers list page`

**Labels**: `enhancement`, `ui`, `blazor`

**Body**:

```markdown
## Overview

Create the ManagePlayers page showing all players in a searchable, sortable table with navigation to create/edit pages.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Blazor UI Pages

## Acceptance Criteria

- [ ] Page route: /players
- [ ] Page title: "Manage Players"
- [ ] MudTable displays all players
- [ ] Columns: Name (sortable), Age (sortable), Actions
- [ ] Search box filters by name
- [ ] "Add Player" button navigates to /players/create
- [ ] Clicking row or Edit icon navigates to /players/edit/{id}
- [ ] Loading indicator while fetching data
- [ ] Empty state message when no players
- [ ] Error display if loading fails

## Technical Details

### Scope

**File to create:**
- `src/GhcSamplePs.Web/Components/Pages/PlayerManagement/ManagePlayers.razor`

### Implementation Guidance

**What to build:**

**Page Structure:**
- MudContainer (MaxWidth.Large)
- MudPaper with padding
- MudText for title
- MudButton for "Add Player" (top-right)
- MudTextField for search
- MudTable for player list
- MudProgressCircular for loading
- MudAlert for errors

**MudTable Configuration:**
- Items bound to player list
- ServerData or Items property based on data size
- Sortable columns (Name, Age)
- Search filtering (client-side for MVP)
- Dense layout for compact display

**User Interactions:**
- Click "Add Player" → NavigationManager.NavigateTo("/players/create")
- Click row → NavigationManager.NavigateTo($"/players/edit/{player.Id}")
- Type in search → Filter displayed items
- Click column header → Sort by that column

**Code-behind Logic:**
- @inject IPlayerService PlayerService
- @inject NavigationManager NavigationManager
- OnInitializedAsync: Load all players
- Handle loading state
- Handle error state

**Patterns to follow:**
- See `src/GhcSamplePs.Web/Components/Pages/Home.razor` for page pattern
- Use @code block for component logic
- Follow MudBlazor component patterns

**Architecture guidelines:**
- Follow `.github/instructions/blazor-architecture.instructions.md`

### Dependencies

- Depends on: Feature 2 (IPlayerService)

## Definition of Done

- [ ] Page displays player list
- [ ] Search/filter working
- [ ] Sorting working
- [ ] Navigation working
- [ ] Loading/error states working
- [ ] Responsive design

## Estimated Effort

Medium: 5 hours
```

---

### Sub-Issue 3.2: CreatePlayer Form Page

**Title**: `[Web/UI] Implement CreatePlayer form page`

**Labels**: `enhancement`, `ui`, `blazor`

**Body**:

```markdown
## Overview

Create the form page for adding new players with client and server-side validation.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Blazor UI Pages

## Acceptance Criteria

- [ ] Page route: /players/create
- [ ] Page title: "Create New Player"
- [ ] Form with all required fields
- [ ] Name field: MudTextField (required)
- [ ] Date of Birth field: MudDatePicker (required, max today)
- [ ] Gender field: MudSelect (optional, predefined options)
- [ ] Photo URL field: MudTextField (optional)
- [ ] Save button validates and submits
- [ ] Cancel button navigates to /players
- [ ] Client-side validation on blur
- [ ] Server-side validation on submit
- [ ] Success redirects to /players/edit/{newId}
- [ ] Errors displayed inline

## Technical Details

### Scope

**File to create:**
- `src/GhcSamplePs.Web/Components/Pages/PlayerManagement/CreatePlayer.razor`

### Implementation Guidance

**What to build:**

**Page Structure:**
- MudContainer (MaxWidth.Medium)
- MudPaper with padding
- MudText for title
- MudForm with validation
- MudTextField for Name
- MudDatePicker for DateOfBirth
- MudSelect for Gender with items:
  - "" (empty/null)
  - "Male"
  - "Female"
  - "Non-binary"
  - "Prefer not to say"
- MudTextField for PhotoUrl
- MudButton "Save" (primary)
- MudButton "Cancel" (secondary)
- MudAlert for errors
- MudProgressCircular for saving state

**Validation:**
- Use MudForm validation
- Required validators for Name and DateOfBirth
- Max date constraint on DatePicker (today)
- URL format validation on PhotoUrl

**Form Submission:**
- OnValidSubmit: Call PlayerService.CreatePlayerAsync
- On success: Navigate to /players/edit/{newId}
- On validation failure: Display errors
- On error: Show error alert

**Patterns to follow:**
- Use EditForm or MudForm pattern
- Bind form fields to CreatePlayerDto or local model
- Use @inject for services

**Architecture guidelines:**
- Follow `.github/instructions/blazor-architecture.instructions.md`

### Dependencies

- Depends on: Feature 2 (IPlayerService)

## Definition of Done

- [ ] Form displays all fields
- [ ] Validation working
- [ ] Save creates player
- [ ] Navigation working
- [ ] Error handling working

## Estimated Effort

Medium: 5 hours
```

---

### Sub-Issue 3.3: EditPlayer Page with Tabs

**Title**: `[Web/UI] Implement EditPlayer page with tabs`

**Labels**: `enhancement`, `ui`, `blazor`

**Body**:

```markdown
## Overview

Create the edit page with tabs for editing player information, plus placeholder tabs for future Teams and Stats features.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Blazor UI Pages

## Acceptance Criteria

- [ ] Page route: /players/edit/{id:int}
- [ ] Page title: "Edit Player: {PlayerName}"
- [ ] MudTabs with three tabs
- [ ] Tab 1 "Player Info": Edit form (active)
- [ ] Tab 2 "Teams": "FUTURE DEV" placeholder
- [ ] Tab 3 "Stats": "FUTURE DEV" placeholder
- [ ] Player Info form same fields as Create
- [ ] Age displayed as read-only
- [ ] Save button validates and submits
- [ ] Cancel button navigates to /players
- [ ] Success redirects to /players
- [ ] Player not found shows error with link to list

## Technical Details

### Scope

**File to create:**
- `src/GhcSamplePs.Web/Components/Pages/PlayerManagement/EditPlayer.razor`

### Implementation Guidance

**What to build:**

**Page Structure:**
- MudContainer (MaxWidth.Large)
- MudPaper with padding
- MudText for dynamic title
- MudTabs with MudTabPanel
- Tab 1: Player Info form
- Tab 2: MudText "FUTURE DEV - Team assignments will be available here"
- Tab 3: MudText "FUTURE DEV - Player statistics will be available here"

**Player Info Tab:**
- Same form fields as CreatePlayer
- Pre-populated with existing data
- Additional read-only Age display (MudText)
- Save Changes button
- Cancel button

**Data Loading:**
- OnInitializedAsync: Load player by ID
- Handle player not found scenario
- Show loading indicator during load

**Form Submission:**
- OnValidSubmit: Call PlayerService.UpdatePlayerAsync
- On success: Navigate to /players
- On validation failure: Display errors
- On error: Show error alert

**Not Found Handling:**
- Display MudAlert with error
- Include MudButton to navigate back to /players

**Patterns to follow:**
- Use route parameter: @page "/players/edit/{Id:int}"
- [Parameter] public int Id { get; set; }
- Handle loading/error/not-found states

**Architecture guidelines:**
- Follow `.github/instructions/blazor-architecture.instructions.md`

### Dependencies

- Depends on: Feature 2 (IPlayerService)

## Definition of Done

- [ ] Page loads player data
- [ ] Tabs display correctly
- [ ] Player Info form editable
- [ ] Age displayed read-only
- [ ] Save updates player
- [ ] Not found handled
- [ ] Navigation working

## Estimated Effort

Medium: 6 hours
```

---

## Feature 4: Player Management - Testing and Integration

### Parent Issue

**Title**: `[Feature] Player Management - Testing and Integration`

**Labels**: `Feature`, `enhancement`, `testing`, `integration`

**Body**:

```markdown
## Feature Overview

Complete the Player Management feature with comprehensive unit tests, service registration in DI container, navigation integration, and documentation updates. Ensure 85%+ code coverage for Core layer.

## Specification Reference

`docs/specs/ManagePlayers_Feature_Specification.md` - Sections: Testing Strategy, Implementation Phases

## Feature Scope

This Feature includes:
- **DI Configuration**: Register services in Program.cs
- **Navigation**: Add Players link to main navigation
- **Unit Tests**: Tests for entity, validator, service, repository
- **Documentation**: README updates

## Feature Success Criteria

- [ ] All sub-issues completed
- [ ] Services registered and resolving correctly
- [ ] Navigation working from main menu
- [ ] Unit tests achieve 85%+ coverage for Core
- [ ] All tests passing
- [ ] Documentation updated

## Sub-Issues

### Phase 1: Configuration
- [ ] #[number] - [Web/Config] Register player services in DI container
- [ ] #[number] - [Web/Navigation] Add Players to main navigation

### Phase 2: Testing
- [ ] #[number] - [Tests] Implement Player entity and validator tests
- [ ] #[number] - [Tests] Implement PlayerService tests
- [ ] #[number] - [Tests] Implement MockPlayerRepository tests

### Phase 3: Documentation
- [ ] #[number] - [Docs] Update README files and documentation

## Implementation Timeline

Estimated: 2.5 days (20 hours)
- Phase 1: 4 hours
- Phase 2: 13 hours
- Phase 3: 3 hours

## Dependencies

- **Requires**: Features 1, 2, and 3

## Acceptance Criteria

- [ ] Services resolve correctly via DI
- [ ] Navigation menu includes Players link
- [ ] All unit tests passing
- [ ] Test coverage ≥85% for Core layer
- [ ] README files updated
- [ ] XML documentation complete
```

---

### Sub-Issue 4.1: DI Container Registration

**Title**: `[Web/Config] Register player services in DI container`

**Labels**: `enhancement`, `configuration`, `dependency-injection`

**Body**:

```markdown
## Overview

Register player management services in the dependency injection container.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Testing and Integration

## Acceptance Criteria

- [ ] IPlayerRepository registered with MockPlayerRepository (Singleton)
- [ ] IPlayerService registered with PlayerService (Scoped)
- [ ] Services resolve correctly
- [ ] Application starts without errors

## Technical Details

### Scope

**File to modify:**
- `src/GhcSamplePs.Web/Program.cs`

### Implementation Guidance

**What to build:**

Add service registrations after existing service configurations:

```csharp
// Player Management Services
builder.Services.AddSingleton<IPlayerRepository, MockPlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
```

**Service Lifetimes:**
- MockPlayerRepository: Singleton (in-memory state persists for app lifetime)
- PlayerService: Scoped (new instance per request)

**Patterns to follow:**
- Add registrations in logical groups with comments
- Follow existing service registration pattern in Program.cs

**Architecture guidelines:**
- Follow `.github/instructions/blazor-architecture.instructions.md`

### Dependencies

- Depends on: Features 1 and 2 (services to register)

## Testing Requirements

- [ ] Application starts without DI errors
- [ ] Services can be injected into pages
- [ ] Repository maintains state across requests

## Definition of Done

- [ ] Services registered
- [ ] Application starts
- [ ] Manual verification of service resolution

## Estimated Effort

Small: 2 hours
```

---

### Sub-Issue 4.2: Navigation Integration

**Title**: `[Web/Navigation] Add Players to main navigation`

**Labels**: `enhancement`, `ui`, `navigation`

**Body**:

```markdown
## Overview

Add Manage Players link to the application's main navigation menu.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Testing and Integration

## Acceptance Criteria

- [ ] "Manage Players" link in navigation menu
- [ ] Clicking navigates to /players
- [ ] Active state shows when on player pages
- [ ] Works on all screen sizes

## Technical Details

### Scope

**File to modify:**
- `src/GhcSamplePs.Web/Components/Layout/NavMenu.razor` (or equivalent navigation component)

### Implementation Guidance

**What to build:**

Add navigation item using MudNavLink or equivalent:
- Text: "Manage Players" or "Players"
- Href: "/players"
- Icon: MudBlazor icon (e.g., Icons.Material.Filled.People or Icons.Material.Filled.SportsKabaddi)

**Patterns to follow:**
- Follow existing navigation item pattern
- Use same styling as other nav items

### Dependencies

- Depends on: Feature 3 (pages to navigate to)
- Depends on: Sub-Issue 4.1 (services registered)

## Definition of Done

- [ ] Navigation link visible
- [ ] Navigation works
- [ ] Active state correct

## Estimated Effort

Small: 2 hours
```

---

### Sub-Issue 4.3: Entity and Validator Tests

**Title**: `[Tests] Implement Player entity and validator tests`

**Labels**: `enhancement`, `testing`

**Body**:

```markdown
## Overview

Create comprehensive unit tests for the Player entity and PlayerValidator classes.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Testing and Integration

## Acceptance Criteria

- [ ] PlayerTests.cs created with all age calculation tests
- [ ] PlayerTests.cs tests Validate() method
- [ ] PlayerValidatorTests.cs created with all validation rules
- [ ] TestPlayerFactory helper created
- [ ] 100% coverage for Player entity
- [ ] 100% coverage for PlayerValidator

## Technical Details

### Scope

**Files to create:**
- `tests/GhcSamplePs.Core.Tests/Models/PlayerManagement/PlayerTests.cs`
- `tests/GhcSamplePs.Core.Tests/Validation/PlayerValidatorTests.cs`
- `tests/GhcSamplePs.Core.Tests/TestHelpers/TestPlayerFactory.cs`

### Implementation Guidance

**What to build:**

**PlayerTests.cs Test Cases:**
- CalculateAge_WhenBirthdayNotYetPassedThisYear_ReturnsCorrectAge
- CalculateAge_WhenBirthdayAlreadyPassedThisYear_ReturnsCorrectAge
- CalculateAge_WhenBirthdayIsToday_ReturnsCorrectAge
- CalculateAge_WhenBornOnLeapDay_HandlesCorrectly
- Validate_WhenAllFieldsValid_ReturnsTrue
- Validate_WhenNameIsEmpty_ReturnsFalse
- Validate_WhenNameIsWhitespace_ReturnsFalse
- Validate_WhenDateOfBirthInFuture_ReturnsFalse
- UpdateLastModified_UpdatesTimestampAndUser

**PlayerValidatorTests.cs Test Cases:**
- Validate_WithValidPlayer_ReturnsSuccess
- Validate_WithEmptyName_ReturnsError
- Validate_WithNameTooLong_ReturnsError
- Validate_WithWhitespaceName_ReturnsError
- Validate_WithFutureDateOfBirth_ReturnsError
- Validate_WithDateOfBirthTooOld_ReturnsError
- Validate_WithInvalidGender_ReturnsError
- Validate_WithValidGender_ReturnsSuccess
- Validate_WithNullGender_ReturnsSuccess
- Validate_WithInvalidPhotoUrl_ReturnsError
- Validate_WithPhotoUrlTooLong_ReturnsError
- Validate_WithValidPhotoUrl_ReturnsSuccess
- Validate_WithNullPhotoUrl_ReturnsSuccess
- Validate_WithMultipleErrors_ReturnsAllErrors

**TestPlayerFactory Helper:**
- CreateValidPlayer(): Returns valid Player entity
- CreateValidCreatePlayerDto(): Returns valid CreatePlayerDto
- CreateValidUpdatePlayerDto(): Returns valid UpdatePlayerDto
- CreatePlayerWithInvalidName(): Returns Player with empty name
- CreatePlayerWithFutureDOB(): Returns Player with future date
- CreatePlayersList(int count): Returns list of valid players

**Patterns to follow:**
- See `tests/GhcSamplePs.Core.Tests/Models/Identity/ApplicationUserTests.cs` for test patterns
- See `tests/GhcSamplePs.Core.Tests/TestHelpers/TestUserFactory.cs` for factory pattern
- Use [Fact(DisplayName = "...")] for descriptive names
- Use naming: MethodName_Condition_ExpectedResult

### Dependencies

- Depends on: Feature 1 (Player, PlayerValidator)

## Definition of Done

- [ ] All tests written and passing
- [ ] 100% coverage for target classes
- [ ] TestPlayerFactory usable for other tests

## Estimated Effort

Medium: 4 hours
```

---

### Sub-Issue 4.4: PlayerService Tests

**Title**: `[Tests] Implement PlayerService tests`

**Labels**: `enhancement`, `testing`

**Body**:

```markdown
## Overview

Create comprehensive unit tests for the PlayerService, mocking the repository dependency.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Testing and Integration

## Acceptance Criteria

- [ ] PlayerServiceTests.cs created
- [ ] All CRUD operations tested
- [ ] Validation scenarios tested
- [ ] Error handling tested
- [ ] Repository properly mocked
- [ ] 95%+ coverage for PlayerService

## Technical Details

### Scope

**File to create:**
- `tests/GhcSamplePs.Core.Tests/Services/PlayerServiceTests.cs`

### Implementation Guidance

**What to build:**

**Test Cases:**
- GetAllPlayersAsync_WhenPlayersExist_ReturnsAllPlayers
- GetAllPlayersAsync_WhenNoPlayers_ReturnsEmptyList
- GetAllPlayersAsync_ReturnsPlayersWithCalculatedAge
- GetPlayerByIdAsync_WhenPlayerExists_ReturnsPlayer
- GetPlayerByIdAsync_WhenPlayerExists_ReturnsCorrectAge
- GetPlayerByIdAsync_WhenPlayerNotFound_ReturnsFailResult
- CreatePlayerAsync_WithValidData_CreatesPlayer
- CreatePlayerAsync_WithValidData_SetsCreatedAtAndCreatedBy
- CreatePlayerAsync_WithInvalidName_ReturnsValidationError
- CreatePlayerAsync_WithFutureDateOfBirth_ReturnsValidationError
- CreatePlayerAsync_WithInvalidGender_ReturnsValidationError
- UpdatePlayerAsync_WithValidData_UpdatesPlayer
- UpdatePlayerAsync_WithValidData_SetsUpdatedAtAndUpdatedBy
- UpdatePlayerAsync_WhenPlayerNotFound_ReturnsFailResult
- UpdatePlayerAsync_WithInvalidData_ReturnsValidationError

**Mocking Strategy:**
- Create mock IPlayerRepository
- Setup mock to return test data
- Verify repository methods called correctly
- Use TestPlayerFactory for test data

**Note:** xUnit is used. Consider adding Moq or NSubstitute if not already available, or create simple mock implementations.

**Patterns to follow:**
- Use Arrange-Act-Assert pattern (no comments needed)
- Mock external dependencies only
- Test one behavior per test

### Dependencies

- Depends on: Feature 2 (PlayerService)
- Depends on: Sub-Issue 4.3 (TestPlayerFactory)

## Definition of Done

- [ ] All tests written and passing
- [ ] 95%+ coverage for PlayerService
- [ ] Repository properly mocked

## Estimated Effort

Medium: 6 hours
```

---

### Sub-Issue 4.5: MockPlayerRepository Tests

**Title**: `[Tests] Implement MockPlayerRepository tests`

**Labels**: `enhancement`, `testing`

**Body**:

```markdown
## Overview

Create unit tests for the MockPlayerRepository to verify CRUD operations and seed data.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Testing and Integration

## Acceptance Criteria

- [ ] MockPlayerRepositoryTests.cs created
- [ ] All repository methods tested
- [ ] Seed data verification
- [ ] Edge cases covered
- [ ] 90%+ coverage

## Technical Details

### Scope

**File to create:**
- `tests/GhcSamplePs.Core.Tests/Repositories/MockPlayerRepositoryTests.cs`

### Implementation Guidance

**What to build:**

**Test Cases:**
- GetAllAsync_ReturnsSeededPlayers
- GetAllAsync_Returns10Players
- GetByIdAsync_WithValidId_ReturnsPlayer
- GetByIdAsync_WithInvalidId_ReturnsNull
- AddAsync_AddsPlayerToStorage
- AddAsync_GeneratesNewId
- AddAsync_IncrementsIdForSubsequentPlayers
- UpdateAsync_UpdatesExistingPlayer
- UpdateAsync_WithNonExistentId_ThrowsException
- DeleteAsync_RemovesPlayer
- DeleteAsync_WithNonExistentId_ReturnsFalse
- ExistsAsync_WithExistingId_ReturnsTrue
- ExistsAsync_WithNonExistentId_ReturnsFalse

**Patterns to follow:**
- Create new instance of MockPlayerRepository for each test
- Verify seed data properties
- Test ID generation sequence

### Dependencies

- Depends on: Feature 1 (MockPlayerRepository)

## Definition of Done

- [ ] All tests written and passing
- [ ] 90%+ coverage for MockPlayerRepository

## Estimated Effort

Small: 3 hours
```

---

### Sub-Issue 4.6: Documentation Updates

**Title**: `[Docs] Update README files and documentation`

**Labels**: `enhancement`, `documentation`

**Body**:

```markdown
## Overview

Update all relevant README files and ensure XML documentation is complete.

## Parent Feature

Part of: #[parent-issue-number] - [Feature] Player Management - Testing and Integration

## Acceptance Criteria

- [ ] Core README updated with player management section
- [ ] Web README updated with player pages section
- [ ] Root README updated with feature overview
- [ ] XML documentation review completed
- [ ] All public APIs documented

## Technical Details

### Scope

**Files to modify:**
- `src/GhcSamplePs.Core/README.md`
- `src/GhcSamplePs.Web/README.md`
- `README.md` (root)

### Implementation Guidance

**What to document:**

**Core README additions:**
- Player Management section
- Models: Player, DTOs
- Services: IPlayerService, PlayerService
- Repositories: IPlayerRepository, MockPlayerRepository
- Validation: PlayerValidator
- Common: ServiceResult, ValidationResult

**Web README additions:**
- Player Management Pages section
- ManagePlayers page description
- CreatePlayer page description
- EditPlayer page description

**Root README additions:**
- Player Management feature overview
- Route information (/players)

**XML Documentation Review:**
- Ensure all public classes have summary
- Ensure all public methods have summary, param, and returns
- Ensure all public properties have summary

### Dependencies

- Depends on: All implementation complete

## Definition of Done

- [ ] READMEs updated
- [ ] XML documentation complete
- [ ] Documentation reviewed

## Estimated Effort

Small: 3 hours
```

---

## Issue Creation Checklist

When creating these issues in GitHub:

### Step 1: Create Parent Feature Issues (4 issues)
1. [ ] Create Feature 1: Core Data Layer
2. [ ] Create Feature 2: Business Logic Services
3. [ ] Create Feature 3: Blazor UI Pages
4. [ ] Create Feature 4: Testing and Integration

### Step 2: Create Sub-Issues and Link to Parents (16 issues)

**Feature 1 Sub-Issues:**
1. [ ] Create 1.1: Player entity and DTOs → Link to Feature 1
2. [ ] Create 1.2: ServiceResult and ValidationResult → Link to Feature 1
3. [ ] Create 1.3: PlayerValidator → Link to Feature 1
4. [ ] Create 1.4: Repository Pattern → Link to Feature 1

**Feature 2 Sub-Issues:**
5. [ ] Create 2.1: IPlayerService interface → Link to Feature 2
6. [ ] Create 2.2: PlayerService Implementation → Link to Feature 2
7. [ ] Create 2.3: Custom Exceptions → Link to Feature 2
8. [ ] Create 2.4: PlayerService Logging → Link to Feature 2

**Feature 3 Sub-Issues:**
9. [ ] Create 3.1: ManagePlayers List Page → Link to Feature 3
10. [ ] Create 3.2: CreatePlayer Form Page → Link to Feature 3
11. [ ] Create 3.3: EditPlayer Page with Tabs → Link to Feature 3

**Feature 4 Sub-Issues:**
12. [ ] Create 4.1: DI Container Registration → Link to Feature 4
13. [ ] Create 4.2: Navigation Integration → Link to Feature 4
14. [ ] Create 4.3: Entity and Validator Tests → Link to Feature 4
15. [ ] Create 4.4: PlayerService Tests → Link to Feature 4
16. [ ] Create 4.5: MockPlayerRepository Tests → Link to Feature 4
17. [ ] Create 4.6: Documentation Updates → Link to Feature 4

### Step 3: Update Parent Issues
After creating all sub-issues, update each parent Feature issue body to include the actual issue numbers in the Sub-Issues section.
