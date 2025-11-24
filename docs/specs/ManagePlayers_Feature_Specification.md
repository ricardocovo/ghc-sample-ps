# Feature Specification: Manage Players

## Executive Summary

**Feature Name**: Player Management System  
**Version**: 1.0 (MVP - Phase 1)  
**Last Updated**: November 24, 2025  

### Brief Description
This feature enables coaches and parents to manage soccer player information within the GhcSamplePs application. It provides a comprehensive interface to create, view, edit, and list players, storing essential information such as name, date of birth, gender, and optional photo.

### Business Value
- Enables coaches and parents to maintain an organized roster of players
- Provides a foundation for future features like team assignment and statistics tracking
- Simplifies player data entry and management with an intuitive UI
- Supports age calculation for eligibility verification
- Centralizes player information for the entire application

### Key Stakeholders
- **Primary Users**: Coaches, Team Administrators, Parents
- **Secondary Users**: System Administrators
- **Technical Owner**: Development Team
- **Business Owner**: Product Owner

---

## Requirements

### Functional Requirements

#### FR1: List Players
- Display all players in the system in a searchable, sortable list
- Show player name and calculated age for each player
- Provide visual feedback when the list is empty (no players yet)
- Navigate to player details when clicking on a player row
- Include an "Add Player" button at the top of the page

#### FR2: Create Player
- Provide a form to create a new player with required and optional fields
- Capture player name (required)
- Capture date of birth (required) with date picker
- Capture gender (optional) via dropdown selection
- Capture photo URL (optional) for future implementation
- Validate all inputs before submission
- Redirect to Edit Player page after successful creation
- Show validation errors inline

#### FR3: Edit Player
- Load existing player information into an editable form
- Allow modification of all player fields (name, date of birth, gender, photo URL)
- Display calculated age as read-only information
- Validate all inputs before saving changes
- Redirect to Manage Players list after successful update
- Show validation errors inline
- Include placeholder tabs for future "Teams" and "Stats" functionality

#### FR4: Data Validation
- Player name is required and must not exceed 200 characters
- Date of birth is required and must be a valid past date
- Gender is optional with predefined options (Male, Female, Non-binary, Prefer not to say)
- Photo URL is optional, max 500 characters, and must be valid URL format if provided
- Display clear error messages for validation failures

#### FR5: Age Calculation
- Automatically calculate player age based on date of birth
- Display age alongside player name in the list view
- Display age as read-only field in edit view
- Use current date for age calculation

### Non-Functional Requirements

#### NFR1: Performance
- Player list should load within 2 seconds with up to 1,000 players
- Form submissions should complete within 1 second
- Search and filtering should be responsive (< 500ms)

#### NFR2: Usability
- Forms should follow Material Design principles using MudBlazor components
- Mobile-responsive design for all screen sizes
- Clear visual feedback for all user actions (loading states, success, errors)
- Consistent navigation patterns across all player management pages

#### NFR3: Maintainability
- Follow clean architecture with strict separation between Core and Web layers
- All business logic in Core project (UI-agnostic)
- Comprehensive unit tests for business logic (minimum 85% coverage)
- XML documentation for all public APIs

#### NFR4: Scalability
- Use mock data provider initially for MVP
- Design data access layer to easily transition to Entity Framework Core
- Prepare for future Azure SQL Server deployment

#### NFR5: Security
- Track who created and modified each player record (audit fields)
- Validate all inputs server-side
- Prevent SQL injection through parameterized queries (future EF implementation)

### User Stories

#### US1: View All Players
**As a** coach  
**I want to** see a list of all players with their ages  
**So that** I can quickly find and access player information

**Acceptance Criteria:**
- Player list displays all players in the system
- Each row shows player name and calculated age
- Clicking a player navigates to the edit page
- Search and filter capabilities work correctly
- Empty state message displays when no players exist

#### US2: Create New Player
**As a** coach  
**I want to** add a new player to the system  
**So that** I can start tracking their information

**Acceptance Criteria:**
- Form displays all required and optional fields
- Name and date of birth are mandatory
- Date picker allows easy date selection
- Validation prevents invalid data submission
- Success message displays after creation
- User is redirected to edit page to continue setup

#### US3: Edit Player Information
**As a** coach  
**I want to** update a player's information  
**So that** I can keep their data current

**Acceptance Criteria:**
- Form loads with existing player data
- All fields are editable
- Age is displayed but not editable (calculated)
- Validation prevents invalid data
- Success message displays after update
- User is redirected to player list

#### US4: Calculate Player Age
**As a** coach  
**I want to** see the player's current age automatically calculated  
**So that** I can verify eligibility without manual calculation

**Acceptance Criteria:**
- Age is calculated from date of birth
- Age updates automatically if date of birth changes
- Age is displayed in list view
- Age is displayed as read-only in edit view

### Acceptance Criteria Summary

**Definition of Done:**
- All three pages (List, Create, Edit) are implemented and functional
- Mock data provider is working with in-memory storage
- All validation rules are enforced
- Unit tests pass with minimum 85% coverage for Core logic
- Pages are responsive on desktop, tablet, and mobile
- No errors or warnings in browser console
- Code follows project conventions from `.github/instructions/`
- Documentation is complete and up-to-date

---

## Technical Design

### Architecture Impact

**Components/Projects Affected:**
- `GhcSamplePs.Core`: New models, services, repositories, and validation logic
- `GhcSamplePs.Web`: New Blazor pages and components for player management
- `GhcSamplePs.Core.Tests`: New unit tests for services and validation

**New Components Required:**
- Domain models for Player entity
- Repository interface and mock implementation for player data access
- Service interface and implementation for player business logic
- Validation logic for player data
- Three Blazor pages: ManagePlayers, CreatePlayer, EditPlayer
- Shared components (if needed) for player-related UI elements

**Integration Points:**
- Service registration in `Program.cs` for dependency injection
- Navigation from main menu to Manage Players page
- Future integration with Teams and Statistics features

