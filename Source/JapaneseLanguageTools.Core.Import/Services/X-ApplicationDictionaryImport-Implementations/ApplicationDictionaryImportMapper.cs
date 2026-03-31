using System;

using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class ApplicationDictionaryJsonImportMapper :
    IImportMapper<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ImportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel> Map(ImportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel> importIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}

public class ApplicationDictionaryXmlImportMapper :
    IImportMapper<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ImportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel> Map(ImportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel> importIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}
