using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Extensions;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Exceptions.Factories;
using AndreyTalanin0x00.Integrations.Import.Requests;
using AndreyTalanin0x00.Integrations.Import.Responses;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Import.Services.Base;

public abstract class ImportSerializerBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage> :
    IImportSerializer<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>
    where TImportRequest : ImportRequest
    where TImportResponse : ImportResponse
    where TImportIntermediateObjectPackageCurrent : class
    where TImportObjectPackage : class
{
    private readonly IBlobManager m_blobManager;

    protected ImportSerializerBase(IBlobManager blobManager)
    {
        m_blobManager = blobManager;
    }

    /// <inheritdoc />
    public abstract bool AcceptsBlobsOfMimeType(string blobMimeType);

    /// <inheritdoc />
    public async Task<ImportIntermediateObjectPackageBatch<TImportIntermediateObjectPackageCurrent, TImportObjectPackage>> DeserializeAsync(ImportIntermediateObjectPackageBatch<TImportIntermediateObjectPackageCurrent, TImportObjectPackage> importIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        int size = importIntermediateObjectPackageBatch.Size;

        ImportSource[] importSources = importIntermediateObjectPackageBatch.ImportSources;
        ImportSourceContext importSourceContext = importIntermediateObjectPackageBatch.ImportSourceContext;

        ImportIntermediateObjectPackageWrapper<TImportIntermediateObjectPackageCurrent>[] importIntermediateObjectPackageWrappers =
            importIntermediateObjectPackageBatch.ImportIntermediateObjectPackageWrappers;

        if (size != importIntermediateObjectPackageWrappers.Length)
            ImportExceptionFactory.CreateImportIntermediateObjectPackageWrapperCountMismatchException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>();

        for (int index = 0; index < size; index++)
        {
            ImportSource importSource = importSources[index];

            // The importIntermediateObjectPackageWrappers array is initialized with null references.
            importIntermediateObjectPackageWrappers[index] ??= new ImportIntermediateObjectPackageWrapper<TImportIntermediateObjectPackageCurrent>()
            {
                // The ImportIntermediateObjectPackage property is set at the end of the current iteration.
                ImportIntermediateObjectPackage = null!,
            };

            ImportIntermediateObjectPackageWrapper<TImportIntermediateObjectPackageCurrent> importIntermediateObjectPackageWrapper =
                importIntermediateObjectPackageWrappers[index];

            BlobReference blobReference = importSource.BlobReference;

            DownloadBlobRequest downloadBlobRequest = new(blobReference);

            TImportIntermediateObjectPackageCurrent? importIntermediateObjectPackage;
            {
                await using DisposableBlobStreamMetadataPair<Stream, DownloadBlobResponse> downloadBlobResponsePair =
                    (await m_blobManager.DownloadBlobAsync(downloadBlobRequest, cancellationToken)).AsDisposable();

                DeleteBlobRequest deleteBlobRequest = new(blobReference);

                await m_blobManager.DeleteBlobAsync(deleteBlobRequest, cancellationToken);

                (Stream downloadBlobStream, DownloadBlobResponse downloadBlobResponse) = downloadBlobResponsePair;

                BlobMetadata blobMetadata = downloadBlobResponse.BlobMetadata;

                string blobMimeType = blobMetadata.MimeType;
                if (!AcceptsBlobsOfMimeType(blobMimeType))
                    throw ImportExceptionFactory.CreateUnsupportedImportSourceBlobMimeTypeException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>(importSource);

                importIntermediateObjectPackage = await DeserializeAsync(downloadBlobStream, cancellationToken);
            }

            if (importIntermediateObjectPackage is null)
                throw ImportExceptionFactory.CreateImportSourceDeserializedAsNullException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>(importSource);

            importIntermediateObjectPackageWrapper.ImportIntermediateObjectPackage = importIntermediateObjectPackage;
        }

        return importIntermediateObjectPackageBatch;
    }

    protected abstract Task<TImportIntermediateObjectPackageCurrent?> DeserializeAsync(Stream stream, CancellationToken cancellationToken);
}

public abstract class JsonImportSerializerBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage> :
    ImportSerializerBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>
    where TImportRequest : ImportRequest
    where TImportResponse : ImportResponse
    where TImportIntermediateObjectPackageCurrent : class
    where TImportObjectPackage : class
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    protected JsonImportSerializerBase(IBlobManager blobManager)
        : base(blobManager)
    {
    }

    /// <inheritdoc />
    public override bool AcceptsBlobsOfMimeType(string blobMimeType)
    {
        return blobMimeType switch
        {
            MediaTypeNames.Application.Json => true,
            _ => false,
        };
    }

    /// <inheritdoc />
    protected override async Task<TImportIntermediateObjectPackageCurrent?> DeserializeAsync(Stream stream, CancellationToken cancellationToken)
    {
        TImportIntermediateObjectPackageCurrent? importIntermediateObjectPackage =
            await JsonSerializer.DeserializeAsync<TImportIntermediateObjectPackageCurrent>(stream, options: s_jsonSerializerOptions, cancellationToken: cancellationToken);

        return importIntermediateObjectPackage;
    }
}

public abstract class XmlImportSerializerBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage> :
    ImportSerializerBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>
    where TImportRequest : ImportRequest
    where TImportResponse : ImportResponse
    where TImportIntermediateObjectPackageCurrent : class
    where TImportObjectPackage : class
{
    protected XmlImportSerializerBase(IBlobManager blobManager)
        : base(blobManager)
    {
    }

    /// <inheritdoc />
    public override bool AcceptsBlobsOfMimeType(string blobMimeType)
    {
        return blobMimeType switch
        {
            MediaTypeNames.Application.Xml or MediaTypeNames.Text.Xml => true,
            _ => false,
        };
    }

    /// <inheritdoc />
    protected override Task<TImportIntermediateObjectPackageCurrent?> DeserializeAsync(Stream stream, CancellationToken cancellationToken)
    {
        XmlSerializer xmlSerializer = new(typeof(TImportIntermediateObjectPackageCurrent));

        TImportIntermediateObjectPackageCurrent? importIntermediateObjectPackage = (TImportIntermediateObjectPackageCurrent?)xmlSerializer.Deserialize(stream);

        Task<TImportIntermediateObjectPackageCurrent?> completedTask = Task.FromResult(importIntermediateObjectPackage);

        return completedTask;
    }
}
