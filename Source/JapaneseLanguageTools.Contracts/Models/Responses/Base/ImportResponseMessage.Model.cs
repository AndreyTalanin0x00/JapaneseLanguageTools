namespace JapaneseLanguageTools.Contracts.Models.Responses.Base;

public class ImportResponseMessageModel
{
    public required ImportResponseMessageType Type { get; set; }

    public required string Text { get; set; }
}
