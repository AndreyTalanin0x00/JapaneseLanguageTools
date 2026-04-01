using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;

using JapaneseLanguageTools.Contracts.Models.Blobs;
using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Services.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JapaneseLanguageTools.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApplicationDictionaryExportController : ControllerBase
{
    private readonly IApplicationDictionaryExportStreamEnabledService m_applicationDictionaryExportService;

    public ApplicationDictionaryExportController(IApplicationDictionaryExportStreamEnabledService applicationDictionaryExportService)
    {
        m_applicationDictionaryExportService = applicationDictionaryExportService;
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ExportApplicationDictionaryResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExportApplicationDictionaryResponseModel>> ExportApplicationDictionaryAsync([Required][FromBody] ExportApplicationDictionaryRequestModel exportApplicationDictionaryRequestModel, CancellationToken cancellationToken)
    {
        ExportApplicationDictionaryResponseModel exportApplicationDictionaryResponseModel = await m_applicationDictionaryExportService.ExportApplicationDictionaryAsync(exportApplicationDictionaryRequestModel, cancellationToken);

        return Ok(exportApplicationDictionaryResponseModel);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GetBlobMetadataResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetBlobMetadataResponseModel>> GetExportBlobMetadataAsync([Required][FromQuery] Guid id, [Required][FromQuery] Uri uri, CancellationToken cancellationToken)
    {
        BlobReferenceModel blobReferenceModel = new(id, uri);

        GetBlobMetadataRequestModel getBlobMetadataRequestModel = new()
        {
            BlobReference = blobReferenceModel,
        };

        GetBlobMetadataResponseModel? getBlobMetadataResponseModel = await m_applicationDictionaryExportService.GetExportBlobMetadataAsync(getBlobMetadataRequestModel, cancellationToken);

        ActionResult<GetBlobMetadataResponseModel> actionResult = getBlobMetadataResponseModel is not null
            ? (ActionResult<GetBlobMetadataResponseModel>)Ok(getBlobMetadataResponseModel)
            : (ActionResult<GetBlobMetadataResponseModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GetBlobExpirationTimeResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetBlobExpirationTimeResponseModel>> GetExportBlobExpirationTimeAsync([Required][FromQuery] Guid id, [Required][FromQuery] Uri uri, CancellationToken cancellationToken)
    {
        BlobReferenceModel blobReferenceModel = new(id, uri);

        GetBlobExpirationTimeRequestModel getBlobExpirationTimeRequestModel = new()
        {
            BlobReference = blobReferenceModel,
        };

        GetBlobExpirationTimeResponseModel? getBlobExpirationTimeResponseModel = await m_applicationDictionaryExportService.GetExportBlobExpirationTimeAsync(getBlobExpirationTimeRequestModel, cancellationToken);

        ActionResult<GetBlobExpirationTimeResponseModel> actionResult = getBlobExpirationTimeResponseModel is not null
            ? (ActionResult<GetBlobExpirationTimeResponseModel>)Ok(getBlobExpirationTimeResponseModel)
            : (ActionResult<GetBlobExpirationTimeResponseModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Octet)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DownloadExportBlobAsync([Required][FromQuery] Guid id, [Required][FromQuery] Uri uri, CancellationToken cancellationToken)
    {
        BlobReferenceModel blobReferenceModel = new(id, uri);

        DownloadBlobRequestModel downloadBlobRequestModel = new()
        {
            BlobReference = blobReferenceModel,
        };

        BlobStreamMetadataPair<Stream, DownloadBlobResponseModel>? downloadBlobResponseModelPair = await m_applicationDictionaryExportService.DownloadExportBlobAsync(downloadBlobRequestModel, cancellationToken);

        if (downloadBlobResponseModelPair is null)
            return NotFound();

        (Stream stream, DownloadBlobResponseModel downloadBlobResponseModel) = downloadBlobResponseModelPair;
        BlobMetadataModel blobMetadataModel = downloadBlobResponseModel.BlobMetadata;

        // Do not use the blobMetadataModel.MimeType property to prevent insertion of the UTF-8 byte order mark.
        string mimeType = MediaTypeNames.Application.Octet;
        string fileName = blobMetadataModel.FileName;

        return File(stream, mimeType, fileName);
    }
}
