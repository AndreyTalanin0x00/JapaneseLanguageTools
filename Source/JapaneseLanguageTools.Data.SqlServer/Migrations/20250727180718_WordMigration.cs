using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class WordMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // dbo.WordGroup
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "WordGroup",
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
                table.PrimaryKey(name: "PK_WordGroup", columns: x => x.Id);

                table.UniqueConstraint(name: "AK_WordGroup_Caption", columns: x => x.Caption);

                table.CheckConstraint(name: "CK_WordGroup_Caption_NotEmpty", sql: "LEN(TRIM([Caption])) > 0");
                table.CheckConstraint(name: "CK_WordGroup_Comment_NullOrNotEmpty", sql: "[Comment] IS NULL OR LEN(TRIM([Comment])) > 0");
            });

        // DF_WordGroup_CreatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[WordGroup]
            ADD CONSTRAINT [DF_WordGroup_CreatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [CreatedOn];");

        // DF_WordGroup_UpdatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[WordGroup]
            ADD CONSTRAINT [DF_WordGroup_UpdatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "UIX_WordGroup_Caption",
            schema: "dbo",
            table: "WordGroup",
            column: "Caption",
            unique: true);

        // TR_WordGroup_AfterUpdate_SetUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_WordGroup_AfterUpdate_SetUpdatedOn]
            ON [dbo].[WordGroup]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [wordGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[WordGroup] AS [wordGroup]
                INNER JOIN [inserted] AS [updatedWordGroup]
                    ON [updatedWordGroup].[Id] = [wordGroup].[Id];
            END;");

        // dbo.udt_WordGroup
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_WordGroup] AS TABLE
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

        // dbo.Word
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "Word",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                WordGroupId = table.Column<int>(type: "int", nullable: true),
                Characters = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                CharacterTypes = table.Column<int>(type: "int", nullable: false),
                Pronunciation = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                Furigana = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                Okurigana = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                Meaning = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Word", columns: x => x.Id);

                table.ForeignKey(
                    name: "FK_Word_WordGroup_WordGroupId",
                    column: x => x.WordGroupId,
                    principalTable: "WordGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);

                table.CheckConstraint(name: "CK_Word_Characters_NotEmpty", sql: "LEN(TRIM([Characters])) > 0");
                table.CheckConstraint(name: "CK_Word_Pronunciation_NullOrNotEmpty", sql: "[Pronunciation] IS NULL OR LEN(TRIM([Pronunciation])) > 0");
                table.CheckConstraint(name: "CK_Word_Furigana_NullOrNotEmpty", sql: "[Furigana] IS NULL OR LEN(TRIM([Furigana])) > 0");
                table.CheckConstraint(name: "CK_Word_Okurigana_NullOrNotEmpty", sql: "[Okurigana] IS NULL OR LEN(TRIM([Okurigana])) > 0");
                table.CheckConstraint(name: "CK_Word_Meaning_NullOrNotEmpty", sql: "[Meaning] IS NULL OR LEN(TRIM([Meaning])) > 0");
            });

        // DF_Word_CreatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Word]
            ADD CONSTRAINT [DF_Word_CreatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [CreatedOn];");

        // DF_Word_UpdatedOn
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[Word]
            ADD CONSTRAINT [DF_Word_UpdatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "IX_Word_WordGroupId",
            schema: "dbo",
            table: "Word",
            column: "WordGroupId");

        // TR_Word_AfterUpdate_SetUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_Word_AfterUpdate_SetUpdatedOn]
            ON [dbo].[Word]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [word]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[Word] AS [word]
                INNER JOIN [inserted] AS [updatedWord]
                    ON [updatedWord].[Id] = [word].[Id];
            END;");

        // dbo.udt_Word
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Word] AS TABLE
            (
                [Id] INT NOT NULL,
                [WordGroupId] INT NULL,
                [Characters] NVARCHAR(256) NOT NULL,
                [CharacterTypes] INT NOT NULL,
                [Pronunciation] NVARCHAR(512) NULL,
                [Furigana] NVARCHAR(512) NULL,
                [Okurigana] NVARCHAR(512) NULL,
                [Meaning] NVARCHAR(512) NULL,
                [CreatedOn] DATETIMEOFFSET NOT NULL,
                [UpdatedOn] DATETIMEOFFSET NOT NULL
            );");

        // dbo.WordTag
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "WordTag",
            schema: "dbo",
            columns: table => new
            {
                WordId = table.Column<int>(type: "int", nullable: false),
                TagId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WordTag", columns: x => new { x.WordId, x.TagId });

                table.ForeignKey(
                    name: "FK_WordTag_Word_WordId",
                    column: x => x.WordId,
                    principalTable: "Word",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WordTag_Tag_TagId",
                    column: x => x.TagId,
                    principalTable: "Tag",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_WordTag_WordId",
            schema: "dbo",
            table: "WordTag",
            column: "WordId");

        migrationBuilder.CreateIndex(
            name: "IX_WordTag_TagId",
            schema: "dbo",
            table: "WordTag",
            column: "TagId");

        // dbo.udt_WordTag
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_WordTag] AS TABLE
            (
                [WordId] INT NOT NULL,
                [TagId] INT NOT NULL
            );");

        // dbo.WordGroup, dbo.Word, dbo.WordTag
        // Functions & Procedures

        // dbo.ufn_GetWord
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWord] (@WordId INT)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [word].*
                FROM [dbo].[Word] AS [word]
                WHERE [word].[Id] = @WordId
            );");

        // dbo.ufn_GetWords
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWords] (@WordIds [dbo].[udt_Int32UnorderedSet] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [word].*
                FROM [dbo].[Word] AS [word]
                INNER JOIN @WordIds AS [wordId] ON [wordId].[Value] = [word].[Id]
            );");

        // dbo.ufn_GetWordGroup
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWordGroup] (@WordGroupId INT)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [wordGroup].*
                FROM [dbo].[WordGroup] AS [wordGroup]
                WHERE [wordGroup].[Id] = @WordGroupId
            );");

        // dbo.ufn_GetWordGroups
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWordGroups] (@WordGroupIds [dbo].[udt_Int32UnorderedSet] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [wordGroup].*
                FROM [dbo].[WordGroup] AS [wordGroup]
                INNER JOIN @WordGroupIds AS [wordGroupId] ON [wordGroupId].[Value] = [wordGroup].[Id]
            );");

        // dbo.ufn_GetWordGroupByCaption
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWordGroupByCaption] (@WordGroupCaption NVARCHAR(256))
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT TOP (1) [wordGroup].*
                FROM [dbo].[WordGroup] AS [wordGroup]
                WHERE [wordGroup].[Caption] = @WordGroupCaption
            );");

        // dbo.ufn_GetWordGroupsByCaption
        migrationBuilder.Sql(@"
            CREATE FUNCTION [dbo].[ufn_GetWordGroupsByCaption] (@WordGroupCaptions [dbo].[udt_String256UnorderedSet] READONLY)
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT [wordGroup].*
                FROM [dbo].[WordGroup] AS [wordGroup]
                INNER JOIN @WordGroupCaptions AS [wordGroupCaption] ON [wordGroupCaption].[Value] = [wordGroup].[Caption]
            );");

        // dbo.sp_MergeWordTags_Internal
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_MergeWordTags_Internal]
            (
                @WordIds [dbo].[udt_Int32UnorderedSet] READONLY,
                @WordTags [dbo].[udt_WordTag] READONLY
            )
            AS
            BEGIN
                DELETE [wordTag]
                FROM [dbo].[WordTag] AS [wordTag]
                INNER JOIN @WordIds AS [wordId] ON [wordId].[Value] = [wordTag].[WordId];

                INSERT INTO [dbo].[WordTag] ([WordId], [TagId])
                SELECT [wordTag].[WordId], [wordTag].[TagId]
                FROM @WordTags AS [wordTag];
            END;");

        // dbo.sp_AddWords
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_AddWords]
            (
                @Words [dbo].[udt_Word] READONLY,
                @WordTags [dbo].[udt_WordTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                DECLARE @WordIdMap AS TABLE
                (
                    [Id] INT NOT NULL,
                    [PreviousId] INT NOT NULL PRIMARY KEY
                );

                MERGE INTO [dbo].[Word] AS [target]
                USING @Words AS [source]
                ON [target].[Id] = [source].[Id]
                WHEN NOT MATCHED THEN INSERT
                (
                    [WordGroupId],
                    [Characters],
                    [CharacterTypes],
                    [Pronunciation],
                    [Furigana],
                    [Okurigana],
                    [Meaning]
                )
                VALUES
                (
                    [source].[WordGroupId],
                    [source].[Characters],
                    [source].[CharacterTypes],
                    [source].[Pronunciation],
                    [source].[Furigana],
                    [source].[Okurigana],
                    [source].[Meaning]
                )
                OUTPUT [inserted].[Id], [source].[Id]
                INTO @WordIdMap ([Id], [PreviousId]);

                DECLARE @WordIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @WordIds ([Value])
                SELECT COALESCE([wordIdMap].[Id], [word].[Id])
                FROM @Words AS [word]
                LEFT JOIN @WordIdMap AS [wordIdMap] ON [wordIdMap].[PreviousId] = [word].[Id];

                DECLARE @WordTagsUpdated AS [dbo].[udt_WordTag];

                INSERT INTO @WordTagsUpdated
                (
                    [WordId],
                    [TagId]
                )
                SELECT
                    COALESCE([wordIdMap].[Id], [wordTag].[WordId]),
                    [wordTag].[TagId]
                FROM @WordTags AS [wordTag]
                LEFT JOIN @WordIdMap AS [wordIdMap] ON [wordIdMap].[PreviousId] = [wordTag].[WordId];

                EXEC [dbo].[sp_MergeWordTags_Internal] @WordIds, @WordTagsUpdated;

                SELECT [Value] FROM @WordIds;

                COMMIT TRANSACTION;
            END;");

        // dbo.sp_UpdateWords
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateWords]
            (
                @Words [dbo].[udt_Word] READONLY,
                @WordTags [dbo].[udt_WordTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                UPDATE [target]
                SET
                    [target].[Characters] = [source].[Characters],
                    [target].[CharacterTypes] = [source].[CharacterTypes],
                    [target].[Pronunciation] = [source].[Pronunciation],
                    [target].[Furigana] = [source].[Furigana],
                    [target].[Okurigana] = [source].[Okurigana],
                    [target].[Meaning] = [source].[Meaning]
                FROM [dbo].[Word] AS [target]
                INNER JOIN @Words AS [source] ON [source].[Id] = [target].[Id];

                DECLARE @WordIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @WordIds ([Value]) SELECT [word].[Id] FROM @Words AS [word];

                EXEC [dbo].[sp_MergeWordTags_Internal]
                    @WordIds,
                    @WordTags;

                COMMIT TRANSACTION;
            END;");

        // dbo.sp_MergeWords_Internal
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_MergeWords_Internal]
            (
                @WordGroupIds [dbo].[udt_Int32UnorderedSet] READONLY,
                @Words [dbo].[udt_Word] READONLY,
                @WordTags [dbo].[udt_WordTag] READONLY
            )
            AS
            BEGIN
                DECLARE @WordIdMap AS TABLE
                (
                    [Id] INT NOT NULL,
                    [PreviousId] INT NOT NULL PRIMARY KEY
                );

                INSERT INTO @WordIdMap ([Id], [PreviousId])
                SELECT [output].[Id], [output].[PreviousId]
                FROM
                (
                    MERGE INTO [dbo].[Word] AS [target]
                    USING @Words AS [source]
                    ON [target].[Id] = [source].[Id]
                    WHEN MATCHED THEN UPDATE
                    SET
                        [target].[WordGroupId] = [source].[WordGroupId],
                        [target].[Characters] = [source].[Characters],
                        [target].[CharacterTypes] = [source].[CharacterTypes],
                        [target].[Pronunciation] = [source].[Pronunciation],
                        [target].[Furigana] = [source].[Furigana],
                        [target].[Okurigana] = [source].[Okurigana],
                        [target].[Meaning] = [source].[Meaning]
                    WHEN NOT MATCHED THEN INSERT
                    (
                        [WordGroupId],
                        [Characters],
                        [CharacterTypes],
                        [Pronunciation],
                        [Furigana],
                        [Okurigana],
                        [Meaning]
                    )
                    VALUES
                    (
                        [source].[WordGroupId],
                        [source].[Characters],
                        [source].[CharacterTypes],
                        [source].[Pronunciation],
                        [source].[Furigana],
                        [source].[Okurigana],
                        [source].[Meaning]
                    )
                    WHEN NOT MATCHED BY SOURCE
                        AND [target].[WordGroupId] IN (SELECT [wordGroupId].[Value] FROM @WordGroupIds AS [wordGroupId])
                        THEN DELETE
                    OUTPUT
                        [inserted].[Id] AS [Id],
                        [source].[Id] AS [PreviousId],
                        $action AS [Action]
                )
                AS [output]
                WHERE [output].[Action] = 'INSERT';

                DECLARE @WordIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @WordIds ([Value])
                SELECT COALESCE([wordIdMap].[Id], [word].[Id])
                FROM @Words AS [word]
                LEFT JOIN @WordIdMap AS [wordIdMap] ON [wordIdMap].[PreviousId] = [word].[Id];

                DECLARE @WordTagsUpdated AS [dbo].[udt_WordTag];

                INSERT INTO @WordTagsUpdated
                (
                    [WordId],
                    [TagId]
                )
                SELECT
                    COALESCE([wordIdMap].[Id], [wordTag].[WordId]) AS [WordId],
                    [wordTag].[TagId]
                FROM @WordTags AS [wordTag]
                LEFT JOIN @WordIdMap AS [wordIdMap] ON [wordIdMap].[PreviousId] = [wordTag].[WordId];

                EXEC [dbo].[sp_MergeWordTags_Internal]
                    @WordIds,
                    @WordTagsUpdated;

                ;
            END;");

        // dbo.sp_AddWordGroups
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_AddWordGroups]
            (
                @WordGroups [dbo].[udt_WordGroup] READONLY,
                @Words [dbo].[udt_Word] READONLY,
                @WordTags [dbo].[udt_WordTag] READONLY
            )
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRANSACTION;

                DECLARE @WordGroupIdMap AS TABLE
                (
                    [Id] INT NOT NULL,
                    [PreviousId] INT NOT NULL PRIMARY KEY
                );

                MERGE INTO [dbo].[WordGroup] AS [target]
                USING @WordGroups AS [source]
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
                INTO @WordGroupIdMap ([Id], [PreviousId]);

                DECLARE @WordGroupIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @WordGroupIds ([Value])
                SELECT COALESCE([wordGroupIdMap].[Id], [wordGroup].[Id])
                FROM @WordGroups AS [wordGroup]
                LEFT JOIN @WordGroupIdMap AS [wordGroupIdMap] ON [wordGroupIdMap].[PreviousId] = [wordGroup].[Id];

                DECLARE @WordsUpdated AS [dbo].[udt_Word];

                INSERT INTO @WordsUpdated
                (
                    [Id],
                    [WordGroupId],
                    [Characters],
                    [CharacterTypes],
                    [Pronunciation],
                    [Furigana],
                    [Okurigana],
                    [Meaning],
                    [CreatedOn],
                    [UpdatedOn]
                )
                SELECT
                    [word].[Id],
                    COALESCE([wordGroupIdMap].[Id], [word].[WordGroupId]) AS [WordGroupId],
                    [word].[Characters],
                    [word].[CharacterTypes],
                    [word].[Pronunciation],
                    [word].[Furigana],
                    [word].[Okurigana],
                    [word].[Meaning],
                    [word].[CreatedOn],
                    [word].[UpdatedOn]
                FROM @Words AS [word]
                LEFT JOIN @WordGroupIdMap AS [wordGroupIdMap] ON [wordGroupIdMap].[PreviousId] = [word].[WordGroupId];

                EXEC [dbo].[sp_MergeWords_Internal]
                    @WordGroupIds,
                    @WordsUpdated,
                    @WordTags;

                SELECT [Value] FROM @WordGroupIds;

                COMMIT TRANSACTION;
            END;");

        // dbo.sp_UpdateWordGroups
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_UpdateWordGroups]
            (
                @WordGroups [dbo].[udt_WordGroup] READONLY,
                @Words [dbo].[udt_Word] READONLY,
                @WordTags [dbo].[udt_WordTag] READONLY
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
                FROM [dbo].[WordGroup] AS [target]
                INNER JOIN @WordGroups AS [source] ON [source].[Id] = [target].[Id];

                DECLARE @WordGroupIds AS [dbo].[udt_Int32UnorderedSet];

                INSERT INTO @WordGroupIds ([Value]) SELECT [wordGroup].[Id] FROM @WordGroups AS [wordGroup];

                EXEC [dbo].[sp_MergeWords_Internal]
                    @WordGroupIds,
                    @Words,
                    @WordTags;

                COMMIT TRANSACTION;
            END;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // dbo.WordGroup, dbo.Word, dbo.WordTag
        // Functions & Procedures

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWord];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWords];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWordGroup];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWordGroups];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWordGroupByCaption];");

        migrationBuilder.Sql("DROP FUNCTION [dbo].[ufn_GetWordGroupsByCaption];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddWordGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWordGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_MergeWords_Internal];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddWords];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWords];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_MergeWordTags_Internal];");

        // dbo.WordTag
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_WordTag];");

        migrationBuilder.Sql("DROP TABLE [dbo].[WordTag];");

        // dbo.Word
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Word];");

        migrationBuilder.Sql("DROP TABLE [dbo].[Word];");

        // dbo.WordGroup
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_WordGroup];");

        migrationBuilder.Sql("DROP TABLE [dbo].[WordGroup];");
    }
}
