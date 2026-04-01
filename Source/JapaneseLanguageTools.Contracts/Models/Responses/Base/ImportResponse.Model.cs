namespace JapaneseLanguageTools.Contracts.Models.Responses.Base;

public class ImportResponseModel
{
    public required ImportStatus Status { get; set; }

    public required ImportResponseMessageModel[] Messages { get; set; }
}
