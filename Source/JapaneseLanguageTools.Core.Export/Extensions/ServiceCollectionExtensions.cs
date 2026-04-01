using System;

using AndreyTalanin0x00.Integrations.Export.Extensions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Core.Export.Constants;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;
using JapaneseLanguageTools.Core.Export.Services;
using JapaneseLanguageTools.Core.Export.Services.Abstractions;
using JapaneseLanguageTools.Core.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

namespace JapaneseLanguageTools.Core.Export.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExportServices(this IServiceCollection services)
    {
        services.AddTransient<IExportBlobDownloader, ExportBlobDownloader>();

        services.AddApplicationDictionaryExportPipeline();
        services.AddApplicationDictionaryExportServices();

        services.AddTagExportPipeline();
        services.AddTagExportServices();

        return services;
    }

    public static IServiceCollection AddApplicationDictionaryExportPipeline(this IServiceCollection services)
    {
        services.AddExportPipeline<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, Object, ApplicationDictionaryObjectPackageIntegrationModel>(exportPipelineOptionsBuilder =>
        {
            exportPipelineOptionsBuilder
                .UseExportWriter<ApplicationDictionaryExportWriter>()
                .AddExportPipelineChannel<ApplicationDictionaryObjectPackageJsonModel>(exportPipelineChannelOptionsBuilder =>
                {
                    exportPipelineChannelOptionsBuilder
                        .UseKey(ApplicationDictionaryExportPipelineChannelKeys.ApplicationDictionaryExportPipelineChannelKeyJson)
                        .UseExportFormatter<ApplicationDictionaryJsonExportFormatter>()
                        .UseExportSerializer<ApplicationDictionaryJsonExportSerializer>()
                        .UseExportMapper<ApplicationDictionaryJsonExportMapper>();
                })
                .AddExportPipelineChannel<ApplicationDictionaryObjectPackageXmlModel>(exportPipelineChannelOptionsBuilder =>
                {
                    exportPipelineChannelOptionsBuilder
                        .UseKey(ApplicationDictionaryExportPipelineChannelKeys.ApplicationDictionaryExportPipelineChannelKeyXml)
                        .UseExportFormatter<ApplicationDictionaryXmlExportFormatter>()
                        .UseExportSerializer<ApplicationDictionaryXmlExportSerializer>()
                        .UseExportMapper<ApplicationDictionaryXmlExportMapper>();
                })
                .UseExportPipelineChannelKeyResolver<ApplicationDictionaryExportPipelineChannelKeyResolver>()
                .UseExportProcessor<ApplicationDictionaryExportProcessor>();
        });

        return services;
    }

    public static IServiceCollection AddApplicationDictionaryExportServices(this IServiceCollection services)
    {
        services.AddTransient<IApplicationDictionaryExportService, ApplicationDictionaryExportStreamEnabledService>();
        services.AddTransient<IApplicationDictionaryExportStreamEnabledService, ApplicationDictionaryExportStreamEnabledService>();

        return services;
    }

    public static IServiceCollection AddTagExportPipeline(this IServiceCollection services)
    {
        services.AddExportPipeline<TagExportRequest, TagExportResponse, Object, TagObjectPackageIntegrationModel>(exportPipelineOptionsBuilder =>
        {
            exportPipelineOptionsBuilder
                .UseExportWriter<TagExportWriter>()
                .AddExportPipelineChannel<TagObjectPackageJsonModel>(exportPipelineChannelOptionsBuilder =>
                {
                    exportPipelineChannelOptionsBuilder
                        .UseKey(TagExportPipelineChannelKeys.TagExportPipelineChannelKeyJson)
                        .UseExportFormatter<TagJsonExportFormatter>()
                        .UseExportSerializer<TagJsonExportSerializer>()
                        .UseExportMapper<TagJsonExportMapper>();
                })
                .AddExportPipelineChannel<TagObjectPackageXmlModel>(exportPipelineChannelOptionsBuilder =>
                {
                    exportPipelineChannelOptionsBuilder
                        .UseKey(TagExportPipelineChannelKeys.TagExportPipelineChannelKeyXml)
                        .UseExportFormatter<TagXmlExportFormatter>()
                        .UseExportSerializer<TagXmlExportSerializer>()
                        .UseExportMapper<TagXmlExportMapper>();
                })
                .UseExportPipelineChannelKeyResolver<TagExportPipelineChannelKeyResolver>()
                .UseExportProcessor<TagExportProcessor>();
        });

        return services;
    }

    public static IServiceCollection AddTagExportServices(this IServiceCollection services)
    {
        services.AddTransient<ITagExportService, TagExportStreamEnabledService>();
        services.AddTransient<ITagExportStreamEnabledService, TagExportStreamEnabledService>();

        return services;
    }
}
