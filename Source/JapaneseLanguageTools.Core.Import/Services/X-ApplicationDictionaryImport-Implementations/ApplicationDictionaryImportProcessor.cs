using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Responses;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;
using JapaneseLanguageTools.Core.Services.Abstractions;
using JapaneseLanguageTools.Data.Contexts;

using Microsoft.EntityFrameworkCore.Storage;

using ImportStatus = AndreyTalanin0x00.Integrations.Import.Enumerations.ImportStatus;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Import.Services;

public class ApplicationDictionaryImportProcessor :
    IImportProcessor<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, Object, ApplicationDictionaryObjectPackageIntegrationModel>
{
    private static readonly string[] s_characterTagSeparators =
        [CharacterIntegrationModel.TagSeparator, CharacterIntegrationModel.AltTagSeparator];
    private static readonly string[] s_wordTagSeparators =
        [WordIntegrationModel.TagSeparator, WordIntegrationModel.AltTagSeparator];

    private readonly IMapper m_mapper;
    private readonly IApplicationDictionaryService m_applicationDictionaryService;
    private readonly ITagService m_tagService;
    private readonly MainDbContext m_context;
    private readonly ISnapshotHashCalculator m_snapshotHashCalculator;

    public ApplicationDictionaryImportProcessor(IMapper mapper, IApplicationDictionaryService applicationDictionaryService, ITagService tagService, MainDbContext context, ISnapshotHashCalculator snapshotHashCalculator)
    {
        m_mapper = mapper;
        m_applicationDictionaryService = applicationDictionaryService;
        m_tagService = tagService;
        m_context = context;
        m_snapshotHashCalculator = snapshotHashCalculator;
    }

    /// <inheritdoc />
    public async Task<ApplicationDictionaryImportResponse> ProcessAsync(ApplicationDictionaryImportRequest applicationDictionaryImportRequest, ImportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel>[] importObjectPackageBatches, CancellationToken cancellationToken = default)
    {
        string snapshotHash = await m_snapshotHashCalculator.CalculateSnapshotHashAsync(cancellationToken);

        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            foreach (ImportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel> importObjectPackageBatch in importObjectPackageBatches)
            {
                int size = importObjectPackageBatch.Size;

                ImportSource[] importSources = importObjectPackageBatch.ImportSources;
                ImportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel>[] importObjectPackageWrappers = importObjectPackageBatch.ImportObjectPackageWrappers;

                for (int index = 0; index < size; index++)
                {
                    ImportSource importSource = importSources[index];
                    ImportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel> importObjectPackageWrapper = importObjectPackageWrappers[index];

                    BlobMetadata blobMetadata = importSource.BlobMetadata;

                    ApplicationDictionaryObjectPackageIntegrationModel applicationDictionaryObjectPackageIntegrationModel = importObjectPackageWrapper.ImportObjectPackage;

                    if (applicationDictionaryObjectPackageIntegrationModel.SnapshotHash != snapshotHash && !applicationDictionaryObjectPackageIntegrationModel.ForceMode)
                        throw new InvalidOperationException($"Current database state is different from one the '{blobMetadata.FileName}' import template was based on. Unable to proceed unless the force-mode is specified.");

                    ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel = applicationDictionaryObjectPackageIntegrationModel.ApplicationDictionary;

                    TagModel[] updatedTagModels = await ProcessTagsAsync(applicationDictionaryIntegrationModel, cancellationToken);

                    Dictionary<string, TagModel> updatedTagModelsByCaption =
                        updatedTagModels.ToDictionary(updatedTagModel => updatedTagModel.Caption);

                    await ProcessCharactersAsync(applicationDictionaryIntegrationModel, updatedTagModelsByCaption, cancellationToken);
                    await ProcessCharacterGroupsAsync(applicationDictionaryIntegrationModel, updatedTagModelsByCaption, cancellationToken);

                    await ProcessWordsAsync(applicationDictionaryIntegrationModel, updatedTagModelsByCaption, cancellationToken);
                    await ProcessWordGroupsAsync(applicationDictionaryIntegrationModel, updatedTagModelsByCaption, cancellationToken);
                }
            }

            await contextTransaction.CommitAsync(cancellationToken);
        }

        ImportStatus importStatus = ImportStatus.Completed;
        ImportResponseMessage[] importResponseMessages = [];

        ApplicationDictionaryImportResponse applicationDictionaryImportResponse = new()
        {
            Status = importStatus,
            Messages = importResponseMessages,
            Request = applicationDictionaryImportRequest,
        };

        return applicationDictionaryImportResponse;
    }

    private async Task<TagModel[]> ProcessTagsAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, CancellationToken cancellationToken)
    {
        TagId[] tagIds = applicationDictionaryIntegrationModel.Tags
            .Select(tagIntegrationModel => new TagId(tagIntegrationModel.Id))
            .ToArray();

        HashSet<int> existingTagIdValues = (await m_tagService.GetTagsAsync(tagIds, cancellationToken))
            .Select(existingTagModel => existingTagModel.Id)
            .ToHashSet();

        TagModel[] tagModels = m_mapper.Map<TagModel[]>(applicationDictionaryIntegrationModel.Tags);

        foreach ((TagModel tagModel, TagIntegrationModel tagIntegrationModel) in tagModels.Zip(applicationDictionaryIntegrationModel.Tags))
        {
            TagId tagId = new(tagModel.Id);

            switch (tagIntegrationModel.Action)
            {
                case SnapshotObjectAction.Add:
                    await m_tagService.AddTagAsync(tagModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Update:
                    await m_tagService.UpdateTagAsync(tagModel, cancellationToken);
                    break;
                case SnapshotObjectAction.AddOrUpdate:
                    if (!existingTagIdValues.Contains(tagModel.Id))
                        await m_tagService.AddTagAsync(tagModel, cancellationToken);
                    else
                        await m_tagService.UpdateTagAsync(tagModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Remove:
                    await m_tagService.RemoveTagAsync(tagId, cancellationToken);
                    break;
                case SnapshotObjectAction.None:
                    break;
                default:
                    throw new UnreachableException($"Unknown import action: {tagIntegrationModel.Action} ({(int)tagIntegrationModel.Action}).");
            }
        }

        TagModel[] updatedTagModels = await m_tagService.GetAllTagsAsync(cancellationToken);

        return updatedTagModels;
    }

    private async Task ProcessCharactersAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, Dictionary<string, TagModel> updatedTagModelsByCaption, CancellationToken cancellationToken)
    {
        CharacterId[] characterIds = applicationDictionaryIntegrationModel.Characters
            .Select(characterModel => new CharacterId(characterModel.Id))
            .ToArray();

        HashSet<int> existingCharacterIdValues = (await m_applicationDictionaryService.GetCharactersAsync(characterIds, cancellationToken))
            .Select(existingCharacterModel => existingCharacterModel.Id)
            .ToHashSet();

        CharacterModel[] characterModels = m_mapper.Map<CharacterModel[]>(applicationDictionaryIntegrationModel.Characters);

        foreach ((CharacterModel characterModel, CharacterIntegrationModel characterIntegrationModel) in characterModels.Zip(applicationDictionaryIntegrationModel.Characters))
        {
            CharacterId characterId = new(characterModel.Id);

            characterModel.CharacterTags = MapTagString(characterIntegrationModel.Tags, updatedTagModelsByCaption, s_characterTagSeparators).ToList();

            switch (characterIntegrationModel.Action)
            {
                case SnapshotObjectAction.Add:
                    await m_applicationDictionaryService.AddCharacterAsync(characterModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Update:
                    await m_applicationDictionaryService.UpdateCharacterAsync(characterModel, cancellationToken);
                    break;
                case SnapshotObjectAction.AddOrUpdate:
                    if (!existingCharacterIdValues.Contains(characterModel.Id))
                        await m_applicationDictionaryService.AddCharacterAsync(characterModel, cancellationToken);
                    else
                        await m_applicationDictionaryService.UpdateCharacterAsync(characterModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Remove:
                    await m_applicationDictionaryService.RemoveCharacterAsync(characterId, cancellationToken);
                    break;
                case SnapshotObjectAction.None:
                    break;
                default:
                    throw new UnreachableException($"Unknown import action: {characterIntegrationModel.Action} ({(int)characterIntegrationModel.Action}).");
            }
        }
    }

    private async Task ProcessCharacterGroupsAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, Dictionary<string, TagModel> updatedTagModelsByCaption, CancellationToken cancellationToken)
    {
        CharacterGroupId[] characterGroupIds = applicationDictionaryIntegrationModel.CharacterGroups
            .Select(characterGroupModel => new CharacterGroupId(characterGroupModel.Id))
            .ToArray();

        CharacterGroupIntegrationModel[] characterGroupIntegrationModels = applicationDictionaryIntegrationModel.CharacterGroups;

        HashSet<int> existingCharacterGroupIdValues = (await m_applicationDictionaryService.GetCharacterGroupsAsync(characterGroupIds, cancellationToken))
            .Select(existingCharacterGroupModel => existingCharacterGroupModel.Id)
            .ToHashSet();

        // Temporarily remove all character group hierarchy records: nested groups may not have been added yet.
        CharacterGroupHierarchyRecordIntegrationModel[][] characterGroupHierarchyRecordIntegrationModelsPartitioned =
            RemoveCharacterGroupHierarchyRecords(characterGroupIds, characterGroupIntegrationModels);

        CharacterGroupModel[] characterGroupModels = m_mapper.Map<CharacterGroupModel[]>(characterGroupIntegrationModels);

        foreach ((CharacterGroupModel characterGroupModel, CharacterGroupIntegrationModel characterGroupIntegrationModel) in characterGroupModels.Zip(characterGroupIntegrationModels))
        {
            CharacterGroupId characterGroupId = new(characterGroupModel.Id);

            foreach ((CharacterModel characterModel, CharacterIntegrationModel characterIntegrationModel) in characterGroupModel.Characters.Zip(characterGroupIntegrationModel.Characters))
                characterModel.CharacterTags = MapTagString(characterIntegrationModel.Tags, updatedTagModelsByCaption, s_characterTagSeparators).ToList();

            CharacterGroupModel? addedCharacterGroupModel = null;
            switch (characterGroupIntegrationModel.Action)
            {
                case SnapshotObjectAction.Add:
                    addedCharacterGroupModel = await m_applicationDictionaryService.AddCharacterGroupAsync(characterGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Update:
                    await m_applicationDictionaryService.UpdateCharacterGroupAsync(characterGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.AddOrUpdate:
                    if (!existingCharacterGroupIdValues.Contains(characterGroupModel.Id))
                        addedCharacterGroupModel = await m_applicationDictionaryService.AddCharacterGroupAsync(characterGroupModel, cancellationToken);
                    else
                        await m_applicationDictionaryService.UpdateCharacterGroupAsync(characterGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.ChangeState:
                    await m_applicationDictionaryService.ChangeCharacterGroupStateAsync(characterGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Remove:
                    await m_applicationDictionaryService.RemoveCharacterGroupAsync(characterGroupId, cancellationToken);
                    break;
                case SnapshotObjectAction.None:
                    break;
                default:
                    throw new UnreachableException($"Unknown import action: {characterGroupIntegrationModel.Action} ({(int)characterGroupIntegrationModel.Action}).");
            }

            if (addedCharacterGroupModel is not null)
            {
                characterGroupModel.Id = addedCharacterGroupModel.Id;
            }
        }

        // Update just processed character groups the second time to re-add the character group hierarchy records.
        await AddCharacterGroupHierarchyRecordsBackAsync(characterGroupIds, characterGroupModels, characterGroupIntegrationModels, characterGroupHierarchyRecordIntegrationModelsPartitioned, cancellationToken);

        static CharacterGroupHierarchyRecordIntegrationModel[][] RemoveCharacterGroupHierarchyRecords(CharacterGroupId[] characterGroupIds, CharacterGroupIntegrationModel[] characterGroupIntegrationModels)
        {
            CharacterGroupHierarchyRecordIntegrationModel[][] characterGroupHierarchyRecordIntegrationModelsPartitioned =
                new CharacterGroupHierarchyRecordIntegrationModel[characterGroupIds.Length][];

            for (int index = 0; index < characterGroupIds.Length; index++)
            {
                CharacterGroupIntegrationModel characterGroupIntegrationModel = characterGroupIntegrationModels[index];

                characterGroupHierarchyRecordIntegrationModelsPartitioned[index] = characterGroupIntegrationModel.CharacterGroupHierarchyRecords;

                characterGroupIntegrationModel.CharacterGroupHierarchyRecords = [];
            }

            return characterGroupHierarchyRecordIntegrationModelsPartitioned;
        }

        async Task AddCharacterGroupHierarchyRecordsBackAsync(CharacterGroupId[] characterGroupIds, CharacterGroupModel[] characterGroupModels, CharacterGroupIntegrationModel[] characterGroupIntegrationModels, CharacterGroupHierarchyRecordIntegrationModel[][] characterGroupHierarchyRecordIntegrationModelsPartitioned, CancellationToken cancellationToken)
        {
            CharacterGroupModel[] allCharacterGroupModels = await m_applicationDictionaryService.GetAllCharacterGroupsAsync(cancellationToken);

            Dictionary<int, CharacterGroupModel> allCharacterGroupModelsByIds = allCharacterGroupModels.ToDictionary(characterGroupModel => characterGroupModel.Id);
            Dictionary<string, CharacterGroupModel> allCharacterGroupModelsByCaptions = allCharacterGroupModels.ToDictionary(characterGroupModel => characterGroupModel.Caption);

            for (int index = 0; index < characterGroupIds.Length; index++)
            {
                CharacterGroupModel characterGroupModel = characterGroupModels[index];
                CharacterGroupIntegrationModel characterGroupIntegrationModel = characterGroupIntegrationModels[index];

                if (characterGroupIntegrationModel.Action == SnapshotObjectAction.Remove)
                    continue;

                CharacterGroupHierarchyRecordIntegrationModel[] characterGroupHierarchyRecordIntegrationModels =
                    characterGroupHierarchyRecordIntegrationModelsPartitioned[index];

                if (characterGroupHierarchyRecordIntegrationModels.Length == 0)
                    continue;

                List<CharacterGroupHierarchyRecordModel> characterGroupHierarchyRecordModels = characterGroupHierarchyRecordIntegrationModels
                    .Select(characterGroupHierarchyRecordIntegrationModel =>
                    {
                        CharacterGroupModel? nestedCharacterGroupModel = null;
                        if (characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupId > 0)
                        {
                            nestedCharacterGroupModel = allCharacterGroupModelsByIds[characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupId];
                        }
                        else // if (!string.IsNullOrEmpty(characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupCaption))
                        {
                            string nestedCharacterGroupCaption = characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupCaption
                                ?? throw new UnreachableException($"Both the {nameof(CharacterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupId)} and {nameof(CharacterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupCaption)} properties are not set. This must have been caught in the import validator.");

                            nestedCharacterGroupModel = allCharacterGroupModelsByCaptions[nestedCharacterGroupCaption];
                        }

                        CharacterGroupHierarchyRecordModel characterGroupHierarchyRecordModel = new()
                        {
                            CharacterGroupId = characterGroupModel.Id,
                            NestedCharacterGroupId = nestedCharacterGroupModel.Id,
                            PreventRecursiveIncludes = characterGroupHierarchyRecordIntegrationModel.PreventRecursiveIncludes,
                            CharacterGroup = characterGroupModel,
                            NestedCharacterGroup = nestedCharacterGroupModel,
                        };

                        return characterGroupHierarchyRecordModel;
                    })
                    .ToList();

                characterGroupModel.CharacterGroupHierarchyRecords = characterGroupHierarchyRecordModels;

                await m_applicationDictionaryService.UpdateCharacterGroupAsync(characterGroupModel, cancellationToken);
            }
        }
    }

    private async Task ProcessWordsAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, Dictionary<string, TagModel> updatedTagModelsByCaption, CancellationToken cancellationToken)
    {
        WordId[] wordIds = applicationDictionaryIntegrationModel.Words
            .Select(wordModel => new WordId(wordModel.Id))
            .ToArray();

        HashSet<int> existingWordIdValues = (await m_applicationDictionaryService.GetWordsAsync(wordIds, cancellationToken))
            .Select(existingWordModel => existingWordModel.Id)
            .ToHashSet();

        WordModel[] wordModels = m_mapper.Map<WordModel[]>(applicationDictionaryIntegrationModel.Words);

        foreach ((WordModel wordModel, WordIntegrationModel wordIntegrationModel) in wordModels.Zip(applicationDictionaryIntegrationModel.Words))
        {
            WordId wordId = new(wordModel.Id);

            wordModel.WordTags = MapTagString(wordIntegrationModel.Tags, updatedTagModelsByCaption, s_wordTagSeparators).ToList();

            switch (wordIntegrationModel.Action)
            {
                case SnapshotObjectAction.Add:
                    await m_applicationDictionaryService.AddWordAsync(wordModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Update:
                    await m_applicationDictionaryService.UpdateWordAsync(wordModel, cancellationToken);
                    break;
                case SnapshotObjectAction.AddOrUpdate:
                    if (!existingWordIdValues.Contains(wordModel.Id))
                        await m_applicationDictionaryService.AddWordAsync(wordModel, cancellationToken);
                    else
                        await m_applicationDictionaryService.UpdateWordAsync(wordModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Remove:
                    await m_applicationDictionaryService.RemoveWordAsync(wordId, cancellationToken);
                    break;
                case SnapshotObjectAction.None:
                    break;
                default:
                    throw new UnreachableException($"Unknown import action: {wordIntegrationModel.Action} ({(int)wordIntegrationModel.Action}).");
            }
        }
    }

    private async Task ProcessWordGroupsAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, Dictionary<string, TagModel> updatedTagModelsByCaption, CancellationToken cancellationToken)
    {
        WordGroupId[] wordGroupIds = applicationDictionaryIntegrationModel.WordGroups
            .Select(wordGroupModel => new WordGroupId(wordGroupModel.Id))
            .ToArray();

        WordGroupIntegrationModel[] wordGroupIntegrationModels = applicationDictionaryIntegrationModel.WordGroups;

        HashSet<int> existingWordGroupIdValues = (await m_applicationDictionaryService.GetWordGroupsAsync(wordGroupIds, cancellationToken))
            .Select(existingWordGroupModel => existingWordGroupModel.Id)
            .ToHashSet();

        // Temporarily remove all word group hierarchy records: nested groups may not have been added yet.
        WordGroupHierarchyRecordIntegrationModel[][] wordGroupHierarchyRecordIntegrationModelsPartitioned =
            RemoveWordGroupHierarchyRecords(wordGroupIds, wordGroupIntegrationModels);

        WordGroupModel[] wordGroupModels = m_mapper.Map<WordGroupModel[]>(wordGroupIntegrationModels);

        foreach ((WordGroupModel wordGroupModel, WordGroupIntegrationModel wordGroupIntegrationModel) in wordGroupModels.Zip(wordGroupIntegrationModels))
        {
            WordGroupId wordGroupId = new(wordGroupModel.Id);

            foreach ((WordModel wordModel, WordIntegrationModel wordIntegrationModel) in wordGroupModel.Words.Zip(wordGroupIntegrationModel.Words))
                wordModel.WordTags = MapTagString(wordIntegrationModel.Tags, updatedTagModelsByCaption, s_wordTagSeparators).ToList();

            WordGroupModel? addedWordGroupModel = null;
            switch (wordGroupIntegrationModel.Action)
            {
                case SnapshotObjectAction.Add:
                    addedWordGroupModel = await m_applicationDictionaryService.AddWordGroupAsync(wordGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Update:
                    await m_applicationDictionaryService.UpdateWordGroupAsync(wordGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.AddOrUpdate:
                    if (!existingWordGroupIdValues.Contains(wordGroupModel.Id))
                        addedWordGroupModel = await m_applicationDictionaryService.AddWordGroupAsync(wordGroupModel, cancellationToken);
                    else
                        await m_applicationDictionaryService.UpdateWordGroupAsync(wordGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.ChangeState:
                    await m_applicationDictionaryService.ChangeWordGroupStateAsync(wordGroupModel, cancellationToken);
                    break;
                case SnapshotObjectAction.Remove:
                    await m_applicationDictionaryService.RemoveWordGroupAsync(wordGroupId, cancellationToken);
                    break;
                case SnapshotObjectAction.None:
                    break;
                default:
                    throw new UnreachableException($"Unknown import action: {wordGroupIntegrationModel.Action} ({(int)wordGroupIntegrationModel.Action}).");
            }

            if (addedWordGroupModel is not null)
            {
                wordGroupModel.Id = addedWordGroupModel.Id;
            }
        }

        // Update just processed word groups the second time to re-add the word group hierarchy records.
        await AddWordGroupHierarchyRecordsBackAsync(wordGroupIds, wordGroupModels, wordGroupIntegrationModels, wordGroupHierarchyRecordIntegrationModelsPartitioned, cancellationToken);

        static WordGroupHierarchyRecordIntegrationModel[][] RemoveWordGroupHierarchyRecords(WordGroupId[] wordGroupIds, WordGroupIntegrationModel[] wordGroupIntegrationModels)
        {
            WordGroupHierarchyRecordIntegrationModel[][] wordGroupHierarchyRecordIntegrationModelsPartitioned =
                new WordGroupHierarchyRecordIntegrationModel[wordGroupIds.Length][];

            for (int index = 0; index < wordGroupIds.Length; index++)
            {
                WordGroupIntegrationModel wordGroupIntegrationModel = wordGroupIntegrationModels[index];

                wordGroupHierarchyRecordIntegrationModelsPartitioned[index] = wordGroupIntegrationModel.WordGroupHierarchyRecords;

                wordGroupIntegrationModel.WordGroupHierarchyRecords = [];
            }

            return wordGroupHierarchyRecordIntegrationModelsPartitioned;
        }

        async Task AddWordGroupHierarchyRecordsBackAsync(WordGroupId[] wordGroupIds, WordGroupModel[] wordGroupModels, WordGroupIntegrationModel[] wordGroupIntegrationModels, WordGroupHierarchyRecordIntegrationModel[][] wordGroupHierarchyRecordIntegrationModelsPartitioned, CancellationToken cancellationToken)
        {
            WordGroupModel[] allWordGroupModels = await m_applicationDictionaryService.GetAllWordGroupsAsync(cancellationToken);

            Dictionary<int, WordGroupModel> allWordGroupModelsByIds = allWordGroupModels.ToDictionary(wordGroupModel => wordGroupModel.Id);
            Dictionary<string, WordGroupModel> allWordGroupModelsByCaptions = allWordGroupModels.ToDictionary(wordGroupModel => wordGroupModel.Caption);

            for (int index = 0; index < wordGroupIds.Length; index++)
            {
                WordGroupModel wordGroupModel = wordGroupModels[index];
                WordGroupIntegrationModel wordGroupIntegrationModel = wordGroupIntegrationModels[index];

                if (wordGroupIntegrationModel.Action == SnapshotObjectAction.Remove)
                    continue;

                WordGroupHierarchyRecordIntegrationModel[] wordGroupHierarchyRecordIntegrationModels =
                    wordGroupHierarchyRecordIntegrationModelsPartitioned[index];

                if (wordGroupHierarchyRecordIntegrationModels.Length == 0)
                    continue;

                List<WordGroupHierarchyRecordModel> wordGroupHierarchyRecordModels = wordGroupHierarchyRecordIntegrationModels
                    .Select(wordGroupHierarchyRecordIntegrationModel =>
                    {
                        WordGroupModel? nestedWordGroupModel = null;
                        if (wordGroupHierarchyRecordIntegrationModel.NestedWordGroupId > 0)
                        {
                            nestedWordGroupModel = allWordGroupModelsByIds[wordGroupHierarchyRecordIntegrationModel.NestedWordGroupId];
                        }
                        else // if (!string.IsNullOrEmpty(wordGroupHierarchyRecordIntegrationModel.NestedWordGroupCaption))
                        {
                            string nestedWordGroupCaption = wordGroupHierarchyRecordIntegrationModel.NestedWordGroupCaption
                                ?? throw new UnreachableException($"Both the {nameof(WordGroupHierarchyRecordIntegrationModel.NestedWordGroupId)} and {nameof(WordGroupHierarchyRecordIntegrationModel.NestedWordGroupCaption)} properties are not set. This must have been caught in the import validator.");

                            nestedWordGroupModel = allWordGroupModelsByCaptions[nestedWordGroupCaption];
                        }

                        WordGroupHierarchyRecordModel wordGroupHierarchyRecordModel = new()
                        {
                            WordGroupId = wordGroupModel.Id,
                            NestedWordGroupId = nestedWordGroupModel.Id,
                            PreventRecursiveIncludes = wordGroupHierarchyRecordIntegrationModel.PreventRecursiveIncludes,
                            WordGroup = wordGroupModel,
                            NestedWordGroup = nestedWordGroupModel,
                        };

                        return wordGroupHierarchyRecordModel;
                    })
                    .ToList();

                wordGroupModel.WordGroupHierarchyRecords = wordGroupHierarchyRecordModels;

                await m_applicationDictionaryService.UpdateWordGroupAsync(wordGroupModel, cancellationToken);
            }
        }
    }

    private static IEnumerable<TagModel> MapTagString(string? tags, Dictionary<string, TagModel> updatedTagModelsByCaption, string[] tagSeparators)
    {
        tags ??= string.Empty;

        StringSplitOptions tagSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

        string[] tagCaptions = tags.Split(tagSeparators, tagSplitOptions);

        foreach (string tagCaption in tagCaptions)
        {
            TagModel tagModel = !updatedTagModelsByCaption.TryGetValue(tagCaption, out TagModel? updatedTagModel)
                ? throw new KeyNotFoundException($"Unable to find a tag by its caption: {tagCaption}.")
                : updatedTagModel;

            yield return tagModel;
        }
    }
}
