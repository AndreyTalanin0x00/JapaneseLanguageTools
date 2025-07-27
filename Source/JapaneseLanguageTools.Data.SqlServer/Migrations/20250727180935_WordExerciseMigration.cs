using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class WordExerciseMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // dbo.WordExercise
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "WordExercise",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                WordId = table.Column<int>(type: "int", nullable: false),
                InstanceData = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                GeneratedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WordExercise", columns: x => x.Id);

                table.ForeignKey(
                    name: "FK_WordExercise_Word_WordId",
                    column: x => x.WordId,
                    principalTable: "Word",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                table.CheckConstraint(name: "CK_WordExercise_InstanceData_NullOrNotEmpty", sql: "[InstanceData] IS NULL OR LEN(TRIM([InstanceData])) > 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_WordExercise_WordId",
            schema: "dbo",
            table: "WordExercise",
            column: "WordId");

        // dbo.udt_WordExercise
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_WordExercise] AS TABLE
            (
                [Id] INT NOT NULL,
                [WordId] INT NOT NULL,
                [InstanceData] NVARCHAR(2048) NULL,
                [GeneratedOn] DATETIMEOFFSET NOT NULL
            );");

        // dbo.CharacterExerciseRerun
        // Table & Related Objects

        migrationBuilder.CreateTable(
            name: "WordExerciseRerun",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                WordExerciseId = table.Column<int>(type: "int", nullable: false),
                RequiredChallengeCount = table.Column<int>(type: "int", nullable: false),
                ContinuousChallengeCount = table.Column<int>(type: "int", nullable: false),
                TotalChallengeCount = table.Column<int>(type: "int", nullable: false),
                InitiallyScheduledOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                RepeatedlyScheduledOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WordExerciseRerun", columns: x => x.Id);

                table.ForeignKey(
                    name: "FK_WordExerciseRerun_WordExercise_WordExerciseId",
                    column: x => x.WordExerciseId,
                    principalTable: "WordExercise",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                table.CheckConstraint(name: "CK_WordExerciseRerun_RequiredChallengeCount_NotNegative", sql: "[RequiredChallengeCount] >= 0");
                table.CheckConstraint(name: "CK_WordExerciseRerun_ContinuousChallengeCount_NotNegative", sql: "[ContinuousChallengeCount] >= 0");
                table.CheckConstraint(name: "CK_WordExerciseRerun_TotalChallengeCount_NotNegative", sql: "[TotalChallengeCount] >= 0");
            });

        // DF_WordExerciseRerun_ContinuousChallengeCount
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[WordExerciseRerun]
            ADD CONSTRAINT [DF_WordExerciseRerun_ContinuousChallengeCount] DEFAULT 0 FOR [ContinuousChallengeCount];");

        // DF_WordExerciseRerun_TotalChallengeCount
        migrationBuilder.Sql(@"
            ALTER TABLE [dbo].[WordExerciseRerun]
            ADD CONSTRAINT [DF_WordExerciseRerun_TotalChallengeCount] DEFAULT 0 FOR [TotalChallengeCount];");

        migrationBuilder.CreateIndex(
            name: "IX_WordExerciseRerun_WordExerciseId",
            schema: "dbo",
            table: "WordExerciseRerun",
            column: "WordExerciseId");

        // dbo.udt_WordExerciseRerun
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_WordExerciseRerun] AS TABLE
            (
                [Id] INT NOT NULL,
                [WordExerciseId] INT NOT NULL,
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
        // dbo.WordExerciseRerun
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_WordExerciseRerun];");

        migrationBuilder.Sql("DROP TABLE [dbo].[WordExerciseRerun];");

        // dbo.WordExercise
        // Table & Related Objects

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_WordExercise];");

        migrationBuilder.Sql("DROP TABLE [dbo].[WordExercise];");
    }
}
