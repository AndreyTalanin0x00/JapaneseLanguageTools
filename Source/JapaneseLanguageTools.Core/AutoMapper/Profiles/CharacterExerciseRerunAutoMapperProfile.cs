using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class CharacterExerciseRerunAutoMapperProfile : Profile
{
    public CharacterExerciseRerunAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<CharacterExerciseRerun, CharacterExerciseRerunModel>().ReverseMap();
    }
}
