using System;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

public class UploadBlobResponseModel
{
    public required BlobReferenceModel BlobReference { get; set; }

    public required DateTimeOffset BlobUploadedOn { get; set; }

    public required DateTimeOffset? BlobExpiresOn { get; set; }

    public required UploadBlobRequestModel Request { get; set; }
}
