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

public class RegularChallengeByExplicitlyDistributedTagsWordExerciseBatchVisitor : IWordExerciseBatchVisitor
{
    private readonly TimeProvider m_timeProvider;
    private readonly Random m_random;

    public RegularChallengeByExplicitlyDistributedTagsWordExerciseBatchVisitor(TimeProvider timeProvider, Random random)
    {
        m_timeProvider = timeProvider;
        m_random = random;
    }

    /// <inheritdoc />
    public Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        foreach ((string tagCaption, int tagInclusions) in wordExerciseGeneratorContext.TagDistributionDictionary)
        {
            WordModel[] wordModelsByTag = wordExerciseGeneratorContext.AvailableWordModels
                .Where(wordModel => wordModel.WordTags.Any(tagModel => tagModel.Caption.Equals(tagCaption, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();

            for (int i = 0; i < tagInclusions && wordExerciseModels.Count < generateWordExerciseBatchRequestModel.Size && wordModelsByTag.Length > 0; i++)
            {
                WordModel wordModel = wordModelsByTag[m_random.Next(wordModelsByTag.Length)];

                WordExerciseModel wordExerciseModel = new()
                {
                    Id = 0,
                    WordId = wordModel.Id,
                    InstanceData = null,
                    GeneratedOn = utcNow,
                    Word = wordModel,
                };

                wordExerciseModels.Add(wordExerciseModel);
            }
        }

        return Task.CompletedTask;
    }
}
