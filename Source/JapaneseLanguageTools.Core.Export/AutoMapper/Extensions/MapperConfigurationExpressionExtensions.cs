using AutoMapper;

using JapaneseLanguageTools.Core.Export.AutoMapper.Profiles;

namespace JapaneseLanguageTools.Core.Export.AutoMapper.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static IMapperConfigurationExpression AddExportAutoMapperProfiles(this IMapperConfigurationExpression mapperConfigurationExpression)
    {
        mapperConfigurationExpression.AddProfile<ExportAutoMapperProfile>();

        mapperConfigurationExpression.AddProfile<ApplicationDictionaryExportAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<TagExportAutoMapperProfile>();

        return mapperConfigurationExpression;
    }
}
