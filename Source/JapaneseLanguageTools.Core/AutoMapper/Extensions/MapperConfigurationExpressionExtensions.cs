using AutoMapper;

using JapaneseLanguageTools.Core.AutoMapper.Profiles;

namespace JapaneseLanguageTools.Core.AutoMapper.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static IMapperConfigurationExpression AddCommonAutoMapperProfiles(this IMapperConfigurationExpression mapperConfigurationExpression)
    {
        mapperConfigurationExpression.AddProfile<CharacterAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<CharacterGroupAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<CharacterExerciseAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<CharacterExerciseRerunAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<WordAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<WordGroupAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<WordExerciseAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<WordExerciseRerunAutoMapperProfile>();

        mapperConfigurationExpression.AddProfile<ApplicationDictionaryAutoMapperProfile>();

        mapperConfigurationExpression.AddProfile<TagAutoMapperProfile>();

        return mapperConfigurationExpression;
    }
}
