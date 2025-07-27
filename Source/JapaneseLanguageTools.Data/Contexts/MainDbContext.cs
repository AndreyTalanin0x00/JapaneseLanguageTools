using JapaneseLanguageTools.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace JapaneseLanguageTools.Data.Contexts;

public class MainDbContext : DbContext
{
    public DbSet<Character> Characters { get; }

    public DbSet<CharacterGroup> CharacterGroups { get; }

    public DbSet<CharacterExercise> CharacterExercises { get; }

    public DbSet<CharacterExerciseRerun> CharacterExerciseReruns { get; }

    public DbSet<CharacterTag> CharacterTags { get; }

    public DbSet<Word> Words { get; }

    public DbSet<WordGroup> WordGroups { get; }

    public DbSet<WordTag> WordTags { get; }

    public DbSet<Tag> Tags { get; }

    protected MainDbContext()
        : base()
    {
        Characters = Set<Character>();
        CharacterGroups = Set<CharacterGroup>();
        CharacterExercises = Set<CharacterExercise>();
        CharacterExerciseReruns = Set<CharacterExerciseRerun>();
        CharacterTags = Set<CharacterTag>();
        Words = Set<Word>();
        WordGroups = Set<WordGroup>();
        WordTags = Set<WordTag>();
        Tags = Set<Tag>();
    }

    protected MainDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
        Characters = Set<Character>();
        CharacterGroups = Set<CharacterGroup>();
        CharacterExercises = Set<CharacterExercise>();
        CharacterExerciseReruns = Set<CharacterExerciseRerun>();
        CharacterTags = Set<CharacterTag>();
        Words = Set<Word>();
        WordGroups = Set<WordGroup>();
        WordTags = Set<WordTag>();
        Tags = Set<Tag>();
    }
}
