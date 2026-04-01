using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;
using JapaneseLanguageTools.Core.Export.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Export.Services;

public class ExportBlobDownloader : IExportBlobDownloader
{
    private readonly IMapper m_mapper;
    private readonly IBlobManager m_blobManager;

    public ExportBlobDownloader(IMapper mapper, IBlobManager blobManager)
    {
        m_mapper = mapper;
        m_blobManager = blobManager;
    }

    /// <inheritdoc />
    public async Task<GetBlobMetadataResponseModel?> GetExportBlobMetadataAsync(GetBlobMetadataRequestModel getBlobMetadataRequestModel, CancellationToken cancellationToken = default)
    {
        GetBlobMetadataRequest getBlobMetadataRequest = m_mapper.Map<GetBlobMetadataRequest>(getBlobMetadataRequestModel);

        GetBlobMetadataResponse? getBlobMetadataResponse = null;

        void SetGetBlobMetadataResponse(GetBlobMetadataResponse outGetBlobMetadataResponse) =>
            getBlobMetadataResponse = outGetBlobMetadataResponse;

        bool result = await m_blobManager.TryGetBlobMetadataAsync(getBlobMetadataRequest, SetGetBlobMetadataResponse, cancellationToken);

        if (getBlobMetadataResponse is null || !result)
            return null;

        GetBlobMetadataResponseModel getBlobMetadataResponseModel = m_mapper.Map<GetBlobMetadataResponseModel>(getBlobMetadataResponse);

        return getBlobMetadataResponseModel;
    }

    /// <inheritdoc />
    public async Task<GetBlobExpirationTimeResponseModel?> GetExportBlobExpirationTimeAsync(GetBlobExpirationTimeRequestModel getBlobExpirationTimeRequestModel, CancellationToken cancellationToken = default)
    {
        GetBlobExpirationTimeRequest getBlobExpirationTimeRequest = m_mapper.Map<GetBlobExpirationTimeRequest>(getBlobExpirationTimeRequestModel);

        GetBlobExpirationTimeResponse? getBlobExpirationTimeResponse = null;

        void SetGetBlobExpirationTimeResponse(GetBlobExpirationTimeResponse outGetBlobExpirationTimeResponse) =>
            getBlobExpirationTimeResponse = outGetBlobExpirationTimeResponse;

        bool result = await m_blobManager.TryGetBlobExpirationTimeAsync(getBlobExpirationTimeRequest, SetGetBlobExpirationTimeResponse, cancellationToken);

        if (getBlobExpirationTimeResponse is null || !result)
            return null;

        GetBlobExpirationTimeResponseModel getBlobExpirationTimeResponseModel = m_mapper.Map<GetBlobExpirationTimeResponseModel>(getBlobExpirationTimeResponse);

        return getBlobExpirationTimeResponseModel;
    }

    /// <inheritdoc />
    public async Task<BlobStreamMetadataPair<Stream, DownloadBlobResponseModel>?> DownloadExportBlobAsync(DownloadBlobRequestModel downloadBlobRequestModel, CancellationToken cancellationToken = default)
    {
        DownloadBlobRequest downloadBlobRequest = m_mapper.Map<DownloadBlobRequest>(downloadBlobRequestModel);

        BlobStreamMetadataPair<Stream, DownloadBlobResponse>? downloadBlobResponsePair = null;

        void SetDownloadBlobResponsePair(BlobStreamMetadataPair<Stream, DownloadBlobResponse> outDownloadBlobResponsePair) =>
            downloadBlobResponsePair = outDownloadBlobResponsePair;

        bool result = await m_blobManager.TryDownloadBlobAsync(downloadBlobRequest, SetDownloadBlobResponsePair, cancellationToken);

        if (downloadBlobResponsePair is null || !result)
            return null;

        (Stream downloadBlobStream, DownloadBlobResponse downloadBlobResponse) = downloadBlobResponsePair;

        DownloadBlobResponseModel downloadBlobResponseModel = m_mapper.Map<DownloadBlobResponseModel>(downloadBlobResponse);
        BlobStreamMetadataPair<Stream, DownloadBlobResponseModel> downloadBlobResponseModelPair = new(downloadBlobStream, downloadBlobResponseModel);

        return downloadBlobResponseModelPair;
    }
}
