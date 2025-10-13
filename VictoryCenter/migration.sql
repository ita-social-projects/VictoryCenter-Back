IF OBJECT_ID(N'[entity_framework].[__EFMigrationsHistory]') IS NULL
BEGIN
    IF SCHEMA_ID(N'entity_framework') IS NULL EXEC(N'CREATE SCHEMA [entity_framework];');
    CREATE TABLE [entity_framework].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [test_entities] (
    [Id] int NOT NULL IDENTITY,
    CONSTRAINT [PK_test_entities] PRIMARY KEY ([Id])
);

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250523215616_InitialMigration', N'9.0.5');

ALTER TABLE [test_entities] ADD [TestName] nvarchar(100) NOT NULL DEFAULT N'';

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250523220104_UpdatedTestEntity', N'9.0.5');

DROP TABLE [test_entities];

CREATE TABLE [Admins] (
    [Id] bigint NOT NULL IDENTITY,
    [UserName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY ([Id])
);

CREATE TABLE [Categories] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

CREATE TABLE [TeamMembers] (
    [Id] bigint NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NULL,
    [CategoryId] bigint NOT NULL,
    [Priority] bigint NOT NULL,
    [Status] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [Photo] varbinary(max) NULL,
    [Email] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TeamMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamMembers_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_TeamMembers_CategoryId_Priority] ON [TeamMembers] ([CategoryId], [Priority]);

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250602143753_AddBasicTables', N'9.0.5');

CREATE TABLE [test_entities] (
    [Id] int NOT NULL IDENTITY,
    [TestName] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_test_entities] PRIMARY KEY ([Id])
);

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250610074421_AddedMissedMigration', N'9.0.5');

ALTER TABLE [Admins] DROP CONSTRAINT [PK_Admins];

EXEC sp_rename N'[Admins]', N'AspNetUsers', 'OBJECT';

EXEC sp_rename N'[AspNetUsers].[Password]', N'RefreshToken', 'COLUMN';

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'UserName');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [UserName] nvarchar(256) NULL;

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Id');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Id] int NOT NULL;

ALTER TABLE [AspNetUsers] ADD [AccessFailedCount] int NOT NULL DEFAULT 0;

ALTER TABLE [AspNetUsers] ADD [ConcurrencyStamp] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [Email] nvarchar(256) NULL;

ALTER TABLE [AspNetUsers] ADD [EmailConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [AspNetUsers] ADD [LockoutEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [AspNetUsers] ADD [LockoutEnd] datetimeoffset NULL;

ALTER TABLE [AspNetUsers] ADD [NormalizedEmail] nvarchar(256) NULL;

ALTER TABLE [AspNetUsers] ADD [NormalizedUserName] nvarchar(256) NULL;

ALTER TABLE [AspNetUsers] ADD [PasswordHash] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [PhoneNumber] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [PhoneNumberConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [AspNetUsers] ADD [RefreshTokenValidTo] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [AspNetUsers] ADD [SecurityStamp] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [TwoFactorEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [AspNetUsers] ADD CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]);

CREATE TABLE [AspNetRoles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250624082822_ChangeAdminModelToIdentity', N'9.0.5');

DROP TABLE [test_entities];

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250624154027_RemovedTestEntity', N'9.0.5');

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamMembers]') AND [c].[name] = N'FirstName');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [TeamMembers] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [TeamMembers] DROP COLUMN [FirstName];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamMembers]') AND [c].[name] = N'MiddleName');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [TeamMembers] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [TeamMembers] DROP COLUMN [MiddleName];

EXEC sp_rename N'[TeamMembers].[LastName]', N'FullName', 'COLUMN';

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250628121148_UpdateTeamMemberToUseFullNameField', N'9.0.5');

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'RefreshToken');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [RefreshToken] nvarchar(max) NULL;

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250701083016_UpdateRefreshToken', N'9.0.5');

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamMembers]') AND [c].[name] = N'Photo');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [TeamMembers] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [TeamMembers] DROP COLUMN [Photo];

IF SCHEMA_ID(N'media') IS NULL EXEC(N'CREATE SCHEMA [media];');

ALTER TABLE [TeamMembers] ADD [ImageId] bigint NULL;

