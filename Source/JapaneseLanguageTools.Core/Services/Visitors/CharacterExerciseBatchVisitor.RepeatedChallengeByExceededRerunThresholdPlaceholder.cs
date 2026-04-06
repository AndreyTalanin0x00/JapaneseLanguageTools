using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Constants;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class RepeatedChallengeByExceededRerunThresholdPlaceholderCharacterExerciseBatchVisitor : ICharacterExerciseBatchVisitor
{
    private readonly IMapper m_mapper;
    private readonly ICharacterExerciseRepository m_characterExerciseRepository;
    private readonly Random m_random;

    public RepeatedChallengeByExceededRerunThresholdPlaceholderCharacterExerciseBatchVisitor(IMapper mapper, ICharacterExerciseRepository characterExerciseRepository, Random random)
    {
        m_mapper = mapper;
        m_characterExerciseRepository = characterExerciseRepository;
        m_random = random;
    }

    /// <inheritdoc />
    public async Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        CharacterExerciseModel[] existingCharacterExerciseModels;
        Dictionary<CharacterTypes, CharacterExerciseModel[]> existingCharacterExerciseModelsByCharacterTypes;
        {
            IQueryable<CharacterExercise> FilterExistingCharacterExercises(IQueryable<CharacterExercise> characterExercises)
            {
                Expression<Func<CharacterExercise, bool>> characterExercisePredicate = characterExercise => characterExercise.CharacterExerciseReruns
                    .Where(characterExerciseRerun => characterExerciseRerun.RequiredChallengeCount < (characterExerciseRerun.TotalChallengeCount - characterExerciseRerun.ContinuousChallengeCount))
                    .Any();

                return characterExercises.Where(characterExercisePredicate);
            }

            CharacterExerciseFilter existingCharacterExerciseFilter = new()
            {
                CustomFilter = FilterExistingCharacterExercises,
            };

            CharacterExercise[] existingCharacterExercises =
                await m_characterExerciseRepository.GetCharacterExercisesAsync(existingCharacterExerciseFilter, cancellationToken);

            existingCharacterExerciseModels = m_mapper.Map<CharacterExerciseModel[]>(existingCharacterExercises);

            for (int i = 0; i < existingCharacterExerciseModels.Length; i++)
            {
                int j = m_random.Next(existingCharacterExerciseModels.Length);
                (existingCharacterExerciseModels[i], existingCharacterExerciseModels[j]) = (existingCharacterExerciseModels[j], existingCharacterExerciseModels[i]);
            }

            CharacterTypes KeySelector(IGrouping<CharacterTypes, CharacterExerciseModel> characterExerciseModelGrouping) =>
                characterExerciseModelGrouping.Key;

            CharacterExerciseModel[] ElementSelector(IGrouping<CharacterTypes, CharacterExerciseModel> characterExerciseModelGrouping) =>
                characterExerciseModelGrouping.ToArray();

            existingCharacterExerciseModelsByCharacterTypes = existingCharacterExerciseModels
                .GroupBy(characterExerciseModel => characterExerciseModel.Character?.Type
                    ?? throw new UnreachableException($"Character exercise (#{characterExerciseModel.Id}) does not contain a reference to a character."))
                .ToDictionary(KeySelector, ElementSelector);

            ;
        }

        if (!characterExerciseModels.Any(IsRepeatedChallengePlaceholder))
            return;

        List<(int CharacterExerciseIndex, CharacterExerciseId[] ExistingCharacterExerciseIds)> characterExerciseIndexPairs = [];
        for (int characterExerciseIndex = 0; characterExerciseIndex < characterExerciseModels.Count; characterExerciseIndex++)
        {
            CharacterExerciseModel characterExerciseModel = characterExerciseModels[characterExerciseIndex];

            if (characterExerciseModel.Character is null || !IsRepeatedChallengePlaceholder(characterExerciseModel))
                continue;

            CharacterTypes characterType = characterExerciseModel.Character.Type;
            if (!existingCharacterExerciseModelsByCharacterTypes.TryGetValue(characterType, out CharacterExerciseModel[]? existingCharacterExerciseModelsForCharacterType))
                existingCharacterExerciseModelsForCharacterType = [];

            if (existingCharacterExerciseModelsForCharacterType.Length == 0)
            {
                characterExerciseModels.RemoveAt(characterExerciseIndex--);
                continue;
            }

            CharacterExerciseId[] existingCharacterExerciseIds = existingCharacterExerciseModelsForCharacterType
                .Select(characterExerciseModel => new CharacterExerciseId(characterExerciseModel.Id))
                .ToArray();

            characterExerciseIndexPairs.Add((characterExerciseIndex, existingCharacterExerciseIds));
        }

        Dictionary<CharacterExerciseId, CharacterExerciseModel> existingCharacterExerciseModelsByIds;
        {
            CharacterExerciseId KeySelector(CharacterExerciseModel characterExercise) => new(characterExercise.Id);

            existingCharacterExerciseModelsByIds = existingCharacterExerciseModels.ToDictionary(KeySelector);
        }

        int extraCharacterExerciseIndexOffset = 0;
        foreach ((int characterExerciseIndex, CharacterExerciseId[] existingCharacterExerciseIds) in characterExerciseIndexPairs)
        {
            bool extraCharacterExercise = false;
            foreach (CharacterExerciseId existingCharacterExerciseId in existingCharacterExerciseIds)
            {
                if (!existingCharacterExerciseModelsByIds.TryGetValue(existingCharacterExerciseId, out CharacterExerciseModel? existingCharacterExercise))
                    continue;

                if (extraCharacterExercise)
                {
                    extraCharacterExerciseIndexOffset++;
                    characterExerciseModels.Insert(characterExerciseIndex + extraCharacterExerciseIndexOffset, existingCharacterExercise);
                }
                else
                {
                    characterExerciseModels[characterExerciseIndex + extraCharacterExerciseIndexOffset] = existingCharacterExercise;
                    extraCharacterExercise = true;
                }
            }
        }
    }

    private static bool IsRepeatedChallengePlaceholder(CharacterExerciseModel characterExerciseModel)
    {
        if (characterExerciseModel.Character is null)
            return false;

        foreach (TagModel tagModel in characterExerciseModel.Character.CharacterTags)
        {
            if (tagModel.PlaceholderMarker?.Equals(SystemReservedTags.RepeatedChallengeExceededRerunThresholdTagPlaceholderMarker, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;
        }

        return false;
    }
}
