# Dashboard Feature Specification

**Version**: 1.0 | **Date**: December 23, 2025

## Overview

Transform the current home page into an informative dashboard that provides users with an at-a-glance view of their player statistics, active teams, and recent game performance. The dashboard will serve as the primary landing page after authentication, offering quick access to key metrics and navigation to detailed features.

**Target Users**: Coaches, Parents (authenticated users with User or Admin role)

---

## Requirements

### Functional Requirements

**Dashboard Components**:
- **Welcome Section**: Personalized greeting with user name
- **Quick Stats Summary**: Key metrics displayed in card format
  - Total Players count
  - Active Teams count
  - Total Games Played (across all players)
  - Total Goals Scored (across all players)
- **Recent Activity**: List of recent game statistics (last 5-10 games)
  - Player name
  - Team name
  - Game date
  - Goals and assists
- **Active Teams Overview**: Display all active team assignments
  - Team name
  - Championship name
  - Number of players on each team
- **Quick Actions**: Navigation shortcuts
  - View All Players
  - Add New Player
  - Manage Teams
- **Performance Highlights**: Top performers
  - Top goal scorers (this season/all-time)
  - Most active players (by games played)

**Authorization**:
- Dashboard only visible to authenticated users (User or Admin role)
- Anonymous users redirected to sign-in
- Admin users see additional system information (optional enhancement)

**Responsive Design**:
- Mobile-first layout using MudBlazor grid system
- Cards stack vertically on mobile devices
- Charts and graphs responsive to screen size

### Non-Functional Requirements

- **Performance**: Dashboard loads in < 2 seconds with up to 100 players
- **Usability**:
  - Clear visual hierarchy with appropriate spacing
  - Consistent with existing MudBlazor design patterns
  - Accessible with proper ARIA labels
- **Maintainability**:
  - Clean architecture - no business logic in Razor components
  - Service-based data retrieval
  - Component reusability where possible
- **Data Freshness**: Real-time data on each page load (no caching initially)

### Definition of Done

- ✅ Dashboard replaces current Home.razor with title "Dashboard"
- ✅ All dashboard components render correctly on desktop and mobile
- ✅ Quick stats display accurate counts from database
- ✅ Recent activity shows latest games with proper sorting
- ✅ Active teams overview displays current team assignments
- ✅ Quick action buttons navigate to correct pages
- ✅ Authorization policies enforced (RequireUserRole)
- ✅ UI consistent with MudBlazor design system
- ✅ Page loads within performance target
- ✅ Unit tests for dashboard service/aggregation logic (if new service created)
- ✅ Documentation updated

---

## Technical Design

### Architecture

**Components Affected**:
- **Web**: Modify `src/GhcSamplePs.Web/Components/Pages/Home.razor` → Rename/redesign as Dashboard
- **Core**: Create `IDashboardService` interface and implementation (optional - may use existing services)
- **Services**: Utilize existing `IPlayerService`, `ITeamPlayerService`, `IPlayerStatisticService`

**New Components** (optional):
- `DashboardStatsCard.razor` - Reusable card component for quick stats
- `RecentActivityList.razor` - Component to display recent games
- `ActiveTeamsOverview.razor` - Component to display active teams

### Data Model

**Dashboard Aggregates DTO** (if creating new service):

**DashboardSummaryDto**:
```csharp
public class DashboardSummaryDto
{
    public int TotalPlayers { get; set; }
    public int ActiveTeams { get; set; }
    public int TotalGamesPlayed { get; set; }
    public int TotalGoals { get; set; }
    public int TotalAssists { get; set; }
    public IReadOnlyList<RecentGameDto> RecentGames { get; set; } = new List<RecentGameDto>();
    public IReadOnlyList<ActiveTeamSummaryDto> ActiveTeams { get; set; } = new List<ActiveTeamSummaryDto>();
    public IReadOnlyList<TopPerformerDto> TopScorers { get; set; } = new List<TopPerformerDto>();
}
```

**RecentGameDto**:
```csharp
public class RecentGameDto
{
    public int PlayerStatisticId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string ChampionshipName { get; set; } = string.Empty;
    public DateTime GameDate { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int MinutesPlayed { get; set; }
}
```

**ActiveTeamSummaryDto**:
```csharp
public class ActiveTeamSummaryDto
{
    public string TeamName { get; set; } = string.Empty;
    public string ChampionshipName { get; set; } = string.Empty;
    public int ActivePlayerCount { get; set; }
    public int TotalGamesPlayed { get; set; }
}
```

