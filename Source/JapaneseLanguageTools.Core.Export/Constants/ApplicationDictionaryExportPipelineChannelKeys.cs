using AndreyTalanin0x00.Integrations.Export;

namespace JapaneseLanguageTools.Core.Export.Constants;

public static class ApplicationDictionaryExportPipelineChannelKeys
{
    public static readonly ExportPipelineChannelKey ApplicationDictionaryExportPipelineChannelKeyUnsupported = new("application-dictionary-export/unsupported");

    public static readonly ExportPipelineChannelKey ApplicationDictionaryExportPipelineChannelKeyJson = new("application-dictionary-export/json");

    public static readonly ExportPipelineChannelKey ApplicationDictionaryExportPipelineChannelKeyXml = new("application-dictionary-export/xml");
}
