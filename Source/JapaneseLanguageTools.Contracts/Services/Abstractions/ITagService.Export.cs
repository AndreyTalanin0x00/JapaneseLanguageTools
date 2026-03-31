using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface ITagExportService
{
    public Task<ExportTagsResponseModel> ExportTagsAsync(ExportTagsRequestModel exportTagsRequestModel, CancellationToken cancellationToken = default);
}
