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

public class TagImportProcessor :
    IImportProcessor<TagImportRequest, TagImportResponse, Object, TagObjectPackageIntegrationModel>
{
    private readonly IMapper m_mapper;
    private readonly ITagService m_tagService;
    private readonly MainDbContext m_context;
    private readonly ISnapshotHashCalculator m_snapshotHashCalculator;

    public TagImportProcessor(IMapper mapper, ITagService tagService, MainDbContext context, ISnapshotHashCalculator snapshotHashCalculator)
    {
        m_mapper = mapper;
        m_tagService = tagService;
        m_context = context;
        m_snapshotHashCalculator = snapshotHashCalculator;
    }

    /// <inheritdoc />
    public async Task<TagImportResponse> ProcessAsync(TagImportRequest tagImportRequest, ImportObjectPackageBatch<Object, TagObjectPackageIntegrationModel>[] importObjectPackageBatches, CancellationToken cancellationToken = default)
    {
        string snapshotHash = await m_snapshotHashCalculator.CalculateSnapshotHashAsync(cancellationToken);

        using (IDbContextTransaction contextTransaction = await m_context.Database.BeginTransactionAsync(cancellationToken))
        {
            foreach (ImportObjectPackageBatch<Object, TagObjectPackageIntegrationModel> importObjectPackageBatch in importObjectPackageBatches)
            {
                int size = importObjectPackageBatch.Size;

                ImportSource[] importSources = importObjectPackageBatch.ImportSources;
                ImportObjectPackageWrapper<TagObjectPackageIntegrationModel>[] importObjectPackageWrappers = importObjectPackageBatch.ImportObjectPackageWrappers;

                for (int index = 0; index < size; index++)
                {
                    ImportSource importSource = importSources[index];
                    ImportObjectPackageWrapper<TagObjectPackageIntegrationModel> importObjectPackageWrapper = importObjectPackageWrappers[index];

                    BlobMetadata blobMetadata = importSource.BlobMetadata;

                    TagObjectPackageIntegrationModel tagObjectPackageIntegrationModel = importObjectPackageWrapper.ImportObjectPackage;

                    if (tagObjectPackageIntegrationModel.SnapshotHash != snapshotHash && !tagObjectPackageIntegrationModel.ForceMode)
                        throw new InvalidOperationException($"Current database state is different from one the '{blobMetadata.FileName}' import template was based on. Unable to proceed unless the force-mode is specified.");

                    await ProcessTagsAsync(tagObjectPackageIntegrationModel, cancellationToken);
                }
            }

            await contextTransaction.CommitAsync(cancellationToken);
        }

        ImportStatus importStatus = ImportStatus.Completed;
        ImportResponseMessage[] importResponseMessages = [];

        TagImportResponse tagImportResponse = new()
        {
            Status = importStatus,
            Messages = importResponseMessages,
            Request = tagImportRequest,
        };

        return tagImportResponse;
    }

    private async Task<TagModel[]> ProcessTagsAsync(TagObjectPackageIntegrationModel tagObjectPackageIntegrationModel, CancellationToken cancellationToken)
    {
        TagId[] tagIds = tagObjectPackageIntegrationModel.Tags
            .Select(tagIntegrationModel => new TagId(tagIntegrationModel.Id))
            .ToArray();

        HashSet<int> existingTagIdValues = (await m_tagService.GetTagsAsync(tagIds, cancellationToken))
            .Select(existingTagModel => existingTagModel.Id)
            .ToHashSet();

        TagModel[] tagModels = m_mapper.Map<TagModel[]>(tagObjectPackageIntegrationModel.Tags);

        foreach ((TagModel tagModel, TagIntegrationModel tagIntegrationModel) in tagModels.Zip(tagObjectPackageIntegrationModel.Tags))
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
}
