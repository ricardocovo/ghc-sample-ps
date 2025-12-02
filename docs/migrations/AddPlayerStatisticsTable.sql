-- AddPlayerStatisticsTable Migration Script
-- Migration ID: 20251202032639_AddPlayerStatisticsTable
-- This script is idempotent and safe to run multiple times

-- Check if PlayerStatistics table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PlayerStatistics')
BEGIN
    CREATE TABLE [PlayerStatistics] (
        [PlayerStatisticId] int NOT NULL IDENTITY(1,1),
        [TeamPlayerId] int NOT NULL,
        [GameDate] datetime2 NOT NULL,
        [MinutesPlayed] int NOT NULL,
        [IsStarter] bit NOT NULL,
        [JerseyNumber] int NOT NULL,
        [Goals] int NOT NULL,
        [Assists] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(450) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(450) NULL,
        CONSTRAINT [PK_PlayerStatistics] PRIMARY KEY ([PlayerStatisticId]),
        CONSTRAINT [FK_PlayerStatistics_TeamPlayers_TeamPlayerId] FOREIGN KEY ([TeamPlayerId]) 
            REFERENCES [TeamPlayers] ([TeamPlayerId]) ON DELETE CASCADE
    );
    
    PRINT 'Created PlayerStatistics table';
END
ELSE
BEGIN
    PRINT 'PlayerStatistics table already exists';
END
GO

-- Create index on TeamPlayerId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PlayerStatistics_TeamPlayerId' AND object_id = OBJECT_ID('PlayerStatistics'))
BEGIN
    CREATE INDEX [IX_PlayerStatistics_TeamPlayerId] ON [PlayerStatistics] ([TeamPlayerId]);
    PRINT 'Created index IX_PlayerStatistics_TeamPlayerId';
END
ELSE
BEGIN
    PRINT 'Index IX_PlayerStatistics_TeamPlayerId already exists';
END
GO

-- Create index on GameDate if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PlayerStatistics_GameDate' AND object_id = OBJECT_ID('PlayerStatistics'))
BEGIN
    CREATE INDEX [IX_PlayerStatistics_GameDate] ON [PlayerStatistics] ([GameDate]);
    PRINT 'Created index IX_PlayerStatistics_GameDate';
END
ELSE
BEGIN
    PRINT 'Index IX_PlayerStatistics_GameDate already exists';
END
GO

-- Create composite index on TeamPlayerId and GameDate if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PlayerStatistics_TeamPlayerId_GameDate' AND object_id = OBJECT_ID('PlayerStatistics'))
BEGIN
    CREATE INDEX [IX_PlayerStatistics_TeamPlayerId_GameDate] ON [PlayerStatistics] ([TeamPlayerId], [GameDate]);
    PRINT 'Created index IX_PlayerStatistics_TeamPlayerId_GameDate';
END
ELSE
BEGIN
    PRINT 'Index IX_PlayerStatistics_TeamPlayerId_GameDate already exists';
END
GO

-- Insert migration history record if Entity Framework is tracking migrations
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251202032639_AddPlayerStatisticsTable')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20251202032639_AddPlayerStatisticsTable', '10.0.0');
        PRINT 'Inserted migration history record';
    END
    ELSE
    BEGIN
        PRINT 'Migration history record already exists';
    END
END
GO

PRINT 'AddPlayerStatisticsTable migration completed successfully';
GO
