# Team Management Feature Specification

**Version**: 1.0 MVP | **Date**: December 1, 2025

## Overview

This feature implements player-to-team assignment tracking on the Teams tab of the player profile page. Players can belong to multiple teams simultaneously, with full historical tracking including join/leave dates and active status.

**Target Users**: Coaches, Team Administrators, Parents

---

## Requirements

### Functional Requirements

**CRUD Operations**:
- **View**: Display all team assignments (active/inactive) sorted by status and join date
- **Add**: Assign player to team with TeamName, ChampionshipName, JoinedDate (defaults to today)
- **Edit**: Modify team details; set LeftDate to mark inactive
- **Remove**: Soft-delete by setting LeftDate with confirmation

**Validation**:
- TeamName & ChampionshipName: Required, max 200 chars
- JoinedDate: Required, valid date
- LeftDate: Must be after JoinedDate, cannot be future
- No duplicate active assignments (same team + championship)
- IsActive: Auto-computed from LeftDate (null = active)

**UI/UX**:
- Empty state with helpful message
- Visual distinction for active/inactive teams  
- Confirmation dialogs for destructive actions
- Field-specific validation errors

### Non-Functional Requirements

- **Performance**: All operations < 1 second
- **Maintainability**: Clean architecture, 85%+ test coverage, XML docs
- **Data Integrity**: FK constraints, audit fields, cascade delete
- **Usability**: MudBlazor consistency, mobile-responsive

### Definition of Done

- ✅ Teams tab fully functional with all CRUD operations
- ✅ TeamPlayer entity, repository, service, validator implemented
- ✅ Database table with proper indexes and FK constraints
- ✅ Unit tests pass with 85%+ coverage
- ✅ UI responsive and consistent with existing patterns
- ✅ Audit fields auto-populated
- ✅ Documentation complete

---

## Technical Design

### Architecture

**New Components**:
- **Core**: TeamPlayer entity, ITeamPlayerRepository/Service + implementations, TeamPlayerValidator, DTOs
- **Web**: Teams tab in EditPlayer.razor  
- **Tests**: Unit tests for all Core components
- **Database**: TeamPlayers table with FK to Players

**Integration**: Service registration in Program.cs, DbContext configuration

### Data Model

**TeamPlayer Entity** (`src/GhcSamplePs.Core/Models/PlayerManagement/TeamPlayer.cs`)

**Properties**:
- TeamPlayerId (PK, int, auto-increment)
- PlayerId (FK, int, required, indexed)
- TeamName (string, 200, required)
- ChampionshipName (string, 200, required)
- JoinedDate (DateTime, required)
- LeftDate (DateTime, nullable)
- IsActive (bool, computed from LeftDate)
- CreatedAt, CreatedBy, UpdatedAt, UpdatedBy (audit fields)

**Navigation**: Player (many-to-one)

**Business Logic**:
- Validate(): Ensures valid state
- MarkAsLeft(leftDate, userId): Sets LeftDate and updates audit
- IsCurrentlyActive(): Returns boolean

**EF Configuration** (`src/GhcSamplePs.Core/Data/Configurations/TeamPlayerConfiguration.cs`)
- Table: "TeamPlayers"
- FK: PlayerId → Players.Id (CASCADE DELETE)
- Indexes: PlayerId, TeamName, IsActive, (PlayerId + IsActive), (PlayerId + TeamName + ChampionshipName)

### Repository Layer

**Interface**: `ITeamPlayerRepository`

**Methods**:
- GetAllByPlayerIdAsync
- GetActiveByPlayerIdAsync
- GetByIdAsync
- AddAsync
- UpdateAsync
- DeleteAsync
- ExistsAsync
- HasActiveDuplicateAsync

**Implementation**: `EfTeamPlayerRepository`
- Use AsNoTracking for reads
- Log all operations
- Handle concurrency conflicts
- Order by JoinedDate DESC

### Service Layer

**Interface**: `ITeamPlayerService`

