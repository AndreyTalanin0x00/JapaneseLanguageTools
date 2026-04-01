using AutoMapper;

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

public class ApplicationDictionaryJsonImportMapper :
    AutoMapperImportMapperBase<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryJsonImportMapper(IMapper mapper)
        : base(mapper)
    {
    }
}

public class ApplicationDictionaryXmlImportMapper :
    AutoMapperImportMapperBase<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryXmlImportMapper(IMapper mapper)
        : base(mapper)
    {
    }
}
