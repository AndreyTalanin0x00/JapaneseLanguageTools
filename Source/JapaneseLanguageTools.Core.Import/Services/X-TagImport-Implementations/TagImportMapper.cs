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

public class TagJsonImportMapper :
    IImportMapper<TagImportRequest, TagImportResponse, TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ImportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel> Map(ImportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel> importIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}

public class TagXmlImportMapper :
    IImportMapper<TagImportRequest, TagImportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public ImportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> Map(ImportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> importIntermediateObjectPackageBatch)
    {
        throw new NotImplementedException();
    }
}
