using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class TagMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tag",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Caption = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                PlaceholderMarker = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(name: "PK_Tag", columns: x => x.Id);

                table.UniqueConstraint(name: "AK_Tag_Caption", columns: x => x.Caption);

                table.CheckConstraint(name: "CK_Tag_Caption_NotEmpty", sql: "LEN(TRIM([Caption])) > 0");
                table.CheckConstraint(name: "CK_Tag_PlaceholderMarker_NullOrNotEmpty", sql: "[PlaceholderMarker] IS NULL OR LEN(TRIM([PlaceholderMarker])) > 0");
            });

        migrationBuilder.Sql(@"
            ALTER TABLE [Tag]
            ADD CONSTRAINT [DF_Tag_CreatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [CreatedOn];");

        migrationBuilder.Sql(@"
            ALTER TABLE [Tag]
            ADD CONSTRAINT [DF_Tag_UpdatedOn] DEFAULT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+00:00') FOR [UpdatedOn];");

        migrationBuilder.CreateIndex(
            name: "UIX_Tag_Caption",
            schema: "dbo",
            table: "Tag",
            column: "Caption",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Tag", schema: "dbo");
    }
}