**Methods**:
- GetTeamsByPlayerIdAsync
- GetActiveTeamsByPlayerIdAsync
- GetTeamAssignmentByIdAsync
- AddPlayerToTeamAsync
- UpdateTeamAssignmentAsync
- RemovePlayerFromTeamAsync
- ValidateTeamAssignmentAsync

**Implementation**: `TeamPlayerService`
- Apply business rules and validation
- Map between DTOs and entities
- Populate audit fields
- Prevent duplicate active assignments
- Return ServiceResult<T>

**DTOs**:
- **TeamPlayerDto**: Full display (includes IsActive, DurationDays)
- **CreateTeamPlayerDto**: For creation (PlayerId, TeamName, ChampionshipName, JoinedDate)
- **UpdateTeamPlayerDto**: For updates (adds LeftDate)

**Validator**: `TeamPlayerValidator`
- Field validation (length, required, format)
- Cross-field validation (LeftDate > JoinedDate)
- Returns ValidationResult with field-specific errors

### UI Layer

**Location**: `EditPlayer.razor` Teams tab

**Components**:
- **List View**: MudTable/MudList showing teams with badges (Active/Inactive)
- **Empty State**: Icon + message + "Add Team" button
- **Add Dialog**: MudForm with TeamName, ChampionshipName, JoinedDate
- **Edit Dialog**: Same as Add + LeftDate field
- **Remove Confirmation**: MudDialog confirming soft-delete

**State Management**:
- `_teams`: List<TeamPlayerDto>
- `_isLoadingTeams`: bool
- `_showAddTeamDialog`: bool
- `_editingTeam`: TeamPlayerDto

**User Flows**:
1. **View**: Tab loads → service call → display list
2. **Add**: Click Add → dialog → validate → save → refresh
3. **Edit**: Click Edit icon → dialog → validate → save → refresh
4. **Remove**: Click Remove → confirm → set LeftDate → refresh

### Code Conventions

Follow existing patterns from Player management:
- C# 14 features (file-scoped namespaces, required properties, init)
- PascalCase for public, camelCase for private
- Interfaces prefixed with "I"
- XML docs for public APIs
- Async/await for I/O
- Test naming: `MethodName_Condition_ExpectedResult`

**File Structure**:
```
Core/
  Models/PlayerManagement/
    TeamPlayer.cs
    DTOs/
  Services/Interfaces/ + Implementations/
  Repositories/Interfaces/ + Implementations/
  Validation/
    TeamPlayerValidator.cs
  Data/Configurations/
    TeamPlayerConfiguration.cs
Web/
  Components/Pages/PlayerManagement/
    EditPlayer.razor (update Teams tab)
Core.Tests/
  Models/, Services/, Repositories/, Validation/
```

### Dependencies

**NuGet**: None (existing packages sufficient)  
**Database**: Azure SQL Server  
**Configuration**: Service registration in Program.cs

### Security

- Leverage existing Entra ID authentication
- Validate player ownership before operations
- Server-side validation (never trust client)
- Sanitize all string inputs (trim)
- Prevent SQL injection (EF Core parameterization)
- Log unauthorized access attempts

### Error Handling

**Expected Exceptions**:
- ValidationException → ValidationFailed ServiceResult
- TeamPlayerNotFoundException → Fail ServiceResult  
- DuplicateTeamAssignmentException → ValidationFailed ServiceResult
- RepositoryException → Fail ServiceResult (log Error level)

**User Messages**:
- Validation: "[Field] is required" or "[Field] must be [constraint]"
- Not Found: "The team assignment could not be found."
- Duplicate: "Player is already active on [Team] in [Championship]."
- Generic: "Unable to complete operation, please try again."

**Logging**: ILogger<T> with structured logging, include user context, entity IDs, timestamps

---

## Testing Strategy

### Unit Tests

**Coverage Target**: 85% minimum for Core

**Test Files**:
- `TeamPlayerTests.cs`: Entity validation, business logic
- `TeamPlayerServiceTests.cs`: All service methods, business rules
- `TeamPlayerValidatorTests.cs`: All validation scenarios
- `EfTeamPlayerRepositoryTests.cs`: CRUD operations, duplicate detection

**Test Helper**: `TestTeamPlayerFactory.cs` for creating test data

