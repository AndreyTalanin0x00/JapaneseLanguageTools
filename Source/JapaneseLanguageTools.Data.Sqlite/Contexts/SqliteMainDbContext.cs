using System;

using JapaneseLanguageTools.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JapaneseLanguageTools.Data.Sqlite.Contexts;

public class SqliteMainDbContext : MainDbContext
{
    public SqliteMainDbContext()
        : base()
    {
        throw new NotImplementedException();
    }

    [ActivatorUtilitiesConstructor]
    public SqliteMainDbContext(DbContextOptions<SqliteMainDbContext> contextOptions)
        : base(contextOptions)
    {
        throw new NotImplementedException();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
