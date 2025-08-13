using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface IWordGroupService
{
    public Task<WordGroupModel?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default);

    public Task<WordGroupModel?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default);

    public Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default);

    public Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default);

    public Task<WordGroupModel[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default);

    public Task<WordGroupModel> AddWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default);

    public Task<bool> ChangeWordGroupStateAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default);
}
