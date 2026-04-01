using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;
using JapaneseLanguageTools.Core.Export.Services.Base;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class ApplicationDictionaryJsonExportMapper :
    AutoMapperExportMapperBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryJsonExportMapper(IMapper mapper)
        : base(mapper)
    {
    }
}

public class ApplicationDictionaryXmlExportMapper :
    AutoMapperExportMapperBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryXmlExportMapper(IMapper mapper)
        : base(mapper)
    {
    }
}
