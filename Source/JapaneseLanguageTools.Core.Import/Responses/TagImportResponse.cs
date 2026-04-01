using AndreyTalanin0x00.Integrations.Import.Responses;

using JapaneseLanguageTools.Core.Import.Requests;

namespace JapaneseLanguageTools.Core.Import.Responses;

public class TagImportResponse : ImportResponse
{
    public required TagImportRequest Request { get; set; }
}
