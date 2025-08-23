using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class WordExerciseRerunAutoMapperProfile : Profile
{
    public WordExerciseRerunAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<WordExerciseRerun, WordExerciseRerunModel>().ReverseMap();
    }
}
