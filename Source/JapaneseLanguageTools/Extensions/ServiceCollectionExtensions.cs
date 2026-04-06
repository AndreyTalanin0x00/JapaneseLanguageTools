using System;
using System.IO;

using AndreyTalanin0x00.Extensions.DependencyInjection;

using AutoMapper;
using AutoMapper.Internal;

using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Core.AutoMapper.Extensions;
using JapaneseLanguageTools.Core.Blobs.AutoMapper.Extensions;
using JapaneseLanguageTools.Core.Configuration;
using JapaneseLanguageTools.Core.Export.AutoMapper.Extensions;
using JapaneseLanguageTools.Core.Import.AutoMapper.Extensions;
using JapaneseLanguageTools.Core.Services;
using JapaneseLanguageTools.Core.Services.Abstractions;
using JapaneseLanguageTools.Core.Services.Hosted;
using JapaneseLanguageTools.Core.Services.Specialized;
using JapaneseLanguageTools.Core.Services.Visitors;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;
using JapaneseLanguageTools.Data.Repositories.Abstractions;
using JapaneseLanguageTools.Data.Sqlite.Contexts;
using JapaneseLanguageTools.Data.Sqlite.Repositories;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Microsoft.OpenApi.Models;

// Disable the IDE0001 (Simplify name) notification to preserve explicit service types.
#pragma warning disable IDE0001