CREATE TABLE [media].[Images] (
    [Id] bigint NOT NULL IDENTITY,
    [BlobName] nvarchar(100) NOT NULL,
    [MimeType] nvarchar(10) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Images] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_TeamMembers_ImageId] ON [TeamMembers] ([ImageId]) WHERE [ImageId] IS NOT NULL;

ALTER TABLE [TeamMembers] ADD CONSTRAINT [FK_TeamMembers_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [media].[Images] ([Id]);

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250716132427_AddImageEntity', N'9.0.5');

CREATE TABLE [ProgramCategories] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ProgramCategories] PRIMARY KEY ([Id])
);

CREATE TABLE [Programs] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [ImageId] bigint NULL,
    CONSTRAINT [PK_Programs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Programs_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [media].[Images] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [ProgramProgramCategories] (
    [CategoriesId] bigint NOT NULL,
    [ProgramsId] bigint NOT NULL,
    CONSTRAINT [PK_ProgramProgramCategories] PRIMARY KEY ([CategoriesId], [ProgramsId]),
    CONSTRAINT [FK_ProgramProgramCategories_ProgramCategories_CategoriesId] FOREIGN KEY ([CategoriesId]) REFERENCES [ProgramCategories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProgramProgramCategories_Programs_ProgramsId] FOREIGN KEY ([ProgramsId]) REFERENCES [Programs] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ProgramProgramCategories_ProgramsId] ON [ProgramProgramCategories] ([ProgramsId]);

CREATE UNIQUE INDEX [IX_Programs_ImageId] ON [Programs] ([ImageId]) WHERE [ImageId] IS NOT NULL;

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250730154237_AddedProgramsAndProgramCategories', N'9.0.5');

ALTER TABLE [ProgramProgramCategories] DROP CONSTRAINT [FK_ProgramProgramCategories_ProgramCategories_CategoriesId];

ALTER TABLE [ProgramProgramCategories] DROP CONSTRAINT [FK_ProgramProgramCategories_Programs_ProgramsId];

ALTER TABLE [ProgramProgramCategories] DROP CONSTRAINT [PK_ProgramProgramCategories];

EXEC sp_rename N'[ProgramProgramCategories]', N'ProgramsProgramCategories', 'OBJECT';

EXEC sp_rename N'[ProgramsProgramCategories].[IX_ProgramProgramCategories_ProgramsId]', N'IX_ProgramsProgramCategories_ProgramsId', 'INDEX';

ALTER TABLE [ProgramsProgramCategories] ADD CONSTRAINT [PK_ProgramsProgramCategories] PRIMARY KEY ([CategoriesId], [ProgramsId]);

ALTER TABLE [ProgramsProgramCategories] ADD CONSTRAINT [FK_ProgramsProgramCategories_ProgramCategories_CategoriesId] FOREIGN KEY ([CategoriesId]) REFERENCES [ProgramCategories] ([Id]) ON DELETE CASCADE;

ALTER TABLE [ProgramsProgramCategories] ADD CONSTRAINT [FK_ProgramsProgramCategories_Programs_ProgramsId] FOREIGN KEY ([ProgramsId]) REFERENCES [Programs] ([Id]) ON DELETE CASCADE;

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250827213904_RenameTable', N'9.0.5');

ALTER TABLE [TeamMembers] DROP CONSTRAINT [FK_TeamMembers_Categories_CategoryId];

DROP TABLE [Categories];

CREATE TABLE [TeamCategories] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TeamCategories] PRIMARY KEY ([Id])
);

ALTER TABLE [TeamMembers] ADD CONSTRAINT [FK_TeamMembers_TeamCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [TeamCategories] ([Id]) ON DELETE NO ACTION;

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250920101338_changeCategoryToTeamCategory', N'9.0.5');

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamMembers]') AND [c].[name] = N'CreatedAt');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [TeamMembers] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [TeamMembers] ALTER COLUMN [CreatedAt] datetimeoffset NOT NULL;

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Programs]') AND [c].[name] = N'CreatedAt');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Programs] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Programs] ALTER COLUMN [CreatedAt] datetimeoffset NOT NULL;

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProgramCategories]') AND [c].[name] = N'CreatedAt');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ProgramCategories] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [ProgramCategories] ALTER COLUMN [CreatedAt] datetimeoffset NOT NULL;

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[media].[Images]') AND [c].[name] = N'CreatedAt');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [media].[Images] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [media].[Images] ALTER COLUMN [CreatedAt] datetimeoffset NOT NULL;

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamCategories]') AND [c].[name] = N'CreatedAt');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [TeamCategories] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [TeamCategories] ALTER COLUMN [CreatedAt] datetimeoffset NOT NULL;

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'RefreshTokenValidTo');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [RefreshTokenValidTo] datetimeoffset NULL;

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'CreatedAt');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [CreatedAt] datetimeoffset NOT NULL;

