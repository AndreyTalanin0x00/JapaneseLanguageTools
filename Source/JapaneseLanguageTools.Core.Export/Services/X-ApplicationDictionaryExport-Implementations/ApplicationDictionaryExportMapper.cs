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

public class ApplicationDictionaryJsonExportMapper :
    IExportMapper<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel> Map(ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}

public class ApplicationDictionaryXmlExportMapper :
    IExportMapper<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel> Map(ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}
