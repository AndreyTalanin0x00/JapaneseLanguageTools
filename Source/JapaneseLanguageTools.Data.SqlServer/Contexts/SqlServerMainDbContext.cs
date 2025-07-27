using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;

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
        modelBuilder.Entity<Tag>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder.HasAlternateKey(entity => entity.Caption);

            entityBuilder
                .HasIndex(entity => entity.Caption)
                .HasDatabaseName("UIX_Tag_Caption")
                .IsUnique();

            entityBuilder
                .Property(entity => entity.CreatedOn)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate();

            entityBuilder.ToTable("Tag", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Tag_Caption_NotEmpty", "LEN(TRIM([Caption])) > 0");
                tableBuilder.HasCheckConstraint("CK_Tag_PlaceholderMarker_NullOrNotEmpty", "[PlaceholderMarker] IS NULL OR LEN(TRIM([PlaceholderMarker])) > 0");
            });
        });

        modelBuilder.Entity<Character>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder
                .HasOne(entity => entity.CharacterGroup)
                .WithMany(entity => entity.Characters)
                .HasForeignKey(entity => entity.CharacterGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            entityBuilder
                .HasIndex(entity => entity.CharacterGroupId)
                .HasDatabaseName("IX_Character_CharacterGroupId");

            entityBuilder
                .Property(entity => entity.CreatedOn)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate();

            entityBuilder.ToTable("Character", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Character_Symbol_NotEmpty", "LEN(TRIM([Symbol])) > 0");
                tableBuilder.HasCheckConstraint("CK_Character_Pronunciation_NullOrNotEmpty", "[Pronunciation] IS NULL OR LEN(TRIM([Pronunciation])) > 0");
                tableBuilder.HasCheckConstraint("CK_Character_Syllable_NullOrNotEmpty", "[Syllable] IS NULL OR LEN(TRIM([Syllable])) > 0");
                tableBuilder.HasCheckConstraint("CK_Character_Onyomi_NullOrNotEmpty", "[Onyomi] IS NULL OR LEN(TRIM([Onyomi])) > 0");
                tableBuilder.HasCheckConstraint("CK_Character_Kunyomi_NullOrNotEmpty", "[Kunyomi] IS NULL OR LEN(TRIM([Kunyomi])) > 0");
                tableBuilder.HasCheckConstraint("CK_Character_Meaning_NullOrNotEmpty", "[Meaning] IS NULL OR LEN(TRIM([Meaning])) > 0");
            });
        });

        modelBuilder.Entity<CharacterGroup>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder.HasAlternateKey(entity => entity.Caption);

            entityBuilder
                .HasIndex(entity => entity.Caption)
                .HasDatabaseName("UIX_CharacterGroup_Caption")
                .IsUnique();

            entityBuilder
                .Property(entity => entity.CreatedOn)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate();

            entityBuilder.ToTable("CharacterGroup", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_CharacterGroup_Caption_NotEmpty", "LEN(TRIM([Caption])) > 0");
                tableBuilder.HasCheckConstraint("CK_CharacterGroup_Comment_NullOrNotEmpty", "[Comment] IS NULL OR LEN(TRIM([Comment])) > 0");
            });
        });

        modelBuilder.Entity<CharacterExercise>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder
                .HasOne(entity => entity.Character)
                .WithMany()
                .HasForeignKey(entity => entity.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            entityBuilder
                .HasIndex(entity => entity.CharacterId)
                .HasDatabaseName("IX_CharacterExercise_CharacterId");

            entityBuilder
                .Property(entity => entity.GeneratedOn)
                .ValueGeneratedOnAdd();

            entityBuilder.ToTable("CharacterExercise", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_CharacterExercise_InstanceData_NullOrNotEmpty", "[InstanceData] IS NULL OR LEN(TRIM([InstanceData])) > 0");
            });
        });

        modelBuilder.Entity<CharacterExerciseRerun>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder
                .HasOne(entity => entity.CharacterExercise)
                .WithMany(entity => entity.CharacterExerciseReruns)
                .HasForeignKey(entity => entity.CharacterExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entityBuilder
                .HasIndex(entity => entity.CharacterExerciseId)
                .HasDatabaseName("IX_CharacterExerciseRerun_CharacterExerciseId");

            entityBuilder
                .Property(entity => entity.InitiallyScheduledOn)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.RepeatedlyScheduledOn)
                .ValueGeneratedOnAdd();

            entityBuilder.ToTable("CharacterExerciseRerun", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_CharacterExerciseRerun_RequiredChallengeCount_NotNegative", "[RequiredChallengeCount] >= 0");
                tableBuilder.HasCheckConstraint("CK_CharacterExerciseRerun_ContinuousChallengeCount_NotNegative", "[ContinuousChallengeCount] >= 0");
                tableBuilder.HasCheckConstraint("CK_CharacterExerciseRerun_TotalChallengeCount_NotNegative", "[TotalChallengeCount] >= 0");
            });
        });

        modelBuilder.Entity<CharacterTag>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => new { entity.CharacterId, entity.TagId });

            entityBuilder
                .HasOne(entity => entity.Character)
                .WithMany(entity => entity.CharacterTags)
                .HasForeignKey(entity => entity.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
            entityBuilder
                .HasOne(entity => entity.Tag)
                .WithMany()
                .HasForeignKey(entity => entity.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            entityBuilder
                .HasIndex(entity => entity.CharacterId)
                .HasDatabaseName("IX_CharacterTag_CharacterId");
            entityBuilder
                .HasIndex(entity => entity.TagId)
                .HasDatabaseName("IX_CharacterTag_TagId");

            entityBuilder.ToTable("CharacterTag", "dbo", tableBuilder =>
            {
            });
        });

        modelBuilder.Entity<Word>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder
                .HasOne(entity => entity.WordGroup)
                .WithMany(entity => entity.Words)
                .HasForeignKey(entity => entity.WordGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            entityBuilder
                .HasIndex(entity => entity.WordGroupId)
                .HasDatabaseName("IX_Word_WordGroupId");

            entityBuilder
                .Property(entity => entity.CreatedOn)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate();

            entityBuilder.ToTable("Word", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Word_Characters_NotEmpty", "LEN(TRIM([Characters])) > 0");
                tableBuilder.HasCheckConstraint("CK_Word_Pronunciation_NullOrNotEmpty", "[Pronunciation] IS NULL OR LEN(TRIM([Pronunciation])) > 0");
                tableBuilder.HasCheckConstraint("CK_Word_Furigana_NullOrNotEmpty", "[Furigana] IS NULL OR LEN(TRIM([Furigana])) > 0");
                tableBuilder.HasCheckConstraint("CK_Word_Okurigana_NullOrNotEmpty", "[Okurigana] IS NULL OR LEN(TRIM([Okurigana])) > 0");
                tableBuilder.HasCheckConstraint("CK_Word_Meaning_NullOrNotEmpty", "[Meaning] IS NULL OR LEN(TRIM([Meaning])) > 0");
            });
        });

        modelBuilder.Entity<WordGroup>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder.HasAlternateKey(entity => entity.Caption);

            entityBuilder
                .HasIndex(entity => entity.Caption)
                .HasDatabaseName("UIX_WordGroup_Caption")
                .IsUnique();

            entityBuilder
                .Property(entity => entity.CreatedOn)
                .ValueGeneratedOnAdd();
            entityBuilder
                .Property(entity => entity.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate();

            entityBuilder.ToTable("WordGroup", "dbo", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_WordGroup_Caption_NotEmpty", "LEN(TRIM([Caption])) > 0");
                tableBuilder.HasCheckConstraint("CK_WordGroup_Comment_NullOrNotEmpty", "[Comment] IS NULL OR LEN(TRIM([Comment])) > 0");
            });
        });

        modelBuilder.Entity<WordTag>(entityBuilder =>
        {
            entityBuilder.HasKey(entity => new { entity.WordId, entity.TagId });

            entityBuilder
                .HasOne(entity => entity.Word)
                .WithMany(entity => entity.WordTags)
                .HasForeignKey(entity => entity.WordId)
                .OnDelete(DeleteBehavior.Cascade);
            entityBuilder
                .HasOne(entity => entity.Tag)
                .WithMany()
                .HasForeignKey(entity => entity.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            entityBuilder
                .HasIndex(entity => entity.WordId)
                .HasDatabaseName("IX_WordTag_WordId");
            entityBuilder
                .HasIndex(entity => entity.TagId)
                .HasDatabaseName("IX_WordTag_TagId");

            entityBuilder.ToTable("WordTag", "dbo", tableBuilder =>
            {
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}
