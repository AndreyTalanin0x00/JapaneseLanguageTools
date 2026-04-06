using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class OrderShuffleCharacterExerciseBatchVisitor : ICharacterExerciseBatchVisitor
{
    private readonly Random m_random;

    public OrderShuffleCharacterExerciseBatchVisitor(Random random)
    {
        m_random = random;
    }

    /// <inheritdoc />
    public Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        IList<CharacterExerciseModel> characterExerciseModels = characterExerciseBatchModel.Items;

        for (int i = 0; i < characterExerciseModels.Count; i++)
        {
            int j = m_random.Next(characterExerciseModels.Count);
            (characterExerciseModels[i], characterExerciseModels[j]) = (characterExerciseModels[j], characterExerciseModels[i]);
        }

        return Task.CompletedTask;
    }
}
