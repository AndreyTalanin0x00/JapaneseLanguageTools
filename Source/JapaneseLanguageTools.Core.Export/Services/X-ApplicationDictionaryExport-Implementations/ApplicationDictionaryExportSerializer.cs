using System;
using System.IO;
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

public class ApplicationDictionaryJsonExportSerializer :
    IExportSerializer<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public void Serialize<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject)
        where TExportIntermediateObject : class
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task SerializeAsync<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject, CancellationToken cancellationToken = default)
        where TExportIntermediateObject : class
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>> SerializeAsync(ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class ApplicationDictionaryXmlExportSerializer :
    IExportSerializer<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public void Serialize<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject)
        where TExportIntermediateObject : class
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task SerializeAsync<TExportIntermediateObject>(Stream stream, TExportIntermediateObject exportIntermediateObject, CancellationToken cancellationToken = default)
        where TExportIntermediateObject : class
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>> SerializeAsync(ExportIntermediateObjectPackageBatch<ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel> exportIntermediateObjectPackageBatch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
