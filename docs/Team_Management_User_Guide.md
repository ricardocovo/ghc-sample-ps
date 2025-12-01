# Team Management User Guide

This guide explains how to manage team assignments for players in the GhcSamplePs application.

## Overview

Team Management allows you to:
- Assign players to teams for specific championships
- Track when players join and leave teams
- View active and inactive team memberships
- Maintain a complete history of team assignments

## Understanding Team Assignments

### Key Concepts

| Term | Description |
|------|-------------|
| **Team Assignment** | A record linking a player to a team for a specific championship |
| **Active Assignment** | Player is currently on the team (no left date set) |
| **Inactive Assignment** | Player has left the team (left date is set) |
| **Championship** | The competition/league context for the team membership |

### Business Rules

1. **No Duplicate Active Assignments**: A player cannot have two active assignments to the same team in the same championship
2. **Left Date Must Be After Joined Date**: When marking a player as left, the date must be after when they joined
3. **Left Date Cannot Be In Future**: You cannot set a future left date
4. **Joined Date Limit**: Joined date cannot be more than 1 year in the future
5. **Required Fields**: Team Name, Championship Name, and Joined Date are always required

## How To

### Add a Team Assignment

> **Note**: UI implementation is planned for a future release. Currently, team assignments are managed through the API.

**When UI is available:**

1. Navigate to the **Players** page
2. Click on a player to open the **Edit Player** page
3. Select the **Teams** tab
4. Click the **Add Team** button
5. Fill in the form:
   - **Team Name** (required): Enter the team name (max 200 characters)
   - **Championship Name** (required): Enter the championship/league name (max 200 characters)
   - **Joined Date** (required): Select when the player joined the team
6. Click **Save**

**Validation errors will appear if:**
- Team Name is empty or exceeds 200 characters
- Championship Name is empty or exceeds 200 characters
- Joined Date is not provided or is more than 1 year in the future
- Player already has an active assignment to the same team/championship

### View Team Assignments

**When UI is available:**

1. Navigate to the **Players** page
2. Click on a player to open the **Edit Player** page
3. Select the **Teams** tab
4. View the list of team assignments showing:
   - Team Name
   - Championship Name
   - Joined Date
   - Left Date (if applicable)
   - Status (Active/Inactive)

### Edit a Team Assignment

**When UI is available:**

1. Navigate to the player's **Teams** tab
2. Click on the team assignment to edit
3. You can modify:
   - Left Date (to mark player as having left the team)
4. Click **Save**

> **Note**: Team Name, Championship Name, and Joined Date cannot be edited after creation. If these need to change, delete the assignment and create a new one.

### Mark Player as Left Team

**When UI is available:**

1. Navigate to the player's **Teams** tab
2. Find the active team assignment
3. Click **Mark as Left** or edit the assignment
4. Enter the **Left Date**:
   - Must be after the Joined Date
   - Cannot be in the future
5. Click **Save**

The assignment status will change from **Active** to **Inactive**.

### Understanding Active vs Inactive Status

| Status | Left Date | Description |
|--------|-----------|-------------|
| **Active** | Not set (null) | Player is currently on the team |
| **Inactive** | Has a date | Player has left the team |

**Key points:**
- A player can have multiple inactive assignments to the same team/championship (different time periods)
- A player can only have ONE active assignment per team/championship combination
- When viewing team assignments, you can filter by active/inactive status

## Tips for Data Entry

### Team Names
- Use consistent naming (e.g., "Team Alpha" not "team alpha" or "TEAM ALPHA")
- Keep names descriptive but concise
- Consider using official team names from championships

### Championship Names
- Use the official championship/league name
- Include year or season if applicable (e.g., "Premier League 2024")
- Be consistent across all players in the same championship

### Dates
- Use the actual date the player joined, not the season start date (unless they joined at season start)
- For left dates, use the actual last day the player was part of the team
- All dates are stored in UTC

### Handling Transfers
If a player:
1. **Transfers to a new team**: Mark them as left on the old team, then create a new assignment for the new team
2. **Returns to a previous team**: Create a new assignment (they can have multiple assignments to the same team at different times)
3. **Is on loan**: Create a new assignment for the loan team; when loan ends, mark left and potentially reactivate/create assignment for original team

## Audit Trail

Every team assignment maintains a complete audit trail:

| Field | Description |
|-------|-------------|
| CreatedAt | When the assignment was created |
| CreatedBy | User who created the assignment |
| UpdatedAt | When the assignment was last modified |
| UpdatedBy | User who last modified the assignment |

This helps track who made changes and when.

## Troubleshooting

### "Player already has an active assignment to this team and championship"
**Cause**: Trying to add a duplicate active assignment.
**Solution**: Either mark the existing assignment as inactive (with a left date) or use a different team/championship combination.

### "Left date must be after the joined date"
**Cause**: Entered a left date that is before or equal to the joined date.
**Solution**: Enter a left date that is after the joined date.

### "Left date cannot be in the future"
**Cause**: Entered a future date for the left date.
**Solution**: Use today's date or earlier.

### "Joined date cannot be more than 1 year in the future"
**Cause**: Entered a joined date too far in the future.
**Solution**: Use a date within 1 year from today.

### "Team name is required" / "Championship name is required"
**Cause**: Required field was left empty or contains only whitespace.
**Solution**: Enter a valid name (1-200 characters).

## Related Documentation

- [Database Connection Setup](Database_Connection_Setup.md) - Database schema and migration information
- [GhcSamplePs.Core README](../src/GhcSamplePs.Core/README.md) - Technical documentation for team player entities and services
- [GhcSamplePs.Web README](../src/GhcSamplePs.Web/README.md) - UI component documentation

---

**Document Version**: 1.0  
**Last Updated**: 2025-12-01  
**Maintained By**: Development Team
