using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.Sqlite.Migrations;

public partial class WordGroupNestingMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WordGroupHierarchyRecord",
            columns: table => new
            {
                WordGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                NestedWordGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                PreventRecursiveIncludes = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WordGroupHierarchyRecord", columns: x => new { x.WordGroupId, x.NestedWordGroupId });

                table.ForeignKey(
                    name: "FK_WordGroupHierarchyRecord_WordGroup_WordGroupId",
                    column: x => x.WordGroupId,
                    principalTable: "WordGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WordGroupHierarchyRecord_WordGroup_NestedWordGroupId",
                    column: x => x.NestedWordGroupId,
                    principalTable: "WordGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_WordGroupHierarchyRecord_WordGroupId",
            table: "WordGroupHierarchyRecord",
            column: "WordGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_WordGroupHierarchyRecord_NestedWordGroupId",
            table: "WordGroupHierarchyRecord",
            column: "NestedWordGroupId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "WordGroupHierarchyRecord");
    }
}
