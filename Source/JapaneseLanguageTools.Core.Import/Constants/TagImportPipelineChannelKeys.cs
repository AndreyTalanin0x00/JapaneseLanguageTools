using AndreyTalanin0x00.Integrations.Import;

namespace JapaneseLanguageTools.Core.Import.Constants;

public static class TagImportPipelineChannelKeys
{
    public static readonly ImportPipelineChannelKey TagImportPipelineChannelKeyUnsupported = new("application-dictionary-import/unsupported");

    public static readonly ImportPipelineChannelKey TagImportPipelineChannelKeyJson = new("application-dictionary-import/json");

    public static readonly ImportPipelineChannelKey TagImportPipelineChannelKeyXml = new("application-dictionary-import/xml");
}
