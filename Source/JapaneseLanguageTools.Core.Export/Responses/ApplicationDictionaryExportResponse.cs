using AndreyTalanin0x00.Integrations.Export.Responses;

using JapaneseLanguageTools.Core.Export.Requests;

namespace JapaneseLanguageTools.Core.Export.Responses;

public class ApplicationDictionaryExportResponse : ExportResponse
{
    public required ApplicationDictionaryExportRequest Request { get; set; }
}
