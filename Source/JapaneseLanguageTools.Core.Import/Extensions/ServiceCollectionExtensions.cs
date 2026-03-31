using System;

using AndreyTalanin0x00.Integrations.Import.Extensions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Core.Export.Services;
using JapaneseLanguageTools.Core.Import.Constants;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;
using JapaneseLanguageTools.Core.Import.Services;
using JapaneseLanguageTools.Core.Import.Services.Abstractions;
using JapaneseLanguageTools.Core.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

namespace JapaneseLanguageTools.Core.Import.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImportServices(this IServiceCollection services)
    {
        services.AddTransient<IImportBlobUploader, ImportBlobUploader>();

        services.AddApplicationDictionaryImportPipeline();
        services.AddApplicationDictionaryImportServices();

        services.AddTagImportPipeline();
        services.AddTagImportServices();

        return services;
    }

    public static IServiceCollection AddApplicationDictionaryImportPipeline(this IServiceCollection services)
    {
        services.AddImportPipeline<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, Object, ApplicationDictionaryObjectPackageIntegrationModel>(importPipelineOptionsBuilder =>
        {
            importPipelineOptionsBuilder
                .UseImportReader<ApplicationDictionaryImportReader>()
                .AddImportPipelineChannel<ApplicationDictionaryObjectPackageJsonModel>(importPipelineChannelOptionsBuilder =>
                {
                    importPipelineChannelOptionsBuilder
                        .UseKey(ApplicationDictionaryImportPipelineChannelKeys.ApplicationDictionaryImportPipelineChannelKeyJson)
                        .UseImportSerializer<ApplicationDictionaryJsonImportSerializer>()
                        .UseImportMapper<ApplicationDictionaryJsonImportMapper>();
                })
                .AddImportPipelineChannel<ApplicationDictionaryObjectPackageXmlModel>(importPipelineChannelOptionsBuilder =>
                {
                    importPipelineChannelOptionsBuilder
                        .UseKey(ApplicationDictionaryImportPipelineChannelKeys.ApplicationDictionaryImportPipelineChannelKeyXml)
                        .UseImportSerializer<ApplicationDictionaryXmlImportSerializer>()
                        .UseImportMapper<ApplicationDictionaryXmlImportMapper>();
                })
                .UseImportPipelineChannelKeyResolver<ApplicationDictionaryImportPipelineChannelKeyResolver>()
                .UseImportValidator<ApplicationDictionaryImportValidator>()
                .UseImportProcessor<ApplicationDictionaryImportProcessor>();
        });

        return services;
    }

    public static IServiceCollection AddApplicationDictionaryImportServices(this IServiceCollection services)
    {
        services.AddTransient<IApplicationDictionaryImportService, ApplicationDictionaryImportStreamEnabledService>();
        services.AddTransient<IApplicationDictionaryImportStreamEnabledService, ApplicationDictionaryImportStreamEnabledService>();

        return services;
    }

    public static IServiceCollection AddTagImportPipeline(this IServiceCollection services)
    {
        services.AddImportPipeline<TagImportRequest, TagImportResponse, Object, TagObjectPackageIntegrationModel>(importPipelineOptionsBuilder =>
        {
            importPipelineOptionsBuilder
                .UseImportReader<TagImportReader>()
                .AddImportPipelineChannel<TagObjectPackageJsonModel>(importPipelineChannelOptionsBuilder =>
                {
                    importPipelineChannelOptionsBuilder
                        .UseKey(TagImportPipelineChannelKeys.TagImportPipelineChannelKeyJson)
                        .UseImportSerializer<TagJsonImportSerializer>()
                        .UseImportMapper<TagJsonImportMapper>();
                })
                .AddImportPipelineChannel<TagObjectPackageXmlModel>(importPipelineChannelOptionsBuilder =>
                {
                    importPipelineChannelOptionsBuilder
                        .UseKey(TagImportPipelineChannelKeys.TagImportPipelineChannelKeyXml)
                        .UseImportSerializer<TagXmlImportSerializer>()
                        .UseImportMapper<TagXmlImportMapper>();
                })
                .UseImportPipelineChannelKeyResolver<TagImportPipelineChannelKeyResolver>()
                .UseImportValidator<TagImportValidator>()
                .UseImportProcessor<TagImportProcessor>();
        });

        return services;
    }

    public static IServiceCollection AddTagImportServices(this IServiceCollection services)
    {
        services.AddTransient<ITagImportService, TagImportStreamEnabledService>();
        services.AddTransient<ITagImportStreamEnabledService, TagImportStreamEnabledService>();

        return services;
    }
}
