using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Core.Configuration;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Hosted;

public class WordExerciseCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory m_serviceScopeFactory;
    private readonly IOptions<WordExerciseConfiguration> m_configurationOptions;

    public WordExerciseCleanupBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<WordExerciseConfiguration> configurationOptions)
    {
        m_serviceScopeFactory = serviceScopeFactory;
        m_configurationOptions = configurationOptions;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Personal preference.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        WordExerciseConfiguration configuration = m_configurationOptions.Value;

        TimeSpan repeatedChallengeExpirationTimeout = configuration.RepeatedChallengeExpirationTimeout;
        if (repeatedChallengeExpirationTimeout == default(TimeSpan))
            repeatedChallengeExpirationTimeout = TimeSpan.FromDays(30);

        static WordExerciseFilter CreateWordExerciseSlidingFilter(int offset, int count)
        {
            IQueryable<WordExercise> FilterWordExercises(IQueryable<WordExercise> wordExercises)
            {
                IQueryable<WordExercise> wordExercisesFiltered = wordExercises
                    .Where(wordExercise => !wordExercise.WordExerciseReruns.Any())
                    .OrderBy(wordExercise => wordExercise.GeneratedOn)
                    .Skip(offset)
                    .Take(count);

                return wordExercisesFiltered;
            }

            WordExerciseFilter wordExerciseFilter = new()
            {
                CustomFilter = FilterWordExercises,
            };

            return wordExerciseFilter;
        }

        async Task RunCleanupAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope serviceScope = m_serviceScopeFactory.CreateScope())
            {
                MainDbContext context = serviceScope.ServiceProvider.GetRequiredService<MainDbContext>();
                IWordExerciseRepository wordExerciseRepository = serviceScope.ServiceProvider.GetRequiredService<IWordExerciseRepository>();
                TimeProvider timeProvider = serviceScope.ServiceProvider.GetRequiredService<TimeProvider>();

                using (IDbContextTransaction contextTransaction = await context.Database.BeginTransactionAsync(stoppingToken))
                {
                    int offset = 0;
                    const int batchSize = 4000;

                    bool reachedEnd = false, reachedRecentWordExercises = false;
                    while (!reachedEnd && !reachedRecentWordExercises)
                    {
                        WordExerciseFilter wordExerciseFilter = CreateWordExerciseSlidingFilter(offset, batchSize);

                        WordExercise[] wordExercises = await wordExerciseRepository.GetWordExercisesAsync(wordExerciseFilter, stoppingToken);

                        offset += batchSize;
                        if (wordExercises.Length < batchSize)
                            reachedEnd = true;

                        DateTimeOffset utcNow = timeProvider.GetUtcNow();

                        WordExerciseId[] wordExerciseIdsToRemove = wordExercises
                            .Where(wordExercise => (utcNow - wordExercise.GeneratedOn) > repeatedChallengeExpirationTimeout)
                            .Select(wordExercise => wordExercise.Id)
                            .Select(wordExerciseId => new WordExerciseId(wordExerciseId))
                            .ToArray();

                        if (wordExerciseIdsToRemove.Length < wordExercises.Length)
                            reachedRecentWordExercises = true;

                        foreach (WordExerciseId wordExerciseIdToRemove in wordExerciseIdsToRemove)
                            await wordExerciseRepository.RemoveWordExerciseAsync(wordExerciseIdToRemove, stoppingToken);

                        ;
                    }

                    await contextTransaction.CommitAsync(stoppingToken);
                }
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCleanupAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
        }
    }
}
