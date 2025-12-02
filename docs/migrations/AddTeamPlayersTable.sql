-- AddTeamPlayersTable Migration Script
-- Migration: 20251201203656_AddTeamPlayersTable
-- Purpose: Creates the TeamPlayers table for tracking player team assignments
-- 
-- This script is idempotent and can be run multiple times safely.
-- It checks for existing migration history before applying changes.

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;

-- Check if migration has already been applied
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251201203656_AddTeamPlayersTable'
)
BEGIN
    -- Create TeamPlayers table
    CREATE TABLE [TeamPlayers] (
        [TeamPlayerId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [TeamName] nvarchar(200) NOT NULL,
        [ChampionshipName] nvarchar(200) NOT NULL,
        [JoinedDate] datetime2 NOT NULL,
        [LeftDate] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(450) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(450) NULL,
        CONSTRAINT [PK_TeamPlayers] PRIMARY KEY ([TeamPlayerId]),
        CONSTRAINT [FK_TeamPlayers_Players_PlayerId] FOREIGN KEY ([PlayerId]) 
            REFERENCES [Players] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Created TeamPlayers table';

    -- Create index for filtering by LeftDate (IsActive)
    CREATE INDEX [IX_TeamPlayers_IsActive] ON [TeamPlayers] ([LeftDate]);
    PRINT 'Created index IX_TeamPlayers_IsActive';

    -- Create index for filtering by PlayerId
    CREATE INDEX [IX_TeamPlayers_PlayerId] ON [TeamPlayers] ([PlayerId]);
    PRINT 'Created index IX_TeamPlayers_PlayerId';

    -- Create composite index for active player teams
    CREATE INDEX [IX_TeamPlayers_PlayerId_IsActive] ON [TeamPlayers] ([PlayerId], [LeftDate]);
    PRINT 'Created index IX_TeamPlayers_PlayerId_IsActive';

    -- Create composite index for duplicate detection
    CREATE INDEX [IX_TeamPlayers_PlayerId_TeamName_ChampionshipName] ON [TeamPlayers] 
        ([PlayerId], [TeamName], [ChampionshipName]);
    PRINT 'Created index IX_TeamPlayers_PlayerId_TeamName_ChampionshipName';

    -- Create index for team-based queries
    CREATE INDEX [IX_TeamPlayers_TeamName] ON [TeamPlayers] ([TeamName]);
    PRINT 'Created index IX_TeamPlayers_TeamName';

    -- Record migration in history
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251201203656_AddTeamPlayersTable', N'10.0.0');
    PRINT 'Recorded migration in history';
END;

COMMIT;
GO

PRINT 'AddTeamPlayersTable migration completed successfully';
GO
