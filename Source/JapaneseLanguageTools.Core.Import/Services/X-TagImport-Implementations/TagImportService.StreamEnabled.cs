using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Services.Abstractions;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class TagImportStreamEnabledService : ITagImportStreamEnabledService
{
    /// <inheritdoc />
    public Task<ImportTagsResponseModel> ImportTagsAsync(ImportTagsRequestModel importTagsRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<UploadBlobResponseModel> UploadImportBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequestModel> uploadBlobRequestModelPair, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
