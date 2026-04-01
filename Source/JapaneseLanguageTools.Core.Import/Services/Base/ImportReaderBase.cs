using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Extensions;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Exceptions;
using AndreyTalanin0x00.Integrations.Import.Requests;
using AndreyTalanin0x00.Integrations.Import.Responses;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using JapaneseLanguageTools.Core.Blobs.Constants;

using Microsoft.IO;

namespace JapaneseLanguageTools.Core.Import.Services.Base;

public class ImportReaderBase<TImportRequest, TImportResponse> :
    IImportReader<TImportRequest, TImportResponse>
    where TImportRequest : ImportRequest
    where TImportResponse : ImportResponse
{
    private readonly IBlobManager m_blobManager;
    private readonly RecyclableMemoryStreamManager m_recyclableMemoryStreamManager;

    public ImportReaderBase(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        m_blobManager = blobManager;
        m_recyclableMemoryStreamManager = recyclableMemoryStreamManager;
    }

    /// <inheritdoc />
    public async Task<ImportSourceBatch[]> ReadAsync(TImportRequest importRequest, CancellationToken cancellationToken = default)
    {
        BlobReference[] blobReferences = importRequest.BlobReferences;

        ImportSourceBatch[] importSourceBatches = new ImportSourceBatch[blobReferences.Length];

        ImportResponseMessage?[] importResponseErrorMessages = new ImportResponseMessage?[blobReferences.Length];

        for (int blobIndex = 0; blobIndex < blobReferences.Length; blobIndex++)
        {
            BlobReference blobReference = blobReferences[blobIndex];

            ImportSourceBatch importSourceBatch;
            {
                DownloadBlobRequest downloadBlobRequest = new(blobReference);

                BlobStreamMetadataPair<Stream, DownloadBlobResponse>? downloadBlobResponsePair = null;

                void SetDownloadBlobResponsePair(BlobStreamMetadataPair<Stream, DownloadBlobResponse> outDownloadBlobResponsePair) =>
                    downloadBlobResponsePair = outDownloadBlobResponsePair;

                bool result = await m_blobManager.TryDownloadBlobAsync(downloadBlobRequest, SetDownloadBlobResponsePair, cancellationToken);

                if (downloadBlobResponsePair is null || !result)
                {
                    ImportResponseMessage importResponseMessage = new()
                    {
                        Type = ImportResponseMessageType.Error,
                        Text = $"The import blob \"{blobReference}\" has expired and no longer exists.",
                    };

                    importResponseErrorMessages[blobIndex] = importResponseMessage;

                    continue;
                }

                await using DisposableBlobStreamMetadataPair<Stream, DownloadBlobResponse> disposableBlobStreamMetadataPair =
                    downloadBlobResponsePair.AsDisposable();

                (Stream downloadBlobStream, DownloadBlobResponse downloadBlobResponse) = disposableBlobStreamMetadataPair;

                BlobMetadata blobMetadata = downloadBlobResponse.BlobMetadata;

                ImportSource[] importSources = [];
                ImportSourceContext importSourceContext = new()
                {
                    ImportResources = [],
                };

                void SetImportSources(ImportSource[] outImportSources) => importSources = outImportSources;
                void SetImportSourceContext(ImportSourceContext outImportSourceContext) => importSourceContext = outImportSourceContext;

                TryCreateImportSourceParameters tryCreateImportSourceParameters = new()
                {
                    Stream = downloadBlobStream,
                    DownloadBlobRequest = downloadBlobRequest,
                    DownloadBlobResponse = downloadBlobResponse,
                    SetImportSourcesCallback = SetImportSources,
                    SetImportSourceContextCallback = SetImportSourceContext,
                };

                switch (blobMetadata.MimeType)
                {
                    case MediaTypeNames.Application.Json:
                    case MediaTypeNames.Application.Xml:
                    case MediaTypeNames.Text.Xml:
                        await TryCreateImportSourceSingleFileAsync(tryCreateImportSourceParameters, cancellationToken);
                        break;

                    case MediaTypeNames.Application.Zip:
                        await TryCreateImportSourceMultipleFilesAsync(tryCreateImportSourceParameters, cancellationToken);
                        break;

                    default:
                        throw new NotSupportedException($"The {blobMetadata.FileName} could not be read. Support for the {blobMetadata.MimeType} has not been added yet.");
                }

                int size = importSources.Length;

                importSourceBatch = new()
                {
                    Size = size,
                    ImportSources = importSources,
                    ImportSourceContext = importSourceContext,
                };
            }

            importSourceBatches[blobIndex] = importSourceBatch;
        }

        if (importResponseErrorMessages.Any(importResponseErrorMessage => importResponseErrorMessage is not null))
        {
            IEnumerable<ImportResponseMessage> importResponseMessagesToThrow = importResponseErrorMessages
                .Where(importResponseErrorMessage => importResponseErrorMessage is not null)
                .Cast<ImportResponseMessage>();

            throw new ImportException("Some of the import blobs are not available.", importResponseMessagesToThrow);
        }

        return importSourceBatches;
    }

    private async Task TryCreateImportSourceSingleFileAsync(TryCreateImportSourceParameters parameters, CancellationToken cancellationToken = default)
    {
        Stream stream = parameters.Stream;

        DownloadBlobRequest downloadBlobRequest = parameters.DownloadBlobRequest;
        DownloadBlobResponse downloadBlobResponse = parameters.DownloadBlobResponse;

        ImportSource importSource;
        using (MemoryStream uploadBlobStream = m_recyclableMemoryStreamManager.GetStream())
        {
            stream.CopyTo(uploadBlobStream);

            uploadBlobStream.Seek(0L, SeekOrigin.Begin);

            string blobBucket = BlobBucketConstants.ImportIntermediateBlobs;

            BlobMetadata blobMetadata = downloadBlobResponse.BlobMetadata;

            TimeSpan blobExpiresIn = BlobBucketExpirationPeriodConstants.GetBlobBucketExpirationPeriod(blobBucket);

            UploadBlobRequest uploadBlobRequest = new(blobBucket, blobMetadata, blobExpiresIn);

            BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair = new(uploadBlobStream, uploadBlobRequest);

            UploadBlobResponse importSourceUploadBlobResponse = await m_blobManager.UploadBlobAsync(uploadBlobRequestPair, cancellationToken);

            BlobReference blobReference = importSourceUploadBlobResponse.BlobReference;

            importSource = new()
            {
                BlobReference = blobReference,
                BlobMetadata = blobMetadata,
            };
        }

        ImportSource[] importSources = [importSource];
        ImportSourceContext importSourceContext = new()
        {
            ImportResources = [],
        };

        parameters.SetImportSourcesCallback(importSources);
        parameters.SetImportSourceContextCallback(importSourceContext);
    }

    private static Task TryCreateImportSourceMultipleFilesAsync(TryCreateImportSourceParameters parameters, CancellationToken cancellationToken = default)
    {
        Task completedTask = Task.FromException(new NotSupportedException("Support for archives has not been added yet."));

        return completedTask;
    }

    private class TryCreateImportSourceParameters
    {
        public required Stream Stream { get; set; }

        public required DownloadBlobRequest DownloadBlobRequest { get; set; }

        public required DownloadBlobResponse DownloadBlobResponse { get; set; }

        public required Action<ImportSource[]> SetImportSourcesCallback { get; set; }

        public required Action<ImportSourceContext> SetImportSourceContextCallback { get; set; }
    }
}
