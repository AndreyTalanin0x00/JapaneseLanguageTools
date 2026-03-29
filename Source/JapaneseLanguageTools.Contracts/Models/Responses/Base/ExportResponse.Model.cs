using JapaneseLanguageTools.Contracts.Models.Blobs;

namespace JapaneseLanguageTools.Contracts.Models.Responses.Base;

public class ExportResponseModel
{
    public required ExportStatus Status { get; set; }

    public required ExportResponseMessageModel[] Messages { get; set; }

    public required BlobReferenceModel BlobReference { get; set; }
}
