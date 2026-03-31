using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;
using JapaneseLanguageTools.Core.Export.Services.Abstractions;
using JapaneseLanguageTools.Core.Services.Abstractions;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class TagExportStreamEnabledService : ITagExportStreamEnabledService
{
    private readonly IMapper m_mapper;
    private readonly IExportPipeline<TagExportRequest, TagExportResponse> m_exportPipeline;
    private readonly IExportBlobDownloader m_exportBlobDownloader;

    public TagExportStreamEnabledService(IMapper mapper, IExportPipeline<TagExportRequest, TagExportResponse> exportPipeline, IExportBlobDownloader exportBlobDownloader)
    {
        m_mapper = mapper;
        m_exportPipeline = exportPipeline;
        m_exportBlobDownloader = exportBlobDownloader;
    }

    /// <inheritdoc />
    public async Task<ExportTagsResponseModel> ExportTagsAsync(ExportTagsRequestModel exportTagsRequestModel, CancellationToken cancellationToken = default)
    {
        TagExportRequest tagExportRequest = m_mapper.Map<TagExportRequest>(exportTagsRequestModel);
        TagExportResponse tagExportResponse = await m_exportPipeline.ExportAsync(tagExportRequest, cancellationToken);

        ExportTagsResponseModel exportTagsResponseModel = m_mapper.Map<ExportTagsResponseModel>(tagExportResponse);

        return exportTagsResponseModel;
    }

    /// <inheritdoc />
    public async Task<GetBlobMetadataResponseModel?> GetExportBlobMetadataAsync(GetBlobMetadataRequestModel getBlobMetadataRequestModel, CancellationToken cancellationToken = default)
    {
        return await m_exportBlobDownloader.GetExportBlobMetadataAsync(getBlobMetadataRequestModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<GetBlobExpirationTimeResponseModel?> GetExportBlobExpirationTimeAsync(GetBlobExpirationTimeRequestModel getBlobExpirationTimeRequestModel, CancellationToken cancellationToken = default)
    {
        return await m_exportBlobDownloader.GetExportBlobExpirationTimeAsync(getBlobExpirationTimeRequestModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BlobStreamMetadataPair<Stream, DownloadBlobResponseModel>?> DownloadExportBlobAsync(DownloadBlobRequestModel downloadBlobRequestModel, CancellationToken cancellationToken = default)
    {
        return await m_exportBlobDownloader.DownloadExportBlobAsync(downloadBlobRequestModel, cancellationToken);
    }
}
