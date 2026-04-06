using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class RegularChallengeByExplicitlyDistributedTagsCharacterExerciseBatchVisitor : ICharacterExerciseBatchVisitor
{
    private readonly TimeProvider m_timeProvider;
    private readonly Random m_random;

    public RegularChallengeByExplicitlyDistributedTagsCharacterExerciseBatchVisitor(TimeProvider timeProvider, Random random)
    {
        m_timeProvider = timeProvider;
        m_random = random;
    }

    /// <inheritdoc />
    public Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        foreach ((string tagCaption, int tagInclusions) in characterExerciseGeneratorContext.TagDistributionDictionary)
        {
            CharacterModel[] characterModelsByTag = characterExerciseGeneratorContext.AvailableCharacterModels
                .Where(characterModel => characterModel.CharacterTags.Any(tagModel => tagModel.Caption.Equals(tagCaption, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();

            for (int i = 0; i < tagInclusions && characterExerciseModels.Count < generateCharacterExerciseBatchRequestModel.Size && characterModelsByTag.Length > 0; i++)
            {
                CharacterModel characterModel = characterModelsByTag[m_random.Next(characterModelsByTag.Length)];

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
        }

        return Task.CompletedTask;
    }
}
