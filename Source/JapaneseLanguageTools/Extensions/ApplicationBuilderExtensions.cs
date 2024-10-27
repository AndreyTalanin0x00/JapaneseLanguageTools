using Microsoft.AspNetCore.Builder;

namespace JapaneseLanguageTools.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseConfiguredSwaggerUI(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/JapaneseLanguageTools/swagger.json", $"JapaneseLanguageTools");
        });

        return applicationBuilder;
    }
}
