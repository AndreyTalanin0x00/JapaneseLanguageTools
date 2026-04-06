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

public class RegularChallengeWordExerciseBatchVisitor : IWordExerciseBatchVisitor
{
    private readonly TimeProvider m_timeProvider;
    private readonly Random m_random;

    public RegularChallengeWordExerciseBatchVisitor(TimeProvider timeProvider, Random random)
    {
        m_timeProvider = timeProvider;
        m_random = random;
    }

    public Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        bool IsWordTagInTagDistributionSettings(TagModel tagModel) =>
            wordExerciseGeneratorContext.TagDistributionDictionary.TryGetValue(tagModel.Caption, out int tagInclusions) && tagInclusions > 0;

        WordModel[] availableWordModels = wordExerciseGeneratorContext.AvailableWordModels
            .Where(wordModel => wordModel.CharacterTypes != CharacterTypes.None)
            .Where(wordModel => !wordModel.WordTags.Any(tagModel => IsWordTagInTagDistributionSettings(tagModel)))
            .Where(wordModel => !wordModel.WordTags.Any(tagModel => !string.IsNullOrEmpty(tagModel.PlaceholderMarker)))
            .ToArray();

        while (wordExerciseModels.Count < generateWordExerciseBatchRequestModel.Size && availableWordModels.Length > 0)
        {
            WordModel wordModel = availableWordModels[m_random.Next(availableWordModels.Length)];

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

        return Task.CompletedTask;
    }
}
