using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class TagImportProcessor :
    IImportProcessor<TagImportRequest, TagImportResponse, Object, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public Task<TagImportResponse> ProcessAsync(TagImportRequest tagImportRequest, ImportObjectPackageBatch<Object, TagObjectPackageIntegrationModel>[] importObjectPackageBatches, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
