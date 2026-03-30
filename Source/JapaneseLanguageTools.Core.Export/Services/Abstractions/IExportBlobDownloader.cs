using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

namespace JapaneseLanguageTools.Core.Export.Services.Abstractions;

public interface IExportBlobDownloader
{
    public Task<GetBlobMetadataResponseModel?> GetExportBlobMetadataAsync(GetBlobMetadataRequestModel getBlobMetadataRequestModel, CancellationToken cancellationToken = default);

    public Task<GetBlobExpirationTimeResponseModel?> GetExportBlobExpirationTimeAsync(GetBlobExpirationTimeRequestModel getBlobExpirationTimeRequestModel, CancellationToken cancellationToken = default);

    public Task<BlobStreamMetadataPair<Stream, DownloadBlobResponseModel>?> DownloadExportBlobAsync(DownloadBlobRequestModel downloadBlobRequestModel, CancellationToken cancellationToken = default);
}
