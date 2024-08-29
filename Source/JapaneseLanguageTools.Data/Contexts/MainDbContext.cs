using Microsoft.EntityFrameworkCore;

namespace JapaneseLanguageTools.Data.Contexts;

public class MainDbContext : DbContext
{
    protected MainDbContext()
        : base()
    {
    }

    protected MainDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
    }
}
