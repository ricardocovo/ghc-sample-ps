# Player Statistics Feature Specification

**Version**: 1.0 MVP | **Date**: December 1, 2025

## Overview

This feature enables coaches and parents to record game-level performance statistics for players. Statistics are tracked per game and linked to a specific team assignment (TeamPlayer), ensuring accurate historical tracking when players move between teams.

**Target Users**: Coaches, Parents

---

## Requirements

### Functional Requirements

**CRUD Operations**:
- **View**: Display all game statistics for a player sorted by game date (newest first)
- **Add**: Record statistics for a specific game with team context
- **Edit**: Modify existing game statistics
- **Delete**: Remove incorrect statistics entries with confirmation

**Statistics Captured**:
- TeamPlayer (which team context - required)
- GameDate (date of the game - required)
- MinutesPlayed (time on field - required, ≥ 0)
- IsStarter (started the game - required boolean)
- JerseyNumber (number worn - required, > 0)
- Goals (goals scored - required, ≥ 0)
- Assists (assists made - required, ≥ 0)

**Validation**:
- All numeric fields must be non-negative (except JerseyNumber > 0)
- GameDate cannot be in the future
- TeamPlayerId must reference an active or historical team assignment
- Required fields enforced

**UI/UX**:
- Statistics tab on player profile page
- Empty state with helpful message
- List view showing game date, team, minutes, goals, assists
- Summary cards showing totals (games played, total goals, total assists)
- Form validation with field-specific errors

### Non-Functional Requirements

- **Performance**: All operations < 1 second
- **Maintainability**: Clean architecture, 85%+ test coverage, XML docs
- **Data Integrity**: FK constraints, audit fields, cascade delete when TeamPlayer deleted
- **Usability**: MudBlazor consistency, mobile-responsive

### Definition of Done

- ✅ Statistics tab fully functional with all CRUD operations
- ✅ PlayerStatistic entity, repository, service, validator implemented
- ✅ Database table with proper indexes and FK constraints
- ✅ Unit tests pass with 85%+ coverage
- ✅ UI responsive and consistent with existing patterns
- ✅ Audit fields auto-populated
- ✅ Basic aggregations displayed (totals)
- ✅ Documentation complete

---

## Technical Design

### Architecture

**New Components**:
- **Core**: PlayerStatistic entity, IPlayerStatisticRepository/Service + implementations, PlayerStatisticValidator, DTOs
- **Web**: Statistics tab in EditPlayer.razor
- **Tests**: Unit tests for all Core components
- **Database**: PlayerStatistics table with FK to TeamPlayers

**Integration**: Service registration in Program.cs, DbContext configuration

### Data Model

**PlayerStatistic Entity** (`src/GhcSamplePs.Core/Models/PlayerManagement/PlayerStatistic.cs`)

**Properties**:
- PlayerStatisticId (PK, int, auto-increment)
- TeamPlayerId (FK, int, required, indexed)
- GameDate (DateTime, required, indexed)
- MinutesPlayed (int, required, ≥ 0)
- IsStarter (bool, required)
- JerseyNumber (int, required, > 0)
- Goals (int, required, ≥ 0)
- Assists (int, required, ≥ 0)
- CreatedAt, CreatedBy, UpdatedAt, UpdatedBy (audit fields)

**Navigation**: TeamPlayer (many-to-one)

**Business Logic**:
- Validate(): Ensures valid state
- CalculateTotalMinutes(statistics): Sum minutes across games
- CalculateTotalGoals(statistics): Sum goals across games
- CalculateTotalAssists(statistics): Sum assists across games

**EF Configuration** (`src/GhcSamplePs.Core/Data/Configurations/PlayerStatisticConfiguration.cs`)
- Table: "PlayerStatistics"
- FK: TeamPlayerId → TeamPlayers.TeamPlayerId (CASCADE DELETE)
- Indexes: TeamPlayerId, GameDate, (TeamPlayerId + GameDate) composite

### Repository Layer

**Interface**: `IPlayerStatisticRepository`

**Methods**:
- GetAllByPlayerIdAsync(playerId, cancellationToken)
- GetAllByTeamPlayerIdAsync(teamPlayerId, cancellationToken)
- GetByIdAsync(statisticId, cancellationToken)
- GetByDateRangeAsync(playerId, startDate, endDate, cancellationToken)
- AddAsync(statistic, cancellationToken)
- UpdateAsync(statistic, cancellationToken)
- DeleteAsync(statisticId, cancellationToken)
- ExistsAsync(statisticId, cancellationToken)
- GetAggregatesAsync(playerId, teamPlayerId, cancellationToken)

**Implementation**: `EfPlayerStatisticRepository`
- Use AsNoTracking for reads
- Log all operations
- Order by GameDate DESC
- Include TeamPlayer navigation for display

### Service Layer

**Interface**: `IPlayerStatisticService`

