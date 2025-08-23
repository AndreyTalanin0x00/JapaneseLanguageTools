using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class CharacterExerciseAutoMapperProfile : Profile
{
    public CharacterExerciseAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<CharacterExercise, CharacterExerciseModel>();

        CreateMap<CharacterExerciseModel, CharacterExercise>()
            .AfterMap(ModelToEntity_SetCharacterExerciseRerunNavigationProperties);

        static void ModelToEntity_SetCharacterExerciseRerunNavigationProperties(CharacterExerciseModel characterExerciseModel, CharacterExercise characterExercise)
        {
            foreach (CharacterExerciseRerun characterExerciseRerun in characterExercise.CharacterExerciseReruns)
            {
                characterExerciseRerun.CharacterExerciseId = characterExercise.Id;
                characterExerciseRerun.CharacterExercise = characterExercise;
            }
        }
    }
}
