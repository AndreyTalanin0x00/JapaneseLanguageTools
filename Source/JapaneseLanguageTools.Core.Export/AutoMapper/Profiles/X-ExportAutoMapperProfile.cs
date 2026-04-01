using AutoMapper;

namespace JapaneseLanguageTools.Core.Export.AutoMapper.Profiles;

public class ExportAutoMapperProfile : Profile
{
    public ExportAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;
    }
}
