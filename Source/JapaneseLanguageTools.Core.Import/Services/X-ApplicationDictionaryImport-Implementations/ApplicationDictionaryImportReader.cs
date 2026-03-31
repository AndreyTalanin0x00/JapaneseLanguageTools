using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class ApplicationDictionaryImportReader :
    IImportReader<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse>
{
    /// <inheritdoc />
    public Task<ImportSourceBatch[]> ReadAsync(ApplicationDictionaryImportRequest applicationDictionaryImportRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
