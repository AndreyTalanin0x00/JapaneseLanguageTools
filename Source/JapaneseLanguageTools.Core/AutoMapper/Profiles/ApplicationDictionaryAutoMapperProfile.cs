using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class ApplicationDictionaryAutoMapperProfile : Profile
{
    public ApplicationDictionaryAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<ApplicationDictionaryObjectPackageIntegrationModel, ApplicationDictionaryObjectPackageJsonModel>().ReverseMap();
        CreateMap<ApplicationDictionaryObjectPackageIntegrationModel, ApplicationDictionaryObjectPackageXmlModel>().ReverseMap();

        CreateMap<ApplicationDictionaryIntegrationModel, ApplicationDictionaryJsonModel>().ReverseMap();
        CreateMap<ApplicationDictionaryIntegrationModel, ApplicationDictionaryXmlModel>().ReverseMap();
    }
}
