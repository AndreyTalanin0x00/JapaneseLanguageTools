using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

public class RepeatedChallengeByExceededRerunThresholdPlaceholderWordExerciseBatchVisitor : IWordExerciseBatchVisitor
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly CharacterTypes[] s_characterTypes = new CharacterTypes[]
    {
        CharacterTypes.Hiragana,
        CharacterTypes.Katakana,
        CharacterTypes.Kanji,
    };

    private readonly IMapper m_mapper;
    private readonly IWordExerciseRepository m_wordExerciseRepository;
    private readonly Random m_random;

    public RepeatedChallengeByExceededRerunThresholdPlaceholderWordExerciseBatchVisitor(IMapper mapper, IWordExerciseRepository wordExerciseRepository, Random random)
    {
        m_mapper = mapper;
        m_wordExerciseRepository = wordExerciseRepository;
        m_random = random;
    }

    /// <inheritdoc />
    public async Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        WordExerciseModel[] existingWordExerciseModels;
        Dictionary<CharacterTypes, WordExerciseModel[]> existingWordExerciseModelsByCharacterTypeCombinations;
        {
            IQueryable<WordExercise> FilterExistingWordExercises(IQueryable<WordExercise> wordExercises)
            {
                Expression<Func<WordExercise, bool>> wordExercisePredicate = wordExercise => wordExercise.WordExerciseReruns
                    .Where(wordExerciseRerun => wordExerciseRerun.RequiredChallengeCount < (wordExerciseRerun.TotalChallengeCount - wordExerciseRerun.ContinuousChallengeCount))
                    .Any();

                return wordExercises.Where(wordExercisePredicate);
            }

            WordExerciseFilter existingWordExerciseFilter = new()
            {
                CustomFilter = FilterExistingWordExercises,
            };

            WordExercise[] existingWordExercises =
                await m_wordExerciseRepository.GetWordExercisesAsync(existingWordExerciseFilter, cancellationToken);

            existingWordExerciseModels = m_mapper.Map<WordExerciseModel[]>(existingWordExercises);

            for (int i = 0; i < existingWordExerciseModels.Length; i++)
            {
                int j = m_random.Next(existingWordExerciseModels.Length);
                (existingWordExerciseModels[i], existingWordExerciseModels[j]) = (existingWordExerciseModels[j], existingWordExerciseModels[i]);
            }

            CharacterTypes GroupingKeySelector((WordExerciseModel WordExerciseModel, CharacterTypes CharacterType) wordExerciseTuple) =>
                wordExerciseTuple.CharacterType;

            WordExerciseModel GroupingElementSelector((WordExerciseModel WordExerciseModel, CharacterTypes CharacterType) wordExerciseTuple) =>
                wordExerciseTuple.WordExerciseModel;

            CharacterTypes DictionaryKeySelector(IGrouping<CharacterTypes, WordExerciseModel> wordExerciseModelGrouping) =>
                wordExerciseModelGrouping.Key;

            WordExerciseModel[] DictionaryElementSelector(IGrouping<CharacterTypes, WordExerciseModel> wordExerciseModelGrouping) =>
                wordExerciseModelGrouping.ToArray();

#pragma warning disable IDE0037 // Use inferred member name
            existingWordExerciseModelsByCharacterTypeCombinations = existingWordExerciseModels
                .Select(wordExerciseModel => (WordExerciseModel: wordExerciseModel, CharacterTypes: wordExerciseModel.Word?.CharacterTypes))
                .Select(wordExerciseTuple => (WordExerciseModel: wordExerciseTuple.WordExerciseModel, CharacterTypeValues: GetCharacterTypeValues(wordExerciseTuple.CharacterTypes)))
                .SelectMany(wordExerciseTuple => wordExerciseTuple.CharacterTypeValues.Select(characterType => (wordExerciseTuple.WordExerciseModel, CharacterType: characterType)))
                .GroupBy(GroupingKeySelector, GroupingElementSelector)
                .ToDictionary(DictionaryKeySelector, DictionaryElementSelector);
#pragma warning restore IDE0037 // Use inferred member name

            ;
        }

        if (!wordExerciseModels.Any(IsRepeatedChallengePlaceholder))
            return;

        List<(int WordExerciseIndex, WordExerciseId[] ExistingWordExerciseIds)> wordExerciseIndexPairs = [];
        for (int wordExerciseIndex = 0; wordExerciseIndex < wordExerciseModels.Count; wordExerciseIndex++)
        {
            WordExerciseModel wordExerciseModel = wordExerciseModels[wordExerciseIndex];

            if (wordExerciseModel.Word is null || !IsRepeatedChallengePlaceholder(wordExerciseModel))
                continue;

            CharacterTypes characterType = wordExerciseModel.Word.CharacterTypes;

            int existingWordExerciseModelsCount = 0;
            IEnumerable<WordExerciseModel> existingWordExerciseModelsForMatchingCharacterTypeCombinations = [];
            foreach (CharacterTypes characterTypeCombination in existingWordExerciseModelsByCharacterTypeCombinations.Keys)
            {
                if ((characterType & characterTypeCombination) != characterType)
                    continue;

                WordExerciseModel[] existingWordExerciseModelsForCharacterTypeCombination =
                    existingWordExerciseModelsByCharacterTypeCombinations[characterTypeCombination];

                existingWordExerciseModelsCount += existingWordExerciseModelsForCharacterTypeCombination.Length;

                existingWordExerciseModelsForMatchingCharacterTypeCombinations =
                    existingWordExerciseModelsForMatchingCharacterTypeCombinations.Concat(existingWordExerciseModelsForCharacterTypeCombination);

                ;
            }

            if (existingWordExerciseModelsCount == 0)
            {
                wordExerciseModels.RemoveAt(wordExerciseIndex--);
                continue;
            }

            WordExerciseId[] existingWordExerciseIds = existingWordExerciseModelsForMatchingCharacterTypeCombinations
                .Select(wordExerciseModel => new WordExerciseId(wordExerciseModel.Id))
                .ToArray();

            wordExerciseIndexPairs.Add((wordExerciseIndex, existingWordExerciseIds));
        }

        Dictionary<WordExerciseId, WordExerciseModel> existingWordExerciseModelsByIds;
        {
            WordExerciseId KeySelector(WordExerciseModel wordExercise) => new(wordExercise.Id);

            existingWordExerciseModelsByIds = existingWordExerciseModels.ToDictionary(KeySelector);
        }

        int extraWordExerciseIndexOffset = 0;
        foreach ((int wordExerciseIndex, WordExerciseId[] existingWordExerciseIds) in wordExerciseIndexPairs)
        {
            bool extraWordExercise = false;
            foreach (WordExerciseId existingWordExerciseId in existingWordExerciseIds)
            {
                if (!existingWordExerciseModelsByIds.TryGetValue(existingWordExerciseId, out WordExerciseModel? existingWordExercise))
                    continue;

                if (extraWordExercise)
                {
                    extraWordExerciseIndexOffset++;
                    wordExerciseModels.Insert(wordExerciseIndex + extraWordExerciseIndexOffset, existingWordExercise);
                }
                else
                {
                    wordExerciseModels[wordExerciseIndex + extraWordExerciseIndexOffset] = existingWordExercise;
                    extraWordExercise = true;
                }
            }
        }
    }

    private static IEnumerable<CharacterTypes> GetCharacterTypeValues(CharacterTypes? characterTypes)
    {
        if ((characterTypes ?? CharacterTypes.None) == CharacterTypes.None)
            return [];

        IEnumerable<CharacterTypes> characterTypeValues = s_characterTypes.Where(characterType => (characterTypes & characterType) == characterType);

        return characterTypeValues;
    }

    private static bool IsRepeatedChallengePlaceholder(WordExerciseModel wordExerciseModel)
    {
        if (wordExerciseModel.Word is null)
            return false;

        foreach (TagModel tagModel in wordExerciseModel.Word.WordTags)
        {
            if (tagModel.PlaceholderMarker?.Equals(SystemReservedTags.RepeatedChallengeExceededRerunThresholdTagPlaceholderMarker, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;
        }

        return false;
    }
}
