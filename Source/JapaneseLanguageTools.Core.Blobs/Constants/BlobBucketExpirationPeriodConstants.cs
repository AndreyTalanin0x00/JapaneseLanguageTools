using System;
using System.Collections.Generic;

namespace JapaneseLanguageTools.Core.Blobs.Constants;

public static class BlobBucketExpirationPeriodConstants
{
    private static readonly Lazy<Dictionary<string, TimeSpan>> s_blobBucketExpirationPeriodDictionaryLazy = new(InitializeBlobBucketExpirationPeriods);

    public static readonly TimeSpan ExportBlobBucketExpirationPeriod = TimeSpan.FromMinutes(30);

    public static readonly TimeSpan ExportIntermediateBlobBucketExpirationPeriod = TimeSpan.FromMinutes(10);

    public static readonly TimeSpan ImportBlobBucketExpirationPeriod = TimeSpan.FromMinutes(30);

    public static readonly TimeSpan ImportIntermediateBlobBucketExpirationPeriod = TimeSpan.FromMinutes(10);

    public static TimeSpan GetBlobBucketExpirationPeriod(string blobBucket)
    {
        Dictionary<string, TimeSpan> blobBucketExpirationPeriodDictionary = s_blobBucketExpirationPeriodDictionaryLazy.Value;

#pragma warning disable format
        if (!blobBucketExpirationPeriodDictionary.TryGetValue(blobBucket, out TimeSpan blobBucketExpirationPeriod))
            throw new KeyNotFoundException($"Could not get the expiration period for the blob bucket \"{blobBucket}\".");
#pragma warning restore format

        return blobBucketExpirationPeriod;
    }

    private static Dictionary<string, TimeSpan> InitializeBlobBucketExpirationPeriods()
    {
        Dictionary<string, TimeSpan> blobBucketExpirationPeriodDictionary = new()
        {
            { BlobBucketConstants.ExportBlobs, ExportBlobBucketExpirationPeriod },
            { BlobBucketConstants.ExportIntermediateBlobs, ExportIntermediateBlobBucketExpirationPeriod },
            { BlobBucketConstants.ImportBlobs, ImportBlobBucketExpirationPeriod },
            { BlobBucketConstants.ImportIntermediateBlobs, ImportIntermediateBlobBucketExpirationPeriod },
        };

        return blobBucketExpirationPeriodDictionary;
    }
}
