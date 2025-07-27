using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class CharacterExerciseMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // dbo.CharacterExercise
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "CharacterExercise",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CharacterId = table.Column<int>(type: "int", nullable: false),
                InstanceData = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                GeneratedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_CharacterExercise", columns: x => x.Id);

                table.ForeignKey(
                    name: "FK_CharacterExercise_Character_CharacterId",
                    column: x => x.CharacterId,
                    principalTable: "Character",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                table.CheckConstraint(name: "CK_CharacterExercise_InstanceData_NullOrNotEmpty", sql: "[InstanceData] IS NULL OR LEN(TRIM([InstanceData])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterExercise_CharacterId",
            schema: "dbo",
            table: "CharacterExercise",
            column: "CharacterId");

        // dbo.udt_CharacterExercise
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_CharacterExercise] AS TABLE
            (
                [Id] INT NOT NULL,
                [CharacterId] INT NOT NULL,
                [InstanceData] NVARCHAR(2048) NULL,
                [GeneratedOn] DATETIMEOFFSET NOT NULL
            );");

        // dbo.CharacterExerciseRerun
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "CharacterExerciseRerun",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CharacterExerciseId = table.Column<int>(type: "int", nullable: false),
                RequiredChallengeCount = table.Column<int>(type: "int", nullable: false),
                ContinuousChallengeCount = table.Column<int>(type: "int", nullable: false),
                TotalChallengeCount = table.Column<int>(type: "int", nullable: false),
                InitiallyScheduledOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                RepeatedlyScheduledOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_CharacterExerciseRerun", columns: x => x.Id);

                table.ForeignKey(
                    name: "FK_CharacterExerciseRerun_CharacterExercise_CharacterExerciseId",
                    column: x => x.CharacterExerciseId,
                    principalTable: "CharacterExercise",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                table.CheckConstraint(name: "CK_CharacterExerciseRerun_RequiredChallengeCount_NotNegative", sql: "[RequiredChallengeCount] >= 0");
                table.CheckConstraint(name: "CK_CharacterExerciseRerun_ContinuousChallengeCount_NotNegative", sql: "[ContinuousChallengeCount] >= 0");
                table.CheckConstraint(name: "CK_CharacterExerciseRerun_TotalChallengeCount_NotNegative", sql: "[TotalChallengeCount] >= 0");
            });

        // DF_CharacterExerciseRerun_ContinuousChallengeCount
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[CharacterExerciseRerun]
            ADD CONSTRAINT [DF_CharacterExerciseRerun_ContinuousChallengeCount] DEFAULT 0 FOR [ContinuousChallengeCount];");

        // DF_CharacterExerciseRerun_TotalChallengeCount
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[CharacterExerciseRerun]
            ADD CONSTRAINT [DF_CharacterExerciseRerun_TotalChallengeCount] DEFAULT 0 FOR [TotalChallengeCount];");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterExerciseRerun_CharacterExerciseId",
            schema: "dbo",
            table: "CharacterExerciseRerun",
            column: "CharacterExerciseId");

        // dbo.udt_CharacterExerciseRerun
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_CharacterExerciseRerun] AS TABLE
            (
                [Id] INT NOT NULL,
                [CharacterExerciseId] INT NOT NULL,
                [RequiredChallengeCount] INT NOT NULL,
                [ContinuousChallengeCount] INT NOT NULL,
                [TotalChallengeCount] INT NOT NULL,
                [InitiallyScheduledOn] DATETIMEOFFSET NOT NULL,
                [RepeatedlyScheduledOn] DATETIMEOFFSET NOT NULL
            );");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // dbo.CharacterExerciseRerun
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_CharacterExerciseRerun];");

        migrationBuilder.Sql("DROP TABLE [dbo].[CharacterExerciseRerun];");

        // dbo.CharacterExercise
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_CharacterExercise];");

        migrationBuilder.Sql("DROP TABLE [dbo].[CharacterExercise];");
    }
}
