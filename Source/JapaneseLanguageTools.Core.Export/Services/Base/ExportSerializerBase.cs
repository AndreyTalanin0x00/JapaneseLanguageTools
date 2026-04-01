using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Exceptions.Factories;
using AndreyTalanin0x00.Integrations.Export.Requests;
using AndreyTalanin0x00.Integrations.Export.Responses;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Core.Blobs.Constants;

using Microsoft.IO;

namespace JapaneseLanguageTools.Core.Export.Services.Base;

public abstract class ExportSerializerBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    IExportSerializer<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    private readonly IBlobManager m_blobManager;
    private readonly RecyclableMemoryStreamManager m_recyclableMemoryStreamManager;

    protected abstract Encoding Encoding { get; }

    protected ExportSerializerBase(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        m_blobManager = blobManager;
        m_recyclableMemoryStreamManager = recyclableMemoryStreamManager;
    }

    /// <inheritdoc />
    public abstract void Serialize<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject)
        where TExportIntermediateObject : class;

    /// <inheritdoc />
    public abstract Task SerializeAsync<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject, CancellationToken cancellationToken = default)
        where TExportIntermediateObject : class;

    /// <inheritdoc />
    public async Task<ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage>> SerializeAsync(ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        int size = exportIntermediateObjectPackageBatch.Size;

        ExportTarget[] exportTargets = exportIntermediateObjectPackageBatch.ExportTargets;

        ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent>[] exportIntermediateObjectPackageWrappers =
            exportIntermediateObjectPackageBatch.ExportIntermediateObjectPackageWrappers;
        ExportObjectPackageWrapper<TExportObjectPackage>[] exportObjectPackageWrappers =
            exportIntermediateObjectPackageBatch.ExportObjectPackageWrappers;

        if (size != exportObjectPackageWrappers.Length)
            throw ExportExceptionFactory.CreateExportObjectPackageWrapperCountMismatchException<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>();
        if (size != exportIntermediateObjectPackageWrappers.Length)
            throw ExportExceptionFactory.CreateExportIntermediateObjectPackageWrapperCountMismatchException<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>();

        for (int index = 0; index < size; index++)
        {
            ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent> exportIntermediateObjectPackageWrapper =
                exportIntermediateObjectPackageWrappers[index];
            ExportObjectPackageWrapper<TExportObjectPackage> exportObjectPackageWrapper =
                exportObjectPackageWrappers[index];

            TExportIntermediateObjectPackageCurrent exportIntermediateObjectPackage = exportIntermediateObjectPackageWrapper.ExportIntermediateObjectPackage;
            TExportObjectPackage exportObjectPackage = exportObjectPackageWrapper.ExportObjectPackage;

            ExportTarget exportTarget;
            using (MemoryStream uploadBlobStream = m_recyclableMemoryStreamManager.GetStream())
            {
                string blobFileName = string.Empty;
                string blobMimeType = string.Empty;

                void SetFileName(string fileName) => blobFileName = fileName;
                void SetMimeType(string mimeType) => blobMimeType = mimeType;

                SerializeObjectPackagePairParameters serializeObjectPackagePairParameters = new()
                {
                    Stream = uploadBlobStream,
                    ExportIntermediateObjectPackageCurrent = exportIntermediateObjectPackage,
                    ExportObjectPackage = exportObjectPackage,
                    SetFileNameCallback = SetFileName,
                    SetMimeTypeCallback = SetMimeType,
                };

                await SerializeObjectPackagePairAsync(serializeObjectPackagePairParameters, cancellationToken);

                uploadBlobStream.Seek(0L, SeekOrigin.Begin);

                string blobBucket = BlobBucketConstants.ExportIntermediateBlobs;

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

                exportTarget = new()
                {
                    BlobReference = blobReference,
                    BlobMetadata = blobMetadata,
                };
            }

            exportTargets[index] = exportTarget;
        }

        return exportIntermediateObjectPackageBatch;
    }

    protected abstract Task SerializeObjectPackagePairAsync(SerializeObjectPackagePairParameters parameters, CancellationToken cancellationToken);

    protected class SerializeObjectPackagePairParameters
    {
        public required Stream Stream { get; init; }

        public required TExportIntermediateObjectPackageCurrent ExportIntermediateObjectPackageCurrent { get; init; }

        public required TExportObjectPackage ExportObjectPackage { get; init; }

        public required Action<string> SetFileNameCallback { get; init; }

        public required Action<string> SetMimeTypeCallback { get; init; }
    }
}

public abstract class JsonExportSerializerBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    ExportSerializerBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    private static readonly Encoding s_encoding = Encoding.UTF8;
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    protected JsonExportSerializerBase(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        : base(blobManager, recyclableMemoryStreamManager)
    {
    }

    /// <inheritdoc />
    protected override Encoding Encoding => s_encoding;

    /// <inheritdoc />
    public override void Serialize<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject)
    {
        // This method uses the UTF-8 encoding by convention.
        JsonSerializer.Serialize(stream, exportIntermediateObject, options: s_jsonSerializerOptions);
    }

    /// <inheritdoc />
    public override async Task SerializeAsync<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject, CancellationToken cancellationToken = default)
    {
        // This method uses the UTF-8 encoding by convention.
        await JsonSerializer.SerializeAsync(stream, exportIntermediateObject, options: s_jsonSerializerOptions, cancellationToken: cancellationToken);
    }
}

public abstract class XmlExportSerializerBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    ExportSerializerBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    private static readonly Encoding s_encoding = Encoding.Unicode;
    private static readonly XmlWriterSettings s_xmlWriterSettings = new()
    {
        Indent = true,
        OmitXmlDeclaration = true,
        Encoding = s_encoding,
        CloseOutput = false,
    };

    protected XmlExportSerializerBase(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        : base(blobManager, recyclableMemoryStreamManager)
    {
    }

    /// <inheritdoc />
    protected override Encoding Encoding => s_encoding;

    /// <inheritdoc />
    public override void Serialize<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject)
    {
        XmlSerializer xmlSerializer = new(typeof(TExportIntermediateObject));

#pragma warning disable IDE0063 // Use simple 'using' statement
        // Steams have to be closed immediately to flush the buffer. Simple using statements can be missed later.
        using (XmlWriter xmlWriter = XmlWriter.Create(stream, s_xmlWriterSettings))
            xmlSerializer.Serialize(xmlWriter, exportIntermediateObject);
#pragma warning restore IDE0063 // Use simple 'using' statement

        return;
    }

    /// <inheritdoc />
    public override Task SerializeAsync<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject, CancellationToken cancellationToken = default)
    {
        Serialize(stream, exportIntermediateObject);

        return Task.CompletedTask;
    }
}
