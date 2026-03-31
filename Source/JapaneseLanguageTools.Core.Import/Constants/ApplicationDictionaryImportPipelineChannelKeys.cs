using AndreyTalanin0x00.Integrations.Import;

namespace JapaneseLanguageTools.Core.Import.Constants;

public static class ApplicationDictionaryImportPipelineChannelKeys
{
    public static readonly ImportPipelineChannelKey ApplicationDictionaryImportPipelineChannelKeyUnsupported = new("application-dictionary-import/unsupported");

    public static readonly ImportPipelineChannelKey ApplicationDictionaryImportPipelineChannelKeyJson = new("application-dictionary-import/json");

    public static readonly ImportPipelineChannelKey ApplicationDictionaryImportPipelineChannelKeyXml = new("application-dictionary-import/xml");
}
