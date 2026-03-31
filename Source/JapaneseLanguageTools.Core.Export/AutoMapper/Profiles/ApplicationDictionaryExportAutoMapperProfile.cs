using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;

namespace JapaneseLanguageTools.Core.Export.AutoMapper.Profiles;

public class ApplicationDictionaryExportAutoMapperProfile : Profile
{
    public ApplicationDictionaryExportAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<ApplicationDictionaryExportRequest, ExportApplicationDictionaryRequestModel>().ReverseMap();
        CreateMap<ApplicationDictionaryExportResponse, ExportApplicationDictionaryResponseModel>().ReverseMap();
    }
}
