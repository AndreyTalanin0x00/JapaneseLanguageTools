using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.Sqlite.Migrations;

public partial class CharacterExerciseMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CharacterExercise",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                InstanceData = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                GeneratedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
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

                table.CheckConstraint(name: "CK_CharacterExercise_InstanceData_NullOrNotEmpty", sql: "\"InstanceData\" IS NULL OR LENGTH(TRIM(\"InstanceData\")) > 0");
                table.CheckConstraint(name: "CK_CharacterExercise_InstanceData_NullOrMaxLength", sql: "\"InstanceData\" IS NULL OR LENGTH(TRIM(\"InstanceData\")) <= 2048");
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterExercise_CharacterId",
            table: "CharacterExercise",
            column: "CharacterId");

        migrationBuilder.CreateTable(
            name: "CharacterExerciseRerun",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CharacterExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                RequiredChallengeCount = table.Column<int>(type: "INTEGER", nullable: false),
                ContinuousChallengeCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                TotalChallengeCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                InitiallyScheduledOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')"),
                RepeatedlyScheduledOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
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

                table.CheckConstraint(name: "CK_CharacterExerciseRerun_RequiredChallengeCount_NotNegative", sql: "\"RequiredChallengeCount\" >= 0");
                table.CheckConstraint(name: "CK_CharacterExerciseRerun_ContinuousChallengeCount_NotNegative", sql: "\"ContinuousChallengeCount\" >= 0");
                table.CheckConstraint(name: "CK_CharacterExerciseRerun_TotalChallengeCount_NotNegative", sql: "\"TotalChallengeCount\" >= 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterExerciseRerun_CharacterExerciseId",
            table: "CharacterExerciseRerun",
            column: "CharacterExerciseId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CharacterExerciseRerun");

        migrationBuilder.DropTable(name: "CharacterExercise");
    }
}
