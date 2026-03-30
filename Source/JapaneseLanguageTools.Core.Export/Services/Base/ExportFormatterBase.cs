using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Extensions;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Requests;
using AndreyTalanin0x00.Integrations.Export.Responses;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Export.Services.Specialized;

using JapaneseLanguageTools.Core.Blobs.Constants;

using Microsoft.IO;

namespace JapaneseLanguageTools.Core.Export.Services.Base;

public abstract class ExportFormatterBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    IExportFormatter<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    protected ExportFormatterBase()
    {
    }

    public abstract Task<ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage>> FormatAsync(ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default);
}

public abstract class JsonExportFormatterBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    PassThroughExportFormatter<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    protected JsonExportFormatterBase()
    {
    }
}

public abstract class XmlExportFormatterBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    ExportFormatterBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    private static readonly Encoding s_encoding = Encoding.Unicode;

    private readonly IBlobManager m_blobManager;
    private readonly RecyclableMemoryStreamManager m_recyclableMemoryStreamManager;

    protected XmlExportFormatterBase(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        m_blobManager = blobManager;
        m_recyclableMemoryStreamManager = recyclableMemoryStreamManager;
    }

    /// <inheritdoc />
    public override async Task<ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage>> FormatAsync(ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        int size = exportIntermediateObjectPackageBatch.Size;

        ExportTarget[] exportTargets = exportIntermediateObjectPackageBatch.ExportTargets;

        ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent>[] exportIntermediateObjectPackageWrappers =
            exportIntermediateObjectPackageBatch.ExportIntermediateObjectPackageWrappers;
        ExportObjectPackageWrapper<TExportObjectPackage>[] exportObjectPackageWrappers =
            exportIntermediateObjectPackageBatch.ExportObjectPackageWrappers;

        for (int index = 0; index < size; index++)
        {
            ExportTarget exportTarget = exportTargets[index];

            ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent> exportIntermediateObjectPackageWrapper =
                exportIntermediateObjectPackageWrappers[index];
            ExportObjectPackageWrapper<TExportObjectPackage> exportObjectPackageWrapper =
                exportObjectPackageWrappers[index];

            BlobReference blobReference = exportTarget.BlobReference;

            string blobBucket = BlobBucketConstants.ExportIntermediateBlobs;

            GetBlobMetadataRequest getBlobMetadataRequest = new(blobReference);

            GetBlobMetadataResponse getBlobMetadataResponse = await m_blobManager.GetBlobMetadataAsync(getBlobMetadataRequest, cancellationToken);

            BlobMetadata blobMetadata = getBlobMetadataResponse.BlobMetadata;

            TimeSpan blobExpiresIn = BlobBucketExpirationPeriodConstants.GetBlobBucketExpirationPeriod(blobBucket);

            XDocument xDocument;
            {
                DownloadBlobRequest downloadBlobRequest = new(blobReference);

                await using DisposableBlobStreamMetadataPair<Stream, DownloadBlobResponse> downloadBlobResponsePair =
                    (await m_blobManager.DownloadBlobAsync(downloadBlobRequest, cancellationToken)).AsDisposable();

                DeleteBlobRequest deleteBlobRequest = new(blobReference);

                await m_blobManager.DeleteBlobAsync(deleteBlobRequest, cancellationToken);

                (Stream downloadBlobStream, DownloadBlobResponse downloadBlobResponse) = downloadBlobResponsePair;

                using (StreamReader downloadBlobStreamReader = new(downloadBlobStream, encoding: s_encoding, leaveOpen: true))
                    xDocument = await XDocument.LoadAsync(downloadBlobStreamReader, LoadOptions.None, cancellationToken);

                ;
            }

            ApplyDocumentChangesParameters applyDocumentChangesParameters = new()
            {
                XDocument = xDocument,
                ExportIntermediateObjectPackageWrapper = exportIntermediateObjectPackageWrapper,
                ExportObjectPackageWrapper = exportObjectPackageWrapper,
            };

            await ApplyDocumentChangesAsync(applyDocumentChangesParameters, cancellationToken);

            using (MemoryStream uploadBlobStream = m_recyclableMemoryStreamManager.GetStream())
            {
                using (StreamWriter uploadBlobStreamWriter = new(uploadBlobStream, encoding: s_encoding, leaveOpen: true))
                {
                    xDocument.Save(uploadBlobStreamWriter, SaveOptions.None);

                    // Append the final new line.
                    await uploadBlobStreamWriter.WriteLineAsync();
                }

                uploadBlobStream.Seek(0L, SeekOrigin.Begin);

                UploadBlobRequest uploadBlobRequest = new(blobBucket, blobMetadata, blobExpiresIn);

                BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair = new(uploadBlobStream, uploadBlobRequest);

                UploadBlobResponse uploadBlobResponse = await m_blobManager.UploadBlobAsync(uploadBlobRequestPair, cancellationToken);

                blobReference = uploadBlobResponse.BlobReference;
            }

            exportTarget.BlobReference = blobReference;
        }

        return exportIntermediateObjectPackageBatch;
    }

    protected abstract Task ApplyDocumentChangesAsync(ApplyDocumentChangesParameters parameters, CancellationToken cancellationToken);

    protected class ApplyDocumentChangesParameters
    {
        public required XDocument XDocument { get; init; }

        public required ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent> ExportIntermediateObjectPackageWrapper { get; init; }

        public required ExportObjectPackageWrapper<TExportObjectPackage> ExportObjectPackageWrapper { get; init; }
    }
}