**Data Flow:**
1. User interacts with Blazor page (Web layer)
2. Page calls service method via injected interface (Core layer)
3. Service performs business logic and validation
4. Service calls repository to persist/retrieve data
5. Repository returns data to service
6. Service returns result to page
7. Page updates UI based on result

### Implementation Details

#### Data Layer

**Player Entity (Domain Model)**

Located in: `src/GhcSamplePs.Core/Models/PlayerManagement/Player.cs`

**Properties:**
- Id: Unique identifier (integer, auto-generated)
- Name: Full name of the player (string, max 200 characters, required)
- DateOfBirth: Player's birth date (DateTime, required)
- Gender: Optional gender identifier (string, max 50 characters, nullable)
- PhotoUrl: Optional URL to player photo (string, max 500 characters, nullable)
- CreatedAt: Timestamp of record creation (DateTime, UTC)
- CreatedBy: User identifier who created the record (string, required)
- UpdatedAt: Timestamp of last modification (DateTime, UTC, nullable)
- UpdatedBy: User identifier who last modified the record (string, nullable)

**Computed Properties:**
- Age: Calculated property based on DateOfBirth and current date (read-only, int)

**Behavior Methods:**
- CalculateAge(): Returns current age based on DateOfBirth
- Validate(): Ensures entity is in valid state before persistence
- UpdateLastModified(userId): Updates audit fields when entity changes

**Business Rules Enforced in Entity:**
- Name cannot be null or empty
- DateOfBirth must be in the past
- Gender, if provided, must be from predefined list
- PhotoUrl, if provided, must be valid URL format

**Repository Pattern**

**Interface**: `src/GhcSamplePs.Core/Repositories/Interfaces/IPlayerRepository.cs`

**Methods:**
- GetAllAsync(cancellationToken): Retrieves all players
- GetByIdAsync(id, cancellationToken): Retrieves a single player by ID
- AddAsync(player, cancellationToken): Adds a new player
- UpdateAsync(player, cancellationToken): Updates an existing player
- DeleteAsync(id, cancellationToken): Deletes a player (for future use)
- ExistsAsync(id, cancellationToken): Checks if player exists

**Mock Implementation**: `src/GhcSamplePs.Core/Repositories/MockPlayerRepository.cs`

**Storage Approach:**
- Use in-memory Dictionary<int, Player> for data storage
- Auto-increment ID generator using static counter
- Thread-safe operations using lock statements or ConcurrentDictionary
- Pre-seed with sample data for testing (5-10 players)
- Implement all CRUD operations
- Simulate async operations with Task.FromResult or Task.Delay

**Sample Data for Mock:**
- 10 diverse players with varied ages (6-16 years old)
- Mix of genders (including some with null)
- Realistic soccer player names
- Birth dates spanning different years
- Some with PhotoUrl populated, some null

#### Business Logic Layer

**Player Service**

**Service Interface**: `src/GhcSamplePs.Core/Services/Interfaces/IPlayerService.cs`

**Methods:**
- GetAllPlayersAsync(cancellationToken): Returns all players with age calculated
- GetPlayerByIdAsync(id, cancellationToken): Returns single player with age
- CreatePlayerAsync(playerDto, currentUserId, cancellationToken): Creates new player with validation
- UpdatePlayerAsync(id, playerDto, currentUserId, cancellationToken): Updates player with validation
- DeletePlayerAsync(id, cancellationToken): Soft delete (for future)
- ValidatePlayerAsync(playerDto, cancellationToken): Validates player data

**Service Implementation**: `src/GhcSamplePs.Core/Services/Implementations/PlayerService.cs`

**Responsibilities:**
- Coordinate between presentation and data layers
- Apply business rules and validation
- Calculate player age
- Map between DTOs and domain models
- Handle audit field population (CreatedBy, UpdatedBy, timestamps)
- Return structured results with success/failure status

**Business Rules Enforced:**
- Name is required, trimmed, and within length limits
- DateOfBirth must be valid past date (not in future)
- DateOfBirth cannot be more than 100 years ago
- Gender, if provided, must match predefined options
- PhotoUrl, if provided, must be valid absolute URL
- All dates stored in UTC

**Data Transfer Objects (DTOs)**

Located in: `src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/`

**PlayerDto** (for display):
- Id: integer
- Name: string
- DateOfBirth: DateTime
- Age: integer (calculated)
- Gender: string (nullable)
- PhotoUrl: string (nullable)
- CreatedAt: DateTime
- UpdatedAt: DateTime (nullable)

**CreatePlayerDto** (for creation):
- Name: string (required)
- DateOfBirth: DateTime (required)
- Gender: string (nullable)
- PhotoUrl: string (nullable)

**UpdatePlayerDto** (for updates):
- Name: string (required)
- DateOfBirth: DateTime (required)
- Gender: string (nullable)
- PhotoUrl: string (nullable)

**Validation Logic**

Located in: `src/GhcSamplePs.Core/Validation/PlayerValidator.cs`

**Validation Rules:**
- Name: Required, not empty/whitespace, length 1-200 characters
- DateOfBirth: Required, must be past date, not more than 100 years ago
- Gender: If provided, must be one of: "Male", "Female", "Non-binary", "Prefer not to say"
- PhotoUrl: If provided, max 500 characters, must be valid HTTP/HTTPS URL

**Validation Approach:**
- Return ValidationResult object with success flag and list of error messages
- Each field validation adds specific error message if invalid
- Stop validation early if critical errors found
- Provide field-specific error messages for UI display

**Validation Result Structure:**
- IsValid: boolean
- Errors: Dictionary<string, List<string>> (field name → error messages)
- ErrorMessages: List<string> (all errors combined)

#### API/Interface Layer

Note: This is a server-side Blazor application without traditional REST APIs. The "interface" is the service layer accessed via dependency injection.

**Service Contracts (Interfaces):**
- IPlayerService: Defines all player management operations
- IPlayerRepository: Defines data access operations

