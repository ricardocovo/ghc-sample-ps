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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    CREATE TABLE [Players] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [DateOfBirth] datetime2 NOT NULL,
        [Gender] nvarchar(50) NULL,
        [PhotoUrl] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(450) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(450) NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Players_DateOfBirth] ON [Players] ([DateOfBirth]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Players_Name] ON [Players] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Players_UserId] ON [Players] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-001'', N''Emma Rodriguez'', ''2014-03-15T00:00:00.0000000'', N''Female'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-001'', N''Liam Johnson'', ''2015-07-22T00:00:00.0000000'', N''Male'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-002'', N''Olivia Martinez'', ''2013-11-08T00:00:00.0000000'', N''Female'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-002'', N''Noah Williams'', ''2016-01-30T00:00:00.0000000'', N''Male'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-003'', N''Ava Brown'', ''2014-09-12T00:00:00.0000000'', N''Female'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-003'', N''Ethan Davis'', ''2015-04-05T00:00:00.0000000'', N''Male'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-004'', N''Sophia Garcia'', ''2013-06-18T00:00:00.0000000'', N''Non-binary'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-004'', N''Mason Miller'', ''2016-12-25T00:00:00.0000000'', N''Male'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-005'', N''Isabella Wilson'', ''2014-02-14T00:00:00.0000000'', N''Female'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] ON;
    EXEC(N'INSERT INTO [Players] ([UserId], [Name], [DateOfBirth], [Gender], [PhotoUrl], [CreatedAt], [CreatedBy])
    VALUES (N''user-005'', N''Lucas Anderson'', ''2015-10-09T00:00:00.0000000'', N''Prefer not to say'', NULL, ''2025-01-01T00:00:00.0000000Z'', N''system'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Name', N'DateOfBirth', N'Gender', N'PhotoUrl', N'CreatedAt', N'CreatedBy') AND [object_id] = OBJECT_ID(N'[Players]'))
        SET IDENTITY_INSERT [Players] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251128205245_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251128205245_InitialCreate', N'10.0.0');
END;

COMMIT;
GO