**Methods**:
- GetStatisticsByPlayerIdAsync(playerId, cancellationToken)
- GetStatisticsByTeamPlayerIdAsync(teamPlayerId, cancellationToken)
- GetStatisticByIdAsync(statisticId, cancellationToken)
- AddStatisticAsync(createDto, currentUserId, cancellationToken)
- UpdateStatisticAsync(statisticId, updateDto, currentUserId, cancellationToken)
- DeleteStatisticAsync(statisticId, cancellationToken)
- GetPlayerAggregatesAsync(playerId, teamPlayerId, cancellationToken)

**Implementation**: `PlayerStatisticService`
- Apply business rules and validation
- Map between DTOs and entities
- Populate audit fields
- Return ServiceResult<T>

**DTOs**:
- **PlayerStatisticDto**: Full display (includes team name, championship name via navigation)
- **CreatePlayerStatisticDto**: For creation (TeamPlayerId, GameDate, MinutesPlayed, IsStarter, JerseyNumber, Goals, Assists)
- **UpdatePlayerStatisticDto**: For updates (same as create + PlayerStatisticId)
- **PlayerAggregatesDto**: Summary stats (GamesPlayed, TotalMinutes, TotalGoals, TotalAssists, AverageGoalsPerGame, AverageAssistsPerGame)

**Validator**: `PlayerStatisticValidator`
- TeamPlayerId: Required, must reference existing TeamPlayer
- GameDate: Required, valid date, not in future
- MinutesPlayed: Required, ≥ 0, ≤ 120 (reasonable max)
- IsStarter: Required boolean
- JerseyNumber: Required, > 0, ≤ 99 (reasonable max)
- Goals: Required, ≥ 0
- Assists: Required, ≥ 0

### UI Layer

**Location**: `EditPlayer.razor` Statistics tab

**Components**:

1. **Summary Cards** (top of tab)
   - MudPaper cards showing: Games Played, Total Goals, Total Assists, Avg Goals/Game
   - Load aggregates on tab access

2. **Statistics List**
   - MudTable showing: Game Date, Team, Championship, Minutes, Goals, Assists, Starter badge
   - Action icons: Edit, Delete
   - Sorted by GameDate DESC

3. **Empty State**
   - Icon + message: "No statistics recorded yet"
   - "Add Statistics" button

