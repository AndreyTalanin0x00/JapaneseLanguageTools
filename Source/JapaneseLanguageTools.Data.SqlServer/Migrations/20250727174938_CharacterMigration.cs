using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class CharacterMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // dbo.CharacterGroup
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "CharacterGroup",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Caption = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Comment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                AlwaysUse = table.Column<bool>(type: "bit", nullable: false),
                Hidden = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_CharacterGroup", columns: x => x.Id);

                table.UniqueConstraint(name: "AK_CharacterGroup_Caption", columns: x => x.Caption);

                table.CheckConstraint(name: "CK_CharacterGroup_Caption_NotEmpty", sql: "LEN(TRIM([Caption])) > 0");
                table.CheckConstraint(name: "CK_CharacterGroup_Comment_NullOrNotEmpty", sql: "[Comment] IS NULL OR LEN(TRIM([Comment])) > 0");
            });

        // DF_CharacterGroup_CreatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[CharacterGroup]
            ADD CONSTRAINT [DF_CharacterGroup_CreatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [CreatedOn];");

        // DF_CharacterGroup_UpdatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[CharacterGroup]
            ADD CONSTRAINT [DF_CharacterGroup_UpdatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "UIX_CharacterGroup_Caption",
            schema: "dbo",
            table: "CharacterGroup",
            column: "Caption",
            unique: true);

        // TR_CharacterGroup_AfterUpdate_SetUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_CharacterGroup_AfterUpdate_SetUpdatedOn]
            ON [dbo].[CharacterGroup]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [characterGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[CharacterGroup] AS [characterGroup]
                INNER JOIN [inserted] AS [updatedCharacterGroup]
                    ON [updatedCharacterGroup].[Id] = [characterGroup].[Id];
            END;");

        // dbo.udt_CharacterGroup
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_CharacterGroup] AS TABLE
            (
                [Id] INT NOT NULL,
                [Caption] NVARCHAR(256) NOT NULL,
                [Comment] NVARCHAR(2048) NULL,
                [Enabled] BIT NOT NULL,
                [AlwaysUse] BIT NOT NULL,
                [Hidden] BIT NOT NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        // dbo.Character
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "Character",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CharacterGroupId = table.Column<int>(type: "int", nullable: true),
                Symbol = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Pronunciation = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                Syllable = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                Onyomi = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Kunyomi = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Meaning = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Character", columns: x => x.Id);

                table.ForeignKey(
                    name: "FK_Character_CharacterGroup_CharacterGroupId",
                    column: x => x.CharacterGroupId,
                    principalTable: "CharacterGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);

                table.CheckConstraint(name: "CK_Character_Symbol_NotEmpty", sql: "LEN(TRIM([Symbol])) > 0");
                table.CheckConstraint(name: "CK_Character_Pronunciation_NullOrNotEmpty", sql: "[Pronunciation] IS NULL OR LEN(TRIM([Pronunciation])) > 0");
                table.CheckConstraint(name: "CK_Character_Syllable_NullOrNotEmpty", sql: "[Syllable] IS NULL OR LEN(TRIM([Syllable])) > 0");
                table.CheckConstraint(name: "CK_Character_Onyomi_NullOrNotEmpty", sql: "[Onyomi] IS NULL OR LEN(TRIM([Onyomi])) > 0");
                table.CheckConstraint(name: "CK_Character_Kunyomi_NullOrNotEmpty", sql: "[Kunyomi] IS NULL OR LEN(TRIM([Kunyomi])) > 0");
                table.CheckConstraint(name: "CK_Character_Meaning_NullOrNotEmpty", sql: "[Meaning] IS NULL OR LEN(TRIM([Meaning])) > 0");
            });

        // DF_Character_CreatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Character]
            ADD CONSTRAINT [DF_Character_CreatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [CreatedOn];");

        // DF_Character_UpdatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Character]
            ADD CONSTRAINT [DF_Character_UpdatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "IX_Character_CharacterGroupId",
            schema: "dbo",
            table: "Character",
            column: "CharacterGroupId");

        // TR_Character_AfterUpdate_SetUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_Character_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Character]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [character]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[Character] AS [character]
                INNER JOIN [inserted] AS [updatedCharacter]
                    ON [updatedCharacter].[Id] = [character].[Id];
            END;");

        // dbo.udt_Character
        migrationBuilder.Sql(@"
            CREATE TYPE [udt_Character] AS TABLE
            (
                [Id] INT NOT NULL,
                [CharacterGroupId] INT NULL,
                [Symbol] NVARCHAR(16) NOT NULL,
                [Type] INT NOT NULL,
                [Pronunciation] NVARCHAR(32) NULL,
                [Syllable] NVARCHAR(32) NULL,
                [Onyomi] NVARCHAR(256) NULL,
                [Kunyomi] NVARCHAR(256) NULL,
                [Meaning] NVARCHAR(512) NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        // dbo.CharacterTag
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "CharacterTag",
            schema: "dbo",
            columns: table => new
            {
                CharacterId = table.Column<int>(type: "int", nullable: false),
                TagId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_CharacterTag", columns: x => new { x.CharacterId, x.TagId });

                table.ForeignKey(
                    name: "FK_CharacterTag_Character_CharacterId",
                    column: x => x.CharacterId,
                    principalTable: "Character",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CharacterTag_Tag_TagId",
                    column: x => x.TagId,
                    principalTable: "Tag",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterTag_CharacterId",
            schema: "dbo",
            table: "CharacterTag",
            column: "CharacterId");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterTag_TagId",
            schema: "dbo",
            table: "CharacterTag",
            column: "TagId");

        // dbo.udt_CharacterTag
        migrationBuilder.Sql(@"
            CREATE TYPE [udt_CharacterTag] AS TABLE
            (
                [CharacterId] INT NOT NULL,
                [TagId] INT NOT NULL
            );");

        // dbo.CharacterGroup, dbo.Character, dbo.CharacterTag
        // Functions & Procedures

        // dbo.ufn_GetCharacter
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetCharacter] (@CharacterId INT)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [character].*
                FROM [dbo].[Character] AS [character]
                WHERE [character].[Id] = @CharacterId
            );");

        // dbo.ufn_GetCharacters
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetCharacters] (@CharacterIds [dbo].[udt_Int32UnorderedSet] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [character].*
                FROM [dbo].[Character] AS [character]
                INNER JOIN @CharacterIds AS [characterId] ON [characterId].[Value] = [character].[Id]
            );");

        // dbo.ufn_GetCharacterGroup
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetCharacterGroup] (@CharacterGroupId INT)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [characterGroup].*
                FROM [dbo].[CharacterGroup] AS [characterGroup]
                WHERE [characterGroup].[Id] = @CharacterGroupId
            );");

        // dbo.ufn_GetCharacterGroups
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetCharacterGroups] (@CharacterGroupIds [dbo].[udt_Int32UnorderedSet] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [characterGroup].*
                FROM [dbo].[CharacterGroup] AS [characterGroup]
                INNER JOIN @CharacterGroupIds AS [characterGroupId] ON [characterGroupId].[Value] = [characterGroup].[Id]
            );");

        // dbo.ufn_GetCharacterGroupByCaption
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetCharacterGroupByCaption] (@CharacterGroupCaption NVARCHAR(256))
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [characterGroup].*
                FROM [dbo].[CharacterGroup] AS [characterGroup]
                WHERE [characterGroup].[Caption] = @CharacterGroupCaption
            );");

        // dbo.ufn_GetCharacterGroupsByCaption
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetCharacterGroupsByCaption] (@CharacterGroupCaptions [dbo].[udt_String256UnorderedSet] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [characterGroup].*
                FROM [dbo].[CharacterGroup] AS [characterGroup]
                INNER JOIN @CharacterGroupCaptions AS [characterGroupCaption] ON [characterGroupCaption].[Value] = [characterGroup].[Caption]
            );");

        // dbo.sp_MergeCharacterTags_Internal
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_MergeCharacterTags_Internal]
            (
                @CharacterIds [dbo].[udt_Int32UnorderedSet] READONLY,
                @CharacterTags [dbo].[udt_CharacterTag] READONLY
            )
            AS
            BEGIN
                DELETE [characterTag]
                FROM [dbo].[CharacterTag] AS [characterTag]
                INNER JOIN @CharacterIds AS [characterId] ON [characterId].[Value] = [characterTag].[CharacterId];

                INSERT INTO [dbo].[CharacterTag] ([CharacterId], [TagId])
                SELECT [characterTag].[CharacterId], [characterTag].[TagId]
                FROM @CharacterTags AS [characterTag];
            END;");

        // dbo.sp_AddCharacters
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_AddCharacters]
            (
                @Characters [dbo].[udt_Character] READONLY,
                @CharacterTags [dbo].[udt_CharacterTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                DECLARE @CharacterIdMap AS TABLE
                (
                    [Id] INT NOT NULL,
                    [PreviousId] INT NOT NULL PRIMARY KEY
                );

                MERGE INTO [dbo].[Character] AS [target]
                USING @Characters AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN NOT MATCHED THEN INSERT
                (
                    [CharacterGroupId],
                    [Symbol],
                    [Type],
                    [Pronunciation],
                    [Syllable],
                    [Onyomi],
                    [Kunyomi],
                    [Meaning]
                )
                VALUES
                (
                    [source].[CharacterGroupId],
                    [source].[Symbol],
                    [source].[Type],
                    [source].[Pronunciation],
                    [source].[Syllable],
                    [source].[Onyomi],
                    [source].[Kunyomi],
                    [source].[Meaning]
                )
                OUTPUT [inserted].[Id], [source].[Id]
                INTO @CharacterIdMap ([Id], [PreviousId]);

                DECLARE @CharacterIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @CharacterIds ([Value])
                SELECT COALESCE([characterIdMap].[Id], [character].[Id])
                FROM @Characters AS [character]
                LEFT JOIN @CharacterIdMap AS [characterIdMap] ON [characterIdMap].[PreviousId] = [character].[Id];

                DECLARE @CharacterTagsUpdated AS [dbo].[udt_CharacterTag];

                INSERT INTO @CharacterTagsUpdated
                (
                    [CharacterId],
                    [TagId]
                )
                SELECT
                    COALESCE([characterIdMap].[Id], [characterTag].[CharacterId]),
                    [characterTag].[TagId]
                FROM @CharacterTags AS [characterTag]
                LEFT JOIN @CharacterIdMap AS [characterIdMap] ON [characterIdMap].[PreviousId] = [characterTag].[CharacterId];

                EXEC [dbo].[sp_MergeCharacterTags_Internal] @CharacterIds, @CharacterTagsUpdated;

                SELECT [Value] FROM @CharacterIds;

                COMMIT TRANSACTION;
            END;");

        // dbo.sp_UpdateCharacters
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateCharacters]
            (
                @Characters [dbo].[udt_Character] READONLY,
                @CharacterTags [dbo].[udt_CharacterTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                UPDATE [target]
                SET
                    [target].[Symbol] = [source].[Symbol],
                    [target].[Type] = [source].[Type],
                    [target].[Pronunciation] = [source].[Pronunciation],
                    [target].[Syllable] = [source].[Syllable],
                    [target].[Onyomi] = [source].[Onyomi],
                    [target].[Kunyomi] = [source].[Kunyomi],
                    [target].[Meaning] = [source].[Meaning]
                FROM [dbo].[Character] AS [target]
                INNER JOIN @Characters AS [source] ON [source].[Id] = [target].[Id];

                DECLARE @CharacterIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @CharacterIds ([Value]) SELECT [character].[Id] FROM @Characters AS [character];

                EXEC [dbo].[sp_MergeCharacterTags_Internal]
                    @CharacterIds,
                    @CharacterTags;

                COMMIT TRANSACTION;
            END;");

        // dbo.sp_MergeCharacters_Internal
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_MergeCharacters_Internal]
            (
                @CharacterGroupIds [dbo].[udt_Int32UnorderedSet] READONLY,
                @Characters [dbo].[udt_Character] READONLY,
                @CharacterTags [dbo].[udt_CharacterTag] READONLY
            )
            AS
            BEGIN
                DECLARE @CharacterIdMap AS TABLE
                (
                    [Id] INT NOT NULL,
                    [PreviousId] INT NOT NULL PRIMARY KEY
                );

                INSERT INTO @CharacterIdMap ([Id], [PreviousId])
                SELECT [output].[Id], [output].[PreviousId]
                FROM
                (
                    MERGE INTO [dbo].[Character] AS [target]
                    USING @Characters AS [source]
                    ON [target].[Id] = [source].[Id]
                    WHEN MATCHED THEN UPDATE
                    SET
                        [target].[CharacterGroupId] = [source].[CharacterGroupId],
                        [target].[Symbol] = [source].[Symbol],
                        [target].[Type] = [source].[Type],
                        [target].[Pronunciation] = [source].[Pronunciation],
                        [target].[Syllable] = [source].[Syllable],
                        [target].[Onyomi] = [source].[Onyomi],
                        [target].[Kunyomi] = [source].[Kunyomi],
                        [target].[Meaning] = [source].[Meaning]
                    WHEN NOT MATCHED THEN INSERT
                    (
                        [CharacterGroupId],
                        [Symbol],
                        [Type],
                        [Pronunciation],
                        [Syllable],
                        [Onyomi],
                        [Kunyomi],
                        [Meaning]
                    )
                    VALUES
                    (
                        [source].[CharacterGroupId],
                        [source].[Symbol],
                        [source].[Type],
                        [source].[Pronunciation],
                        [source].[Syllable],
                        [source].[Onyomi],
                        [source].[Kunyomi],
                        [source].[Meaning]
                    )
                    WHEN NOT MATCHED BY SOURCE
                        AND [target].[CharacterGroupId] IN (SELECT [characterGroupId].[Value] FROM @CharacterGroupIds AS [characterGroupId])
                        THEN DELETE
                    OUTPUT
                        [inserted].[Id] AS [Id],
                        [source].[Id] AS [PreviousId],
                        $action AS [Action]
                )
                AS [output]
                WHERE [output].[Action] = 'INSERT';

                DECLARE @CharacterIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @CharacterIds ([Value])
                SELECT COALESCE([characterIdMap].[Id], [character].[Id])
                FROM @Characters AS [character]
                LEFT JOIN @CharacterIdMap AS [characterIdMap] ON [characterIdMap].[PreviousId] = [character].[Id];

                DECLARE @CharacterTagsUpdated AS [dbo].[udt_CharacterTag];

                INSERT INTO @CharacterTagsUpdated
                (
                    [CharacterId],
                    [TagId]
                )
                SELECT
                    COALESCE([characterIdMap].[Id], [characterTag].[CharacterId]) AS [CharacterId],
                    [characterTag].[TagId]
                FROM @CharacterTags AS [characterTag]
                LEFT JOIN @CharacterIdMap AS [characterIdMap] ON [characterIdMap].[PreviousId] = [characterTag].[CharacterId];

                EXEC [dbo].[sp_MergeCharacterTags_Internal]
                    @CharacterIds,
                    @CharacterTagsUpdated;

                ;
            END;");

        // dbo.sp_AddCharacterGroups
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_AddCharacterGroups]
            (
                @CharacterGroups [dbo].[udt_CharacterGroup] READONLY,
                @Characters [dbo].[udt_Character] READONLY,
                @CharacterTags [dbo].[udt_CharacterTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                DECLARE @CharacterGroupIdMap AS TABLE
                (
                    [Id] INT NOT NULL,
                    [PreviousId] INT NOT NULL PRIMARY KEY
                );

                MERGE INTO [dbo].[CharacterGroup] AS [target]
                USING @CharacterGroups AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN NOT MATCHED THEN INSERT
                (
                    [Caption],
                    [Comment],
                    [Enabled],
                    [AlwaysUse],
                    [Hidden],
                    [CreatedOn],
                    [UpdatedOn]
                )
                VALUES
                (
                    [source].[Caption],
                    [source].[Comment],
                    [source].[Enabled],
                    [source].[AlwaysUse],
                    [source].[Hidden],
                    [source].[CreatedOn],
                    [source].[UpdatedOn]
                )
                OUTPUT [inserted].[Id], [source].[Id]
                INTO @CharacterGroupIdMap ([Id], [PreviousId]);

                DECLARE @CharacterGroupIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @CharacterGroupIds ([Value])
                SELECT COALESCE([characterGroupIdMap].[Id], [characterGroup].[Id])
                FROM @CharacterGroups AS [characterGroup]
                LEFT JOIN @CharacterGroupIdMap AS [characterGroupIdMap] ON [characterGroupIdMap].[PreviousId] = [characterGroup].[Id];

                DECLARE @CharactersUpdated AS [dbo].[udt_Character];

                INSERT INTO @CharactersUpdated
                (
                    [Id],
                    [CharacterGroupId],
                    [Symbol],
                    [Type],
                    [Pronunciation],
                    [Syllable],
                    [Onyomi],
                    [Kunyomi],
                    [Meaning],
                    [CreatedOn],
                    [UpdatedOn]
                )
                SELECT
                    [character].[Id],
                    COALESCE([characterGroupIdMap].[Id], [character].[CharacterGroupId]) AS [CharacterGroupId],
                    [character].[Symbol],
                    [character].[Type],
                    [character].[Pronunciation],
                    [character].[Syllable],
                    [character].[Onyomi],
                    [character].[Kunyomi],
                    [character].[Meaning],
                    [character].[CreatedOn],
                    [character].[UpdatedOn]
                FROM @Characters AS [character]
                LEFT JOIN @CharacterGroupIdMap AS [characterGroupIdMap] ON [characterGroupIdMap].[PreviousId] = [character].[CharacterGroupId];

                EXEC [dbo].[sp_MergeCharacters_Internal]
                    @CharacterGroupIds,
                    @CharactersUpdated,
                    @CharacterTags;

                SELECT [Value] FROM @CharacterGroupIds;

                COMMIT TRANSACTION;
            END;");

        // dbo.sp_UpdateCharacterGroups
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateCharacterGroups]
            (
                @CharacterGroups [dbo].[udt_CharacterGroup] READONLY,
                @Characters [dbo].[udt_Character] READONLY,
                @CharacterTags [dbo].[udt_CharacterTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                UPDATE [target]
                SET
                    [target].[Caption] = [source].[Caption],
                    [target].[Comment] = [source].[Comment],
                    [target].[Enabled] = [source].[Enabled],
                    [target].[AlwaysUse] = [source].[AlwaysUse],
                    [target].[Hidden] = [source].[Hidden]
                FROM [dbo].[CharacterGroup] AS [target]
                INNER JOIN @CharacterGroups AS [source] ON [source].[Id] = [target].[Id];

                DECLARE @CharacterGroupIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @CharacterGroupIds ([Value]) SELECT [characterGroup].[Id] FROM @CharacterGroups AS [characterGroup];

                EXEC [dbo].[sp_MergeCharacters_Internal]
                    @CharacterGroupIds,
                    @Characters,
                    @CharacterTags;

                COMMIT TRANSACTION;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // dbo.CharacterGroup, dbo.Character, dbo.CharacterTag
        // Functions & Procedures

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetCharacter];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetCharacters];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetCharacterGroup];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetCharacterGroups];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetCharacterGroupByCaption];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetCharacterGroupsByCaption];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddCharacterGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateCharacterGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_MergeCharacters_Internal];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddCharacters];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateCharacters];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_MergeCharacterTags_Internal];");

        // dbo.CharacterTag
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_CharacterTag];");

        migrationBuilder.Sql("DROP TABLE [dbo].[CharacterTag];");

        // dbo.Character
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Character];");

        migrationBuilder.Sql("DROP TABLE [dbo].[Character];");

        // dbo.CharacterGroup
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_CharacterGroup];");

        migrationBuilder.Sql("DROP TABLE [dbo].[CharacterGroup];");
    }
}
