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

public class AlwaysPresentRandomChallengeByPlaceholderWordExerciseBatchVisitor : IWordExerciseBatchVisitor
{
    private readonly TimeProvider m_timeProvider;
    private readonly Random m_random;

    public AlwaysPresentRandomChallengeByPlaceholderWordExerciseBatchVisitor(TimeProvider timeProvider, Random random)
    {
        m_timeProvider = timeProvider;
        m_random = random;
    }

    /// <inheritdoc />
    public Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        IList<WordExerciseModel> wordExerciseModels = wordExerciseBatchModel.Items;

        WordModel[] allWordModels = wordExerciseGeneratorContext.AllWordModels
            .Where(wordModel => wordModel.CharacterTypes != CharacterTypes.None)
            .Where(wordModel => !wordModel.WordTags.Any(tagModel => wordExerciseGeneratorContext.TagDistributionDictionary.TryGetValue(tagModel.Caption, out int tagInclusions) && tagInclusions >= 0))
            .ToArray();

        for (int wordExerciseIndex = 0; wordExerciseIndex < wordExerciseModels.Count; wordExerciseIndex++)
        {
            WordExerciseModel wordExerciseModel = wordExerciseModels[wordExerciseIndex];
            if (wordExerciseModel.Word is null || !IsAlwaysPresentRandomChallengePlaceholder(wordExerciseModel))
                continue;

            WordModel wordModel = allWordModels[m_random.Next(allWordModels.Length)];

            wordExerciseModel = new()
            {
                Id = 0,
                WordId = wordModel.Id,
                InstanceData = null,
                GeneratedOn = utcNow,
                Word = wordModel,
            };

            wordExerciseModels[wordExerciseIndex] = wordExerciseModel;
        }

        return Task.CompletedTask;
    }

    private static bool IsAlwaysPresentRandomChallengePlaceholder(WordExerciseModel wordExerciseModel)
    {
        if (wordExerciseModel.Word is null)
            return false;

        foreach (TagModel tagModel in wordExerciseModel.Word.WordTags)
        {
            if (tagModel.PlaceholderMarker?.Equals(SystemReservedTags.AlwaysPresentRandomChallengeTagPlaceholderMarker, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;
        }

        return false;
    }
}