namespace JapaneseLanguageTools.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRandom(this IServiceCollection services)
    {
        services.AddSingleton<Random>(Random.Shared);

        return services;
    }

    public static IServiceCollection AddTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }

    public static IServiceCollection AddRecyclableMemoryStreamManager(this IServiceCollection services)
    {
        services.AddSingleton<RecyclableMemoryStreamManager>(new RecyclableMemoryStreamManager());

        return services;
    }

    public static IServiceCollection AddFileExtensionContentTypeProvider(this IServiceCollection services)
    {
        services.AddTransient<FileExtensionContentTypeProvider>();
        services.AddTransient<IContentTypeProvider, FileExtensionContentTypeProvider>(serviceProvider => serviceProvider.GetRequiredService<FileExtensionContentTypeProvider>());

        return services;
    }

    public static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringName = SqliteMainDbContextConnectionStrings.MainConnectionString;

        // TODO: Add an option to select the database implementation provider.
        services.AddKeyedSingleton<SqliteMainDbContextConnectionString>(connectionStringName, (serviceProvider, serviceKey) =>
        {
            string? connectionString = configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Unable to get the connection string using the \"{connectionStringName}\" configuration key.");
            }

            SqliteConnectionStringBuilder connectionStringBuilder = new(connectionString);
            if (connectionStringBuilder.DataSource.Contains('~'))
            {
                string homeDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                connectionStringBuilder.DataSource = connectionStringBuilder.DataSource.Replace("~", homeDirectoryPath);
                connectionString = connectionStringBuilder.ToString();
            }

            return !File.Exists(connectionStringBuilder.DataSource)
                ? throw new FileNotFoundException($"The data source file ({connectionStringBuilder.DataSource}) does not exist.")
                : new SqliteMainDbContextConnectionString(connectionString);
        });

        // TODO: Add an option to select the database implementation provider.
        services.AddDbContext<MainDbContext, SqliteMainDbContext>((serviceProvider, contextOptions) =>
        {
            SqliteMainDbContextConnectionString connectionString =
                serviceProvider.GetRequiredKeyedService<SqliteMainDbContextConnectionString>(connectionStringName);

            contextOptions.UseSqlite(connectionString.Value, sqliteOptions =>
            {
                sqliteOptions
                    .MigrationsAssembly(typeof(SqliteMainDbContext).Assembly.FullName)
                    .CommandTimeout(120);
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        // TODO: Add an option to select the database implementation provider.
        services.AddTransient<SqliteCharacterRepository>();
        services.AddTransient<ICharacterRepository, SqliteCharacterRepository>(serviceProvider => serviceProvider.GetRequiredService<SqliteCharacterRepository>());
        services.AddTransient<ICharacterGroupRepository, SqliteCharacterGroupRepository>();
        services.AddTransient<ICharacterExerciseRepository, SqliteCharacterExerciseRepository>();
        services.AddTransient<SqliteCharacterExerciseRerunRepository>();
        services.AddTransient<ICharacterExerciseRerunRepository, SqliteCharacterExerciseRerunRepository>(serviceProvider => serviceProvider.GetRequiredService<SqliteCharacterExerciseRerunRepository>());
        services.AddTransient<SqliteWordRepository>();
        services.AddTransient<IWordRepository, SqliteWordRepository>(serviceProvider => serviceProvider.GetRequiredService<SqliteWordRepository>());
        services.AddTransient<IWordGroupRepository, SqliteWordGroupRepository>();
        services.AddTransient<IWordExerciseRepository, SqliteWordExerciseRepository>();
        services.AddTransient<SqliteWordExerciseRerunRepository>();
        services.AddTransient<IWordExerciseRerunRepository, SqliteWordExerciseRerunRepository>(serviceProvider => serviceProvider.GetRequiredService<SqliteWordExerciseRerunRepository>());

        services.AddTransient<IApplicationDictionaryRepository, ApplicationDictionaryRepository>();

        services.AddTransient<ITagRepository, SqliteTagRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICharacterService, CharacterService>();
        services.AddTransient<ICharacterGroupService, CharacterGroupService>();

        services.AddTransient<ICharacterExerciseService, CharacterExerciseService>();
        services.AddTransient<ICharacterExerciseGenerator, CharacterExerciseGenerator>();
        services.AddTransient<ICharacterExerciseBatchVisitor, RegularChallengeByExplicitlyDistributedTagsCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitor, RepeatedChallengeByPlaceholderCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitor, RepeatedChallengeByExceededRerunThresholdPlaceholderCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitor, AlwaysPresentRandomChallengeByPlaceholderCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitor, RegularChallengeCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitor, RepeatedChallengeByOriginalCharacterMatchCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitor, OrderShuffleCharacterExerciseBatchVisitor>();
        services.AddTransient<ICharacterExerciseBatchVisitorSequence, CharacterExerciseBatchVisitorSequence>();

        services.AddHosted<CharacterExerciseCleanupBackgroundService>();

        services.AddTransient<IWordService, WordService>();
        services.AddTransient<IWordGroupService, WordGroupService>();

        services.AddTransient<IWordExerciseService, WordExerciseService>();
        services.AddTransient<IWordExerciseGenerator, WordExerciseGenerator>();
        services.AddTransient<IWordExerciseBatchVisitor, RegularChallengeByExplicitlyDistributedTagsWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitor, RepeatedChallengeByPlaceholderWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitor, RepeatedChallengeByExceededRerunThresholdPlaceholderWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitor, AlwaysPresentRandomChallengeByPlaceholderWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitor, RegularChallengeWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitor, RepeatedChallengeByOriginalWordMatchWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitor, OrderShuffleWordExerciseBatchVisitor>();
        services.AddTransient<IWordExerciseBatchVisitorSequence, WordExerciseBatchVisitorSequence>();

        services.AddHosted<WordExerciseCleanupBackgroundService>();

        services.AddTransient<IApplicationDictionaryService, ApplicationDictionaryService>();

        services.AddTransient<ITagService, TagService>();

        services.AddTransient<ISnapshotHashCalculator, AlwaysZeroSnapshotHashCalculator>();

        services.AddHosted<MigrationsHistoryValidatorBackgroundService>();

        services.Configure<CharacterExerciseConfiguration>(configuration.GetSection(CharacterExerciseConfiguration.SectionName));
        services.Configure<WordExerciseConfiguration>(configuration.GetSection(WordExerciseConfiguration.SectionName));

        return services;
    }

    public static IServiceCollection AddConfiguredAutoMapper(this IServiceCollection services)
    {
        static void Configure(IMapperConfigurationExpression mapperConfigurationExpression)
        {
            mapperConfigurationExpression.AddBlobAutoMapperProfile();

            mapperConfigurationExpression.AddCommonAutoMapperProfiles();

            mapperConfigurationExpression.AddExportAutoMapperProfiles();
            mapperConfigurationExpression.AddImportAutoMapperProfiles();

            IGlobalConfigurationExpression globalConfigurationExpression = mapperConfigurationExpression.Internal();
            globalConfigurationExpression.ForAllMaps((_, map) =>
            {
                // GHSA-rvv3-g6hj-g44x : AutoMapper Vulnerable to Denial of Service (DoS) via Uncontrolled Recursion
                map.MaxDepth(64);
            });
        }

        services.AddAutoMapper(Configure);

        return services;
    }

    public static IServiceCollection AddConfiguredSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            string version = "v0.1.0-dev0";
            OpenApiContact contact = new()
            {
                Name = "Andrey Talanin",
                Email = "andrey.talanin@outlook.com",
                Url = new Uri("https://github.com/AndreyTalanin0x00"),
            };
            OpenApiLicense license = new()
            {
                Name = "The MIT License",
                Url = new Uri("https://github.com/AndreyTalanin0x00/JapaneseLanguageTools/blob/main/LICENSE.md"),
            };

            options.SwaggerDoc("JapaneseLanguageTools", new OpenApiInfo()
            {
                Title = $"Japanese Language Tools {version}",
                Description = "Initial pre-release (unstable) API version.",
                Version = version,
                Contact = contact,
                License = license,
            });

            options.SupportNonNullableReferenceTypes();
        });

        return services;
    }
}
