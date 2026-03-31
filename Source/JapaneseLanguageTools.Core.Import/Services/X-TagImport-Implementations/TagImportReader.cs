using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;
using JapaneseLanguageTools.Core.Import.Services.Base;

using Microsoft.IO;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class TagImportReader :
    ImportReaderBase<TagImportRequest, TagImportResponse>
{
    public TagImportReader(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        : base(blobManager, recyclableMemoryStreamManager)
    {
    }
}
