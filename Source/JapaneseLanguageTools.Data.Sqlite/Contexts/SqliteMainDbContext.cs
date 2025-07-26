using System;

using JapaneseLanguageTools.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace JapaneseLanguageTools.Data.Sqlite.Contexts;

public class SqliteMainDbContext : MainDbContext
{
    public const string EmptyTimeSpanSql = "00:00:00";

    public const string EmptyDateTimeSql = "0001-01-01 00:00:00";
    public const string EmptyDateTimeOffsetSql = "0001-01-01 00:00:00+00:00";

    public const string CurrentDateTimeSql = "STRFTIME('%F %T', 'now')";
    public const string CurrentDateTimeOffsetSql = "STRFTIME('%F %T+00:00', 'now')";

    public SqliteMainDbContext()
        : base()
    {
    }

    [ActivatorUtilitiesConstructor]
    public SqliteMainDbContext(DbContextOptions<SqliteMainDbContext> contextOptions)
        : base(contextOptions)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TimeSpan>().HaveConversion<TimeSpanToStringConverter>();

        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeToStringConverter>();
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToStringConverter>();

        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyToStringConverter>();
        configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyToStringConverter>();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
