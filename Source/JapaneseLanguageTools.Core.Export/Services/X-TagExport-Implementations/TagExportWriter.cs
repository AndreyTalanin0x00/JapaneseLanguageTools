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

public class TagExportWriter :
    ExportWriterBase<TagExportRequest, TagExportResponse>
{
    private readonly TimeProvider m_timeProvider;

    public TagExportWriter(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager, TimeProvider timeProvider)
        : base(blobManager, recyclableMemoryStreamManager)
    {
        m_timeProvider = timeProvider;
    }

    protected override string GetFileName(string mimeType, TagExportRequest tagExportRequest)
    {
        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        SnapshotType snapshotType = tagExportRequest.SnapshotType;

        const string tagExportFileNameFormat = "JLT Tag {0} Export {1}.json";

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

        string tagExportFileName = string.Format(tagExportFileNameFormat, snapshotTypeString, dateTimeString);

        return tagExportFileName;
    }

    protected override TagExportResponse CreateExportResponse(ExportStatus exportStatus, TagExportRequest tagExportRequest, BlobReference blobReference, BlobMetadata blobMetadata)
    {
        TagExportResponse tagExportResponse = new()
        {
            Status = exportStatus,
            Messages = [],
            BlobReference = blobReference,
            Request = tagExportRequest,
        };

        return tagExportResponse;
    }
}
