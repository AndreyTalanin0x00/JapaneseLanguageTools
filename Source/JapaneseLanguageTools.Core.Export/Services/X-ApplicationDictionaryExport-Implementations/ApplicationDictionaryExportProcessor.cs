using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;
using JapaneseLanguageTools.Core.Services.Abstractions;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Export.Services;

public class ApplicationDictionaryExportProcessor :
    IExportProcessor<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, Object, ApplicationDictionaryObjectPackageIntegrationModel>
{
    private readonly TimeProvider m_timeProvider;
    private readonly IApplicationDictionaryService m_applicationDictionaryService;
    private readonly ITagService m_tagService;
    private readonly ISnapshotHashCalculator m_snapshotHashCalculator;
    private readonly IMapper m_mapper;

    public ApplicationDictionaryExportProcessor(TimeProvider timeProvider, IApplicationDictionaryService applicationDictionaryService, ITagService tagService, ISnapshotHashCalculator snapshotHashCalculator, IMapper mapper)
    {
        m_timeProvider = timeProvider;
        m_applicationDictionaryService = applicationDictionaryService;
        m_tagService = tagService;
        m_snapshotHashCalculator = snapshotHashCalculator;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ExportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel>> ProcessAsync(ApplicationDictionaryExportRequest applicationDictionaryExportRequest, CancellationToken cancellationToken = default)
    {
        SnapshotType snapshotType = applicationDictionaryExportRequest.SnapshotType;

        DateTimeOffset snapshotTime = m_timeProvider.GetUtcNow();

        string snapshotHash = await m_snapshotHashCalculator.CalculateSnapshotHashAsync(cancellationToken);

        ApplicationDictionaryModel applicationDictionaryModel = await GetApplicationDictionaryModelAsync(snapshotType, cancellationToken);

        ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel = await MapApplicationDictionaryModelAsync(applicationDictionaryModel, cancellationToken);

        applicationDictionaryIntegrationModel = snapshotType switch
        {
            SnapshotType.General => await ProcessApplicationDictionaryIntegrationModelGeneralAsync(applicationDictionaryIntegrationModel, cancellationToken),
            SnapshotType.GeneralNoAction => await ProcessApplicationDictionaryIntegrationModelGeneralNoActionAsync(applicationDictionaryIntegrationModel, cancellationToken),
            SnapshotType.ChangeState => await ProcessApplicationDictionaryIntegrationModelChangeStateAsync(applicationDictionaryIntegrationModel, cancellationToken),
            SnapshotType.Patch => await ProcessApplicationDictionaryIntegrationModelPatchAsync(applicationDictionaryIntegrationModel, cancellationToken),
            _ or SnapshotType.Unknown => throw new NotSupportedException($"Unknown snapshot type: {(int)snapshotType} ({snapshotType})."),
        };

        const bool forceMode = false;
        ApplicationDictionaryObjectPackageIntegrationModel applicationDictionaryObjectPackageIntegrationModel = new()
        {
            SnapshotType = snapshotType,
            SnapshotTime = snapshotTime,
            SnapshotHash = snapshotHash,
            ApplicationDictionary = applicationDictionaryIntegrationModel,
            ForceMode = forceMode,
        };

        const int size = 1;
        ExportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel> exportObjectPackageBatch = new()
        {
            Size = size,
            ExportTargets = new ExportTarget[size],
            ExportIntermediateObjectPackageWrappers = new ExportIntermediateObjectPackageWrapper<Object>[size],
            ExportObjectPackageWrappers = new ExportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel>[size],
            ExportTargetContext = new ExportTargetContext()
            {
                ExportResources = [],
            },
        };

        ExportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel>[] exportObjectPackageWrappers =
            exportObjectPackageBatch.ExportObjectPackageWrappers;

        exportObjectPackageWrappers[0] = new ExportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel>()
        {
            ExportObjectPackage = applicationDictionaryObjectPackageIntegrationModel,
        };

        return exportObjectPackageBatch;
    }

    private async Task<ApplicationDictionaryModel> GetApplicationDictionaryModelAsync(SnapshotType snapshotType, CancellationToken cancellationToken)
    {
        CharacterModel[] characterModels = !(snapshotType == SnapshotType.Patch || snapshotType == SnapshotType.Unknown)
            ? await m_applicationDictionaryService.GetAllCharactersAsync(cancellationToken)
            : [];
        CharacterGroupModel[] characterGroupModels = !(snapshotType == SnapshotType.Patch || snapshotType == SnapshotType.Unknown)
            ? await m_applicationDictionaryService.GetAllCharacterGroupsAsync(cancellationToken)
            : [];

        WordModel[] wordModels = !(snapshotType == SnapshotType.Patch || snapshotType == SnapshotType.Unknown)
            ? await m_applicationDictionaryService.GetAllWordsAsync(cancellationToken)
            : [];
        WordGroupModel[] wordGroupModels = !(snapshotType == SnapshotType.Patch || snapshotType == SnapshotType.Unknown)
            ? await m_applicationDictionaryService.GetAllWordGroupsAsync(cancellationToken)
            : [];

        characterModels = characterModels
            .Where(characterModel => characterModel.CharacterGroup is null)
            .ToArray();

        wordModels = wordModels
            .Where(wordModel => wordModel.WordGroup is null)
            .ToArray();

        // Tags are always exported with no default action to provide tag names for newly added characters and words.
        TagModel[] tagModels = await m_tagService.GetAllTagsAsync(cancellationToken);

        ApplicationDictionaryModel applicationDictionaryModel = new()
        {
            Characters = characterModels,
            CharacterGroups = characterGroupModels,
            Words = wordModels,
            WordGroups = wordGroupModels,
            Tags = tagModels,
        };

        return applicationDictionaryModel;
    }

    private Task<ApplicationDictionaryIntegrationModel> MapApplicationDictionaryModelAsync(ApplicationDictionaryModel applicationDictionaryModel, CancellationToken cancellationToken)
    {
        CharacterIntegrationModel[] characterIntegrationModels = m_mapper.Map<CharacterIntegrationModel[]>(applicationDictionaryModel.Characters);
        CharacterGroupIntegrationModel[] characterGroupIntegrationModels = m_mapper.Map<CharacterGroupIntegrationModel[]>(applicationDictionaryModel.CharacterGroups);

        WordIntegrationModel[] wordIntegrationModels = m_mapper.Map<WordIntegrationModel[]>(applicationDictionaryModel.Words);
        WordGroupIntegrationModel[] wordGroupIntegrationModels = m_mapper.Map<WordGroupIntegrationModel[]>(applicationDictionaryModel.WordGroups);

        TagIntegrationModel[] tagIntegrationModels = m_mapper.Map<TagIntegrationModel[]>(applicationDictionaryModel.Tags);

        ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel = new()
        {
            Characters = characterIntegrationModels,
            CharacterGroups = characterGroupIntegrationModels,
            Words = wordIntegrationModels,
            WordGroups = wordGroupIntegrationModels,
            Tags = tagIntegrationModels,
        };

        Task<ApplicationDictionaryIntegrationModel> completedTask = Task.FromResult(applicationDictionaryIntegrationModel);

        return completedTask;
    }

    private static async Task<ApplicationDictionaryIntegrationModel> ProcessApplicationDictionaryIntegrationModelGeneralAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.AddOrUpdate;

        ApplicationDictionaryIntegrationModel processedApplicationDictionaryIntegrationModel =
            await ProcessApplicationDictionaryIntegrationModelAsync(applicationDictionaryIntegrationModel, snapshotObjectAction, cancellationToken);

        return applicationDictionaryIntegrationModel;
    }

    private static async Task<ApplicationDictionaryIntegrationModel> ProcessApplicationDictionaryIntegrationModelGeneralNoActionAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.None;

        ApplicationDictionaryIntegrationModel processedApplicationDictionaryIntegrationModel =
            await ProcessApplicationDictionaryIntegrationModelAsync(applicationDictionaryIntegrationModel, snapshotObjectAction, cancellationToken);

        return applicationDictionaryIntegrationModel;
    }

    private static async Task<ApplicationDictionaryIntegrationModel> ProcessApplicationDictionaryIntegrationModelChangeStateAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.ChangeState;

        ApplicationDictionaryIntegrationModel processedApplicationDictionaryIntegrationModel =
            await ProcessApplicationDictionaryIntegrationModelAsync(applicationDictionaryIntegrationModel, snapshotObjectAction, cancellationToken);

        processedApplicationDictionaryIntegrationModel.Characters = [];
        foreach (CharacterGroupIntegrationModel characterGroupIntegrationModel in processedApplicationDictionaryIntegrationModel.CharacterGroups)
            characterGroupIntegrationModel.Characters = [];

        processedApplicationDictionaryIntegrationModel.Words = [];
        foreach (WordGroupIntegrationModel wordGroupIntegrationModel in processedApplicationDictionaryIntegrationModel.WordGroups)
            wordGroupIntegrationModel.Words = [];

        processedApplicationDictionaryIntegrationModel.Tags = [];

        return applicationDictionaryIntegrationModel;
    }

    private static async Task<ApplicationDictionaryIntegrationModel> ProcessApplicationDictionaryIntegrationModelPatchAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.None;

        ApplicationDictionaryIntegrationModel processedApplicationDictionaryIntegrationModel =
            await ProcessApplicationDictionaryIntegrationModelAsync(applicationDictionaryIntegrationModel, snapshotObjectAction, cancellationToken);

        processedApplicationDictionaryIntegrationModel.Characters = [];
        processedApplicationDictionaryIntegrationModel.CharacterGroups = [];

        processedApplicationDictionaryIntegrationModel.Words = [];
        processedApplicationDictionaryIntegrationModel.WordGroups = [];

        return applicationDictionaryIntegrationModel;
    }

    private static Task<ApplicationDictionaryIntegrationModel> ProcessApplicationDictionaryIntegrationModelAsync(ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel, SnapshotObjectAction snapshotObjectAction, CancellationToken cancellationToken)
    {
        foreach (CharacterIntegrationModel characterIntegrationModel in applicationDictionaryIntegrationModel.Characters)
            characterIntegrationModel.Action = snapshotObjectAction;
        foreach (CharacterGroupIntegrationModel characterGroupIntegrationModel in applicationDictionaryIntegrationModel.CharacterGroups)
            characterGroupIntegrationModel.Action = snapshotObjectAction;

        foreach (WordIntegrationModel wordIntegrationModel in applicationDictionaryIntegrationModel.Words)
            wordIntegrationModel.Action = snapshotObjectAction;
        foreach (WordGroupIntegrationModel wordGroupIntegrationModel in applicationDictionaryIntegrationModel.WordGroups)
            wordGroupIntegrationModel.Action = snapshotObjectAction;

        foreach (TagIntegrationModel tagIntegrationModel in applicationDictionaryIntegrationModel.Tags)
            tagIntegrationModel.Action = SnapshotObjectAction.None;

        Task<ApplicationDictionaryIntegrationModel> completedTask = Task.FromResult(applicationDictionaryIntegrationModel);

        return completedTask;
    }

    private class ApplicationDictionaryModel
    {
        public required CharacterModel[] Characters { get; set; } = [];

        public required CharacterGroupModel[] CharacterGroups { get; set; } = [];

        public required WordModel[] Words { get; set; } = [];

        public required WordGroupModel[] WordGroups { get; set; } = [];

        public required TagModel[] Tags { get; set; } = [];
    }
}
