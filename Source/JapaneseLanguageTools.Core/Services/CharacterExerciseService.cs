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

public class CharacterExerciseService : ICharacterExerciseService
{
    private static readonly TimeSpan s_characterExerciseBatchExpirationTimeout = TimeSpan.FromMinutes(2);

    private readonly IMapper m_mapper;
    private readonly IMemoryCache m_memoryCache;
    private readonly ICharacterExerciseGenerator m_characterExerciseGenerator;
    private readonly ICharacterExerciseRepository m_characterExerciseRepository;
    private readonly ICharacterExerciseRerunRepository m_characterExerciseRerunRepository;
    private readonly MainDbContext m_context;
    private readonly TimeProvider m_timeProvider;

    public CharacterExerciseService(IMapper mapper, IMemoryCache memoryCache, ICharacterExerciseGenerator characterExerciseGenerator, ICharacterExerciseRepository characterExerciseRepository, ICharacterExerciseRerunRepository characterExerciseRerunRepository, MainDbContext context, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_memoryCache = memoryCache;
        m_characterExerciseGenerator = characterExerciseGenerator;
        m_characterExerciseRepository = characterExerciseRepository;
        m_characterExerciseRerunRepository = characterExerciseRerunRepository;
        m_context = context;
        m_timeProvider = timeProvider;
    }

    #region CRUD Methods

