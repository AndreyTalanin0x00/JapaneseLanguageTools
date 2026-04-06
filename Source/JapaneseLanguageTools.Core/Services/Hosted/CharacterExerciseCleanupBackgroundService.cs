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

public class CharacterExerciseCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory m_serviceScopeFactory;
    private readonly IOptions<CharacterExerciseConfiguration> m_configurationOptions;

    public CharacterExerciseCleanupBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<CharacterExerciseConfiguration> configurationOptions)
    {
        m_serviceScopeFactory = serviceScopeFactory;
        m_configurationOptions = configurationOptions;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Personal preference.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CharacterExerciseConfiguration configuration = m_configurationOptions.Value;

        TimeSpan repeatedChallengeExpirationTimeout = configuration.RepeatedChallengeExpirationTimeout;
        if (repeatedChallengeExpirationTimeout == default(TimeSpan))
            repeatedChallengeExpirationTimeout = TimeSpan.FromDays(30);

        static CharacterExerciseFilter CreateCharacterExerciseSlidingFilter(int offset, int count)
        {
            IQueryable<CharacterExercise> FilterCharacterExercises(IQueryable<CharacterExercise> characterExercises)
            {
                IQueryable<CharacterExercise> characterExercisesFiltered = characterExercises
                    .Where(characterExercise => !characterExercise.CharacterExerciseReruns.Any())
                    .OrderBy(characterExercise => characterExercise.GeneratedOn)
                    .Skip(offset)
                    .Take(count);

                return characterExercisesFiltered;
            }

            CharacterExerciseFilter characterExerciseFilter = new()
            {
                CustomFilter = FilterCharacterExercises,
            };

            return characterExerciseFilter;
        }

        async Task RunCleanupAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope serviceScope = m_serviceScopeFactory.CreateScope())
            {
                MainDbContext context = serviceScope.ServiceProvider.GetRequiredService<MainDbContext>();
                ICharacterExerciseRepository characterExerciseRepository = serviceScope.ServiceProvider.GetRequiredService<ICharacterExerciseRepository>();
                TimeProvider timeProvider = serviceScope.ServiceProvider.GetRequiredService<TimeProvider>();

                using (IDbContextTransaction contextTransaction = await context.Database.BeginTransactionAsync(stoppingToken))
                {
                    int offset = 0;
                    const int batchSize = 4000;

                    bool reachedEnd = false, reachedRecentCharacterExercises = false;
                    while (!reachedEnd && !reachedRecentCharacterExercises)
                    {
                        CharacterExerciseFilter characterExerciseFilter = CreateCharacterExerciseSlidingFilter(offset, batchSize);

                        CharacterExercise[] characterExercises = await characterExerciseRepository.GetCharacterExercisesAsync(characterExerciseFilter, stoppingToken);

                        offset += batchSize;
                        if (characterExercises.Length < batchSize)
                            reachedEnd = true;

                        DateTimeOffset utcNow = timeProvider.GetUtcNow();

                        CharacterExerciseId[] characterExerciseIdsToRemove = characterExercises
                            .Where(characterExercise => (utcNow - characterExercise.GeneratedOn) > repeatedChallengeExpirationTimeout)
                            .Select(characterExercise => characterExercise.Id)
                            .Select(characterExerciseId => new CharacterExerciseId(characterExerciseId))
                            .ToArray();

                        if (characterExerciseIdsToRemove.Length < characterExercises.Length)
                            reachedRecentCharacterExercises = true;

                        foreach (CharacterExerciseId characterExerciseIdToRemove in characterExerciseIdsToRemove)
                            await characterExerciseRepository.RemoveCharacterExerciseAsync(characterExerciseIdToRemove, stoppingToken);

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
