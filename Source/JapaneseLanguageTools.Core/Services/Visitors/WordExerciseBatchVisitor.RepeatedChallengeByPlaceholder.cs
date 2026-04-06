using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

public class RepeatedChallengeByPlaceholderWordExerciseBatchVisitor : IWordExerciseBatchVisitor
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

    public RepeatedChallengeByPlaceholderWordExerciseBatchVisitor(IMapper mapper, IWordExerciseRepository wordExerciseRepository, Random random)
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
            IQueryable<WordExercise> FilterExistingWordExercises(IQueryable<WordExercise> wordExercises) =>
                wordExercises.Where(wordExercise => wordExercise.WordExerciseReruns.Any());

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

        List<(int WordExerciseIndex, WordExerciseId ExistingWordExerciseId)> wordExerciseIndexPairs = [];
        for (int wordExerciseIndex = 0, existingWordExerciseIndex = 0; wordExerciseIndex < wordExerciseModels.Count; wordExerciseIndex++)
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

            WordExerciseModel existingWordExerciseModel =
                existingWordExerciseModelsForMatchingCharacterTypeCombinations.ElementAt(existingWordExerciseIndex++ % existingWordExerciseModelsCount);
            WordExerciseId existingWordExerciseId = new(existingWordExerciseModel.Id);

            wordExerciseIndexPairs.Add((wordExerciseIndex, existingWordExerciseId));
        }

        Dictionary<WordExerciseId, WordExerciseModel> existingWordExerciseModelsByIds;
        {
            WordExerciseId KeySelector(WordExerciseModel wordExercise) => new(wordExercise.Id);

            existingWordExerciseModelsByIds = existingWordExerciseModels.ToDictionary(KeySelector);
        }

        foreach ((int wordExerciseIndex, WordExerciseId existingWordExerciseId) in wordExerciseIndexPairs)
        {
            if (!existingWordExerciseModelsByIds.TryGetValue(existingWordExerciseId, out WordExerciseModel? existingWordExerciseModel))
                continue;

            wordExerciseModels[wordExerciseIndex] = existingWordExerciseModel;
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
            if (tagModel.PlaceholderMarker?.Equals(SystemReservedTags.RepeatedChallengeTagPlaceholderMarker, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;
        }

        return false;
    }
}
