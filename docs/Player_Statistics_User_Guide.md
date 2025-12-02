# Player Statistics User Guide

This guide explains how to manage game statistics for players in the GhcSamplePs application.

## Overview

Player Statistics allow you to track individual game performance for players assigned to teams. Each statistic record captures:

- **Game Date** - When the game was played
- **Minutes Played** - How long the player was on the field
- **Starter Status** - Whether the player started the game or came off the bench
- **Jersey Number** - The player's number for that game
- **Goals** - Number of goals scored
- **Assists** - Number of assists made

Statistics are always recorded in the context of a specific team assignment (TeamPlayer), so the same player can have statistics tracked separately for different teams/championships.

## Prerequisites

Before adding game statistics, ensure:

1. The player exists in the system
2. The player has at least one active team assignment
3. You have the appropriate permissions to edit player data

## Adding Game Statistics

### Step 1: Navigate to the Player

1. Go to the **Players** page
2. Find the player in the list (use search if needed)
3. Click the **Edit** button for that player

### Step 2: Access the Stats Tab

1. On the Edit Player page, click the **Stats** tab
2. You'll see the statistics table (empty if no statistics exist)
3. You'll also see summary cards showing aggregate statistics

### Step 3: Select the Team Context

If the player is assigned to multiple teams:

1. Use the team selector dropdown at the top of the Stats tab
2. Select the team for which you want to add statistics
3. The table will filter to show only statistics for that team

### Step 4: Add New Game Statistics

1. Click the **Add Game Stats** button
2. A form dialog will appear with the following fields:

| Field | Description | Required | Validation |
|-------|-------------|----------|------------|
| Game Date | The date the game was played | Yes | Cannot be in the future |
| Minutes Played | Total minutes played (0-120) | Yes | Must be 0-120 |
| Started | Check if player started the game | Yes | Boolean (checkbox) |
| Jersey Number | Player's number for this game | Yes | Must be 1-99 |
| Goals | Goals scored in the game | Yes | Must be 0 or more |
| Assists | Assists made in the game | Yes | Must be 0 or more |

3. Fill in all fields
4. Click **Save** to add the statistics

### Common Validation Errors

| Error | Cause | Solution |
|-------|-------|----------|
| "Game date cannot be in the future" | Selected date is after today | Select a past or current date |
| "Minutes played should not exceed 120" | Value greater than 120 | Enter value between 0 and 120 |
| "Jersey number must be greater than 0" | Zero or negative number | Enter value between 1 and 99 |
| "Goals must be non-negative" | Negative number entered | Enter 0 or a positive number |
| "Assists must be non-negative" | Negative number entered | Enter 0 or a positive number |

## Editing Game Statistics

### Step 1: Find the Game Entry

1. Navigate to the player's Stats tab
2. Locate the game in the statistics table
3. Games are sorted by date (most recent first)

### Step 2: Edit the Statistics

1. Click the **Edit** button (pencil icon) for the game entry
2. Modify the values as needed
3. Click **Save** to update the statistics

**Note:** You cannot change the team assignment for an existing statistic. If you need to move a statistic to a different team, delete it and create a new one.

## Deleting Game Statistics

### Step 1: Find the Game Entry

1. Navigate to the player's Stats tab
2. Locate the game entry you want to delete

### Step 2: Delete the Statistics

1. Click the **Delete** button (trash icon) for the game entry
2. A confirmation dialog will appear
3. Click **Confirm** to permanently delete the statistics

**Warning:** This action cannot be undone. The statistics will be permanently removed.

## Understanding Summary Cards

The Stats tab displays summary cards that show aggregate statistics:

### Games Played
- **Formula:** COUNT(all game statistics)
- **Description:** Total number of games where statistics were recorded
- **Example:** If you have 10 game entries, this shows "10"

### Total Goals
- **Formula:** SUM(goals from all games)
- **Description:** Total goals scored across all recorded games
- **Example:** If the player scored 2, 1, 0, 3 goals in 4 games, this shows "6"

### Total Assists
- **Formula:** SUM(assists from all games)
- **Description:** Total assists made across all recorded games
- **Example:** If the player had 1, 2, 1, 0 assists in 4 games, this shows "4"

