using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.YieldHelpers;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace JapaneseLanguageTools.Core.Services;

public class WordExerciseService : IWordExerciseService
{
    private static readonly TimeSpan s_wordExerciseBatchExpirationTimeout = TimeSpan.FromMinutes(2);

    private readonly IMapper m_mapper;
    private readonly IMemoryCache m_memoryCache;
    private readonly IWordExerciseGenerator m_wordExerciseGenerator;
    private readonly IWordExerciseRepository m_wordExerciseRepository;
    private readonly IWordExerciseRerunRepository m_wordExerciseRerunRepository;
    private readonly MainDbContext m_context;
    private readonly TimeProvider m_timeProvider;

    public WordExerciseService(IMapper mapper, IMemoryCache memoryCache, IWordExerciseGenerator wordExerciseGenerator, IWordExerciseRepository wordExerciseRepository, IWordExerciseRerunRepository wordExerciseRerunRepository, MainDbContext context, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_memoryCache = memoryCache;
        m_wordExerciseGenerator = wordExerciseGenerator;
        m_wordExerciseRepository = wordExerciseRepository;
        m_wordExerciseRerunRepository = wordExerciseRerunRepository;
        m_context = context;
        m_timeProvider = timeProvider;
    }

    #region CRUD Methods

    public async Task<WordExerciseModel?> GetWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default)
    {
        WordExerciseId[] wordExerciseIds = [wordExerciseId];

        WordExerciseModel? wordExerciseModel = (await GetWordExercisesAsync(wordExerciseIds, cancellationToken)).SingleOrDefault();

        return wordExerciseModel;
    }

    public async Task<WordExerciseModel[]> GetWordExercisesAsync(IEnumerable<WordExerciseId> wordExerciseIds, CancellationToken cancellationToken = default)
    {
        WordExercise[] wordExercises = await m_wordExerciseRepository.GetWordExercisesAsync(wordExerciseIds, cancellationToken);
        if (wordExercises.Length == 0)
            return [];

        WordExerciseModel[] wordExerciseModels = m_mapper.Map<WordExerciseModel[]>(wordExercises);

        RemoveNavigationPropertyCycles(wordExerciseModels);
        RemoveNavigationPropertyCycles(wordExerciseModels.Select(wordExerciseModel => wordExerciseModel.Word));

        return wordExerciseModels;
    }

    public async Task<WordExerciseModel[]> GetAllWordExercisesAsync(CancellationToken cancellationToken = default)
    {
        WordExercise[] wordExercises = await m_wordExerciseRepository.GetAllWordExercisesAsync(cancellationToken);
        if (wordExercises.Length == 0)
            return [];

        WordExerciseModel[] wordExerciseModels = m_mapper.Map<WordExerciseModel[]>(wordExercises);

        RemoveNavigationPropertyCycles(wordExerciseModels);
        RemoveNavigationPropertyCycles(wordExerciseModels.Select(wordExerciseModel => wordExerciseModel.Word));

        return wordExerciseModels;
    }

    public async Task<WordExerciseModel> AddWordExerciseAsync(WordExerciseModel wordExerciseModel, CancellationToken cancellationToken = default)
    {
        WordExercise wordExercise = m_mapper.Map<WordExercise>(wordExerciseModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        wordExercise.GeneratedOn = utcNow;

        WordExercise addedWordExercise = await m_wordExerciseRepository.AddWordExerciseAsync(wordExercise, cancellationToken);

        WordExerciseModel addedWordExerciseModel = m_mapper.Map<WordExerciseModel>(addedWordExercise);

        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedWordExerciseModel));
        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedWordExerciseModel.Word));

        return addedWordExerciseModel;
    }

    public async Task<bool> UpdateWordExerciseAsync(WordExerciseModel wordExerciseModel, CancellationToken cancellationToken = default)
    {
        WordExercise wordExercise = m_mapper.Map<WordExercise>(wordExerciseModel);

        bool updated = await m_wordExerciseRepository.UpdateWordExerciseAsync(wordExercise, cancellationToken);

        return updated;
    }

    public async Task<bool> RemoveWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_wordExerciseRepository.RemoveWordExerciseAsync(wordExerciseId, cancellationToken);

        return removed;
    }

    #endregion

    #region Request & Response Methods

    /// <inheritdoc />
    public Task<GetWordExerciseBatchResponseModel?> GetWordExerciseBatchAsync(GetWordExerciseBatchRequestModel getWordExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        WordExerciseBatchId wordExerciseBatchId = new(getWordExerciseBatchRequestModel.WordExerciseBatchId);

        string wordExerciseBatchCacheKey = GetWordExerciseBatchCacheKey(wordExerciseBatchId);

        Task<GetWordExerciseBatchResponseModel?> completedTask;
        if (m_memoryCache.TryGetValue(wordExerciseBatchCacheKey, out WordExerciseBatchModel? wordExerciseBatchModel) && wordExerciseBatchModel is not null)
        {
            GetWordExerciseBatchResponseModel getWordExerciseBatchResponseModel = new()
            {
                WordExerciseBatch = wordExerciseBatchModel,
                Request = getWordExerciseBatchRequestModel,
            };

            completedTask = Task.FromResult<GetWordExerciseBatchResponseModel?>(getWordExerciseBatchResponseModel);

            return completedTask;
        }

        completedTask = Task.FromResult<GetWordExerciseBatchResponseModel?>(null);

        return completedTask;
    }

    /// <inheritdoc />
    public async Task<GenerateWordExerciseBatchResponseModel> GenerateWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        GenerateWordExerciseBatchFullResponseModel generateWordExerciseBatchFullResponseModel =
            await m_wordExerciseGenerator.GenerateWordExerciseBatchAsync(generateWordExerciseBatchRequestModel, cancellationToken);

        WordExerciseBatchId wordExerciseBatchId = new(generateWordExerciseBatchFullResponseModel.WordExerciseBatchId);
        WordExerciseBatchModel wordExerciseBatchModel = generateWordExerciseBatchFullResponseModel.WordExerciseBatch;

        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        RemoveNavigationPropertyCycles(wordExerciseModels);
        RemoveNavigationPropertyCycles(wordExerciseModels.Select(wordExerciseModel => wordExerciseModel.Word));

        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            await SaveGeneratedExercisesAsync(wordExerciseBatchModel, cancellationToken);

            await contextTransaction.CommitAsync(cancellationToken);
        }

        string wordExerciseBatchCacheKey = GetWordExerciseBatchCacheKey(wordExerciseBatchId);

        m_memoryCache.Set(wordExerciseBatchCacheKey, wordExerciseBatchModel, s_wordExerciseBatchExpirationTimeout);

        GenerateWordExerciseBatchResponseModel generateWordExerciseBatchResponseModel = new()
        {
            WordExerciseBatchId = wordExerciseBatchId.Value,
            Request = generateWordExerciseBatchRequestModel,
        };

        return generateWordExerciseBatchResponseModel;
    }

    /// <inheritdoc />
    public async Task<SubmitCompletedWordExerciseDetailsResponseModel> SubmitCompletedWordExerciseDetailsAsync(SubmitCompletedWordExerciseDetailsRequestModel submitCompletedWordExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            HashSet<WordExerciseRerunId> wordExerciseRerunIds = [];
            foreach (WordExerciseModel wordExerciseModel in submitCompletedWordExerciseDetailsRequestModel.CompletedItems)
            {
                if (!wordExerciseModel.WordExerciseReruns.Any())
                    continue;

                WordExerciseRerunId wordExerciseRerunId;
                WordExerciseRerunModel wordExerciseRerunModel = wordExerciseModel.WordExerciseReruns.First();

                wordExerciseRerunId = new(wordExerciseRerunModel.Id);

                if (wordExerciseRerunIds.Contains(wordExerciseRerunId))
                    continue;

                wordExerciseRerunModel.ContinuousChallengeCount++;
                wordExerciseRerunModel.TotalChallengeCount++;

                if (wordExerciseRerunModel.ContinuousChallengeCount < wordExerciseRerunModel.RequiredChallengeCount)
                {
                    WordExerciseRerun wordExerciseRerun = m_mapper.Map<WordExerciseRerun>(wordExerciseRerunModel);
                    WordExerciseRerun? wordExerciseRerunToUpdate = await m_wordExerciseRerunRepository.GetWordExerciseRerunAsync(wordExerciseRerunId, cancellationToken);

                    if (wordExerciseRerunToUpdate is null)
                        continue;

                    wordExerciseRerunToUpdate.ContinuousChallengeCount = wordExerciseRerun.ContinuousChallengeCount;
                    wordExerciseRerunToUpdate.TotalChallengeCount = wordExerciseRerun.TotalChallengeCount;

                    await m_wordExerciseRerunRepository.UpdateWordExerciseRerunAsync(wordExerciseRerunToUpdate, cancellationToken);
                }
                else
                {
                    await m_wordExerciseRerunRepository.RemoveWordExerciseRerunAsync(wordExerciseRerunId, cancellationToken);
                }

                wordExerciseRerunIds.Add(wordExerciseRerunId);
            }

            await contextTransaction.CommitAsync(cancellationToken);
        }

        SubmitCompletedWordExerciseDetailsResponseModel submitCompletedWordExerciseDetailsResponseModel = new()
        {
            Request = submitCompletedWordExerciseDetailsRequestModel,
        };

        return submitCompletedWordExerciseDetailsResponseModel;
    }

    /// <inheritdoc />
    public async Task<SubmitFailedWordExerciseDetailsResponseModel> SubmitFailedWordExerciseDetailsAsync(SubmitFailedWordExerciseDetailsRequestModel submitFailedWordExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        GenerateWordExerciseBatchResponseModel generateWordExerciseBatchResponseModel = submitFailedWordExerciseDetailsRequestModel.OriginalResponse;
        GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel = generateWordExerciseBatchResponseModel.Request;

        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            HashSet<WordId> wordIds = [];
            foreach (WordExerciseModel wordExerciseModel in submitFailedWordExerciseDetailsRequestModel.FailedItems)
            {
                WordId wordId = new(wordExerciseModel.WordId);

                if (wordIds.Contains(wordId))
                    continue;

                if (!wordExerciseModel.WordExerciseReruns.Any())
                {
                    int requiredChallengeCount = GetRequiredChallengeCount(0, generateWordExerciseBatchRequestModel.UseWordExerciseProfile);

                    WordExerciseRerunModel wordExerciseRerunModel = new()
                    {
                        Id = 0,
                        WordExerciseId = wordExerciseModel.Id,
                        RequiredChallengeCount = requiredChallengeCount,
                        ContinuousChallengeCount = 0,
                        TotalChallengeCount = 0,
                        InitiallyScheduledOn = utcNow,
                        RepeatedlyScheduledOn = utcNow,
                        WordExercise = wordExerciseModel,
                    };

                    WordExerciseRerun wordExerciseRerun = m_mapper.Map<WordExerciseRerun>(wordExerciseRerunModel);

                    await m_wordExerciseRerunRepository.AddWordExerciseRerunAsync(wordExerciseRerun, cancellationToken);
                }
                else
                {
                    WordExerciseRerunId wordExerciseRerunId;
                    WordExerciseRerunModel wordExerciseRerunModel = wordExerciseModel.WordExerciseReruns.First();

                    wordExerciseRerunId = new(wordExerciseRerunModel.Id);

                    int currentChallengeCount = wordExerciseRerunModel.RequiredChallengeCount;
                    int updatedChallengeCount = GetRequiredChallengeCount(currentChallengeCount, generateWordExerciseBatchRequestModel.UseWordExerciseProfile);

                    wordExerciseRerunModel.RequiredChallengeCount = updatedChallengeCount;
                    wordExerciseRerunModel.ContinuousChallengeCount = 0;
                    wordExerciseRerunModel.TotalChallengeCount++;

                    wordExerciseRerunModel.RepeatedlyScheduledOn = utcNow;

                    WordExerciseRerun wordExerciseRerun = m_mapper.Map<WordExerciseRerun>(wordExerciseRerunModel);
                    WordExerciseRerun? wordExerciseRerunToUpdate = await m_wordExerciseRerunRepository.GetWordExerciseRerunAsync(wordExerciseRerunId, cancellationToken);

                    if (wordExerciseRerunToUpdate is null)
                        continue;

                    wordExerciseRerunToUpdate.RequiredChallengeCount = wordExerciseRerun.RequiredChallengeCount;
                    wordExerciseRerunToUpdate.ContinuousChallengeCount = wordExerciseRerun.ContinuousChallengeCount;
                    wordExerciseRerunToUpdate.TotalChallengeCount = wordExerciseRerun.TotalChallengeCount;

                    wordExerciseRerunToUpdate.RepeatedlyScheduledOn = wordExerciseRerun.RepeatedlyScheduledOn;

                    await m_wordExerciseRerunRepository.UpdateWordExerciseRerunAsync(wordExerciseRerun, cancellationToken);
                }

                wordIds.Add(wordId);
            }

            await contextTransaction.CommitAsync(cancellationToken);
        }

        SubmitFailedWordExerciseDetailsResponseModel submitFailedWordExerciseDetailsResponseModel = new()
        {
            Request = submitFailedWordExerciseDetailsRequestModel,
        };

        return submitFailedWordExerciseDetailsResponseModel;
    }

    #endregion

    protected static void RemoveNavigationPropertyCycles(IEnumerable<WordExerciseModel> wordExerciseModels)
    {
        foreach (WordExerciseModel wordExerciseModel in wordExerciseModels)
        {
            foreach (WordExerciseRerunModel wordExerciseRerunModel in wordExerciseModel.WordExerciseReruns)
                wordExerciseRerunModel.WordExercise = null;

            ;
        }
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<WordModel?> wordModels)
    {
        foreach (WordModel? wordModel in wordModels)
        {
            if (wordModel?.WordGroup?.Words.Count > 0)
                wordModel.WordGroup.Words = [];

            ;
        }
    }

    private static string GetWordExerciseBatchCacheKey(WordExerciseBatchId wordExerciseBatchId)
    {
        string wordExerciseBatchCacheKey = $"WordExerciseService:WordExerciseBatches:{wordExerciseBatchId}";
        return wordExerciseBatchCacheKey;
    }

    private static int GetRequiredChallengeCount(int currentChallengeCount, WordExerciseProfileModel? wordExerciseProfileModel)
    {
        IList<int> repeatedChallengeCountProgression =
            wordExerciseProfileModel?.RepeatedChallengeCountProgression ?? [];

        int updatedChallengeCount;
        updatedChallengeCount = repeatedChallengeCountProgression.FirstOrDefault(number => number > currentChallengeCount);
        updatedChallengeCount = updatedChallengeCount > 0 ? updatedChallengeCount : currentChallengeCount;

        return updatedChallengeCount;
    }

    private async Task SaveGeneratedExercisesAsync(WordExerciseBatchModel wordExerciseBatchModel, CancellationToken cancellationToken)
    {
        foreach (WordExerciseModel wordExerciseModel in wordExerciseBatchModel.Items)
        {
            if (wordExerciseModel.Id > 0)
                continue;

            WordExercise wordExercise = m_mapper.Map<WordExercise>(wordExerciseModel);
            wordExercise = await m_wordExerciseRepository.AddWordExerciseAsync(wordExercise, cancellationToken);

            wordExerciseModel.Id = wordExercise.Id;
        }
    }
}
