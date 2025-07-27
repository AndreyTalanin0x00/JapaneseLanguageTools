using JapaneseLanguageTools.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace JapaneseLanguageTools.Data.Contexts;

public class MainDbContext : DbContext
{
    public DbSet<Character> Characters { get; }

    public DbSet<CharacterGroup> CharacterGroups { get; }

    public DbSet<CharacterTag> CharacterTags { get; }

    public DbSet<Tag> Tags { get; }

    protected MainDbContext()
        : base()
    {
        Characters = Set<Character>();
        CharacterGroups = Set<CharacterGroup>();
        CharacterTags = Set<CharacterTag>();
        Tags = Set<Tag>();
    }

    protected MainDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
        Characters = Set<Character>();
        CharacterGroups = Set<CharacterGroup>();
        CharacterTags = Set<CharacterTag>();
        Tags = Set<Tag>();
    }
}
