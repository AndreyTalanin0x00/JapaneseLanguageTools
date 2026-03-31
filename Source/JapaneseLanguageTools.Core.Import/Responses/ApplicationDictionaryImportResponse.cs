using AndreyTalanin0x00.Integrations.Import.Responses;

using JapaneseLanguageTools.Core.Import.Requests;

namespace JapaneseLanguageTools.Core.Import.Responses;

public class ApplicationDictionaryImportResponse : ImportResponse
{
    public required ApplicationDictionaryImportRequest Request { get; set; }
}
