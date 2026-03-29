using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface IApplicationDictionaryExportService
{
    public Task<ExportApplicationDictionaryResponseModel> ExportApplicationDictionaryAsync(ExportApplicationDictionaryRequestModel exportApplicationDictionaryRequestModel, CancellationToken cancellationToken = default);
}
