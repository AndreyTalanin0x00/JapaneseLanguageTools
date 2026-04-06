using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class RegularChallengeCharacterExerciseBatchVisitor : ICharacterExerciseBatchVisitor
{
    private readonly TimeProvider m_timeProvider;
    private readonly Random m_random;

    public RegularChallengeCharacterExerciseBatchVisitor(TimeProvider timeProvider, Random random)
    {
        m_timeProvider = timeProvider;
        m_random = random;
    }

    public Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        bool IsCharacterTagInTagDistributionSettings(TagModel tagModel) =>
            characterExerciseGeneratorContext.TagDistributionDictionary.TryGetValue(tagModel.Caption, out int tagInclusions) && tagInclusions > 0;

        CharacterModel[] availableCharacterModels = characterExerciseGeneratorContext.AvailableCharacterModels
            .Where(characterModel => characterModel.Type != CharacterTypes.None)
            .Where(characterModel => !characterModel.CharacterTags.Any(tagModel => IsCharacterTagInTagDistributionSettings(tagModel)))
            .Where(characterModel => !characterModel.CharacterTags.Any(tagModel => !string.IsNullOrEmpty(tagModel.PlaceholderMarker)))
            .ToArray();

        while (characterExerciseModels.Count < generateCharacterExerciseBatchRequestModel.Size && availableCharacterModels.Length > 0)
        {
            CharacterModel characterModel = availableCharacterModels[m_random.Next(availableCharacterModels.Length)];

            CharacterExerciseModel characterExerciseModel = new()
            {
                Id = 0,
                CharacterId = characterModel.Id,
                InstanceData = null,
                GeneratedOn = utcNow,
                Character = characterModel,
            };

            characterExerciseModels.Add(characterExerciseModel);
        }

        return Task.CompletedTask;
    }
}