**TopPerformerDto**:
```csharp
public class TopPerformerDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int TotalGoals { get; set; }
    public int GamesPlayed { get; set; }
    public decimal GoalsPerGame { get; set; }
}
```

### Service Layer (Option A - Use Existing Services)

**Approach**: Use existing services directly in the Razor component
- Call `IPlayerService.GetAllPlayersAsync()` for player count
- Call `ITeamPlayerService` for active teams
- Call `IPlayerStatisticService.GetStatisticsByPlayerIdAsync()` for statistics across all players
- Aggregate data in code-behind or Razor component

**Pros**: No new service layer code, leverages existing infrastructure
**Cons**: Component becomes heavier with aggregation logic

### Service Layer (Option B - Create Dashboard Service)

**Interface**: `IDashboardService`

**Methods**:
```csharp
public interface IDashboardService
{
    /// <summary>
    /// Retrieves a complete dashboard summary for the current user's players and teams.
    /// </summary>
    Task<ServiceResult<DashboardSummaryDto>> GetDashboardSummaryAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves recent game statistics across all players owned by the user.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<RecentGameDto>>> GetRecentGamesAsync(
        string userId,
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves top scoring players across all the user's players.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<TopPerformerDto>>> GetTopScorersAsync(
        string userId,
        int count = 5,
        CancellationToken cancellationToken = default);
}
```

**Implementation**: `DashboardService`
- Aggregate data from Player, TeamPlayer, and PlayerStatistic repositories
- Apply user-based filtering (UserId) to ensure data isolation
- Calculate totals and averages
- Sort and limit results for top performers

**Pros**: Clean separation of concerns, testable aggregation logic
**Cons**: Additional service layer code

### UI Design

**Page Structure** (`Home.razor` → Dashboard):
```
┌─────────────────────────────────────────────────────┐
│ Dashboard (Page Title)                               │
├─────────────────────────────────────────────────────┤
│ Welcome Section                                      │
│ ┌─────────────────────────────────────────────────┐ │
│ │ 👤 Welcome, [User Name]!                         │ │
│ │ Here's your soccer statistics overview           │ │
│ └─────────────────────────────────────────────────┘ │
│                                                      │
│ Quick Stats (Grid: 4 Cards on Desktop, 1 on Mobile) │
│ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐   │
│ │ 👥 12   │ │ ⚽ 8    │ │ 🎮 45   │ │ 🥅 78   │   │
│ │ Players │ │ Teams   │ │ Games   │ │ Goals   │   │
│ └─────────┘ └─────────┘ └─────────┘ └─────────┘   │
│                                                      │
│ Two-Column Layout (Stack on Mobile)                 │
│ ┌──────────────────────┐ ┌──────────────────────┐  │
│ │ Recent Activity      │ │ Active Teams         │  │
│ │ ┌──────────────────┐ │ │ ┌──────────────────┐ │  │
│ │ │ Player A         │ │ │ │ Team Alpha       │ │  │
│ │ │ U12 - 2 goals    │ │ │ │ Championship '25 │ │  │
│ │ │ Dec 20, 2025     │ │ │ │ 5 players        │ │  │
│ │ └──────────────────┘ │ │ └──────────────────┘ │  │
│ │ [More recent games]  │ │ [More teams]         │  │
│ └──────────────────────┘ └──────────────────────┘  │
│                                                      │
│ Quick Actions (Button Row)                          │
│ ┌────────────┐ ┌────────────┐ ┌────────────┐      │
│ │ View All   │ │ Add New    │ │ Manage     │      │
│ │ Players    │ │ Player     │ │ Teams      │      │
│ └────────────┘ └────────────┘ └────────────┘      │
└─────────────────────────────────────────────────────┘
```

**MudBlazor Components**:
- `<MudContainer>` - Main container
- `<MudPaper>` - Card backgrounds
- `<MudGrid>` / `<MudItem>` - Responsive layout
- `<MudCard>` - Individual stat cards
- `<MudSimpleTable>` - Recent activity list
- `<MudButton>` - Quick action buttons
- `<MudIcon>` - Icons for visual interest
- `<AuthorizeView>` - Role-based rendering

### Component Code Structure

