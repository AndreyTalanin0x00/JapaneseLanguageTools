using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

namespace JapaneseLanguageTools.Core.Import.AutoMapper.Profiles;

public class TagImportAutoMapperProfile : Profile
{
    public TagImportAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<TagImportRequest, ImportTagsRequestModel>().ReverseMap();
        CreateMap<TagImportResponse, ImportTagsResponseModel>().ReverseMap();
    }
}
