using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services;

public class WordExerciseGenerator : IWordExerciseGenerator
{
    private readonly IApplicationDictionaryService m_applicationDictionaryService;
    private readonly IWordExerciseBatchVisitorSequence m_wordExerciseBatchVisitorSequence;

    public WordExerciseGenerator(IApplicationDictionaryService applicationDictionaryService, IWordExerciseBatchVisitorSequence wordExerciseBatchVisitorSequence)
    {
        m_applicationDictionaryService = applicationDictionaryService;
        m_wordExerciseBatchVisitorSequence = wordExerciseBatchVisitorSequence;
    }

    /// <inheritdoc />
    public async Task<GenerateWordExerciseBatchFullResponseModel> GenerateWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        WordExerciseBatchId wordExerciseBatchId = WordExerciseBatchId.CreateRandom();

        WordExerciseBatchModel wordExerciseBatchModel = new()
        {
            Id = wordExerciseBatchId.Value,
        };

        WordExerciseGeneratorContext wordExerciseGeneratorContext = await InitializeContextAsync(generateWordExerciseBatchRequestModel, cancellationToken);

        IEnumerable<IWordExerciseBatchVisitor> wordExerciseBatchVisitors = m_wordExerciseBatchVisitorSequence;

        foreach (IWordExerciseBatchVisitor wordExerciseBatchVisitor in wordExerciseBatchVisitors)
            await wordExerciseBatchVisitor.VisitWordExerciseBatchAsync(generateWordExerciseBatchRequestModel, wordExerciseBatchModel, wordExerciseGeneratorContext, cancellationToken);

        GenerateWordExerciseBatchFullResponseModel generateWordExerciseBatchFullResponseModel = new()
        {
            Request = generateWordExerciseBatchRequestModel,
            WordExerciseBatchId = wordExerciseBatchId.Value,
            WordExerciseBatch = wordExerciseBatchModel,
        };

        return generateWordExerciseBatchFullResponseModel;
    }

    private async Task<WordExerciseGeneratorContext> InitializeContextAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, CancellationToken cancellationToken)
    {
        WordGroupModel[] allWordGroupModels;
        WordGroupModel[] availableWordGroupModels;
        {
            allWordGroupModels = await m_applicationDictionaryService.GetAllWordGroupsAsync(cancellationToken);

            allWordGroupModels = allWordGroupModels
                .Where(wordGroupModel => wordGroupModel.Enabled)
                .ToArray();

            HashSet<int> wordGroupIds = [.. generateWordExerciseBatchRequestModel.UseWordGroupIds];
            IEnumerable<WordGroupModel> filteredWordGroupModels =
                allWordGroupModels.Where(wordGroupModel => wordGroupIds.Contains(wordGroupModel.Id));
            IEnumerable<WordGroupModel> alwaysUsedWordGroupModels =
                allWordGroupModels.Where(wordGroupModel => !wordGroupIds.Contains(wordGroupModel.Id) && wordGroupModel.AlwaysUse);

            availableWordGroupModels = wordGroupIds.Count > 0
                ? [.. filteredWordGroupModels, .. alwaysUsedWordGroupModels]
                : allWordGroupModels;
        }

        WordModel[] standaloneWordModels = (await m_applicationDictionaryService.GetAllWordsAsync(cancellationToken))
            .Where(wordModel => wordModel.WordGroup is null)
            .ToArray();

        WordModel[] allWordModels = standaloneWordModels
            .Concat(allWordGroupModels.SelectMany(wordGroupModel => wordGroupModel.Words))
            .ToArray();
        WordModel[] availableWordModels = standaloneWordModels
            .Concat(availableWordGroupModels.SelectMany(wordGroupModel => wordGroupModel.Words))
            .ToArray();

        Dictionary<string, int> tagDistributionDictionary;
        {
            WordExerciseProfileModel? wordExerciseProfileModel = generateWordExerciseBatchRequestModel.UseWordExerciseProfile;

            TagDistributionPreferencesModel? tagDistributionPreferencesModel = wordExerciseProfileModel?.TagDistributionPreferences;
            IList<TagDistributionRuleModel> tagDistributionRuleModels = tagDistributionPreferencesModel?.Rules ?? [];

            string KeySelector(TagDistributionRuleModel tagDistributionRuleModel) => tagDistributionRuleModel.TagCaption;

            int ElementSelector(TagDistributionRuleModel tagDistributionRuleModel) => GetTagInclusions(tagDistributionRuleModel);

            int GetTagInclusions(TagDistributionRuleModel tagDistributionRuleModel)
            {
                int tagInclusions;
                tagInclusions = (int)Math.Floor(tagDistributionRuleModel.ExerciseBatchFraction * generateWordExerciseBatchRequestModel.Size);
                tagInclusions = Math.Max(tagInclusions, tagDistributionRuleModel.MinInclusions ?? int.MinValue);
                tagInclusions = Math.Min(tagInclusions, tagDistributionRuleModel.MaxInclusions ?? int.MaxValue);

                return tagInclusions;
            }

            tagDistributionDictionary = tagDistributionRuleModels.ToDictionary(KeySelector, ElementSelector);
        }

        WordExerciseGeneratorContext wordExerciseGeneratorContext = new()
        {
            AllWordModels = allWordModels,
            AvailableWordModels = availableWordModels,
            AllWordGroupModels = allWordGroupModels,
            AvailableWordGroupModels = availableWordGroupModels,
            TagDistributionDictionary = tagDistributionDictionary,
        };

        return wordExerciseGeneratorContext;
    }
}
