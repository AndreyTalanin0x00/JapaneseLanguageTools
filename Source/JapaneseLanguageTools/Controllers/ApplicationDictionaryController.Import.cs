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
using JapaneseLanguageTools.Contracts.Models.Responses.Base;
using JapaneseLanguageTools.Core.Services.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace JapaneseLanguageTools.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApplicationDictionaryImportController : ControllerBase
{
    private readonly IApplicationDictionaryImportStreamEnabledService m_applicationDictionaryImportService;
    private readonly FileExtensionContentTypeProvider m_fileExtensionContentTypeProvider;

    public ApplicationDictionaryImportController(IApplicationDictionaryImportStreamEnabledService applicationDictionaryImportService, FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        m_applicationDictionaryImportService = applicationDictionaryImportService;
        m_fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<UploadBlobResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UploadBlobResponseModel>> UploadImportBlobAsync([Required] IFormFile blob, CancellationToken cancellationToken)
    {
        string blobFileName = blob.FileName;
        string blobMimeType = !m_fileExtensionContentTypeProvider.TryGetContentType(blobFileName, out string? mimeType)
            ? MediaTypeNames.Application.Octet
            : mimeType;

        BlobMetadataModel blobMetadataModel = new()
        {
            FileName = blobFileName,
            MimeType = blobMimeType,
        };

        UploadBlobRequestModel uploadBlobRequestModel = new()
        {
            BlobMetadata = blobMetadataModel,
        };

        using Stream blobStream = blob.OpenReadStream();

        BlobStreamMetadataPair<Stream, UploadBlobRequestModel> uploadBlobRequestModelPair = new(blobStream, uploadBlobRequestModel);

        UploadBlobResponseModel uploadBlobResponseModel = await m_applicationDictionaryImportService.UploadImportBlobAsync(uploadBlobRequestModelPair, cancellationToken);

        return Ok(uploadBlobResponseModel);
    }

    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ImportApplicationDictionaryResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ImportApplicationDictionaryResponseModel>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ImportApplicationDictionaryResponseModel>> ImportApplicationDictionaryAsync([Required][FromBody] ImportApplicationDictionaryRequestModel importApplicationDictionaryRequestModel, CancellationToken cancellationToken)
    {
        ImportApplicationDictionaryResponseModel importApplicationDictionaryResponseModel =
            await m_applicationDictionaryImportService.ImportApplicationDictionaryAsync(importApplicationDictionaryRequestModel, cancellationToken);

#pragma warning disable format
        if (importApplicationDictionaryResponseModel.Status == ImportStatus.Failed)
            return Conflict(importApplicationDictionaryResponseModel);
#pragma warning disable format

        return Ok(importApplicationDictionaryResponseModel);
    }
}
