using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class WordGroupNestingMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // dbo.WordGroupHierarchyRecord
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "WordGroupHierarchyRecord",
            schema: "dbo",
            columns: table => new
            {
                WordGroupId = table.Column<int>(type: "int", nullable: false),
                NestedWordGroupId = table.Column<int>(type: "int", nullable: false),
                PreventRecursiveIncludes = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WordGroupHierarchyRecord", columns: x => new { x.WordGroupId, x.NestedWordGroupId });

                table.ForeignKey(
                    name: "FK_WordGroupHierarchyRecord_WordGroup_WordGroupId",
                    column: x => x.WordGroupId,
                    principalSchema: "dbo",
                    principalTable: "WordGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WordGroupHierarchyRecord_WordGroup_NestedWordGroupId",
                    column: x => x.NestedWordGroupId,
                    principalSchema: "dbo",
                    principalTable: "WordGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        // DF_WordGroupHierarchyRecord_PreventRecursiveIncludes
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[WordGroupHierarchyRecord]
            ADD CONSTRAINT [DF_WordGroupHierarchyRecord_PreventRecursiveIncludes] DEFAULT 0 FOR [PreventRecursiveIncludes];");

        migrationBuilder.CreateIndex(
            name: "IX_WordGroupHierarchyRecord_WordGroupId",
            schema: "dbo",
            table: "WordGroupHierarchyRecord",
            column: "WordGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_WordGroupHierarchyRecord_NestedWordGroupId",
            schema: "dbo",
            table: "WordGroupHierarchyRecord",
            column: "NestedWordGroupId");

        // TR_WordGroupHierarchyRecord_AfterInsert_SetAggregateRootUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_WordGroupHierarchyRecord_AfterInsert_SetAggregateRootUpdatedOn]
            ON [dbo].[WordGroupHierarchyRecord]
            AFTER INSERT
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [wordGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[WordGroupHierarchyRecord] AS [wordGroupHierarchyRecord]
                INNER JOIN [dbo].[WordGroup] AS [wordGroup] ON [wordGroup].[Id] = [wordGroupHierarchyRecord].[WordGroupId]
                INNER JOIN [inserted] AS [insertedWordGroupHierarchyRecord]
                    ON [insertedWordGroupHierarchyRecord].[WordGroupId] = [wordGroupHierarchyRecord].[WordGroupId]
                        AND [insertedWordGroupHierarchyRecord].[NestedWordGroupId] = [wordGroupHierarchyRecord].[NestedWordGroupId];
            END;");

        // TR_WordGroupHierarchyRecord_AfterUpdate_SetAggregateRootUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_WordGroupHierarchyRecord_AfterUpdate_SetAggregateRootUpdatedOn]
            ON [dbo].[WordGroupHierarchyRecord]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [wordGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[WordGroupHierarchyRecord] AS [wordGroupHierarchyRecord]
                INNER JOIN [dbo].[WordGroup] AS [wordGroup] ON [wordGroup].[Id] = [wordGroupHierarchyRecord].[WordGroupId]
                INNER JOIN [inserted] AS [updatedWordGroupHierarchyRecord]
                    ON [updatedWordGroupHierarchyRecord].[WordGroupId] = [wordGroupHierarchyRecord].[WordGroupId]
                        AND [updatedWordGroupHierarchyRecord].[NestedWordGroupId] = [wordGroupHierarchyRecord].[NestedWordGroupId];
            END;");

        // TR_WordGroupHierarchyRecord_AfterDelete_SetAggregateRootUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_WordGroupHierarchyRecord_AfterDelete_SetAggregateRootUpdatedOn]
            ON [dbo].[WordGroupHierarchyRecord]
            AFTER DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [wordGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[WordGroup] AS [wordGroup]
                INNER JOIN [deleted] AS [deletedWordGroupHierarchyRecord]
                    ON [deletedWordGroupHierarchyRecord].[WordGroupId] = [wordGroup].[Id];
            END;");

        // dbo.udt_WordGroupHierarchyRecord
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_WordGroupHierarchyRecord] AS TABLE
            (
                [WordGroupId] INT NOT NULL,
                [NestedWordGroupId] INT NOT NULL,
                [PreventRecursiveIncludes] BIT NOT NULL
            );");

        // dbo.sp_MergeWordGroupHierarchyRecords_Internal
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_MergeWordGroupHierarchyRecords_Internal]
            (
                @WordGroupIds [dbo].[udt_Int32UnorderedSet] READONLY,
                @WordGroupHierarchyRecords [dbo].[udt_WordGroupHierarchyRecord] READONLY
            )
            AS
            BEGIN
                DELETE [wordGroupHierarchyRecord]
                FROM [dbo].[WordGroupHierarchyRecord] AS [wordGroupHierarchyRecord]
                INNER JOIN @WordGroupIds AS [wordGroupId] ON [wordGroupId].[Value] = [wordGroupHierarchyRecord].[WordGroupId];

                INSERT INTO [dbo].[WordGroupHierarchyRecord]
                (
                    [WordGroupId],
                    [NestedWordGroupId],
                    [PreventRecursiveIncludes]
                )
                SELECT
                    [wordGroupHierarchyRecord].[WordGroupId],
                    [wordGroupHierarchyRecord].[NestedWordGroupId],
                    [wordGroupHierarchyRecord].[PreventRecursiveIncludes]
                FROM @WordGroupHierarchyRecords AS [wordGroupHierarchyRecord];
            END;");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddWordGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWordGroups];");

        // dbo.sp_AddWordGroups
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_AddWordGroups]
            (
                @WordGroups [dbo].[udt_WordGroup] READONLY,
                @WordGroupHierarchyRecords [dbo].[udt_WordGroupHierarchyRecord] READONLY,
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

                DECLARE @WordGroupHierarchyRecordsUpdated AS [dbo].[udt_WordGroupHierarchyRecord];

                INSERT INTO @WordGroupHierarchyRecordsUpdated
                (
                    [WordGroupId],
                    [NestedWordGroupId],
                    [PreventRecursiveIncludes]
                )
                SELECT
                    COALESCE([wordGroupIdMap].[Id], [wordGroupHierarchyRecord].[WordGroupId]) AS [WordGroupId],
                    [wordGroupHierarchyRecord].[NestedWordGroupId],
                    [wordGroupHierarchyRecord].[PreventRecursiveIncludes]
                FROM @WordGroupHierarchyRecords AS [wordGroupHierarchyRecord]
                LEFT JOIN @WordGroupIdMap AS [wordGroupIdMap] ON [wordGroupIdMap].[PreviousId] = [wordGroupHierarchyRecord].[WordGroupId];

                EXEC [dbo].[sp_MergeWordGroupHierarchyRecords_Internal]
                    @WordGroupIds,
                    @WordGroupHierarchyRecordsUpdated;

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
                @WordGroupHierarchyRecords [dbo].[udt_WordGroupHierarchyRecord] READONLY,
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

                EXEC [dbo].[sp_MergeWordGroupHierarchyRecords_Internal]
                    @WordGroupIds,
                    @WordGroupHierarchyRecords;

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

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddWordGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateWordGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_MergeWordGroupHierarchyRecords_Internal];");

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

        // dbo.WordGroupHierarchyRecord
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_WordGroupHierarchyRecord];");

        migrationBuilder.Sql("DROP TABLE [dbo].[WordGroupHierarchyRecord];");
    }
}