**Communication Pattern:**
- Blazor components inject IPlayerService
- Service methods return Task<ServiceResult<T>>
- ServiceResult contains: Success flag, Data, ErrorMessages, ValidationErrors

**ServiceResult Structure:**
Located in: `src/GhcSamplePs.Core/Common/ServiceResult.cs`

**Properties:**
- Success: boolean indicating operation success
- Data: Generic type T containing result data
- ErrorMessages: List of general error messages
- ValidationErrors: Dictionary of field-specific errors
- StatusCode: Optional status code (for future API use)

**Factory Methods:**
- ServiceResult.Ok(data): Create success result
- ServiceResult.Fail(message): Create failure result
- ServiceResult.ValidationFailed(errors): Create validation failure result

#### UI/Presentation Layer

**Blazor Pages Overview**

All pages located in: `src/GhcSamplePs.Web/Components/Pages/PlayerManagement/`

**Common UI Patterns:**
- Use MudBlazor components for consistent Material Design
- Implement loading states with MudProgressCircular
- Display errors using MudAlert component
- Use MudDialog for confirmations (future delete functionality)
- Implement responsive design with MudGrid and MudContainer

**Page 1: ManagePlayers.razor**

**Route**: `/players`

**Purpose**: Display list of all players with search and navigation capabilities

**UI Components:**
- MudContainer: Main page container (MaxWidth.Large)
- MudPaper: Card wrapper for content
- MudText: Page title ("Manage Players")
- MudButton: "Add Player" button (top-right, navigates to /players/create)
- MudTable: Player list table with columns:
  - Name (sortable)
  - Age (sortable, calculated from DOB)
  - Actions (Edit icon button)
- MudTextField: Search box (filters by name)
- MudProgressCircular: Loading indicator while data loads
- MudAlert: Error display if loading fails

**User Interactions:**
- Click "Add Player" button → Navigate to /players/create
- Click player row or Edit icon → Navigate to /players/edit/{id}
- Type in search box → Filter table by player name
- Click column headers → Sort table

**Data Loading:**
- OnInitializedAsync: Load all players via IPlayerService
- Display loading spinner during load
- Calculate age for each player using DateOfBirth
- Handle errors gracefully with error message display

**Empty State:**
- Display message: "No players found. Click 'Add Player' to create your first player."
- Show Add Player button prominently

**Page 2: CreatePlayer.razor**

**Route**: `/players/create`

**Purpose**: Form to create a new player

**UI Components:**
- MudContainer: Main page container (MaxWidth.Medium)
- MudPaper: Card wrapper for form
- MudText: Page title ("Create New Player")
- MudForm: Form wrapper with validation
- MudTextField: Player name input (required)
- MudDatePicker: Date of birth selector (required, max date: today)
- MudSelect: Gender dropdown (optional, with predefined options)
- MudTextField: Photo URL input (optional, URL validation)
- MudButton: "Save" button (primary, submits form)
- MudButton: "Cancel" button (navigates back to /players)
- MudAlert: Success/error message display
- MudProgressCircular: Loading indicator during save

**User Interactions:**
- Fill in form fields
- Click Save → Validate, save, navigate to /players/edit/{newId}
- Click Cancel → Navigate back to /players
- Form validates on blur and submit
- Display validation errors inline below each field

**Validation:**
- Client-side validation using MudBlazor form validation
- Server-side validation via PlayerService
- Display field-specific errors below inputs
- Disable Save button if form invalid

**Success Flow:**
- Show success message briefly
- Navigate to edit page with new player ID
- Pass success notification to edit page

**Page 3: EditPlayer.razor**

**Route**: `/players/edit/{id:int}`

**Purpose**: Edit existing player information with tabs for future features

**UI Components:**
- MudContainer: Main page container (MaxWidth.Large)
- MudPaper: Card wrapper
- MudText: Page title ("Edit Player: {PlayerName}")
- MudTabs: Tab container with three tabs:
  - **Player Info Tab** (active):
    - MudForm: Edit form (same fields as Create)
    - MudTextField: Player name (required, pre-filled)
    - MudDatePicker: Date of birth (required, pre-filled)
    - MudSelect: Gender (optional, pre-filled if exists)
    - MudTextField: Photo URL (optional, pre-filled if exists)
    - MudText: Age display (read-only, calculated)
    - MudButton: "Save Changes" (submits form)
    - MudButton: "Cancel" (navigates to /players)
  - **Teams Tab** (future):
    - MudText: "FUTURE DEV" placeholder
  - **Stats Tab** (future):
    - MudText: "FUTURE DEV" placeholder
- MudAlert: Success/error message display
- MudProgressCircular: Loading indicator

**User Interactions:**
- Page loads with player data pre-filled in form
- Edit any field
- Click Save Changes → Validate, save, navigate to /players
- Click Cancel → Navigate to /players without saving
- Switch tabs (only Player Info functional in Phase 1)

**Data Loading:**
- OnInitializedAsync: Load player by ID via IPlayerService
- Handle "player not found" scenario with error message
- Calculate age from DateOfBirth for display

**Validation:**
- Same validation rules as Create page
- Show validation errors inline
- Disable Save button if form invalid

**Success Flow:**
- Show success message briefly
- Navigate back to /players list
- Player list should reflect updated data

**Error Handling:**
- If player ID not found: Display error and provide link to go back to list
- If save fails: Display error message, keep user on page, retain form data

### Code Conventions to Follow

**From `.github/instructions/csharp.instructions.md`:**
- Use C# 14 features (file-scoped namespaces, required properties, init accessors)
- PascalCase for classes, methods, public members
- camelCase for private fields and local variables
- Prefix interfaces with "I"
- Use nullable reference types consistently
- Use `is null` and `is not null` for null checks
- XML documentation for all public APIs
- Use `nameof` instead of string literals for member names

