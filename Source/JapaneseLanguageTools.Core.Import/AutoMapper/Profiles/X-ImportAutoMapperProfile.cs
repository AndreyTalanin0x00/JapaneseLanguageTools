using AndreyTalanin0x00.Integrations.Import.Responses;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Responses.Base;

namespace JapaneseLanguageTools.Core.Import.AutoMapper.Profiles;

public class ImportAutoMapperProfile : Profile
{
    public ImportAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<ImportResponseMessage, ImportResponseMessageModel>().ReverseMap();
    }
}
