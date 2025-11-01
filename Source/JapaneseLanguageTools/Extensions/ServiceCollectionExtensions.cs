using System;
using System.IO;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Sqlite.Contexts;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

// Disable the IDE0001 (Simplify name) notification to preserve explicit service types.
#pragma warning disable IDE0001

namespace JapaneseLanguageTools.Extensions;

public static class ServiceCollectionExtensions
{
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
                    .MigrationsHistoryTable("MigrationsHistory")
                    .CommandTimeout(120);
            });
        });

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
