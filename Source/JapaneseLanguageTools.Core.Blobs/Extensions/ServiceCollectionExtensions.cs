using System.Diagnostics.CodeAnalysis;

using AndreyTalanin0x00.Integrations.Blobs.Extensions;
using AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;
using AndreyTalanin0x00.Integrations.Blobs.Services;

using JapaneseLanguageTools.Core.Blobs.Constants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JapaneseLanguageTools.Core.Blobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBlobManager(blobManagerOptionsBuilder =>
        {
            blobManagerOptionsBuilder.AddInMemoryBlobStorageProvider(configuration, ConfigureInMemoryBlobStorageProvider);
        });

        return services;
    }

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static void ConfigureInMemoryBlobStorageProvider(IBlobStorageProviderOptionsBuilder<InMemoryBlobStorageProvider> blobStorageProviderOptionsBuilder)
    {
        string[] blobBuckets = new string[]
        {
            BlobBucketConstants.ExportBlobs,
            BlobBucketConstants.ExportIntermediateBlobs,
            BlobBucketConstants.ImportBlobs,
            BlobBucketConstants.ImportIntermediateBlobs,
        };

        blobStorageProviderOptionsBuilder.AddBlobBuckets(blobBuckets);
    }
}
