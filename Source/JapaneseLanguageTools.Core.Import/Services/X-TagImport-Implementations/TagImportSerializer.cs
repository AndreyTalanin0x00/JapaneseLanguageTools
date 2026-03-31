using System;
using System.Threading;
using System.Threading.Tasks;

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

public class TagJsonImportSerializer :
    IImportSerializer<TagImportRequest, TagImportResponse, TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public bool AcceptsBlobsOfMimeType(string blobMimeType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ImportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>> DeserializeAsync(ImportIntermediateObjectPackageBatch<TagObjectPackageJsonModel, TagObjectPackageIntegrationModel> importIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class TagXmlImportSerializer :
    IImportSerializer<TagImportRequest, TagImportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public bool AcceptsBlobsOfMimeType(string blobMimeType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ImportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>> DeserializeAsync(ImportIntermediateObjectPackageBatch<TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> importIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
