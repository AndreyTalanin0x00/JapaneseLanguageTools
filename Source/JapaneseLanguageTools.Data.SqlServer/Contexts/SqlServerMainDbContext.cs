using System;

using JapaneseLanguageTools.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JapaneseLanguageTools.Data.SqlServer.Contexts;

public class SqlServerMainDbContext : MainDbContext
{
    public SqlServerMainDbContext()
        : base()
    {
        throw new NotImplementedException();
    }

    [ActivatorUtilitiesConstructor]
    public SqlServerMainDbContext(DbContextOptions<SqlServerMainDbContext> contextOptions)
        : base(contextOptions)
    {
        throw new NotImplementedException();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