**From `.github/instructions/blazor-architecture.instructions.md`:**
- Strict separation: Web layer references Core, never the reverse
- Business logic only in Core project
- Blazor components handle UI concerns only, delegate to services
- Use dependency injection throughout (@inject directive)
- Async/await for all I/O operations
- Keep components small and focused

**From `.github/instructions/dotnet-architecture-good-practices.instructions.md`:**
- Follow DDD principles and ubiquitous language
- Apply SOLID principles (especially SRP, DIP)
- Rich domain models with behavior, not anemic models
- Repository pattern for data access abstraction
- Service layer orchestrates business operations
- DTOs for data transfer between layers
- Explicit validation logic
- Test naming: `MethodName_Condition_ExpectedResult()`

**Player Management Specific Conventions:**

**Naming:**
- Namespace: `GhcSamplePs.Core.Models.PlayerManagement`
- Service: `IPlayerService`, `PlayerService`
- Repository: `IPlayerRepository`, `MockPlayerRepository`
- DTOs: `PlayerDto`, `CreatePlayerDto`, `UpdatePlayerDto`
- Validator: `PlayerValidator`
- Pages: `ManagePlayers.razor`, `CreatePlayer.razor`, `EditPlayer.razor`

**File Organization:**
```
Core/
  Models/
    PlayerManagement/
      Player.cs
      DTOs/
        PlayerDto.cs
        CreatePlayerDto.cs
        UpdatePlayerDto.cs
  Services/
    Interfaces/
      IPlayerService.cs
    Implementations/
      PlayerService.cs
  Repositories/
    Interfaces/
      IPlayerRepository.cs
    Implementations/
      MockPlayerRepository.cs
  Validation/
    PlayerValidator.cs
  Common/
    ServiceResult.cs

Web/
  Components/
    Pages/
      PlayerManagement/
        ManagePlayers.razor
        CreatePlayer.razor
        EditPlayer.razor
```

### Dependencies

**NuGet Packages Required:**
- No new packages required for MVP (all dependencies already in place)
- MudBlazor already installed (v8.15.0)
- .NET 10.0 SDK

**Future Dependencies (Post-MVP):**
- Entity Framework Core (when moving from mock to real database)
- Entity Framework Core SQL Server provider
- Entity Framework Core Tools (for migrations)

**Configuration Requirements:**

**appsettings.json** additions (for future):
```
{
  "PlayerManagement": {
    "MaxPlayersPerList": 1000,
    "MinPlayerAge": 5,
    "MaxPlayerAge": 18,
    "DefaultPhotoUrl": "/images/default-player.png"
  }
}
```

**Program.cs** service registrations:
- Register IPlayerRepository with MockPlayerRepository (Singleton for in-memory)
- Register IPlayerService with PlayerService (Scoped)
- No additional middleware required for MVP

### Security Considerations

**Authentication/Authorization:**
- Phase 1 MVP: No authentication required (open access)
- All users can create, read, update players
- Audit fields (CreatedBy, UpdatedBy) populated with placeholder "system" user

**Future Security Enhancements (Post-MVP):**
- Integrate with existing Entra ID authentication
- Role-based access control (Coach, Parent, Admin)
- Parents can only view/edit their own children
- Coaches can view/edit players on their teams
- Admins can manage all players

**Data Validation:**
- Server-side validation for all inputs (never trust client)
- Client-side validation for UX improvement
- Sanitize all string inputs (trim whitespace)
- Validate URL format for PhotoUrl
- Prevent injection attacks through parameterization (future EF)

**Sensitive Data Handling:**
- Date of birth is personal information (PII)
- Gender is optional for privacy
- Photos require consent (handled via optional PhotoUrl)
- Audit trail tracks all changes (who, when)

**Input Sanitization:**
- Trim all string inputs
- Encode HTML in displayed values (Blazor does this by default)
- Validate URL format before storing
- Limit string lengths to prevent buffer overflow

### Error Handling

**Expected Exceptions:**

**ValidationException:**
- Thrown when: Player data fails validation rules
- Handled by: Service layer returns ValidationFailed ServiceResult
- User sees: Inline field-specific error messages
- Logged: Warning level

**PlayerNotFoundException:**
- Thrown when: Requested player ID doesn't exist
- Handled by: Service returns Fail ServiceResult
- User sees: "Player not found" error message with navigation option
- Logged: Warning level

**RepositoryException:**
- Thrown when: Data access operation fails
- Handled by: Service catches and returns Fail ServiceResult
- User sees: Generic "Unable to complete operation" message
- Logged: Error level with full exception details

**Error Messages:**

**User-Facing Messages:**
- Validation: "[Field] is required" or "[Field] must be [constraint]"
- Not Found: "The requested player could not be found."
- Save Failed: "Unable to save player. Please try again."
- Load Failed: "Unable to load players. Please refresh the page."

**Internal Logging Messages:**
- Include full exception details
- Include user context (future: user ID)
- Include operation attempted (Create, Update, Get)
- Include entity ID (for specific operations)

**Logging Requirements:**

**Log Levels:**
- Information: Successful operations (Create, Update, Load)
- Warning: Validation failures, Not Found scenarios
- Error: Repository exceptions, unexpected failures
- Debug: Detailed operation flow (development only)

**Log Messages Include:**
- Timestamp (UTC)
- Operation name
- Entity type (Player)
- Entity ID (if applicable)
- User ID (future)
- Success/failure status
- Error details (if failed)

**Logging Approach:**
- Use ILogger<T> injected into services
- Structured logging with named parameters
- No sensitive data in logs (mask PII)
- Log exception details at Error level

---

## Testing Strategy

### Unit Test Requirements

**Test Project**: `tests/GhcSamplePs.Core.Tests/PlayerManagement/`

**Coverage Target**: Minimum 85% for Core layer business logic

**Test Categories:**

**1. Player Entity Tests**
File: `PlayerTests.cs`

