using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class OrderShuffleWordExerciseBatchVisitor : IWordExerciseBatchVisitor
{
    private readonly Random m_random;

    public OrderShuffleWordExerciseBatchVisitor(Random random)
    {
        m_random = random;
    }

    /// <inheritdoc />
    public Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        for (int i = 0; i < wordExerciseModels.Count; i++)
        {
            int j = m_random.Next(wordExerciseModels.Count);
            (wordExerciseModels[i], wordExerciseModels[j]) = (wordExerciseModels[j], wordExerciseModels[i]);
        }

        return Task.CompletedTask;
    }
}