CREATE TABLE [FaqQuestions] (
    [Id] bigint NOT NULL IDENTITY,
    [QuestionText] nvarchar(max) NOT NULL,
    [AnswerText] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FaqQuestions] PRIMARY KEY ([Id])
);

CREATE TABLE [VisitorPages] (
    [Id] bigint NOT NULL IDENTITY,
    [Slug] nvarchar(450) NOT NULL,
    [Title] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_VisitorPages] PRIMARY KEY ([Id])
);

CREATE TABLE [FaqPlacements] (
    [PageId] bigint NOT NULL,
    [QuestionId] bigint NOT NULL,
    [Priority] bigint NOT NULL,
    CONSTRAINT [PK_FaqPlacements] PRIMARY KEY ([PageId], [QuestionId]),
    CONSTRAINT [FK_FaqPlacements_FaqQuestions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [FaqQuestions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FaqPlacements_VisitorPages_PageId] FOREIGN KEY ([PageId]) REFERENCES [VisitorPages] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_FaqPlacements_PageId_Priority] ON [FaqPlacements] ([PageId], [Priority]);

CREATE INDEX [IX_FaqPlacements_QuestionId] ON [FaqPlacements] ([QuestionId]);

CREATE UNIQUE INDEX [IX_VisitorPages_Slug] ON [VisitorPages] ([Slug]);

CREATE UNIQUE INDEX [IX_VisitorPages_Title] ON [VisitorPages] ([Title]);

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250925134209_AddedFaq', N'9.0.5');

ALTER TABLE [Programs] DROP CONSTRAINT [FK_Programs_Images_ImageId];

ALTER TABLE [ProgramsProgramCategories] DROP CONSTRAINT [FK_ProgramsProgramCategories_ProgramCategories_CategoriesId];

ALTER TABLE [ProgramsProgramCategories] DROP CONSTRAINT [FK_ProgramsProgramCategories_Programs_ProgramsId];

ALTER TABLE [Programs] DROP CONSTRAINT [PK_Programs];

ALTER TABLE [ProgramCategories] DROP CONSTRAINT [PK_ProgramCategories];

EXEC sp_rename N'[Programs]', N'HypotherapyPrograms', 'OBJECT';

EXEC sp_rename N'[ProgramCategories]', N'HypotherapyProgramCategories', 'OBJECT';

EXEC sp_rename N'[HypotherapyPrograms].[IX_Programs_ImageId]', N'IX_HypotherapyPrograms_ImageId', 'INDEX';

ALTER TABLE [HypotherapyPrograms] ADD CONSTRAINT [PK_HypotherapyPrograms] PRIMARY KEY ([Id]);

ALTER TABLE [HypotherapyProgramCategories] ADD CONSTRAINT [PK_HypotherapyProgramCategories] PRIMARY KEY ([Id]);

ALTER TABLE [HypotherapyPrograms] ADD CONSTRAINT [FK_HypotherapyPrograms_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [media].[Images] ([Id]) ON DELETE SET NULL;

ALTER TABLE [ProgramsProgramCategories] ADD CONSTRAINT [FK_ProgramsProgramCategories_HypotherapyProgramCategories_CategoriesId] FOREIGN KEY ([CategoriesId]) REFERENCES [HypotherapyProgramCategories] ([Id]) ON DELETE CASCADE;

ALTER TABLE [ProgramsProgramCategories] ADD CONSTRAINT [FK_ProgramsProgramCategories_HypotherapyPrograms_ProgramsId] FOREIGN KEY ([ProgramsId]) REFERENCES [HypotherapyPrograms] ([Id]) ON DELETE CASCADE;

INSERT INTO [entity_framework].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251008092103_changeProgramCategoryToHypotherapyProgramCategory', N'9.0.5');

COMMIT;
GO

