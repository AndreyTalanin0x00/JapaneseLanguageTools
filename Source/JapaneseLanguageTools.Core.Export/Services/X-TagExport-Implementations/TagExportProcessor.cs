using System;
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

namespace JapaneseLanguageTools.Core.Export.Services;

public class TagExportProcessor :
    IExportProcessor<TagExportRequest, TagExportResponse, Object, TagObjectPackageIntegrationModel>
{
    private readonly TimeProvider m_timeProvider;
    private readonly ITagService m_tagService;
    private readonly ISnapshotHashCalculator m_snapshotHashCalculator;
    private readonly IMapper m_mapper;

    public TagExportProcessor(TimeProvider timeProvider, ITagService tagService, ISnapshotHashCalculator snapshotHashCalculator, IMapper mapper)
    {
        m_timeProvider = timeProvider;
        m_tagService = tagService;
        m_snapshotHashCalculator = snapshotHashCalculator;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ExportObjectPackageBatch<Object, TagObjectPackageIntegrationModel>> ProcessAsync(TagExportRequest tagExportRequest, CancellationToken cancellationToken = default)
    {
        SnapshotType snapshotType = tagExportRequest.SnapshotType;

        DateTimeOffset snapshotTime = m_timeProvider.GetUtcNow();

        string snapshotHash = await m_snapshotHashCalculator.CalculateSnapshotHashAsync(cancellationToken);

        TagModel[] tagModels = !(snapshotType == SnapshotType.Patch || snapshotType == SnapshotType.Unknown)
            ? await m_tagService.GetAllTagsAsync(cancellationToken)
            : [];

        TagIntegrationModel[] tagIntegrationModels = m_mapper.Map<TagIntegrationModel[]>(tagModels);

        tagIntegrationModels = snapshotType switch
        {
            SnapshotType.General => await ProcessTagIntegrationModelArrayGeneralAsync(tagIntegrationModels, cancellationToken),
            SnapshotType.GeneralNoAction => await ProcessTagIntegrationModelArrayGeneralNoActionAsync(tagIntegrationModels, cancellationToken),
            SnapshotType.ChangeState => await ProcessTagIntegrationModelArrayChangeStateAsync(tagIntegrationModels, cancellationToken),
            SnapshotType.Patch => await ProcessTagIntegrationModelArrayPatchAsync(tagIntegrationModels, cancellationToken),
            _ or SnapshotType.Unknown => throw new NotSupportedException($"Unknown snapshot type: {(int)snapshotType} ({snapshotType})."),
        };

        const bool forceMode = false;
        TagObjectPackageIntegrationModel tagObjectPackageIntegrationModel = new()
        {
            SnapshotType = snapshotType,
            SnapshotTime = snapshotTime,
            SnapshotHash = snapshotHash,
            Tags = tagIntegrationModels,
            ForceMode = forceMode,
        };

        const int size = 1;
        ExportObjectPackageBatch<Object, TagObjectPackageIntegrationModel> exportObjectPackageBatch = new()
        {
            Size = size,
            ExportTargets = new ExportTarget[size],
            ExportIntermediateObjectPackageWrappers = new ExportIntermediateObjectPackageWrapper<Object>[size],
            ExportObjectPackageWrappers = new ExportObjectPackageWrapper<TagObjectPackageIntegrationModel>[size],
            ExportTargetContext = new ExportTargetContext()
            {
                ExportResources = [],
            },
        };

        ExportObjectPackageWrapper<TagObjectPackageIntegrationModel>[] exportObjectPackageWrappers =
            exportObjectPackageBatch.ExportObjectPackageWrappers;

        exportObjectPackageWrappers[0] = new ExportObjectPackageWrapper<TagObjectPackageIntegrationModel>()
        {
            ExportObjectPackage = tagObjectPackageIntegrationModel,
        };

        return exportObjectPackageBatch;
    }

    private static async Task<TagIntegrationModel[]> ProcessTagIntegrationModelArrayGeneralAsync(TagIntegrationModel[] tagIntegrationModels, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.AddOrUpdate;

        TagIntegrationModel[] processedTagIntegrationModels =
            await ProcessTagIntegrationModelArrayAsync(tagIntegrationModels, snapshotObjectAction, cancellationToken);

        return processedTagIntegrationModels;
    }

    private static async Task<TagIntegrationModel[]> ProcessTagIntegrationModelArrayGeneralNoActionAsync(TagIntegrationModel[] tagIntegrationModels, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.None;

        TagIntegrationModel[] processedTagIntegrationModels =
            await ProcessTagIntegrationModelArrayAsync(tagIntegrationModels, snapshotObjectAction, cancellationToken);

        return processedTagIntegrationModels;
    }

    private static async Task<TagIntegrationModel[]> ProcessTagIntegrationModelArrayChangeStateAsync(TagIntegrationModel[] tagIntegrationModels, CancellationToken cancellationToken)
    {
        SnapshotObjectAction snapshotObjectAction = SnapshotObjectAction.None;

        TagIntegrationModel[] processedTagIntegrationModels =
            await ProcessTagIntegrationModelArrayAsync(tagIntegrationModels, snapshotObjectAction, cancellationToken);

        return processedTagIntegrationModels;
    }

    private static async Task<TagIntegrationModel[]> ProcessTagIntegrationModelArrayPatchAsync(TagIntegrationModel[] tagIntegrationModels, CancellationToken cancellationToken)
    {
        TagIntegrationModel[] processedTagIntegrationModels = [];

        return processedTagIntegrationModels;
    }

    private static Task<TagIntegrationModel[]> ProcessTagIntegrationModelArrayAsync(TagIntegrationModel[] tagIntegrationModels, SnapshotObjectAction snapshotObjectAction, CancellationToken cancellationToken)
    {
        foreach (TagIntegrationModel tagIntegrationModel in tagIntegrationModels)
            tagIntegrationModel.Action = snapshotObjectAction;

        Task<TagIntegrationModel[]> completedTask = Task.FromResult(tagIntegrationModels);

        return completedTask;
    }
}
