using JapaneseLanguageTools.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JapaneseLanguageTools.Data.SqlServer.Contexts;

public class SqlServerMainDbContext : MainDbContext
{
    public SqlServerMainDbContext()
        : base()
    {
    }

    [ActivatorUtilitiesConstructor]
    public SqlServerMainDbContext(DbContextOptions<SqlServerMainDbContext> contextOptions)
        : base(contextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
