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

public class CharacterExerciseGenerator : ICharacterExerciseGenerator
{
    private readonly IApplicationDictionaryService m_applicationDictionaryService;
    private readonly ICharacterExerciseBatchVisitorSequence m_characterExerciseBatchVisitorSequence;

    public CharacterExerciseGenerator(IApplicationDictionaryService applicationDictionaryService, ICharacterExerciseBatchVisitorSequence characterExerciseBatchVisitorSequence)
    {
        m_applicationDictionaryService = applicationDictionaryService;
        m_characterExerciseBatchVisitorSequence = characterExerciseBatchVisitorSequence;
    }

    /// <inheritdoc />
    public async Task<GenerateCharacterExerciseBatchFullResponseModel> GenerateCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        CharacterExerciseBatchId characterExerciseBatchId = CharacterExerciseBatchId.CreateRandom();

        CharacterExerciseBatchModel characterExerciseBatchModel = new()
        {
            Id = characterExerciseBatchId.Value,
        };

        CharacterExerciseGeneratorContext characterExerciseGeneratorContext = await InitializeContextAsync(generateCharacterExerciseBatchRequestModel, cancellationToken);

        IEnumerable<ICharacterExerciseBatchVisitor> characterExerciseBatchVisitors = m_characterExerciseBatchVisitorSequence;

        foreach (ICharacterExerciseBatchVisitor characterExerciseBatchVisitor in characterExerciseBatchVisitors)
            await characterExerciseBatchVisitor.VisitCharacterExerciseBatchAsync(generateCharacterExerciseBatchRequestModel, characterExerciseBatchModel, characterExerciseGeneratorContext, cancellationToken);

        GenerateCharacterExerciseBatchFullResponseModel generateCharacterExerciseBatchFullResponseModel = new()
        {
            Request = generateCharacterExerciseBatchRequestModel,
            CharacterExerciseBatchId = characterExerciseBatchId.Value,
            CharacterExerciseBatch = characterExerciseBatchModel,
        };

        return generateCharacterExerciseBatchFullResponseModel;
    }

    private async Task<CharacterExerciseGeneratorContext> InitializeContextAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken)
    {
        CharacterGroupModel[] allCharacterGroupModels;
        CharacterGroupModel[] availableCharacterGroupModels;
        {
            allCharacterGroupModels = await m_applicationDictionaryService.GetAllCharacterGroupsAsync(cancellationToken);

            allCharacterGroupModels = allCharacterGroupModels
                .Where(characterGroupModel => characterGroupModel.Enabled)
                .ToArray();

            HashSet<int> characterGroupIds = [.. generateCharacterExerciseBatchRequestModel.UseCharacterGroupIds];
            IEnumerable<CharacterGroupModel> filteredCharacterGroupModels =
                allCharacterGroupModels.Where(characterGroupModel => characterGroupIds.Contains(characterGroupModel.Id));
            IEnumerable<CharacterGroupModel> alwaysUsedCharacterGroupModels =
                allCharacterGroupModels.Where(characterGroupModel => !characterGroupIds.Contains(characterGroupModel.Id) && characterGroupModel.AlwaysUse);

            availableCharacterGroupModels = characterGroupIds.Count > 0
                ? [.. filteredCharacterGroupModels, .. alwaysUsedCharacterGroupModels]
                : allCharacterGroupModels;
        }

        CharacterModel[] standaloneCharacterModels = (await m_applicationDictionaryService.GetAllCharactersAsync(cancellationToken))
            .Where(characterModel => characterModel.CharacterGroup is null)
            .ToArray();

        CharacterModel[] allCharacterModels = standaloneCharacterModels
            .Concat(allCharacterGroupModels.SelectMany(characterGroupModel => characterGroupModel.Characters))
            .Where(characterModel => (characterModel.Type & generateCharacterExerciseBatchRequestModel.UseCharacterTypes) == characterModel.Type)
            .ToArray();
        CharacterModel[] availableCharacterModels = standaloneCharacterModels
            .Concat(availableCharacterGroupModels.SelectMany(characterGroupModel => characterGroupModel.Characters))
            .Where(characterModel => (characterModel.Type & generateCharacterExerciseBatchRequestModel.UseCharacterTypes) == characterModel.Type)
            .ToArray();

        Dictionary<string, int> tagDistributionDictionary;
        {
            CharacterExerciseProfileModel? characterExerciseProfileModel = generateCharacterExerciseBatchRequestModel.UseCharacterExerciseProfile;

            TagDistributionPreferencesModel? tagDistributionPreferencesModel = characterExerciseProfileModel?.TagDistributionPreferences;
            IList<TagDistributionRuleModel> tagDistributionRuleModels = tagDistributionPreferencesModel?.Rules ?? [];

            string KeySelector(TagDistributionRuleModel tagDistributionRuleModel) => tagDistributionRuleModel.TagCaption;

            int ElementSelector(TagDistributionRuleModel tagDistributionRuleModel) => GetTagInclusions(tagDistributionRuleModel);

            int GetTagInclusions(TagDistributionRuleModel tagDistributionRuleModel)
            {
                int tagInclusions;
                tagInclusions = (int)Math.Floor(tagDistributionRuleModel.ExerciseBatchFraction * generateCharacterExerciseBatchRequestModel.Size);
                tagInclusions = Math.Max(tagInclusions, tagDistributionRuleModel.MinInclusions ?? int.MinValue);
                tagInclusions = Math.Min(tagInclusions, tagDistributionRuleModel.MaxInclusions ?? int.MaxValue);

                return tagInclusions;
            }

            tagDistributionDictionary = tagDistributionRuleModels.ToDictionary(KeySelector, ElementSelector);
        }

        CharacterExerciseGeneratorContext characterExerciseGeneratorContext = new()
        {
            AllCharacterModels = allCharacterModels,
            AvailableCharacterModels = availableCharacterModels,
            AllCharacterGroupModels = allCharacterGroupModels,
            AvailableCharacterGroupModels = availableCharacterGroupModels,
            TagDistributionDictionary = tagDistributionDictionary,
        };

        return characterExerciseGeneratorContext;
    }
}
