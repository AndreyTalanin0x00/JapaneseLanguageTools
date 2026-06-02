using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class CharacterGroupNestingMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // dbo.CharacterGroupHierarchyRecord
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "CharacterGroupHierarchyRecord",
            schema: "dbo",
            columns: table => new
            {
                CharacterGroupId = table.Column<int>(type: "int", nullable: false),
                NestedCharacterGroupId = table.Column<int>(type: "int", nullable: false),
                PreventRecursiveIncludes = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_CharacterGroupHierarchyRecord", columns: x => new { x.CharacterGroupId, x.NestedCharacterGroupId });

                table.ForeignKey(
                    name: "FK_CharacterGroupHierarchyRecord_CharacterGroup_CharacterGroupId",
                    column: x => x.CharacterGroupId,
                    principalSchema: "dbo",
                    principalTable: "CharacterGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CharacterGroupHierarchyRecord_CharacterGroup_NestedCharacterGroupId",
                    column: x => x.NestedCharacterGroupId,
                    principalSchema: "dbo",
                    principalTable: "CharacterGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        // DF_CharacterGroupHierarchyRecord_PreventRecursiveIncludes
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[CharacterGroupHierarchyRecord]
            ADD CONSTRAINT [DF_CharacterGroupHierarchyRecord_PreventRecursiveIncludes] DEFAULT 0 FOR [PreventRecursiveIncludes];");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterGroupHierarchyRecord_CharacterGroupId",
            schema: "dbo",
            table: "CharacterGroupHierarchyRecord",
            column: "CharacterGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterGroupHierarchyRecord_NestedCharacterGroupId",
            schema: "dbo",
            table: "CharacterGroupHierarchyRecord",
            column: "NestedCharacterGroupId");

        // TR_CharacterGroupHierarchyRecord_AfterInsert_SetAggregateRootUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_CharacterGroupHierarchyRecord_AfterInsert_SetAggregateRootUpdatedOn]
            ON [dbo].[CharacterGroupHierarchyRecord]
            AFTER INSERT
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [characterGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[CharacterGroupHierarchyRecord] AS [characterGroupHierarchyRecord]
                INNER JOIN [dbo].[CharacterGroup] AS [characterGroup] ON [characterGroup].[Id] = [characterGroupHierarchyRecord].[CharacterGroupId]
                INNER JOIN [inserted] AS [insertedCharacterGroupHierarchyRecord]
                    ON [insertedCharacterGroupHierarchyRecord].[CharacterGroupId] = [characterGroupHierarchyRecord].[CharacterGroupId]
                        AND [insertedCharacterGroupHierarchyRecord].[NestedCharacterGroupId] = [characterGroupHierarchyRecord].[NestedCharacterGroupId];
            END;");

        // TR_CharacterGroupHierarchyRecord_AfterUpdate_SetAggregateRootUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_CharacterGroupHierarchyRecord_AfterUpdate_SetAggregateRootUpdatedOn]
            ON [dbo].[CharacterGroupHierarchyRecord]
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [characterGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[CharacterGroupHierarchyRecord] AS [characterGroupHierarchyRecord]
                INNER JOIN [dbo].[CharacterGroup] AS [characterGroup] ON [characterGroup].[Id] = [characterGroupHierarchyRecord].[CharacterGroupId]
                INNER JOIN [inserted] AS [updatedCharacterGroupHierarchyRecord]
                    ON [updatedCharacterGroupHierarchyRecord].[CharacterGroupId] = [characterGroupHierarchyRecord].[CharacterGroupId]
                        AND [updatedCharacterGroupHierarchyRecord].[NestedCharacterGroupId] = [characterGroupHierarchyRecord].[NestedCharacterGroupId];
            END;");

        // TR_CharacterGroupHierarchyRecord_AfterDelete_SetAggregateRootUpdatedOn
        migrationBuilder.Sql(@"
            CREATE TRIGGER [TR_CharacterGroupHierarchyRecord_AfterDelete_SetAggregateRootUpdatedOn]
            ON [dbo].[CharacterGroupHierarchyRecord]
            AFTER DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE [characterGroup]
                SET [UpdatedOn] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00')
                FROM [dbo].[CharacterGroup] AS [characterGroup]
                INNER JOIN [deleted] AS [deletedCharacterGroupHierarchyRecord]
                    ON [deletedCharacterGroupHierarchyRecord].[CharacterGroupId] = [characterGroup].[Id];
            END;");

        // dbo.udt_CharacterGroupHierarchyRecord
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_CharacterGroupHierarchyRecord] AS TABLE
            (
                [CharacterGroupId] INT NOT NULL,
                [NestedCharacterGroupId] INT NOT NULL,
                [PreventRecursiveIncludes] BIT NOT NULL
            );");

        // dbo.sp_MergeCharacterGroupHierarchyRecords_Internal
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_MergeCharacterGroupHierarchyRecords_Internal]
            (
                @CharacterGroupIds [dbo].[udt_Int32UnorderedSet] READONLY,
                @CharacterGroupHierarchyRecords [dbo].[udt_CharacterGroupHierarchyRecord] READONLY
            )
            AS
            BEGIN
                DELETE [characterGroupHierarchyRecord]
                FROM [dbo].[CharacterGroupHierarchyRecord] AS [characterGroupHierarchyRecord]
                INNER JOIN @CharacterGroupIds AS [characterGroupId] ON [characterGroupId].[Value] = [characterGroupHierarchyRecord].[CharacterGroupId];

                INSERT INTO [dbo].[CharacterGroupHierarchyRecord]
                (
                    [CharacterGroupId],
                    [NestedCharacterGroupId],
                    [PreventRecursiveIncludes]
                )
                SELECT
                    [characterGroupHierarchyRecord].[CharacterGroupId],
                    [characterGroupHierarchyRecord].[NestedCharacterGroupId],
                    [characterGroupHierarchyRecord].[PreventRecursiveIncludes]
                FROM @CharacterGroupHierarchyRecords AS [characterGroupHierarchyRecord];
            END;");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddCharacterGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateCharacterGroups];");

        // dbo.sp_AddCharacterGroups
        migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sp_AddCharacterGroups]
            (
                @CharacterGroups [dbo].[udt_CharacterGroup] READONLY,
                @CharacterGroupHierarchyRecords [dbo].[udt_CharacterGroupHierarchyRecord] READONLY,
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

                DECLARE @CharacterGroupHierarchyRecordsUpdated AS [dbo].[udt_CharacterGroupHierarchyRecord];

                INSERT INTO @CharacterGroupHierarchyRecordsUpdated
                (
                    [CharacterGroupId],
                    [NestedCharacterGroupId],
                    [PreventRecursiveIncludes]
                )
                SELECT
                    COALESCE([characterGroupIdMap].[Id], [characterGroupHierarchyRecord].[CharacterGroupId]) AS [CharacterGroupId],
                    [characterGroupHierarchyRecord].[NestedCharacterGroupId],
                    [characterGroupHierarchyRecord].[PreventRecursiveIncludes]
                FROM @CharacterGroupHierarchyRecords AS [characterGroupHierarchyRecord]
                LEFT JOIN @CharacterGroupIdMap AS [characterGroupIdMap] ON [characterGroupIdMap].[PreviousId] = [characterGroupHierarchyRecord].[CharacterGroupId];

                EXEC [dbo].[sp_MergeCharacterGroupHierarchyRecords_Internal]
                    @CharacterGroupIds,
                    @CharacterGroupHierarchyRecordsUpdated;

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
                @CharacterGroupHierarchyRecords [dbo].[udt_CharacterGroupHierarchyRecord] READONLY,
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

                EXEC [dbo].[sp_MergeCharacterGroupHierarchyRecords_Internal]
                    @CharacterGroupIds,
                    @CharacterGroupHierarchyRecords;

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

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_AddCharacterGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_UpdateCharacterGroups];");

        migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_MergeCharacterGroupHierarchyRecords_Internal];");

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

        // dbo.CharacterGroupHierarchyRecord
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_CharacterGroupHierarchyRecord];");

        migrationBuilder.Sql("DROP TABLE [dbo].[CharacterGroupHierarchyRecord];");
    }
}
