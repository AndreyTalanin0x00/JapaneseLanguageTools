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

public class RepeatedChallengeByOriginalCharacterMatchCharacterExerciseBatchVisitor : ICharacterExerciseBatchVisitor
{
    private readonly IMapper m_mapper;
    private readonly ICharacterExerciseRepository m_characterExerciseRepository;

    public RepeatedChallengeByOriginalCharacterMatchCharacterExerciseBatchVisitor(IMapper mapper, ICharacterExerciseRepository characterExerciseRepository)
    {
        m_mapper = mapper;
        m_characterExerciseRepository = characterExerciseRepository;
    }

    /// <inheritdoc />
    public async Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        HashSet<int> characterIds = characterExerciseModels
            .Select(characterExerciseModel => characterExerciseModel.CharacterId)
            .ToHashSet();

        IQueryable<CharacterExercise> FilterCharacterExercises(IQueryable<CharacterExercise> characterExercises) => characterExercises
            .Where(characterExercise => characterIds.Contains(characterExercise.CharacterId))
            .Where(characterExercise => characterExercise.CharacterExerciseReruns.Any());

        CharacterExerciseFilter characterExerciseFilter = new()
        {
            CustomFilter = FilterCharacterExercises,
        };

        CharacterExercise[] existingCharacterExercises = await m_characterExerciseRepository.GetCharacterExercisesAsync(characterExerciseFilter, cancellationToken);

        CharacterId KeySelector(IGrouping<int, CharacterExerciseModel> characterExerciseModelGrouping) => new(characterExerciseModelGrouping.Key);

        CharacterExerciseModel ElementSelector(IGrouping<int, CharacterExerciseModel> characterExerciseModelGrouping) => characterExerciseModelGrouping.First();

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        Dictionary<CharacterId, CharacterExerciseModel> existingCharacterExerciseModelsByCharacterIds = existingCharacterExercises
            .Select(characterExercise => m_mapper.Map<CharacterExerciseModel>(characterExercise))
            .OrderBy(characterExerciseModel => characterExerciseModel.GeneratedOn)
            .GroupBy(characterExerciseModel => characterExerciseModel.CharacterId)
            .ToDictionary(KeySelector, ElementSelector);
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        for (int i = 0; i < characterExerciseModels.Count; i++)
        {
            CharacterId characterId;
            CharacterExerciseModel characterExerciseModel = characterExerciseModels[i];

            characterId = new(characterExerciseModel.CharacterId);

            if (!existingCharacterExerciseModelsByCharacterIds.TryGetValue(characterId, out CharacterExerciseModel? existingCharacterExerciseModel))
                continue;

            characterExerciseModels[i] = existingCharacterExerciseModel;
        }
    }
}
