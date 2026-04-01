using AndreyTalanin0x00.Integrations.Export;

namespace JapaneseLanguageTools.Core.Export.Constants;

public static class TagExportPipelineChannelKeys
{
    public static readonly ExportPipelineChannelKey TagExportPipelineChannelKeyUnsupported = new("application-dictionary-export/unsupported");

    public static readonly ExportPipelineChannelKey TagExportPipelineChannelKeyJson = new("application-dictionary-export/json");

    public static readonly ExportPipelineChannelKey TagExportPipelineChannelKeyXml = new("application-dictionary-export/xml");
}
