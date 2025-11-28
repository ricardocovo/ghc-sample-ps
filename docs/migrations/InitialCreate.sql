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
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251128205245_InitialCreate', N'10.0.0');
END;

COMMIT;
GO

