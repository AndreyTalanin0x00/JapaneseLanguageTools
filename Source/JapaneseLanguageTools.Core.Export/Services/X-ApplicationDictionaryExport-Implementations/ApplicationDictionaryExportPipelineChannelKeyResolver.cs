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

public class ApplicationDictionaryExportPipelineChannelKeyResolver :
    IExportPipelineChannelKeyResolver<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse>
{
    /// <inheritdoc />
    public ExportPipelineChannelKey ResolveExportPipelineChannelKey(ApplicationDictionaryExportRequest applicationDictionaryExportRequest)
    {
        ExportPipelineChannelKey exportPipelineChannelKey = applicationDictionaryExportRequest.SnapshotFileFormat switch
        {
            SnapshotFileFormat.Json => ApplicationDictionaryExportPipelineChannelKeys.ApplicationDictionaryExportPipelineChannelKeyJson,
            SnapshotFileFormat.Xml => ApplicationDictionaryExportPipelineChannelKeys.ApplicationDictionaryExportPipelineChannelKeyXml,
            _ or SnapshotFileFormat.Unknown => ApplicationDictionaryExportPipelineChannelKeys.ApplicationDictionaryExportPipelineChannelKeyUnsupported,
        };

        return exportPipelineChannelKey;
    }
}
