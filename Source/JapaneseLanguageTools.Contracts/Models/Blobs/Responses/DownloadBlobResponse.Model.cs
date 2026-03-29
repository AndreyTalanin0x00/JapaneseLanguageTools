using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

public class DownloadBlobResponseModel
{
    public required BlobMetadataModel BlobMetadata { get; set; }

    public required DownloadBlobRequestModel Request { get; set; }
}