Test scenarios:
- CalculateAge_WhenBirthdayNotYetThisYear_ReturnsCorrectAge
- CalculateAge_WhenBirthdayPassedThisYear_ReturnsCorrectAge
- CalculateAge_WhenBirthdayToday_ReturnsCorrectAge
- Validate_WhenNameIsEmpty_ReturnsFalse
- Validate_WhenDateOfBirthInFuture_ReturnsFalse
- Validate_WhenAllFieldsValid_ReturnsTrue
- UpdateLastModified_UpdatesTimestampAndUser

**2. PlayerService Tests**
File: `Services/PlayerServiceTests.cs`

Test scenarios:
- GetAllPlayersAsync_WhenPlayersExist_ReturnsAllPlayers
- GetAllPlayersAsync_WhenNoPlayers_ReturnsEmptyList
- GetPlayerByIdAsync_WhenPlayerExists_ReturnsPlayer
- GetPlayerByIdAsync_WhenPlayerNotFound_ReturnsFailResult
- CreatePlayerAsync_WithValidData_CreatesPlayerSuccessfully
- CreatePlayerAsync_WithInvalidName_ReturnsValidationError
- CreatePlayerAsync_WithFutureDateOfBirth_ReturnsValidationError
- UpdatePlayerAsync_WithValidData_UpdatesPlayerSuccessfully
- UpdatePlayerAsync_WhenPlayerNotFound_ReturnsFailResult
- UpdatePlayerAsync_WithInvalidData_ReturnsValidationError

**3. PlayerValidator Tests**
File: `Validation/PlayerValidatorTests.cs`

Test scenarios:
- Validate_WithValidPlayer_ReturnsSuccess
- Validate_WithEmptyName_ReturnsError
- Validate_WithNameTooLong_ReturnsError
- Validate_WithFutureDateOfBirth_ReturnsError
- Validate_WithInvalidGender_ReturnsError
- Validate_WithInvalidPhotoUrl_ReturnsError
- Validate_WithNullOptionalFields_ReturnsSuccess

**4. MockPlayerRepository Tests**
File: `Repositories/MockPlayerRepositoryTests.cs`

Test scenarios:
- GetAllAsync_ReturnsPreSeededPlayers
- GetByIdAsync_WithValidId_ReturnsPlayer
- GetByIdAsync_WithInvalidId_ReturnsNull
- AddAsync_AddsPlayerWithGeneratedId
- AddAsync_IncrementsIdForSubsequentPlayers
- UpdateAsync_UpdatesExistingPlayer
- UpdateAsync_WithNonExistentId_ThrowsException
- ExistsAsync_WithExistingId_ReturnsTrue
- ExistsAsync_WithNonExistentId_ReturnsFalse

**Test Data Setup:**
- Create TestPlayerFactory helper class
- Provide factory methods for creating valid/invalid test players
- Use realistic test data (names, dates)
- Create reusable test fixtures

**Mocking Strategy:**
- Mock IPlayerRepository in service tests
- Use in-memory mock repository (no external dependencies)
- No mocking of domain entities (test actual logic)
- Mock ILogger to verify logging calls

**Test Helpers:**
File: `TestHelpers/TestPlayerFactory.cs`

Provides:
- CreateValidPlayer(): Returns valid Player entity
- CreateValidCreatePlayerDto(): Returns valid CreatePlayerDto
- CreateInvalidPlayer(reason): Returns Player violating specific rule
- CreatePlayersList(count): Returns list of test players

### Integration Test Scenarios

Note: Integration tests are optional for Phase 1 MVP since we're using mock data. These would become important when integrating with real database.

**Future Integration Tests (Post-MVP):**
- End-to-end page navigation flows
- Form submission with validation
- Repository operations with real database (EF Core InMemory provider)
- Service operations with real repository

### Test Data Needed

**Mock Repository Seed Data** (10 players):

| Name | Date of Birth | Gender | PhotoUrl | Age (approx) |
|------|---------------|--------|----------|--------------|
| Emma Rodriguez | 2014-03-15 | Female | null | 11 |
| Liam Johnson | 2015-07-22 | Male | null | 9 |
| Olivia Martinez | 2013-11-08 | Female | /photos/olivia.jpg | 12 |
| Noah Williams | 2016-01-30 | Male | null | 8 |
| Ava Brown | 2014-09-12 | Female | /photos/ava.jpg | 10 |
| Ethan Davis | 2015-04-05 | Male | null | 10 |
| Sophia Garcia | 2013-06-18 | Non-binary | null | 11 |
| Mason Miller | 2016-12-25 | Male | /photos/mason.jpg | 8 |
| Isabella Wilson | 2014-02-14 | Female | null | 11 |
| Lucas Anderson | 2015-10-09 | Prefer not to say | null | 9 |

**Test Edge Cases:**
- Player born on leap day (Feb 29)
- Player with very long name (190 characters)
- Player with minimum age (today - 5 years)
- Player with maximum age (today - 18 years)
- Player with null gender
- Player with null photo URL
- Player with international characters in name (José, François, etc.)

### Code Coverage Expectations

**Minimum Coverage Targets:**
- Player Entity: 100% (simple, critical domain logic)
- PlayerValidator: 100% (critical validation logic)
- PlayerService: 95% (some exception paths hard to test)
- MockPlayerRepository: 90% (basic CRUD operations)
- Overall Core.PlayerManagement: 85%

**Excluded from Coverage:**
- DTOs (simple data carriers)
- Auto-generated code
- Blazor pages (tested manually in Phase 1)

---

## Implementation Phases

### Phase 1: MVP (This Specification)

**Scope**: Basic player management with mock data provider

**Deliverables:**
1. **Data Models** (Week 1, Days 1-2)
   - Player entity with all properties
   - PlayerDto, CreatePlayerDto, UpdatePlayerDto
   - ServiceResult wrapper class

2. **Mock Repository** (Week 1, Days 2-3)
   - IPlayerRepository interface
   - MockPlayerRepository implementation
   - Pre-seeded sample data (10 players)