**Home.razor** (conceptual structure):
```cshtml
@page "/"
@attribute [Authorize(Policy = "RequireUserRole")]
@inject IPlayerService PlayerService
@inject ITeamPlayerService TeamPlayerService
@inject IPlayerStatisticService StatisticService
@inject ICurrentUserProvider CurrentUserProvider

<PageTitle>Dashboard</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    @if (_loading)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else if (_dashboardData is not null)
    {
        @* Welcome Section *@
        <MudPaper Elevation="2" Class="pa-4 mb-4">
            <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                <MudIcon Icon="@Icons.Material.Filled.Dashboard" Size="Size.Large" Color="Color.Primary" />
                <MudText Typo="Typo.h4">Dashboard</MudText>
            </MudStack>
            <MudText Typo="Typo.body1" Class="mt-2">
                Welcome, @_userName! Here's your soccer statistics overview.
            </MudText>
        </MudPaper>

        @* Quick Stats Grid *@
        <MudGrid Class="mb-4">
            <MudItem xs="12" sm="6" md="3">
                @* Total Players Card *@
            </MudItem>
            <MudItem xs="12" sm="6" md="3">
                @* Active Teams Card *@
            </MudItem>
            <MudItem xs="12" sm="6" md="3">
                @* Games Played Card *@
            </MudItem>
            <MudItem xs="12" sm="6" md="3">
                @* Total Goals Card *@
            </MudItem>
        </MudGrid>

        @* Two-Column Layout *@
        <MudGrid>
            <MudItem xs="12" md="6">
                @* Recent Activity Component *@
            </MudItem>
            <MudItem xs="12" md="6">
                @* Active Teams Overview Component *@
            </MudItem>
        </MudGrid>

        @* Quick Actions *@
        <MudPaper Elevation="2" Class="pa-4 mt-4">
            <MudText Typo="Typo.h6" Class="mb-3">Quick Actions</MudText>
            <MudStack Row="true" Spacing="2">
                <MudButton Variant="Variant.Filled" Color="Color.Primary"
                           Href="/player-management">View All Players</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary"
                           Href="/player-management/create">Add New Player</MudButton>
            </MudStack>
        </MudPaper>
    }
    else
    {
        <MudAlert Severity="Severity.Warning">Unable to load dashboard data.</MudAlert>
    }
</MudContainer>

@code {
    private bool _loading = true;
    private string _userName = string.Empty;
    private DashboardData? _dashboardData;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;

        // Get current user
        var userId = CurrentUserProvider.GetUserId();
        _userName = CurrentUserProvider.GetUserName() ?? "User";

        // Load dashboard data
        await LoadDashboardDataAsync(userId);

        _loading = false;
    }

    private async Task LoadDashboardDataAsync(string userId)
    {
        // Aggregate data from existing services
        // Calculate totals, get recent games, etc.
    }
}
```

### Database Considerations

**No Database Changes Required** - All data is retrieved from existing tables:
- `Players` table for player count
- `TeamPlayers` table for active team assignments
- `PlayerStatistics` table for game data and aggregations

**Query Performance**:
- Ensure indexes exist on:
  - `TeamPlayers.IsActive`
  - `PlayerStatistics.GameDate`
  - `Players.UserId` (for filtering by owner)
- Consider adding a composite index on `(TeamPlayerId, GameDate)` if not present

### Navigation Changes

**Current Home Page**:
- Route: `/`
- Content: Basic welcome message with authorization views

**New Dashboard**:
- Route: `/` (no change)
- Page Title: "Dashboard" (changed from "Home")
- Content: Comprehensive dashboard with metrics and navigation

### Error Handling

- Display user-friendly error messages if data loading fails
- Graceful degradation if specific metrics unavailable
- Loading states for async operations
- Empty states when no data exists (e.g., "No players yet. Add your first player to get started.")

### Future Enhancements (Out of Scope for MVP)

- Charts and graphs for visual statistics (goals over time, etc.)
- Filterable date ranges (this week, this month, this season)
- Export functionality for statistics
- Comparison view (compare multiple players)
- Notification center for upcoming games or milestones
- Caching for improved performance
- Real-time updates using SignalR

---

## User Experience

### User Stories

**As a Parent**, I want to:
- See a summary of all my children's soccer activities at a glance
- Quickly view recent game performances
- Navigate to detailed player profiles from the dashboard

**As a Coach**, I want to:
- View aggregate statistics for all players on my teams
- Identify top performers quickly
- Access player and team management features efficiently

### User Journey

1. User signs in with Microsoft Entra ID
2. User lands on Dashboard (home page)
3. Dashboard displays personalized welcome with user's name
4. User views quick stats cards showing totals
5. User reviews recent game activity in left panel
6. User sees active team assignments in right panel
7. User clicks "View All Players" to manage player profiles
8. OR User clicks specific player name in recent activity to view details

### Accessibility

- Semantic HTML structure with proper headings (h1 for Dashboard)
- ARIA labels for icon-only buttons
- Sufficient color contrast (MudBlazor default theme compliant)
- Keyboard navigation support
- Screen reader friendly content structure

