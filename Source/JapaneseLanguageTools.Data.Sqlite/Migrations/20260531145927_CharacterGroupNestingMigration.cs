using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.Sqlite.Migrations;

public partial class CharacterGroupNestingMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CharacterGroupHierarchyRecord",
            columns: table => new
            {
                CharacterGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                NestedCharacterGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                PreventRecursiveIncludes = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_CharacterGroupHierarchyRecord", columns: x => new { x.CharacterGroupId, x.NestedCharacterGroupId });

                table.ForeignKey(
                    name: "FK_CharacterGroupHierarchyRecord_CharacterGroup_CharacterGroupId",
                    column: x => x.CharacterGroupId,
                    principalTable: "CharacterGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CharacterGroupHierarchyRecord_CharacterGroup_NestedCharacterGroupId",
                    column: x => x.NestedCharacterGroupId,
                    principalTable: "CharacterGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterGroupHierarchyRecord_CharacterGroupId",
            table: "CharacterGroupHierarchyRecord",
            column: "CharacterGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterGroupHierarchyRecord_NestedCharacterGroupId",
            table: "CharacterGroupHierarchyRecord",
            column: "NestedCharacterGroupId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CharacterGroupHierarchyRecord");
    }
}
