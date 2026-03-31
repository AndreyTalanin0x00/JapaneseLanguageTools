using System;

using AndreyTalanin0x00.Integrations.Blobs;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;
using JapaneseLanguageTools.Core.Export.Services.Base;

using Microsoft.IO;

using ExportStatus = AndreyTalanin0x00.Integrations.Export.Enumerations.ExportStatus;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class ApplicationDictionaryExportWriter :
    ExportWriterBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse>
{
    private readonly TimeProvider m_timeProvider;

    public ApplicationDictionaryExportWriter(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager, TimeProvider timeProvider)
        : base(blobManager, recyclableMemoryStreamManager)
    {
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    protected override string GetFileName(string mimeType, ApplicationDictionaryExportRequest applicationDictionaryExportRequest)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        SnapshotType snapshotType = applicationDictionaryExportRequest.SnapshotType;

        const string applicationDictionaryExportFileNameFormat = "JLT Application Dictionary {0} Export {1}.json";

        string snapshotTypeString = snapshotType switch
        {
            SnapshotType.General => nameof(SnapshotType.General),
            SnapshotType.GeneralNoAction => nameof(SnapshotType.GeneralNoAction),
            SnapshotType.ChangeState => nameof(SnapshotType.ChangeState),
            SnapshotType.Patch => nameof(SnapshotType.Patch),
            _ or SnapshotType.Unknown => throw new NotSupportedException($"Unknown snapshot type: {(int)snapshotType} ({snapshotType})."),
        };

        string dateTimeString = utcNow.ToString("u")
            .Replace("Z", " " + "UTC")
            .Replace(':', '-');

        string applicationDictionaryExportFileName = string.Format(applicationDictionaryExportFileNameFormat, snapshotTypeString, dateTimeString);

        return applicationDictionaryExportFileName;
    }

    /// <inheritdoc />
    protected override ApplicationDictionaryExportResponse CreateExportResponse(ExportStatus exportStatus, ApplicationDictionaryExportRequest applicationDictionaryExportRequest, BlobReference blobReference, BlobMetadata blobMetadata)
    {
        ApplicationDictionaryExportResponse applicationDictionaryExportResponse = new()
        {
            Status = exportStatus,
            Messages = [],
            BlobReference = blobReference,
            Request = applicationDictionaryExportRequest,
        };

        return applicationDictionaryExportResponse;
    }
}
