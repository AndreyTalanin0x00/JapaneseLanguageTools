using System;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;

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
        modelBuilder.Entity<Tag>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder
                .HasIndex(entity => entity.Caption)
                .HasDatabaseName("UIX_Tag_Caption")
                .IsUnique();

            entityBuilder
                .Property(entity => entity.CreatedOn)
                .HasDefaultValueSql(CurrentDateTimeOffsetSql)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.UpdatedOn)
                .HasDefaultValueSql(CurrentDateTimeOffsetSql)
                .ValueGeneratedOnAdd();

            entityBuilder.ToTable("Tag", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Tag_Caption_NotEmpty", "LENGTH(TRIM(\"Caption\")) > 0");
                tableBuilder.HasCheckConstraint("CK_Tag_Caption_MaxLength", "LENGTH(TRIM(\"Caption\")) <= 256");
                tableBuilder.HasCheckConstraint("CK_Tag_PlaceholderMarker_NullOrNotEmpty", "\"PlaceholderMarker\" IS NULL OR LENGTH(TRIM(\"PlaceholderMarker\")) > 0");
                tableBuilder.HasCheckConstraint("CK_Tag_PlaceholderMarker_NullOrMaxLength", "\"PlaceholderMarker\" IS NULL OR LENGTH(TRIM(\"PlaceholderMarker\")) <= 2048");
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}
