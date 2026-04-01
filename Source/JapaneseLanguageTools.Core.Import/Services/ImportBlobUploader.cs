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
using JapaneseLanguageTools.Core.Blobs.Constants;
using JapaneseLanguageTools.Core.Import.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Import.Services;

public class ImportBlobUploader : IImportBlobUploader
{
    private readonly IMapper m_mapper;
    private readonly IBlobManager m_blobManager;

    public ImportBlobUploader(IMapper mapper, IBlobManager blobManager)
    {
        m_mapper = mapper;
        m_blobManager = blobManager;
    }

    /// <inheritdoc />
    public async Task<UploadBlobResponseModel> UploadImportBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequestModel> uploadBlobRequestModelPair, CancellationToken cancellationToken = default)
    {
        (Stream uploadBlobStream, UploadBlobRequestModel uploadBlobRequestModel) = uploadBlobRequestModelPair;

        UploadBlobRequest uploadBlobRequest = m_mapper.Map<UploadBlobRequest>(uploadBlobRequestModel);

        uploadBlobRequest.BlobBucket = BlobBucketConstants.ImportBlobs;
        uploadBlobRequest.BlobExpiresIn = BlobBucketExpirationPeriodConstants.GetBlobBucketExpirationPeriod(uploadBlobRequest.BlobBucket);

        BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair = new(uploadBlobStream, uploadBlobRequest);
        UploadBlobResponse uploadBlobResponse = await m_blobManager.UploadBlobAsync(uploadBlobRequestPair, cancellationToken);

        UploadBlobResponseModel uploadBlobResponseModel = m_mapper.Map<UploadBlobResponseModel>(uploadBlobResponse);

        return uploadBlobResponseModel;
    }
}
