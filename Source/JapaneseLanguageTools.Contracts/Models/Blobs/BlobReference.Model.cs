using System;
using System.Diagnostics.CodeAnalysis;

namespace JapaneseLanguageTools.Contracts.Models.Blobs;

public class BlobReferenceModel
{
    public required Guid Id { get; set; }

    public required Uri Uri { get; set; }

    public BlobReferenceModel()
    {
    }

    [SetsRequiredMembers]
    public BlobReferenceModel(Guid id, Uri uri)
    {
        Id = id;
        Uri = uri;
    }
}
