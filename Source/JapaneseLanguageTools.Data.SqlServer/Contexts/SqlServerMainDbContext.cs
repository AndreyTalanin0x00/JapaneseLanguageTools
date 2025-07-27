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

        base.OnModelCreating(modelBuilder);
    }
}
