using Microsoft.Data.SqlClient;

namespace JapaneseLanguageTools.Data.SqlServer.Contexts;

public class SqlServerMainDbContextConnectionString
{
    public string Value { get; set; }

    public SqlServerMainDbContextConnectionString(string value)
    {
        Value = value;
    }

    public SqlConnectionStringBuilder CreateConnectionStringBuilder()
    {
        return new SqlConnectionStringBuilder(Value);
    }
}