4. **Add Statistics Dialog**
   - MudForm with fields:
     - TeamPlayer (MudSelect - dropdown of player's teams)
     - Game Date (MudDatePicker)
     - Minutes Played (MudNumericField)
     - Is Starter (MudCheckbox)
     - Jersey Number (MudNumericField)
     - Goals (MudNumericField)
     - Assists (MudNumericField)
   - Buttons: Add, Cancel

5. **Edit Statistics Dialog**
   - Same as Add, pre-filled with existing data
   - Buttons: Save, Cancel

6. **Delete Confirmation Dialog**
   - Shows game date and team
   - Buttons: Delete (error color), Cancel

**State Management**:
- `_statistics`: List<PlayerStatisticDto>
- `_aggregates`: PlayerAggregatesDto
- `_teamPlayers`: List<TeamPlayerDto> (for dropdown)
- `_isLoadingStatistics`: bool
- `_showAddDialog`, `_showEditDialog`: bool
- `_editingStatistic`: PlayerStatisticDto

### Code Conventions

Follow existing patterns from Player and Team management:
- C# 14 features
- Clean architecture (Core/Web separation)
- Repository pattern with EF Core
- Service layer with ServiceResult
- Async/await throughout
- Test naming: `MethodName_Condition_ExpectedResult`
- XML docs on public APIs

**File Structure**:
```
Core/
  Models/PlayerManagement/
    PlayerStatistic.cs
    DTOs/
      PlayerStatisticDto.cs
      CreatePlayerStatisticDto.cs
      UpdatePlayerStatisticDto.cs
      PlayerAggregatesDto.cs
  Services/Interfaces/ + Implementations/
  Repositories/Interfaces/ + Implementations/
  Validation/
    PlayerStatisticValidator.cs
  Data/Configurations/
    PlayerStatisticConfiguration.cs
Web/
  Components/Pages/PlayerManagement/
    EditPlayer.razor (add Statistics tab)
Core.Tests/
  Models/, Services/, Repositories/, Validation/
```

### Dependencies

**NuGet**: None (existing packages sufficient)
**Prerequisites**: Team Management feature (#134) complete - TeamPlayer entity must exist
**Configuration**: Service registration in Program.cs

### Security

- Leverage existing Entra ID authentication
- Validate player ownership before operations
- Server-side validation
- Sanitize numeric inputs
- Log unauthorized access attempts

### Error Handling

**Expected Exceptions**:
- ValidationException → ValidationFailed ServiceResult
- PlayerStatisticNotFoundException → Fail ServiceResult
- TeamPlayerNotFoundException → Fail ServiceResult (invalid team reference)
- RepositoryException → Fail ServiceResult

**User Messages**:
- Validation: "[Field] must be [constraint]"
- Not Found: "Statistics not found"
- Invalid Team: "Selected team is not valid for this player"
- Generic: "Unable to complete operation"

**Logging**: ILogger<T>, include user context, entity IDs, timestamps

---

## Testing Strategy

### Unit Tests

**Coverage Target**: 85% minimum for Core

**Test Files**:
- `PlayerStatisticTests.cs`: Entity validation, business logic
- `PlayerStatisticServiceTests.cs`: All service methods, business rules
- `PlayerStatisticValidatorTests.cs`: All validation scenarios
- `EfPlayerStatisticRepositoryTests.cs`: CRUD operations, aggregates

**Key Test Scenarios**:
- Valid statistic creation
- Negative numbers rejected
- Future GameDate rejected
- JerseyNumber validation (> 0, ≤ 99)
- Invalid TeamPlayerId rejected
- Aggregate calculations correct
- Repository queries work correctly
- Service handles errors properly

**Test Helper**: `TestPlayerStatisticFactory.cs` for creating test data

**Mocking**: Mock repositories in service tests, use EF InMemory for repository tests

### Integration Tests

`PlayerStatisticIntegrationTests.cs`:
- Complete add/retrieve/update/delete workflow
- Cascade delete when TeamPlayer deleted
- Aggregate calculations with multiple stats
- Date range queries
- Multiple players and teams scenarios

---

## Implementation Plan

### Phase 1: MVP (3 weeks)

**Week 1: Data Layer**
- Days 1-2: PlayerStatistic entity, EF configuration, migration
- Days 3-4: Repository interface + implementation + tests
- Day 5: Validator, DTOs + tests

**Week 2: Service & UI**
- Days 1-2: Service implementation + tests (85%+ coverage)
- Days 3-4: UI implementation (Statistics tab, dialogs, aggregates)
- Day 5: Integration testing

**Week 3: Polish & Documentation**
- Days 1-2: Bug fixes, UX polish, aggregate calculations
- Day 3: Documentation (README, XML docs, user guide)

**Success Criteria**: All Definition of Done items checked

### Phase 2: Enhancements (Future)

- Filter statistics by team
- Filter by date range
- Export to CSV
- Charts/visualizations (goals over time, etc.)
- Comparison across teams
- Season-level aggregations

---

## Deployment

### Database Migration

**Migration**: `AddPlayerStatisticsTable`

**Creates**:
- PlayerStatistics table with all columns
- PK on PlayerStatisticId
- FK to TeamPlayers (CASCADE DELETE)
- Indexes on TeamPlayerId, GameDate, composite

**Commands**:
```powershell
# Development
dotnet ef migrations add AddPlayerStatisticsTable --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web
dotnet ef database update

# Production
dotnet ef migrations script --output migrations/AddPlayerStatisticsTable.sql
```

### Deployment Steps

1. **Pre-deployment**: Run tests, verify migration, backup database
2. **Deploy**: Deploy to Azure Container Apps, auto-run migrations
3. **Post-deployment**: Monitor logs, smoke test, verify statistics entry

**Rollback**: Redeploy previous version or apply DOWN migration

---

## Success Metrics

### Adoption (3 months post-launch)
- 70% of players have at least one statistic recorded
- Average 5+ games per player
- Statistics tab accessed 300+ times/week
- < 5% error rate

### Performance
- Load statistics: < 500ms with 50 games
- Add/edit/delete: < 1 second
- Aggregate calculations: < 200ms

### Quality
- Code coverage: ≥ 85%
- No critical bugs
- All tests pass
- Documentation complete

---

## Appendix

### Related Documentation

- Requirements: `docs/playerstats-requirements.md`
- Team Management Spec: `docs/specs/TeamManagement_Feature_Specification.md`
- Architecture: `.github/instructions/`

### Reference Patterns

- **Entity**: `Player.cs`, `TeamPlayer.cs`
- **Repository**: `EfPlayerRepository.cs`, `EfTeamPlayerRepository.cs`
- **Service**: `PlayerService.cs`, `TeamPlayerService.cs`
- **UI**: `EditPlayer.razor` (Player Info and Teams tabs)

### Glossary

- **PlayerStatistic**: Performance data for a single game
- **TeamPlayer**: Team assignment context for statistics
- **Aggregates**: Calculated totals across multiple games
- **Game Date**: Date the game was played
- **Starter**: Player who began the game on the field

### Key Business Rules

1. Statistics must reference a valid TeamPlayer (ensures team context)
2. Multiple statistics per player per day allowed (doubleheaders)
3. All numeric stats must be non-negative (except jersey > 0)
4. GameDate cannot be future
5. Statistics cascade delete when TeamPlayer removed
6. Aggregates calculated on-demand (not stored)

### Open Questions

1. Should we allow backdating statistics? (Proposed: Yes, any past date)
2. Maximum minutes per game? (Proposed: 120 as soft limit)
3. Maximum jersey number? (Proposed: 99 as reasonable max)
4. Should we validate GameDate within team's active period? (Proposed: Phase 2)
5. Default values for quick entry? (Proposed: IsStarter=false, others required)

---

**Document Owner**: Development Team  
**Status**: Draft (pending stakeholder review)

---

**End of Specification**
