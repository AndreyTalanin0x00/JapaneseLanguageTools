using AndreyTalanin0x00.Integrations.Export.Responses;

using JapaneseLanguageTools.Core.Export.Requests;

namespace JapaneseLanguageTools.Core.Export.Responses;

public class TagExportResponse : ExportResponse
{
    public required TagExportRequest Request { get; set; }
}
