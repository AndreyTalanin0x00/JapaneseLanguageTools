using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;

namespace JapaneseLanguageTools.Core.Export.AutoMapper.Profiles;

public class TagExportAutoMapperProfile : Profile
{
    public TagExportAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<TagExportRequest, ExportTagsRequestModel>().ReverseMap();
        CreateMap<TagExportResponse, ExportTagsResponseModel>().ReverseMap();
    }
}
