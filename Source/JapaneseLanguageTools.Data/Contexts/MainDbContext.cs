using JapaneseLanguageTools.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace JapaneseLanguageTools.Data.Contexts;

public class MainDbContext : DbContext
{
    public DbSet<Tag> Tags { get; }

    protected MainDbContext()
        : base()
    {
        Tags = Set<Tag>();
    }

    protected MainDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
        Tags = Set<Tag>();
    }
}
