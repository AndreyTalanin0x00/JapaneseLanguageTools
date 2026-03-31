using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Core.Export.Constants;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class TagExportPipelineChannelKeyResolver :
    IExportPipelineChannelKeyResolver<TagExportRequest, TagExportResponse>
{
    /// <inheritdoc />
    public ExportPipelineChannelKey ResolveExportPipelineChannelKey(TagExportRequest tagExportRequest)
    {
        ExportPipelineChannelKey exportPipelineChannelKey = tagExportRequest.SnapshotFileFormat switch
        {
            SnapshotFileFormat.Json => TagExportPipelineChannelKeys.TagExportPipelineChannelKeyJson,
            SnapshotFileFormat.Xml => TagExportPipelineChannelKeys.TagExportPipelineChannelKeyXml,
            _ or SnapshotFileFormat.Unknown => TagExportPipelineChannelKeys.TagExportPipelineChannelKeyUnsupported,
        };

        return exportPipelineChannelKey;
    }
}
