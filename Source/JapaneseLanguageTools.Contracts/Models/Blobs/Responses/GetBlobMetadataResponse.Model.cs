using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

public class GetBlobMetadataResponseModel
{
    public required BlobMetadataModel BlobMetadata { get; set; }

    public required GetBlobMetadataRequestModel Request { get; set; }
}