3. **Business Logic** (Week 1, Days 3-5)
   - IPlayerService interface
   - PlayerService implementation
   - PlayerValidator validation logic
   - Age calculation logic
   - Error handling

4. **Unit Tests** (Week 2, Days 1-2)
   - Tests for all Core components
   - Achieve 85%+ coverage
   - Test data factories and helpers

5. **Blazor Pages** (Week 2, Days 3-5)
   - ManagePlayers list page
   - CreatePlayer form page
   - EditPlayer form page with future tabs
   - Navigation integration

6. **Service Registration** (Week 2, Day 5)
   - Register services in Program.cs
   - Verify dependency injection works

7. **Manual Testing & Bug Fixes** (Week 3, Days 1-2)
   - Test all user workflows
   - Fix bugs
   - Polish UI/UX

8. **Documentation** (Week 3, Day 3)
   - Update README files
   - Add code comments
   - Update specification with any changes

**Timeline**: 3 weeks (15 working days)

**Success Criteria:**
- All three pages functional
- Mock data persists during session
- All validation rules enforced
- Unit tests pass with 85%+ coverage
- UI is responsive and follows MudBlazor design
- Code review approved
- Documentation complete

### Phase 2: Database Integration (Future)

**Scope**: Replace mock repository with Entity Framework Core and Azure SQL

**Deliverables:**
1. EF Core DbContext configuration
2. Player entity configuration (Fluent API)
3. SQL Server repository implementation
4. Database migrations
5. Connection string configuration
6. Update service registration to use SQL repository
7. Integration tests with EF InMemory provider

**Prerequisites:**
- Phase 1 complete and stable
- Azure SQL Server provisioned
- Connection strings configured

**Timeline**: 1-2 weeks

### Phase 3: Team Assignment (Future)

**Scope**: Enable assigning players to teams

**Deliverables:**
1. Team and TeamPlayer entities
2. Team management services
3. "Teams" tab on Edit Player page
4. Multi-select team assignment UI
5. Roster history tracking

**Prerequisites:**
- Phase 2 complete
- Team entity domain model defined

**Timeline**: 2 weeks

### Phase 4: Statistics Tracking (Future)

**Scope**: Enable recording game statistics for players

**Deliverables:**
1. PlayerStatistic entity
2. Statistics recording service
3. "Stats" tab on Edit Player page
4. Statistics entry forms
5. Statistics display and reporting

**Prerequisites:**
- Phase 3 complete
- Championship and Season entities implemented

**Timeline**: 3 weeks

### Phase 5: Enhanced Features (Future)

**Scope**: Photo management, search enhancements, bulk operations

**Deliverables:**
1. Azure Blob Storage integration for photos
2. Photo upload component
3. Advanced search and filtering
4. Bulk player import (CSV)
5. Export player data
6. Player archiving (soft delete)

**Prerequisites:**
- Phase 4 complete
- Azure Blob Storage provisioned

**Timeline**: 2-3 weeks

---

## Migration/Deployment Considerations

### Database Migrations (Future - Phase 2)

