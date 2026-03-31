namespace JapaneseLanguageTools.Contracts.Models.Responses.Base;

public class ExportResponseMessageModel
{
    public required ExportResponseMessageType Type { get; set; }

    public required string Text { get; set; }
}
