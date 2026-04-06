using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class RepeatedChallengeByOriginalWordMatchWordExerciseBatchVisitor : IWordExerciseBatchVisitor
{
    private readonly IMapper m_mapper;
    private readonly IWordExerciseRepository m_wordExerciseRepository;

    public RepeatedChallengeByOriginalWordMatchWordExerciseBatchVisitor(IMapper mapper, IWordExerciseRepository wordExerciseRepository)
    {
        m_mapper = mapper;
        m_wordExerciseRepository = wordExerciseRepository;
    }

    /// <inheritdoc />
    public async Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        HashSet<int> wordIds = wordExerciseModels
            .Select(wordExerciseModel => wordExerciseModel.WordId)
            .ToHashSet();

        IQueryable<WordExercise> FilterWordExercises(IQueryable<WordExercise> wordExercises) => wordExercises
            .Where(wordExercise => wordIds.Contains(wordExercise.WordId))
            .Where(wordExercise => wordExercise.WordExerciseReruns.Any());

        WordExerciseFilter wordExerciseFilter = new()
        {
            CustomFilter = FilterWordExercises,
        };

        WordExercise[] existingWordExercises = await m_wordExerciseRepository.GetWordExercisesAsync(wordExerciseFilter, cancellationToken);

        WordId KeySelector(IGrouping<int, WordExerciseModel> wordExerciseModelGrouping) => new(wordExerciseModelGrouping.Key);

        WordExerciseModel ElementSelector(IGrouping<int, WordExerciseModel> wordExerciseModelGrouping) => wordExerciseModelGrouping.First();

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        Dictionary<WordId, WordExerciseModel> existingWordExerciseModelsByWordIds = existingWordExercises
            .Select(wordExercise => m_mapper.Map<WordExerciseModel>(wordExercise))
            .OrderBy(wordExerciseModel => wordExerciseModel.GeneratedOn)
            .GroupBy(wordExerciseModel => wordExerciseModel.WordId)
            .ToDictionary(KeySelector, ElementSelector);
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        for (int i = 0; i < wordExerciseModels.Count; i++)
        {
            WordId wordId;
            WordExerciseModel wordExerciseModel = wordExerciseModels[i];

            wordId = new(wordExerciseModel.WordId);

            if (!existingWordExerciseModelsByWordIds.TryGetValue(wordId, out WordExerciseModel? existingWordExerciseModel))
                continue;

            wordExerciseModels[i] = existingWordExerciseModel;
        }
    }
}
