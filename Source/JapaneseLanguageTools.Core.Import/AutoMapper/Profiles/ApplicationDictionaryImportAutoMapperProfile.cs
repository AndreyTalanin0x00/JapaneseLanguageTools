using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

namespace JapaneseLanguageTools.Core.Import.AutoMapper.Profiles;

public class ApplicationDictionaryImportAutoMapperProfile : Profile
{
    public ApplicationDictionaryImportAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<ApplicationDictionaryImportRequest, ImportApplicationDictionaryRequestModel>().ReverseMap();
        CreateMap<ApplicationDictionaryImportResponse, ImportApplicationDictionaryResponseModel>().ReverseMap();
    }
}
