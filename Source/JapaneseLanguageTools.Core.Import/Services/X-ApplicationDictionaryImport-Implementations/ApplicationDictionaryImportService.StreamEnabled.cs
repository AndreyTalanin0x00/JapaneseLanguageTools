using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Import.Enumerations;
using AndreyTalanin0x00.Integrations.Import.Exceptions;
using AndreyTalanin0x00.Integrations.Import.Responses;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;
using JapaneseLanguageTools.Core.Import.Services.Abstractions;
using JapaneseLanguageTools.Core.Services.Abstractions;

using Microsoft.Extensions.Logging;

namespace JapaneseLanguageTools.Core.Export.Services;

public class ApplicationDictionaryImportStreamEnabledService : IApplicationDictionaryImportStreamEnabledService
{
    private readonly IMapper m_mapper;
    private readonly IImportPipeline<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse> m_importPipeline;
    private readonly IImportBlobUploader m_importBlobUploader;
    private readonly ILogger<ApplicationDictionaryImportStreamEnabledService> m_logger;

    public ApplicationDictionaryImportStreamEnabledService(IMapper mapper, IImportPipeline<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse> importPipeline, IImportBlobUploader importBlobUploader, ILogger<ApplicationDictionaryImportStreamEnabledService> logger)
    {
        m_mapper = mapper;
        m_importPipeline = importPipeline;
        m_importBlobUploader = importBlobUploader;
        m_logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public async Task<ImportApplicationDictionaryResponseModel> ImportApplicationDictionaryAsync(ImportApplicationDictionaryRequestModel importApplicationDictionaryRequestModel, CancellationToken cancellationToken = default)
    {
        ApplicationDictionaryImportRequest applicationDictionaryImportRequest =
            m_mapper.Map<ApplicationDictionaryImportRequest>(importApplicationDictionaryRequestModel);

        ApplicationDictionaryImportResponse applicationDictionaryImportResponse;
        try
        {
            applicationDictionaryImportResponse =
                await m_importPipeline.ImportAsync(applicationDictionaryImportRequest, cancellationToken);
        }
        catch (Exception exception)
        {
            m_logger.LogError(exception, "An unexpected error has occurred during import.");

            ImportStatus importStatus = ImportStatus.Failed;
            ImportResponseMessage[] importResponseMessages = exception is ImportException importException
                ? [.. importException.ImportResponseMessages]
                : new ImportResponseMessage[]
                {
                    new()
                    {
                        Type = ImportResponseMessageType.Error,
                        Text = "An unexpected error has occurred during import (see server-side logs).",
                    },
                };

            applicationDictionaryImportResponse = new ApplicationDictionaryImportResponse()
            {
                Status = importStatus,
                Messages = importResponseMessages,
                Request = applicationDictionaryImportRequest,
            };
        }

        ImportApplicationDictionaryResponseModel importApplicationDictionaryResponseModel =
            m_mapper.Map<ImportApplicationDictionaryResponseModel>(applicationDictionaryImportResponse);

        return importApplicationDictionaryResponseModel;
    }

    /// <inheritdoc />
    public async Task<UploadBlobResponseModel> UploadImportBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequestModel> uploadBlobRequestModelPair, CancellationToken cancellationToken = default)
    {
        return await m_importBlobUploader.UploadImportBlobAsync(uploadBlobRequestModelPair, cancellationToken);
    }
}
