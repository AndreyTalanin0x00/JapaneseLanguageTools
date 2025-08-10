using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.Sqlite.Migrations;

public partial class TagMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tag",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Caption = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                PlaceholderMarker = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                CreatedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')"),
                UpdatedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Tag", columns: x => x.Id);

                table.UniqueConstraint(name: "AK_Tag_Caption", columns: x => x.Caption);

                table.CheckConstraint(name: "CK_Tag_Caption_NotEmpty", sql: "LENGTH(TRIM(\"Caption\")) > 0");
                table.CheckConstraint(name: "CK_Tag_Caption_MaxLength", sql: "LENGTH(TRIM(\"Caption\")) <= 256");
                table.CheckConstraint(name: "CK_Tag_PlaceholderMarker_NullOrNotEmpty", sql: "\"PlaceholderMarker\" IS NULL OR LENGTH(TRIM(\"PlaceholderMarker\")) > 0");
                table.CheckConstraint(name: "CK_Tag_PlaceholderMarker_NullOrMaxLength", sql: "\"PlaceholderMarker\" IS NULL OR LENGTH(TRIM(\"PlaceholderMarker\")) <= 2048");
            });

        migrationBuilder.CreateIndex(
            name: "UIX_Tag_Caption",
            table: "Tag",
            column: "Caption",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Tag");
    }
}
