using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

namespace JapaneseLanguageTools.Core.Import.Services.Abstractions;

public interface IImportBlobUploader
{
    public Task<UploadBlobResponseModel> UploadImportBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequestModel> uploadBlobRequestModelPair, CancellationToken cancellationToken = default);
}
