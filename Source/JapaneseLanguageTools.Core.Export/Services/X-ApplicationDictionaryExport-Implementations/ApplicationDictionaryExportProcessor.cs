using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class ApplicationDictionaryExportProcessor :
    IExportProcessor<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, Object, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public Task<ExportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel>> ProcessAsync(ApplicationDictionaryExportRequest applicationDictionaryExportRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
