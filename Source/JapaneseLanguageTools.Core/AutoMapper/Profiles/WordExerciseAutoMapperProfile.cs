using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class WordExerciseAutoMapperProfile : Profile
{
    public WordExerciseAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<WordExercise, WordExerciseModel>();

        CreateMap<WordExerciseModel, WordExercise>()
            .AfterMap(ModelToEntity_SetWordExerciseRerunNavigationProperties);

        static void ModelToEntity_SetWordExerciseRerunNavigationProperties(WordExerciseModel wordExerciseModel, WordExercise wordExercise)
        {
            foreach (WordExerciseRerun wordExerciseRerun in wordExercise.WordExerciseReruns)
            {
                wordExerciseRerun.WordExerciseId = wordExercise.Id;
                wordExerciseRerun.WordExercise = wordExercise;
            }
        }
    }
}
