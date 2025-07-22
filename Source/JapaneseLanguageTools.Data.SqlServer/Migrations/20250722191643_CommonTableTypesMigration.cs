using Microsoft.EntityFrameworkCore.Migrations;

namespace JapaneseLanguageTools.Data.SqlServer.Migrations;

public partial class CommonTableTypesMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 8-bit integer value - System.Byte - TINYINT
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_ByteArray] AS TABLE
            (
                [Value] TINYINT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_ByteUnorderedSet] AS TABLE
            (
                [Value] TINYINT NOT NULL PRIMARY KEY CLUSTERED
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_ByteOrderedSet] AS TABLE
            (
                [Value] TINYINT NOT NULL PRIMARY KEY CLUSTERED,
                [Order] INT NOT NULL
            );");

        // 16-bit signed integer value - System.Int16 - SMALLINT
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int16Array] AS TABLE
            (
                [Value] SMALLINT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int16UnorderedSet] AS TABLE
            (
                [Value] SMALLINT NOT NULL PRIMARY KEY CLUSTERED
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int16OrderedSet] AS TABLE
            (
                [Value] SMALLINT NOT NULL PRIMARY KEY CLUSTERED,
                [Order] INT NOT NULL
            );");

        // 32-bit signed integer value - System.Int32 - INT
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int32Array] AS TABLE
            (
                [Value] INT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int32UnorderedSet] AS TABLE
            (
                [Value] INT NOT NULL PRIMARY KEY CLUSTERED
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int32OrderedSet] AS TABLE
            (
                [Value] INT NOT NULL PRIMARY KEY CLUSTERED,
                [Order] INT NOT NULL
            );");

        // 64-bit signed integer value - System.Int64 - BIGINT
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int64Array] AS TABLE
            (
                [Value] BIGINT NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int64UnorderedSet] AS TABLE
            (
                [Value] BIGINT NOT NULL PRIMARY KEY CLUSTERED
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_Int64OrderedSet] AS TABLE
            (
                [Value] BIGINT NOT NULL PRIMARY KEY CLUSTERED,
                [Order] INT NOT NULL
            );");

        // 128-bit non-numerical integer value (GUID/UUID) - System.Guid - UNIQUEIDENTIFIER
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_GuidArray] AS TABLE
            (
                [Value] UNIQUEIDENTIFIER NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_GuidUnorderedSet] AS TABLE
            (
                [Value] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY CLUSTERED
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_GuidOrderedSet] AS TABLE
            (
                [Value] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY CLUSTERED,
                [Order] INT NOT NULL
            );");

        // UTF-16 string value - System.String - NVARCHAR(MAX)
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_StringArray] AS TABLE
            (
                [Value] NVARCHAR(MAX) NOT NULL
            );");

        // UTF-16 string value of 256 byte pairs - System.String - NVARCHAR(256)
        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_String256Array] AS TABLE
            (
                [Value] NVARCHAR(256) NOT NULL
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_String256UnorderedSet] AS TABLE
            (
                [Value] NVARCHAR(256) NOT NULL PRIMARY KEY CLUSTERED
            );");

        migrationBuilder.Sql(@"
            CREATE TYPE [dbo].[udt_String256OrderedSet] AS TABLE
            (
                [Value] NVARCHAR(256) NOT NULL PRIMARY KEY CLUSTERED,
                [Order] INT NOT NULL
            );");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // 8-bit integer value - System.Byte - TINYINT
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_ByteArray];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_ByteUnorderedSet];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_ByteOrderedSet];");

        // 16-bit signed integer value - System.Int16 - SMALLINT
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int16Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int16UnorderedSet];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int16OrderedSet];");

        // 32-bit signed integer value - System.Int32 - INT
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int32Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int32UnorderedSet];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int32OrderedSet];");

        // 64-bit signed integer value - System.Int64 - BIGINT
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int64Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int64UnorderedSet];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_Int64OrderedSet];");

        // 128-bit non-numerical integer value (GUID/UUID) - System.Guid - UNIQUEIDENTIFIER
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_GuidArray];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_GuidUnorderedSet];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_GuidOrderedSet];");

        // UTF-16 string value - System.String - NVARCHAR(MAX)
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_StringArray];");

        // UTF-16 string value of 256 byte pairs - System.String - NVARCHAR(256)
        migrationBuilder.Sql("DROP TYPE [dbo].[udt_String256Array];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_String256UnorderedSet];");

        migrationBuilder.Sql("DROP TYPE [dbo].[udt_String256OrderedSet];");
    }
}
