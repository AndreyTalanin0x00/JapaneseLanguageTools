using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using JapaneseLanguageTools.Core.Import.Constants;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class ApplicationDictionaryImportPipelineChannelKeyResolver :
    IImportPipelineChannelKeyResolver<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse>
{
    /// <inheritdoc />
    public bool SupportsAsyncMode { get; } = false;

    /// <inheritdoc />
    public Task<ImportPipelineChannelKey> ResolveImportPipelineChannelKeyAsync(ApplicationDictionaryImportRequest applicationDictionaryImportRequest, ImportSource importSource, ImportSourceContext importSourceContext, CancellationToken cancellationToken = default)
    {
        ImportPipelineChannelKey importPipelineChannelKey = ResolveImportPipelineChannelKey(applicationDictionaryImportRequest, importSource, importSourceContext);

        Task<ImportPipelineChannelKey> importPipelineChannelKeyTask = Task.FromResult(importPipelineChannelKey);

        return importPipelineChannelKeyTask;
    }

    /// <inheritdoc />
    public ImportPipelineChannelKey ResolveImportPipelineChannelKey(ApplicationDictionaryImportRequest applicationDictionaryImportRequest, ImportSource importSource, ImportSourceContext importSourceContext)
    {
        BlobMetadata blobMetadata = importSource.BlobMetadata;

        ImportPipelineChannelKey importPipelineChannelKey = blobMetadata.MimeType switch
        {
            MediaTypeNames.Application.Json => ApplicationDictionaryImportPipelineChannelKeys.ApplicationDictionaryImportPipelineChannelKeyJson,
            MediaTypeNames.Application.Xml or MediaTypeNames.Text.Xml => ApplicationDictionaryImportPipelineChannelKeys.ApplicationDictionaryImportPipelineChannelKeyXml,
            _ => ApplicationDictionaryImportPipelineChannelKeys.ApplicationDictionaryImportPipelineChannelKeyUnsupported,
        };

        return importPipelineChannelKey;
    }
}
