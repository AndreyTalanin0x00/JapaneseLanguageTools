using AutoMapper;
using AutoMapper.Internal;

namespace JapaneseLanguageTools.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static IMapperConfigurationExpression AddApplicationAutoMapperProfiles(this IMapperConfigurationExpression mapperConfigurationExpression)
    {
        IGlobalConfigurationExpression globalConfigurationExpression = mapperConfigurationExpression.Internal();
        globalConfigurationExpression.ForAllMaps((_, map) =>
        {
            // GHSA-rvv3-g6hj-g44x : AutoMapper Vulnerable to Denial of Service (DoS) via Uncontrolled Recursion
            map.MaxDepth(64);
        });

        return mapperConfigurationExpression;
    }
}