**Initial Migration:**
- Create "Players" table with all columns
- Include audit columns (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- Add indexes on Name for search performance
- Add index on DateOfBirth for age-based queries

**Migration Strategy:**
- Use EF Core migrations for schema changes
- Apply migrations during application startup (development)
- Use migration scripts for production deployment
- Always backup database before migration
- Test migrations in staging environment first

### Configuration Changes

**Program.cs Updates:**
- Register IPlayerRepository
- Register IPlayerService
- Set repository lifetime (Singleton for mock, Scoped for EF)

**No Breaking Changes:**
- All new functionality, no existing features affected
- Safe to deploy to production after testing

### Deployment Steps (Phase 1)

1. **Build and Test:**
   - Run all unit tests
   - Verify build succeeds
   - Run manual tests on all pages

2. **Deploy to Azure Container Apps:**
   - Build Docker container
   - Push to container registry
   - Deploy to Azure Container Apps
   - Verify application starts successfully

3. **Smoke Tests:**
   - Access /players page
   - Create a test player
   - Edit the test player
   - Verify list displays correctly

4. **Rollback Strategy:**
   - Keep previous container version available
   - Rollback by deploying previous version
   - No database rollback needed (using in-memory mock)

### Rollback Strategy (Future - Phase 2)

**If Deployment Fails:**
- Revert to previous container version
- Roll back database migration if needed
- Verify previous version works
- Investigate and fix issues before redeploying

**Database Rollback:**
- Keep migration rollback scripts
- Test rollback in staging first
- Backup database before production rollback
- Restore from backup if migration rollback fails

---

## Success Metrics

### Feature Adoption Metrics

**User Engagement:**
- Number of players created per week
- Number of player edits per week
- Number of users accessing player management pages
- Average session duration on player pages

**Target Goals (3 months post-launch):**
- At least 50 players created
- At least 80% of created players have complete information
- Player management pages accessed at least 100 times per week
- Average of 2-3 edits per player (indicating active maintenance)

### Performance Benchmarks

**Page Load Times:**
- Player list page: < 2 seconds with 1000 players
- Create player page: < 1 second
- Edit player page: < 1 second
- Form submission: < 1 second

**Measurement:**
- Use browser DevTools Performance tab
- Monitor Application Insights (once deployed)
- Test with various player counts (10, 100, 1000)

### User Acceptance Criteria

**Functional Acceptance:**
- [x] User can view list of all players
- [x] User can search/filter players by name
- [x] User can create a new player
- [x] User can edit existing player information
- [x] Age is calculated correctly for all players
- [x] Validation prevents invalid data entry
- [x] Error messages are clear and helpful
- [x] Success feedback is provided after operations

**Usability Acceptance:**
- [x] Pages are responsive on mobile, tablet, desktop
- [x] Navigation is intuitive and consistent
- [x] Forms are easy to complete
- [x] Loading states provide clear feedback
- [x] Empty states guide user to next action

**Technical Acceptance:**
- [x] All unit tests pass with 85%+ coverage
- [x] No console errors or warnings
- [x] Code follows project conventions
- [x] Performance meets benchmarks
- [x] Documentation is complete and accurate

### Quality Metrics

**Code Quality:**
- Code coverage: ≥ 85% for Core layer
- No critical or high severity bugs
- Code review approval from at least 2 developers
- All TODO comments resolved
- XML documentation complete for public APIs

**Testing Metrics:**
- All unit tests pass
- No flaky tests
- Test execution time: < 10 seconds for all unit tests
- Clear test failure messages

**Documentation:**
- README files updated at all levels
- API documentation complete (XML comments)
- This specification updated with any implementation changes
- Wireframes match final implementation

---

## Risks and Mitigations

### Technical Risks

**Risk 1: Mock Data Loss on App Restart**
- **Impact**: High (users lose entered data)
- **Probability**: Certain (in-memory storage cleared on restart)
- **Mitigation**: 
  - Document clearly this is temporary mock data
  - Display warning banner on player pages
  - Accelerate Phase 2 (database integration)
  - Consider localStorage persistence as interim solution

**Risk 2: Performance with Large Player Lists**
- **Impact**: Medium (slow page loads, poor UX)
- **Probability**: Low (unlikely to exceed 1000 players in Phase 1)
- **Mitigation**:
  - Implement pagination in MudTable
  - Add debouncing to search input
  - Monitor performance metrics
  - Optimize if issues arise

**Risk 3: Validation Logic Complexity**
- **Impact**: Medium (validation bugs, security issues)
- **Probability**: Medium (complex date validation)
- **Mitigation**:
  - Comprehensive unit tests for validation
  - Peer review of validation logic
  - Explicit edge case testing
  - Clear validation error messages for debugging

**Risk 4: Integration Issues with Existing Authentication**
- **Impact**: Low (Phase 1 has no auth requirement)
- **Probability**: Low (no current integration)
- **Mitigation**:
  - Design with future auth integration in mind
  - Use "system" as placeholder for CreatedBy/UpdatedBy
  - Plan auth integration for Phase 2
  - Keep service interface auth-ready

### Business Risks

**Risk 1: User Confusion About Missing Features**
- **Impact**: Medium (user frustration, support burden)
- **Probability**: Medium (tabs show "FUTURE DEV")
- **Mitigation**:
  - Clear messaging on placeholder tabs
  - Provide roadmap visibility
  - Collect feedback on priority features
  - Communicate timeline for Teams and Stats

**Risk 2: Data Privacy Concerns**
- **Impact**: High (legal/compliance issues)
- **Probability**: Low (Phase 1 has limited user access)
- **Mitigation**:
  - Document that Phase 1 is for testing only
  - Warn users not to enter real personal data yet
  - Plan privacy features for Phase 2 (auth, access control)
  - Review LGPD compliance requirements

**Risk 3: Scope Creep**
- **Impact**: High (delayed delivery, quality issues)
- **Probability**: Medium (feature requests during development)
- **Mitigation**:
  - Clear Phase 1 scope definition
  - Document future features in Phase 2+
  - Strict change control process
  - Regular scope review with stakeholders

### Mitigation Strategies Summary

**Proactive Measures:**
1. Comprehensive testing (unit tests, manual testing)
2. Clear documentation of Phase 1 limitations
3. Regular stakeholder communication
4. Performance monitoring from day one
5. Code reviews for quality assurance

**Reactive Measures:**
1. Bug tracking and prioritization process
2. Quick feedback loop with users
3. Hotfix deployment capability
4. Rollback procedures documented and tested
5. Support channel for user issues

**Contingency Plans:**
1. If mock data proves problematic: Fast-track Phase 2 database integration
2. If performance issues arise: Implement pagination and lazy loading immediately
3. If validation bugs found: Hotfix deployment within 24 hours
4. If scope expands: Re-negotiate timeline or defer features to later phases

---

## Open Questions

### Technical Decisions

- [ ] **Q1**: Should we implement client-side caching of player list for better performance?
  - **Context**: Could reduce service calls, but adds complexity
  - **Decision needed by**: Week 1, Day 3
  - **Stakeholders**: Tech Lead, Frontend Developer

- [ ] **Q2**: Should we use localStorage for mock data persistence between sessions?
  - **Context**: Would improve UX but adds implementation complexity
  - **Decision needed by**: Week 1, Day 4
  - **Stakeholders**: Product Owner, Tech Lead

- [ ] **Q3**: What should be the default behavior for empty PhotoUrl? Show placeholder image or no image?
  - **Context**: Affects UI design and asset management
  - **Decision needed by**: Week 2, Day 3
  - **Stakeholders**: UX Designer, Product Owner

- [ ] **Q4**: Should player list support multiple sort columns (e.g., sort by name then age)?
  - **Context**: MudTable supports it, but adds complexity
  - **Decision needed by**: Week 2, Day 4
  - **Stakeholders**: Product Owner, Users

### Business Decisions

- [ ] **Q5**: What is the maximum number of players expected in the system?
  - **Context**: Affects performance optimization decisions
  - **Decision needed by**: Week 1, Day 1
  - **Stakeholders**: Product Owner, Business Analysts

- [ ] **Q6**: Should we include "soft delete" functionality in Phase 1?
  - **Context**: Safety feature but adds complexity
  - **Decision needed by**: Week 1, Day 3
  - **Stakeholders**: Product Owner, Tech Lead

- [ ] **Q7**: What gender options should be available in the dropdown?
  - **Context**: Inclusivity and privacy considerations
  - **Decision needed by**: Week 1, Day 2
  - **Stakeholders**: Product Owner, Legal/Compliance, Users
  - **Proposed**: Male, Female, Non-binary, Prefer not to say

- [ ] **Q8**: Is there a minimum age requirement for players?
  - **Context**: Business rule for youth soccer leagues
  - **Decision needed by**: Week 1, Day 2
  - **Stakeholders**: Product Owner, Domain Experts (Coaches)

### UX/Design Questions

- [ ] **Q9**: Should the player list show any additional information beyond Name and Age?
  - **Context**: Could show team assignments (future) or last updated date
  - **Decision needed by**: Week 2, Day 3
  - **Stakeholders**: UX Designer, Product Owner

- [ ] **Q10**: How should we handle very long player names in the list view?
  - **Context**: Display truncation strategy
  - **Decision needed by**: Week 2, Day 3
  - **Stakeholders**: UX Designer, Frontend Developer

- [ ] **Q11**: Should the form show a preview of the calculated age as user types date of birth?
  - **Context**: Improved UX but adds complexity
  - **Decision needed by**: Week 2, Day 4
  - **Stakeholders**: UX Designer, Frontend Developer

### Data Questions

- [ ] **Q12**: Should player names be stored with first name and last name separately?
  - **Context**: Current design uses single "Name" field
  - **Decision needed by**: Week 1, Day 1
  - **Stakeholders**: Product Owner, Tech Lead
  - **Note**: Single name field chosen for simplicity in Phase 1

- [ ] **Q13**: What format should be used for PhotoUrl storage?
  - **Context**: Absolute URLs, relative paths, or mixed?
  - **Decision needed by**: Week 1, Day 2
  - **Stakeholders**: Tech Lead, DevOps (future Azure Blob integration)

---

## Appendix

### Related Documentation

**Project Documentation:**
- Main README: `c:\playground\ghc-sample-ps\README.md`
- Core README: `c:\playground\ghc-sample-ps\src\GhcSamplePs.Core\README.md`
- Web README: `c:\playground\ghc-sample-ps\src\GhcSamplePs.Web\README.md`
- Requirements: `c:\playground\ghc-sample-ps\docs\playerstats-requirements.md`

**Architecture Guidelines:**
- Blazor Architecture: `c:\playground\ghc-sample-ps\.github\instructions\blazor-architecture.instructions.md`
- C# Guidelines: `c:\playground\ghc-sample-ps\.github\instructions\csharp.instructions.md`
- DDD Guidelines: `c:\playground\ghc-sample-ps\.github\instructions\dotnet-architecture-good-practices.instructions.md`

**Wireframes:**
- Manage Players: `c:\playground\ghc-sample-ps\docs\wireframes\ManagePlayers.png`
- Create Player: `c:\playground\ghc-sample-ps\docs\wireframes\CreatePlayer.png`
- Edit Player (Info): `c:\playground\ghc-sample-ps\docs\wireframes\EditPlayer-PlayerInfo.png`
- Edit Player (Teams): `c:\playground\ghc-sample-ps\docs\wireframes\EditPlayer-Teams.png`
- Edit Player (Stats): `c:\playground\ghc-sample-ps\docs\wireframes\EditPlayer-Stats.png`

### References to Existing Code Patterns

**Similar Entity Pattern:**
- Reference: `src/GhcSamplePs.Core/Models/Identity/ApplicationUser.cs`
- Pattern: Rich domain model with properties, computed properties, and behavior methods
- Use for: Player entity design

**Service Interface Pattern:**
- Reference: `src/GhcSamplePs.Core/Services/Interfaces/IAuthenticationService.cs`
- Pattern: Clean service interface with async methods, XML documentation
- Use for: IPlayerService design

**Blazor Page Pattern:**
- Reference: `src/GhcSamplePs.Web/Components/Pages/Home.razor`
- Pattern: MudBlazor components, MudContainer, MudPaper for layout
- Use for: All player management pages

**Project Structure:**
- Reference: Current solution structure with Core/Web/Tests separation
- Pattern: Clean architecture with dependency flow (Web → Core)
- Use for: Organizing new player management code

### Glossary

**Domain Terms:**
- **Player**: An individual soccer player tracked in the system
- **Age**: Calculated years from date of birth to current date
- **Gender**: Optional self-identified gender of player
- **Roster**: Collection of players on a team
- **MVP**: Minimum Viable Product, initial basic functionality

**Technical Terms:**
- **DTO**: Data Transfer Object, used for data exchange between layers
- **Repository Pattern**: Abstraction for data access logic
- **Service Layer**: Business logic coordination layer
- **Mock Repository**: In-memory implementation for testing without database
- **Dependency Injection**: Design pattern for providing dependencies
- **Clean Architecture**: Separation of concerns with dependency inversion

**UI Terms:**
- **MudBlazor**: Material Design component library for Blazor
- **Form Validation**: Client and server-side input checking
- **Responsive Design**: UI adapts to different screen sizes
- **Loading State**: Visual feedback during asynchronous operations
- **Empty State**: UI displayed when no data exists

### Assumptions

**Technical Assumptions:**
1. .NET 10 and C# 14 are being used
2. MudBlazor is the chosen UI component library
3. Server-side Blazor rendering mode
4. Visual Studio or VS Code is the development environment
5. Windows development environment with PowerShell

**Business Assumptions:**
1. Players are youth soccer players (ages 5-18 typically)
2. One player can belong to multiple teams
3. Player information is updated infrequently (not real-time)
4. Users are coaches, parents, or administrators
5. Initial user base is small (< 100 users)

**Data Assumptions:**
1. Player names are stored as single field (full name)
2. Dates are stored in UTC
3. Gender is optional and self-identified
4. Photo storage is deferred to future phase
5. No internationalization required in Phase 1 (English only)

### Change Log

| Date | Version | Change Description | Author |
|------|---------|-------------------|--------|
| 2025-11-24 | 1.0 | Initial specification created | Software Architect Agent |

---

## Document Control

**Document Owner**: Development Team  
**Review Frequency**: After each implementation phase  
**Next Review Date**: Upon Phase 1 completion  
**Approval Status**: Draft (pending stakeholder review)  

**Distribution List:**
- Development Team
- Product Owner
- Technical Lead
- QA Team
- UX Designer

---

**End of Specification**
