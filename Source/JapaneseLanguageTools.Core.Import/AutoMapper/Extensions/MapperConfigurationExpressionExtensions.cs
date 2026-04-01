using AutoMapper;

using JapaneseLanguageTools.Core.Import.AutoMapper.Profiles;

namespace JapaneseLanguageTools.Core.Import.AutoMapper.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static IMapperConfigurationExpression AddImportAutoMapperProfiles(this IMapperConfigurationExpression mapperConfigurationExpression)
    {
        mapperConfigurationExpression.AddProfile<ImportAutoMapperProfile>();

        mapperConfigurationExpression.AddProfile<ApplicationDictionaryImportAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<TagImportAutoMapperProfile>();

        return mapperConfigurationExpression;
    }
}
