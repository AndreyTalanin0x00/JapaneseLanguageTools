using AutoMapper;

using JapaneseLanguageTools.Core.Blobs.AutoMapper.Profiles;

namespace JapaneseLanguageTools.Core.Blobs.AutoMapper.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static IMapperConfigurationExpression AddBlobAutoMapperProfile(this IMapperConfigurationExpression mapperConfigurationExpression)
    {
        mapperConfigurationExpression.AddProfile<BlobAutoMapperProfile>();
        mapperConfigurationExpression.AddProfile<BlobManagerAutoMapperProfile>();

        return mapperConfigurationExpression;
    }
}
