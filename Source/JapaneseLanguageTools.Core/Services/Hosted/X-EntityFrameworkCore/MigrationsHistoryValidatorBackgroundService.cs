using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Data.Contexts;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Services.Hosted;

public class MigrationsHistoryValidatorBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory m_serviceScopeFactory;

    public MigrationsHistoryValidatorBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        m_serviceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Personal preference.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (IServiceScope serviceScope = m_serviceScopeFactory.CreateScope())
        {
            MainDbContext context = serviceScope.ServiceProvider.GetRequiredService<MainDbContext>();
            IServiceProvider efCoreServiceProvider = (context as IInfrastructure<IServiceProvider>).Instance;

            string currentMigrationId = GetCurrentMigrationId(efCoreServiceProvider);
            string currentInstalledMigrationId = await GetCurrentInstalledMigrationIdAsync(efCoreServiceProvider, stoppingToken);

            if (currentMigrationId != currentInstalledMigrationId)
            {
                string exceptionMessage = $"The currently installed database migration ({currentInstalledMigrationId}) does not match the desired one ({currentMigrationId}). "
                    + "Please install the latest database migration before launching the application.";

                throw new Exception(exceptionMessage);
            }
        }
    }

    private static string GetCurrentMigrationId(IServiceProvider efCoreServiceProvider)
    {
        IMigrationsAssembly migrationsAssembly = efCoreServiceProvider.GetRequiredService<IMigrationsAssembly>();

        (string? currentMigrationId, System.Reflection.TypeInfo? currentMigrationType) = migrationsAssembly.Migrations
            .OrderByDescending(migrationIdValuePair => migrationIdValuePair.Key)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(currentMigrationId))
        {
            string exceptionMessage = "Unable to get a list of .NET types representing database migrations.";

            throw new Exception(exceptionMessage);
        }

        return currentMigrationId;
    }

    private static async Task<string> GetCurrentInstalledMigrationIdAsync(IServiceProvider efCoreServiceProvider, CancellationToken cancellationToken)
    {
        IHistoryRepository historyRepository = efCoreServiceProvider.GetRequiredService<IHistoryRepository>();

        HistoryRow? currentInstalledMigration = (await historyRepository.GetAppliedMigrationsAsync(cancellationToken))
            .OrderByDescending(historyRow => historyRow.MigrationId)
            .FirstOrDefault();

        string? currentInstalledMigrationId = currentInstalledMigration?.MigrationId;

        if (string.IsNullOrEmpty(currentInstalledMigrationId))
        {
            string exceptionMessage = "Unable to get at least one installed Entity Framework Core database migration. "
                + "Make sure that the InitialMigration migration is installed.";

            throw new Exception(exceptionMessage);
        }

        return currentInstalledMigrationId;
    }
}
