namespace JapaneseLanguageTools.Contracts.Models.Blobs.Requests;

public class GetBlobExpirationTimeRequestModel
{
    public required BlobReferenceModel BlobReference { get; set; }
}
