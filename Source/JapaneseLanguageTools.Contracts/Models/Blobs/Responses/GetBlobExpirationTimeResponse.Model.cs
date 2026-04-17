using System;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

public class GetBlobExpirationTimeResponseModel
{
    public required DateTimeOffset? BlobExpirationTime { get; set; }

    public required GetBlobExpirationTimeRequestModel Request { get; set; }
}
