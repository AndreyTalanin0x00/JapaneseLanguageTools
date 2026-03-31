using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class TagImportPipelineChannelKeyResolver :
    IImportPipelineChannelKeyResolver<TagImportRequest, TagImportResponse>
{
    /// <inheritdoc />
    public bool SupportsAsyncMode => throw new NotImplementedException();

    /// <inheritdoc />
    public ImportPipelineChannelKey ResolveImportPipelineChannelKey(TagImportRequest tagImportRequest, ImportSource importSource, ImportSourceContext importSourceContext)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ImportPipelineChannelKey> ResolveImportPipelineChannelKeyAsync(TagImportRequest tagImportRequest, ImportSource importSource, ImportSourceContext importSourceContext, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