    /// <inheritdoc />
    public async Task<CharacterExerciseModel?> GetCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default)
    {
        CharacterExerciseId[] characterExerciseIds = [characterExerciseId];

        CharacterExerciseModel? characterExerciseModel = (await GetCharacterExercisesAsync(characterExerciseIds, cancellationToken)).SingleOrDefault();

        return characterExerciseModel;
    }

    /// <inheritdoc />
    public async Task<CharacterExerciseModel[]> GetCharacterExercisesAsync(IEnumerable<CharacterExerciseId> characterExerciseIds, CancellationToken cancellationToken = default)
    {
        CharacterExercise[] characterExercises = await m_characterExerciseRepository.GetCharacterExercisesAsync(characterExerciseIds, cancellationToken);
        if (characterExercises.Length == 0)
            return [];

        CharacterExerciseModel[] characterExerciseModels = m_mapper.Map<CharacterExerciseModel[]>(characterExercises);

        RemoveNavigationPropertyCycles(characterExerciseModels);
        RemoveNavigationPropertyCycles(characterExerciseModels.Select(characterExerciseModel => characterExerciseModel.Character));

        return characterExerciseModels;
    }

    /// <inheritdoc />
    public async Task<CharacterExerciseModel[]> GetAllCharacterExercisesAsync(CancellationToken cancellationToken = default)
    {
        CharacterExercise[] characterExercises = await m_characterExerciseRepository.GetAllCharacterExercisesAsync(cancellationToken);
        if (characterExercises.Length == 0)
            return [];

        CharacterExerciseModel[] characterExerciseModels = m_mapper.Map<CharacterExerciseModel[]>(characterExercises);

        RemoveNavigationPropertyCycles(characterExerciseModels);
        RemoveNavigationPropertyCycles(characterExerciseModels.Select(characterExerciseModel => characterExerciseModel.Character));

        return characterExerciseModels;
    }

    /// <inheritdoc />
    public async Task<CharacterExerciseModel> AddCharacterExerciseAsync(CharacterExerciseModel characterExerciseModel, CancellationToken cancellationToken = default)
    {
        CharacterExercise characterExercise = m_mapper.Map<CharacterExercise>(characterExerciseModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        characterExercise.GeneratedOn = utcNow;

        CharacterExercise addedCharacterExercise = await m_characterExerciseRepository.AddCharacterExerciseAsync(characterExercise, cancellationToken);

        CharacterExerciseModel addedCharacterExerciseModel = m_mapper.Map<CharacterExerciseModel>(addedCharacterExercise);

        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedCharacterExerciseModel));
        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedCharacterExerciseModel.Character));

        return addedCharacterExerciseModel;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterExerciseAsync(CharacterExerciseModel characterExerciseModel, CancellationToken cancellationToken = default)
    {
        CharacterExercise characterExercise = m_mapper.Map<CharacterExercise>(characterExerciseModel);

        bool updated = await m_characterExerciseRepository.UpdateCharacterExerciseAsync(characterExercise, cancellationToken);

        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_characterExerciseRepository.RemoveCharacterExerciseAsync(characterExerciseId, cancellationToken);

        return removed;
    }

    #endregion

    #region Request & Response Methods

    /// <inheritdoc />
    public Task<GetCharacterExerciseBatchResponseModel?> GetCharacterExerciseBatchAsync(GetCharacterExerciseBatchRequestModel getCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        CharacterExerciseBatchId characterExerciseBatchId = new(getCharacterExerciseBatchRequestModel.CharacterExerciseBatchId);

        string characterExerciseBatchCacheKey = GetCharacterExerciseBatchCacheKey(characterExerciseBatchId);

        Task<GetCharacterExerciseBatchResponseModel?> completedTask;
        if (m_memoryCache.TryGetValue(characterExerciseBatchCacheKey, out CharacterExerciseBatchModel? characterExerciseBatchModel) && characterExerciseBatchModel is not null)
        {
            GetCharacterExerciseBatchResponseModel getCharacterExerciseBatchResponseModel = new()
            {
                CharacterExerciseBatch = characterExerciseBatchModel,
                Request = getCharacterExerciseBatchRequestModel,
            };

            completedTask = Task.FromResult<GetCharacterExerciseBatchResponseModel?>(getCharacterExerciseBatchResponseModel);

            return completedTask;
        }

        completedTask = Task.FromResult<GetCharacterExerciseBatchResponseModel?>(null);

        return completedTask;
    }

    /// <inheritdoc />
    public async Task<GenerateCharacterExerciseBatchResponseModel> GenerateCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        GenerateCharacterExerciseBatchFullResponseModel generateCharacterExerciseBatchFullResponseModel =
            await m_characterExerciseGenerator.GenerateCharacterExerciseBatchAsync(generateCharacterExerciseBatchRequestModel, cancellationToken);

        CharacterExerciseBatchId characterExerciseBatchId = new(generateCharacterExerciseBatchFullResponseModel.CharacterExerciseBatchId);
        CharacterExerciseBatchModel characterExerciseBatchModel = generateCharacterExerciseBatchFullResponseModel.CharacterExerciseBatch;

        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        RemoveNavigationPropertyCycles(characterExerciseModels);
        RemoveNavigationPropertyCycles(characterExerciseModels.Select(characterExerciseModel => characterExerciseModel.Character));

        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            await SaveGeneratedExercisesAsync(characterExerciseBatchModel, cancellationToken);

            await contextTransaction.CommitAsync(cancellationToken);
        }

        string characterExerciseBatchCacheKey = GetCharacterExerciseBatchCacheKey(characterExerciseBatchId);

        m_memoryCache.Set(characterExerciseBatchCacheKey, characterExerciseBatchModel, s_characterExerciseBatchExpirationTimeout);

        GenerateCharacterExerciseBatchResponseModel generateCharacterExerciseBatchResponseModel = new()
        {
            CharacterExerciseBatchId = characterExerciseBatchId.Value,
            Request = generateCharacterExerciseBatchRequestModel,
        };

        return generateCharacterExerciseBatchResponseModel;
    }

    /// <inheritdoc />
    public async Task<SubmitCompletedCharacterExerciseDetailsResponseModel> SubmitCompletedCharacterExerciseDetailsAsync(SubmitCompletedCharacterExerciseDetailsRequestModel submitCompletedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            HashSet<CharacterExerciseRerunId> characterExerciseRerunIds = [];
            foreach (CharacterExerciseModel characterExerciseModel in submitCompletedCharacterExerciseDetailsRequestModel.CompletedItems)
            {
                if (!characterExerciseModel.CharacterExerciseReruns.Any())
                    continue;

                CharacterExerciseRerunId characterExerciseRerunId;
                CharacterExerciseRerunModel characterExerciseRerunModel = characterExerciseModel.CharacterExerciseReruns.First();

                characterExerciseRerunId = new(characterExerciseRerunModel.Id);

                if (characterExerciseRerunIds.Contains(characterExerciseRerunId))
                    continue;

                characterExerciseRerunModel.ContinuousChallengeCount++;
                characterExerciseRerunModel.TotalChallengeCount++;

                if (characterExerciseRerunModel.ContinuousChallengeCount < characterExerciseRerunModel.RequiredChallengeCount)
                {
                    CharacterExerciseRerun characterExerciseRerun = m_mapper.Map<CharacterExerciseRerun>(characterExerciseRerunModel);
                    CharacterExerciseRerun? characterExerciseRerunToUpdate = await m_characterExerciseRerunRepository.GetCharacterExerciseRerunAsync(characterExerciseRerunId, cancellationToken);

                    if (characterExerciseRerunToUpdate is null)
                        continue;

                    characterExerciseRerunToUpdate.ContinuousChallengeCount = characterExerciseRerun.ContinuousChallengeCount;
                    characterExerciseRerunToUpdate.TotalChallengeCount = characterExerciseRerun.TotalChallengeCount;

                    await m_characterExerciseRerunRepository.UpdateCharacterExerciseRerunAsync(characterExerciseRerunToUpdate, cancellationToken);
                }
                else
                {
                    await m_characterExerciseRerunRepository.RemoveCharacterExerciseRerunAsync(characterExerciseRerunId, cancellationToken);
                }

                characterExerciseRerunIds.Add(characterExerciseRerunId);
            }

            await contextTransaction.CommitAsync(cancellationToken);
        }

        SubmitCompletedCharacterExerciseDetailsResponseModel submitCompletedCharacterExerciseDetailsResponseModel = new()
        {
            Request = submitCompletedCharacterExerciseDetailsRequestModel,
        };

        return submitCompletedCharacterExerciseDetailsResponseModel;
    }

    /// <inheritdoc />
    public async Task<SubmitFailedCharacterExerciseDetailsResponseModel> SubmitFailedCharacterExerciseDetailsAsync(SubmitFailedCharacterExerciseDetailsRequestModel submitFailedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        GenerateCharacterExerciseBatchResponseModel generateCharacterExerciseBatchResponseModel = submitFailedCharacterExerciseDetailsRequestModel.OriginalResponse;
        GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel = generateCharacterExerciseBatchResponseModel.Request;

        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            HashSet<CharacterId> characterIds = [];
            foreach (CharacterExerciseModel characterExerciseModel in submitFailedCharacterExerciseDetailsRequestModel.FailedItems)
            {
                CharacterId characterId = new(characterExerciseModel.CharacterId);

                if (characterIds.Contains(characterId))
                    continue;

                if (!characterExerciseModel.CharacterExerciseReruns.Any())
                {
                    int requiredChallengeCount = GetRequiredChallengeCount(0, generateCharacterExerciseBatchRequestModel.UseCharacterExerciseProfile);

                    CharacterExerciseRerunModel characterExerciseRerunModel = new()
                    {
                        Id = 0,
                        CharacterExerciseId = characterExerciseModel.Id,
                        RequiredChallengeCount = requiredChallengeCount,
                        ContinuousChallengeCount = 0,
                        TotalChallengeCount = 0,
                        InitiallyScheduledOn = utcNow,
                        RepeatedlyScheduledOn = utcNow,
                        CharacterExercise = characterExerciseModel,
                    };

                    CharacterExerciseRerun characterExerciseRerun = m_mapper.Map<CharacterExerciseRerun>(characterExerciseRerunModel);

                    await m_characterExerciseRerunRepository.AddCharacterExerciseRerunAsync(characterExerciseRerun, cancellationToken);
                }
                else
                {
                    CharacterExerciseRerunId characterExerciseRerunId;
                    CharacterExerciseRerunModel characterExerciseRerunModel = characterExerciseModel.CharacterExerciseReruns.First();

                    characterExerciseRerunId = new(characterExerciseRerunModel.Id);

                    int currentChallengeCount = characterExerciseRerunModel.RequiredChallengeCount;
                    int updatedChallengeCount = GetRequiredChallengeCount(currentChallengeCount, generateCharacterExerciseBatchRequestModel.UseCharacterExerciseProfile);

                    characterExerciseRerunModel.RequiredChallengeCount = updatedChallengeCount;
                    characterExerciseRerunModel.ContinuousChallengeCount = 0;
                    characterExerciseRerunModel.TotalChallengeCount++;

                    characterExerciseRerunModel.RepeatedlyScheduledOn = utcNow;

                    CharacterExerciseRerun characterExerciseRerun = m_mapper.Map<CharacterExerciseRerun>(characterExerciseRerunModel);
                    CharacterExerciseRerun? characterExerciseRerunToUpdate = await m_characterExerciseRerunRepository.GetCharacterExerciseRerunAsync(characterExerciseRerunId, cancellationToken);

                    if (characterExerciseRerunToUpdate is null)
                        continue;

                    characterExerciseRerunToUpdate.RequiredChallengeCount = characterExerciseRerun.RequiredChallengeCount;
                    characterExerciseRerunToUpdate.ContinuousChallengeCount = characterExerciseRerun.ContinuousChallengeCount;
                    characterExerciseRerunToUpdate.TotalChallengeCount = characterExerciseRerun.TotalChallengeCount;

                    characterExerciseRerunToUpdate.RepeatedlyScheduledOn = characterExerciseRerun.RepeatedlyScheduledOn;

                    await m_characterExerciseRerunRepository.UpdateCharacterExerciseRerunAsync(characterExerciseRerun, cancellationToken);
                }

                characterIds.Add(characterId);
            }

            await contextTransaction.CommitAsync(cancellationToken);
        }

        SubmitFailedCharacterExerciseDetailsResponseModel submitFailedCharacterExerciseDetailsResponseModel = new()
        {
            Request = submitFailedCharacterExerciseDetailsRequestModel,
        };

        return submitFailedCharacterExerciseDetailsResponseModel;
    }

    #endregion

    protected static void RemoveNavigationPropertyCycles(IEnumerable<CharacterExerciseModel> characterExerciseModels)
    {
        foreach (CharacterExerciseModel characterExerciseModel in characterExerciseModels)
        {
            foreach (CharacterExerciseRerunModel characterExerciseRerunModel in characterExerciseModel.CharacterExerciseReruns)
                characterExerciseRerunModel.CharacterExercise = null;

            ;
        }
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<CharacterModel?> characterModels)
    {
        foreach (CharacterModel? characterModel in characterModels)
        {
            if (characterModel?.CharacterGroup?.Characters.Count > 0)
                characterModel.CharacterGroup.Characters = [];

            ;
        }
    }

    private static string GetCharacterExerciseBatchCacheKey(CharacterExerciseBatchId characterExerciseBatchId)
    {
        string characterExerciseBatchCacheKey = $"CharacterExerciseService:CharacterExerciseBatches:{characterExerciseBatchId}";
        return characterExerciseBatchCacheKey;
    }

    private static int GetRequiredChallengeCount(int currentChallengeCount, CharacterExerciseProfileModel? characterExerciseProfileModel)
    {
        IList<int> repeatedChallengeCountProgression =
            characterExerciseProfileModel?.RepeatedChallengeCountProgression ?? [];

        int updatedChallengeCount;
        updatedChallengeCount = repeatedChallengeCountProgression.FirstOrDefault(number => number > currentChallengeCount);
        updatedChallengeCount = updatedChallengeCount > 0 ? updatedChallengeCount : currentChallengeCount;

        return updatedChallengeCount;
    }

    private async Task SaveGeneratedExercisesAsync(CharacterExerciseBatchModel characterExerciseBatchModel, CancellationToken cancellationToken)
    {
        foreach (CharacterExerciseModel characterExerciseModel in characterExerciseBatchModel.Items)
        {
            if (characterExerciseModel.Id > 0)
                continue;

            CharacterExercise characterExercise = m_mapper.Map<CharacterExercise>(characterExerciseModel);
            characterExercise = await m_characterExerciseRepository.AddCharacterExerciseAsync(characterExercise, cancellationToken);

            characterExerciseModel.Id = characterExercise.Id;
        }
    }
}
