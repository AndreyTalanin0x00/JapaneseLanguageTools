using JapaneseLanguageTools.Contracts.Models.Blobs;

namespace JapaneseLanguageTools.Contracts.Models.Requests.Base;

public class ImportRequestModel
{
    public required BlobReferenceModel[] BlobReferences { get; set; } = [];
}
