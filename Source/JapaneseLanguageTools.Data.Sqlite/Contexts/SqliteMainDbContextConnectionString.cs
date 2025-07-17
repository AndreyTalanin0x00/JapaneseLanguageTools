using Microsoft.Data.Sqlite;

namespace JapaneseLanguageTools.Data.Sqlite.Contexts;

public class SqliteMainDbContextConnectionString
{
    public string Value { get; set; }

    public SqliteMainDbContextConnectionString(string value)
    {
        Value = value;
    }

    public SqliteConnectionStringBuilder CreateConnectionStringBuilder()
    {
        return new SqliteConnectionStringBuilder(Value);
    }
}