### Average Goals Per Game
- **Formula:** Total Goals / Games Played
- **Description:** Average goals scored per game
- **Example:** 6 goals in 10 games = 0.60 average
- **Note:** Displays 0 if no games are recorded (avoids division by zero)

### Average Assists Per Game
- **Formula:** Total Assists / Games Played
- **Description:** Average assists made per game
- **Example:** 8 assists in 10 games = 0.80 average
- **Note:** Displays 0 if no games are recorded (avoids division by zero)

### Average Minutes Per Game
- **Formula:** Total Minutes / Games Played
- **Description:** Average playing time per game
- **Example:** 720 minutes in 10 games = 72.00 average

## Team Context and Filtering

### Why Team Context Matters

Statistics are tied to specific team assignments (TeamPlayer records). This allows:

- Tracking statistics separately for each team a player belongs to
- Viewing totals for a specific championship or season
- Maintaining accurate historical records when players change teams

### Filtering by Team

1. Use the team dropdown at the top of the Stats tab
2. Select "All Teams" to see all statistics
3. Select a specific team to filter statistics

When a team is selected:
- Only statistics for that team are shown
- Summary cards reflect only that team's totals
- New statistics are added to the selected team

## Tips for Data Entry

### Best Practices

1. **Enter statistics promptly** - Add game statistics soon after each game to maintain accuracy
2. **Verify jersey numbers** - Players may use different numbers in different competitions
3. **Double-check goals and assists** - Ensure accurate attribution
4. **Use consistent dates** - Always use the actual game date, not when you're entering data

### Handling Special Cases

**Player did not play:**
- If player was on roster but didn't enter the game, you can still record a statistic with:
  - Minutes Played: 0
  - Started: unchecked
  - Goals: 0
  - Assists: 0

**Extra time (overtime):**
- Minutes can exceed 90 for games with extra time
- Maximum allowed is 120 minutes

**Own goals:**
- Own goals are typically not counted in player statistics
- Only record goals scored for the player's team

**Penalty shootouts:**
- Goals scored in penalty shootouts are typically recorded separately
- Follow your league's standard practice

## Understanding the Statistics Table

The statistics table displays:

| Column | Description |
|--------|-------------|
| Date | Game date (formatted as MM/DD/YYYY or your locale) |
| Team | Team name (if viewing all teams) |
| Championship | Championship name (if viewing all teams) |
| Started | "Yes" or "No" indicating starter status |
| Minutes | Minutes played |
| Jersey | Jersey number worn |
| Goals | Goals scored |
| Assists | Assists made |
| Actions | Edit and Delete buttons |

### Sorting and Filtering

- Default sort is by Game Date (most recent first)
- Use the team dropdown to filter by team assignment
- Use search to find specific games (if available)

## Troubleshooting

### Statistics Not Saving

**Possible causes:**
- Validation errors (check for red error messages)
- Network connectivity issues
- Session timeout

**Solutions:**
1. Review all fields for validation errors
2. Check your internet connection
3. Refresh the page and try again
4. If session expired, sign in again

### Wrong Team Showing

**Possible causes:**
- Team filter is applied
- Player has multiple team assignments

**Solutions:**
1. Check the team dropdown selection
2. Select "All Teams" to see all statistics
3. Verify the player is assigned to the expected team

### Missing Statistics

**Possible causes:**
- Statistics recorded under different team
- Statistics deleted
- Filter applied

**Solutions:**
1. Select "All Teams" in the team dropdown
2. Check if statistics exist under a different team assignment
3. Contact an administrator if data appears to be missing

## Related Documentation

- [Team Management User Guide](Team_Management_User_Guide.md) - Managing player team assignments
- [Database Connection Setup](Database_Connection_Setup.md) - Database schema including PlayerStatistics table
- [GhcSamplePs.Core README](../src/GhcSamplePs.Core/README.md) - Technical documentation for the statistics service

---

**Document Version**: 1.0  
**Last Updated**: 2025-12-02  
**Maintained By**: Development Team