---

## Testing Strategy

### Unit Tests (Core Layer)

If creating `DashboardService`:
- Test `GetDashboardSummaryAsync` with various data scenarios
- Test aggregation calculations (totals, averages)
- Test empty data handling
- Test user-based filtering
- Test error scenarios (service failures)

**Test Coverage Target**: 85%+

### Integration Tests

- Verify dashboard loads for authenticated users
- Verify authorization policies (User and Admin roles)
- Verify redirect for anonymous users
- Test data accuracy (counts match database)
- Test responsive layout on different screen sizes

### Manual Testing

- [ ] Dashboard displays correctly on desktop (1920x1080)
- [ ] Dashboard displays correctly on tablet (768x1024)
- [ ] Dashboard displays correctly on mobile (375x667)
- [ ] All quick stat cards show accurate counts
- [ ] Recent activity displays last 10 games sorted by date
- [ ] Active teams show correct team names and player counts
- [ ] Quick action buttons navigate to correct pages
- [ ] Loading state displays during data fetch
- [ ] Error state displays if data loading fails
- [ ] Empty state displays when no data exists
- [ ] Performance: Page loads within 2 seconds

---

## Implementation Notes

### Phase 1: Core Dashboard (MVP)
1. Update `Home.razor` with new page title "Dashboard"
2. Add welcome section with user name
3. Implement quick stats cards (4 metrics)
4. Add authorization policy enforcement
5. Style with MudBlazor components

### Phase 2: Activity & Teams
1. Implement recent activity section
2. Implement active teams overview
3. Add quick action buttons
4. Test responsive layout

### Phase 3: Polish & Performance
1. Add loading states
2. Add error handling and empty states
3. Optimize database queries
4. Add unit tests (if new service created)
5. Update documentation

### Recommended Approach

**Use Existing Services (Simpler)**:
- Best for MVP and faster implementation
- Aggregate data directly in `Home.razor` code-behind
- Keep business logic minimal (simple counts and sorting)
- Refactor to dedicated service later if dashboard grows complex

**Create Dashboard Service (More Robust)**:
- Best for long-term maintainability
- Better for complex aggregations and calculations
- Easier to unit test
- Overkill if dashboard remains simple

---

## Dependencies

### Required
- Existing services: `IPlayerService`, `ITeamPlayerService`, `IPlayerStatisticService`
- `ICurrentUserProvider` for user context
- MudBlazor component library (already in use)
- Authorization policies: `RequireUserRole` (already configured)

### Optional
- New `IDashboardService` (if choosing service approach)
- Reusable child components (for cleaner component structure)

---

## Security Considerations

- Dashboard only accessible to authenticated users (User or Admin role)
- User can only see their own players and teams (filtered by UserId)
- No sensitive user information exposed (names only, no emails/phone numbers)
- Authorization policies enforced at page level with `[Authorize]` attribute

---

## Performance Considerations

- Initial load may query multiple tables (Players, TeamPlayers, PlayerStatistics)
- Optimize with:
  - Proper database indexes
  - Efficient LINQ queries using AsNoTracking
  - Limit result sets (recent activity: 10 records, top scorers: 5 records)
- Consider async loading of sections if performance issues arise
- Monitor query execution times in Application Insights

---

## Documentation Updates

- Update `README.md` to reflect Dashboard as the home page
- Add dashboard screenshots to documentation
- Update user guide with dashboard feature description
- Document any new DTOs or services in XML comments

---

## Success Metrics

- Dashboard loads within 2 seconds for typical user (10-20 players)
- User engagement: 80%+ of authenticated sessions start with dashboard view
- Navigation efficiency: Reduced clicks to reach player management (1 click from dashboard)
- User satisfaction: Positive feedback on at-a-glance statistics visibility

---

## Appendix

### Related Specifications
- [ManagePlayers_Feature_Specification.md](ManagePlayers_Feature_Specification.md)
- [PlayerStatistics_Feature_Specification.md](PlayerStatistics_Feature_Specification.md)
- [TeamManagement_Feature_Specification_Concise.md](TeamManagement_Feature_Specification_Concise.md)

### Wireframe Reference
- `docs/wireframes/Dashboard.png` - Visual design reference for dashboard layout

### Technical References
- MudBlazor Documentation: https://mudblazor.com/
- Blazor Authorization: https://learn.microsoft.com/en-us/aspnet/core/blazor/security/
- Entity Framework Core Performance: https://learn.microsoft.com/en-us/ef/core/performance/
