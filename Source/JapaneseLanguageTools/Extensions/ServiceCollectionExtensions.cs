using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace JapaneseLanguageTools.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            string version = "v0.1.0-dev0";
            OpenApiContact contact = new()
            {
                Name = "Andrey Talanin",
                Email = "andrey.talanin@outlook.com",
                Url = new Uri("https://github.com/AndreyTalanin0x00"),
            };
            OpenApiLicense license = new()
            {
                Name = "The MIT License",
                Url = new Uri("https://github.com/AndreyTalanin0x00/JapaneseLanguageTools/blob/main/LICENSE.md"),
            };

            options.SwaggerDoc("JapaneseLanguageTools", new OpenApiInfo()
            {
                Title = $"Japanese Language Tools {version}",
                Description = "Initial pre-release (unstable) API version.",
                Version = version,
                Contact = contact,
                License = license,
            });

            options.SupportNonNullableReferenceTypes();
        });

        return services;
    }
}
