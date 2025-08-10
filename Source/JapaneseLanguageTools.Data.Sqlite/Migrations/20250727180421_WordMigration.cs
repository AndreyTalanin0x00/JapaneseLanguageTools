using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.Sqlite.Migrations;

public partial class WordMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WordGroup",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Caption = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Comment = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                AlwaysUse = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                Hidden = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                CreatedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')"),
                UpdatedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_WordGroup", columns: x => x.Id);

                table.CheckConstraint(name: "CK_WordGroup_Caption_NotEmpty", sql: "LENGTH(TRIM(\"Caption\")) > 0");
                table.CheckConstraint(name: "CK_WordGroup_Caption_MaxLength", sql: "LENGTH(TRIM(\"Caption\")) <= 256");
                table.CheckConstraint(name: "CK_WordGroup_Comment_NullOrNotEmpty", sql: "\"Comment\" IS NULL OR LENGTH(TRIM(\"Comment\")) > 0");
                table.CheckConstraint(name: "CK_WordGroup_Comment_NullOrMaxLength", sql: "\"Comment\" IS NULL OR LENGTH(TRIM(\"Comment\")) <= 2048");
            });

        migrationBuilder.CreateIndex(
            name: "UIX_WordGroup_Caption",
            table: "WordGroup",
            column: "Caption",
            unique: true);

        migrationBuilder.CreateTable(
            name: "Word",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                WordGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                Characters = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                CharacterTypes = table.Column<int>(type: "INTEGER", nullable: false),
                Pronunciation = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                Furigana = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                Okurigana = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                Meaning = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                CreatedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')"),
                UpdatedOn = table.Column<string>(type: "TEXT", nullable: false, defaultValueSql: "STRFTIME('%F %T+00:00', 'now')")
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

                table.CheckConstraint(name: "CK_Word_Characters_NotEmpty", sql: "LENGTH(TRIM(\"Characters\")) > 0");
                table.CheckConstraint(name: "CK_Word_Characters_MaxLength", sql: "LENGTH(TRIM(\"Characters\")) <= 256");
                table.CheckConstraint(name: "CK_Word_Pronunciation_NullOrNotEmpty", sql: "\"Pronunciation\" IS NULL OR LENGTH(TRIM(\"Pronunciation\")) > 0");
                table.CheckConstraint(name: "CK_Word_Pronunciation_NullOrMaxLength", sql: "\"Pronunciation\" IS NULL OR LENGTH(TRIM(\"Pronunciation\")) <= 512");
                table.CheckConstraint(name: "CK_Word_Furigana_NullOrNotEmpty", sql: "\"Furigana\" IS NULL OR LENGTH(TRIM(\"Furigana\")) > 0");
                table.CheckConstraint(name: "CK_Word_Furigana_NullOrMaxLength", sql: "\"Furigana\" IS NULL OR LENGTH(TRIM(\"Furigana\")) <= 512");
                table.CheckConstraint(name: "CK_Word_Okurigana_NullOrNotEmpty", sql: "\"Okurigana\" IS NULL OR LENGTH(TRIM(\"Okurigana\")) > 0");
                table.CheckConstraint(name: "CK_Word_Okurigana_NullOrMaxLength", sql: "\"Okurigana\" IS NULL OR LENGTH(TRIM(\"Okurigana\")) <= 512");
                table.CheckConstraint(name: "CK_Word_Meaning_NullOrNotEmpty", sql: "\"Meaning\" IS NULL OR LENGTH(TRIM(\"Meaning\")) > 0");
                table.CheckConstraint(name: "CK_Word_Meaning_NullOrMaxLength", sql: "\"Meaning\" IS NULL OR LENGTH(TRIM(\"Meaning\")) <= 512");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Word_WordGroupId",
            table: "Word",
            column: "WordGroupId");

        migrationBuilder.CreateTable(
            name: "WordTag",
            columns: table => new
            {
                WordId = table.Column<int>(type: "INTEGER", nullable: false),
                TagId = table.Column<int>(type: "INTEGER", nullable: false)
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
            table: "WordTag",
            column: "WordId");

        migrationBuilder.CreateIndex(
            name: "IX_WordTag_TagId",
            table: "WordTag",
            column: "TagId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "WordTag");

        migrationBuilder.DropTable(name: "Word");

        migrationBuilder.DropTable(name: "WordGroup");
    }
}
