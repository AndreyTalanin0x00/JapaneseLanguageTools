using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface ITagImportService
{
    public Task<ImportTagsResponseModel> ImportTagsAsync(ImportTagsRequestModel importTagsRequestModel, CancellationToken cancellationToken = default);
}
