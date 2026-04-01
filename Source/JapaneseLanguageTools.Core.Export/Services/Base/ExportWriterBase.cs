using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Extensions;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Enumerations;
using AndreyTalanin0x00.Integrations.Export.Requests;
using AndreyTalanin0x00.Integrations.Export.Responses;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Core.Blobs.Constants;

using Microsoft.IO;

namespace JapaneseLanguageTools.Core.Export.Services.Base;

public abstract class ExportWriterBase<TExportRequest, TExportResponse> :
    IExportWriter<TExportRequest, TExportResponse>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
{
    private readonly IBlobManager m_blobManager;
    private readonly RecyclableMemoryStreamManager m_recyclableMemoryStreamManager;

    protected ExportWriterBase(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        m_blobManager = blobManager;
        m_recyclableMemoryStreamManager = recyclableMemoryStreamManager;
    }

    /// <inheritdoc />
    public async Task<TExportResponse> WriteAsync(ExportTargetBatch exportTargetBatch, TExportRequest exportRequest, CancellationToken cancellationToken = default)
    {
        ExportTarget[] exportTargets = exportTargetBatch.ExportTargets;
        ExportTargetContext exportTargetContext = exportTargetBatch.ExportTargetContext;

        TExportResponse exportResponse;

        using (MemoryStream uploadBlobStream = m_recyclableMemoryStreamManager.GetStream())
        {
            string blobFileName = string.Empty;
            string blobMimeType = string.Empty;

            void SetFileName(string fileName) => blobFileName = fileName;
            void SetMimeType(string mimeType) => blobMimeType = mimeType;

            WriteExportTargetParameters writeExportTargetParameters = new()
            {
                Stream = uploadBlobStream,
                ExportTargetBatch = exportTargetBatch,
                ExportRequest = exportRequest,
                SetFileNameCallback = SetFileName,
                SetMimeTypeCallback = SetMimeType,
            };

            bool writeMultipleFilesAsArchive = exportTargets.Length > 1 || exportTargetContext.ExportResources.Length > 0;

            if (writeMultipleFilesAsArchive)
                await WriteExportTargetMultipleFilesAsync(writeExportTargetParameters, cancellationToken);
            else
                await WriteExportTargetSingleFileAsync(writeExportTargetParameters, cancellationToken);

            uploadBlobStream.Seek(0L, SeekOrigin.Begin);

            string blobBucket = BlobBucketConstants.ExportBlobs;

            BlobMetadata blobMetadata = new()
            {
                FileName = blobFileName,
                MimeType = blobMimeType,
            };

            TimeSpan blobExpiresIn = BlobBucketExpirationPeriodConstants.GetBlobBucketExpirationPeriod(blobBucket);

            UploadBlobRequest uploadBlobRequest = new(blobBucket, blobMetadata, blobExpiresIn);

            BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair = new(uploadBlobStream, uploadBlobRequest);

            UploadBlobResponse uploadBlobResponse = await m_blobManager.UploadBlobAsync(uploadBlobRequestPair, cancellationToken);

            BlobReference blobReference = uploadBlobResponse.BlobReference;

            ExportStatus exportStatus = ExportStatus.Completed;

            exportResponse = CreateExportResponse(exportStatus, exportRequest, blobReference, blobMetadata);
        }

        return exportResponse;
    }

    protected abstract string GetFileName(string mimeType, TExportRequest exportRequest);

    protected abstract TExportResponse CreateExportResponse(ExportStatus exportStatus, TExportRequest exportRequest, BlobReference blobReference, BlobMetadata blobMetadata);

    private async Task WriteExportTargetSingleFileAsync(WriteExportTargetParameters parameters, CancellationToken cancellationToken = default)
    {
        Stream stream = parameters.Stream;

        ExportTargetBatch exportTargetBatch = parameters.ExportTargetBatch;

        ExportTarget exportTarget = exportTargetBatch.ExportTargets.Single();

        BlobReference exportTargetBlobReference = exportTarget.BlobReference;

        DownloadBlobRequest exportTargetDownloadBlobRequest = new(exportTargetBlobReference);

        await using DisposableBlobStreamMetadataPair<Stream, DownloadBlobResponse> exportTargetDownloadBlobResponsePair =
           (await m_blobManager.DownloadBlobAsync(exportTargetDownloadBlobRequest, cancellationToken)).AsDisposable();

        DeleteBlobRequest exportTargetDeleteBlobRequest = new(exportTargetBlobReference);

        await m_blobManager.DeleteBlobAsync(exportTargetDeleteBlobRequest, cancellationToken);

        (Stream exportTargetDownloadStream, DownloadBlobResponse exportTargetDownloadBlobResponse) = exportTargetDownloadBlobResponsePair;

        BlobMetadata exportTargetBlobMetadata = exportTargetDownloadBlobResponse.BlobMetadata;

        await exportTargetDownloadStream.CopyToAsync(stream, cancellationToken);

        string exportTargetBlobMimeType = exportTargetBlobMetadata.MimeType;
        string exportTargetBlobFileName = exportTargetBlobMetadata.FileName;

        parameters.SetMimeTypeCallback(exportTargetBlobMimeType);
        parameters.SetFileNameCallback(exportTargetBlobFileName);
    }

    private static Task WriteExportTargetMultipleFilesAsync(WriteExportTargetParameters parameters, CancellationToken cancellationToken = default)
    {
        Task completedTask = Task.FromException(new NotSupportedException("Support for archives has not been added yet."));

        return completedTask;
    }

    private class WriteExportTargetParameters
    {
        public required Stream Stream { get; init; }

        public required ExportTargetBatch ExportTargetBatch { get; init; }

        public required TExportRequest ExportRequest { get; init; }

        public required Action<string> SetFileNameCallback { get; init; }

        public required Action<string> SetMimeTypeCallback { get; init; }
    }
}
