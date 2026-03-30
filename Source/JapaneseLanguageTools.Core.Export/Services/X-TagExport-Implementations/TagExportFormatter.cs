using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class TagJsonExportFormatter :
    IExportFormatter<TagExportRequest, TagExportResponse, TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public Task<ExportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>> FormatAsync(ExportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class TagXmlExportFormatter :
    IExportFormatter<TagExportRequest, TagExportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public Task<ExportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>> FormatAsync(ExportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
