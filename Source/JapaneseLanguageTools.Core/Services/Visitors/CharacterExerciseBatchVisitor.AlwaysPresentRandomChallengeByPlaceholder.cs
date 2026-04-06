using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Constants;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class AlwaysPresentRandomChallengeByPlaceholderCharacterExerciseBatchVisitor : ICharacterExerciseBatchVisitor
{
    private readonly TimeProvider m_timeProvider;
    private readonly Random m_random;

    public AlwaysPresentRandomChallengeByPlaceholderCharacterExerciseBatchVisitor(TimeProvider timeProvider, Random random)
    {
        m_timeProvider = timeProvider;
        m_random = random;
    }

    /// <inheritdoc />
    public Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        CharacterModel[] allCharacterModels = characterExerciseGeneratorContext.AllCharacterModels
            .Where(characterModel => characterModel.Type != CharacterTypes.None)
            .Where(characterModel => !characterModel.CharacterTags.Any(tagModel => characterExerciseGeneratorContext.TagDistributionDictionary.TryGetValue(tagModel.Caption, out int tagInclusions) && tagInclusions >= 0))
            .ToArray();

        for (int characterExerciseIndex = 0; characterExerciseIndex < characterExerciseModels.Count; characterExerciseIndex++)
        {
            CharacterExerciseModel characterExerciseModel = characterExerciseModels[characterExerciseIndex];
            if (characterExerciseModel.Character is null || !IsAlwaysPresentRandomChallengePlaceholder(characterExerciseModel))
                continue;

            CharacterModel characterModel = allCharacterModels[m_random.Next(allCharacterModels.Length)];

            characterExerciseModel = new()
            {
                Id = 0,
                CharacterId = characterModel.Id,
                InstanceData = null,
                GeneratedOn = utcNow,
                Character = characterModel,
            };

            characterExerciseModels[characterExerciseIndex] = characterExerciseModel;
        }

        return Task.CompletedTask;
    }

    private static bool IsAlwaysPresentRandomChallengePlaceholder(CharacterExerciseModel characterExerciseModel)
    {
        if (characterExerciseModel.Character is null)
            return false;

        foreach (TagModel tagModel in characterExerciseModel.Character.CharacterTags)
        {
            if (tagModel.PlaceholderMarker?.Equals(SystemReservedTags.AlwaysPresentRandomChallengeTagPlaceholderMarker, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;
        }

        return false;
    }
}
