using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.Sqlite.Migrations;

public partial class WordExerciseMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WordExercise",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                WordId = table.Column<int>(type: "INTEGER", nullable: false),
                InstanceData = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                GeneratedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
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

                table.CheckConstraint(name: "CK_WordExercise_InstanceData_NullOrNotEmpty", sql: "\"InstanceData\" IS NULL OR LENGTH(TRIM(\"InstanceData\")) > 0");
                table.CheckConstraint(name: "CK_WordExercise_InstanceData_NullOrMaxLength", sql: "\"InstanceData\" IS NULL OR LENGTH(TRIM(\"InstanceData\")) <= 2048");
            });

        migrationBuilder.CreateIndex(
            name: "IX_WordExercise_WordId",
            table: "WordExercise",
            column: "WordId");

        migrationBuilder.CreateTable(
            name: "WordExerciseRerun",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                WordExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                RequiredChallengeCount = table.Column<int>(type: "INTEGER", nullable: false),
                ContinuousChallengeCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                TotalChallengeCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                InitiallyScheduledOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')"),
                RepeatedlyScheduledOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
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

                table.CheckConstraint(name: "CK_WordExerciseRerun_RequiredChallengeCount_NotNegative", sql: "\"RequiredChallengeCount\" >= 0");
                table.CheckConstraint(name: "CK_WordExerciseRerun_ContinuousChallengeCount_NotNegative", sql: "\"ContinuousChallengeCount\" >= 0");
                table.CheckConstraint(name: "CK_WordExerciseRerun_TotalChallengeCount_NotNegative", sql: "\"TotalChallengeCount\" >= 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_WordExerciseRerun_WordExerciseId",
            table: "WordExerciseRerun",
            column: "WordExerciseId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "WordExerciseRerun");

        migrationBuilder.DropTable(name: "WordExercise");
    }
}
