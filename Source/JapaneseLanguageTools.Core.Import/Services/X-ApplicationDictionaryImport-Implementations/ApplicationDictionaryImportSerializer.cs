using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;
using JapaneseLanguageTools.Core.Import.Services.Base;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class ApplicationDictionaryJsonImportSerializer :
    JsonImportSerializerBase<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryJsonImportSerializer(IBlobManager blobManager)
        : base(blobManager)
    {
    }
}

public class ApplicationDictionaryXmlImportSerializer :
    XmlImportSerializerBase<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryXmlImportSerializer(IBlobManager blobManager)
        : base(blobManager)
    {
    }
}