**Mocking**: Mock repositories in service tests, use EF InMemory for repository tests

### Integration Tests

`TeamPlayerIntegrationTests.cs`:
- Complete workflows (add → retrieve → update → delete)
- Concurrent updates
- Cascade delete verification
- Duplicate prevention

### Test Data

Sample teams for testing:
- Emma Rodriguez: Thunder FC (Spring 2025, active), Lightning United (Fall 2024, inactive)
- Liam Johnson: Rapids SC (Spring 2025, active)

**Edge Cases**: Same-day operations, long names (190 chars), 10+ teams per player

---

## Implementation Plan

### Phase 1: MVP (3 weeks)

**Week 1: Data Layer**
- Days 1-2: TeamPlayer entity, EF configuration, migration
- Days 3-4: Repository interface + implementation + tests
- Day 5: Service interfaces, DTOs, validator + tests

**Week 2: Service & UI**
- Days 1-2: Service implementation + tests (85%+ coverage)
- Days 3-4: UI implementation (Teams tab, dialogs, state)
- Day 5: Integration testing (complete workflows)

**Week 3: Polish**
- Days 1-2: Bug fixes, UX polish, query optimization
- Day 3: Documentation (README, XML docs, user guide)

**Success Criteria**: All Definition of Done items checked

### Phase 2: Enhanced Features (Future)

Bulk assignment, team roster view, statistics aggregation, reporting (2-3 weeks)

### Phase 3: Statistics Integration (Future)

PlayerStatistic entity with TeamPlayerId FK, team-based reports (3-4 weeks)

---

## Deployment

### Database Migration

**Migration**: `AddTeamPlayersTable`

**Creates**:
- TeamPlayers table with all columns
- PK on TeamPlayerId
- FK to Players (CASCADE DELETE)
- Indexes on PlayerId, TeamName, IsActive, composites

**Commands**:
```powershell
# Development
dotnet ef migrations add AddTeamPlayersTable --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web
dotnet ef database update

# Production
dotnet ef migrations script --output migrations/AddTeamPlayersTable.sql
```

### Deployment Steps

1. **Pre-deployment**: Run tests, verify migration, backup database
2. **Deploy**: Deploy to Azure Container Apps, auto-run migrations
3. **Post-deployment**: Monitor logs, smoke test, collect feedback

**Rollback**: Redeploy previous version or apply DOWN migration

---

## Success Metrics

### Adoption (3 months post-launch)
- 80% of players have team assignments
- Average 2-3 teams per player
- Teams tab accessed 200+ times/week
- < 5% error rate

### Performance
- Load teams: < 500ms
- Add/edit/remove: < 1 second
- Database queries: < 100ms

### Quality
- Code coverage: ≥ 85%
- No critical bugs
- All tests pass
- Documentation complete

---

## Appendix

### Related Documentation

- Requirements: `docs/playerstats-requirements.md`
- Player Management Spec: `docs/specs/ManagePlayers_Feature_Specification.md`
- Architecture: `.github/instructions/`
- Database Setup: `docs/Database_Connection_Setup.md`

### Reference Patterns

- **Entity**: `Player.cs`
- **EF Config**: `PlayerConfiguration.cs`
- **Repository**: `EfPlayerRepository.cs`
- **Service**: `PlayerService.cs`
- **UI**: `EditPlayer.razor`

### Glossary

- **TeamPlayer**: Entity representing player's team membership
- **Active**: Team assignment without LeftDate
- **Inactive**: Team assignment with LeftDate set
- **Cascade Delete**: Auto-delete team assignments when player deleted
- **Soft Delete**: Mark inactive via LeftDate (preserve history)

### Open Questions

1. Max teams per player? (Proposed: no limit, warn if > 10)
2. Allow backdating? (Proposed: yes, up to 10 years)
3. Team name standardization? (Phase 2)
4. Edit inactive assignments? (Yes, for corrections)
5. Concurrency control? (Add if needed based on usage)

---

**Document Owner**: Development Team  
**Status**: Draft (pending stakeholder review)

---

**End of Specification**
