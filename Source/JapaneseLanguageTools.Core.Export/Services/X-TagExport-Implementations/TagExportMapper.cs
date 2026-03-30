using System;

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

public class TagJsonExportMapper :
    IExportMapper<TagExportRequest, TagExportResponse, TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ExportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel> Map(ExportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}

public class TagXmlExportMapper :
    IExportMapper<TagExportRequest, TagExportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ExportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> Map(ExportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}
