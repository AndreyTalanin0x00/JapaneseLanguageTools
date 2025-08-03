using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface IWordGroupRepository
{
    public Task<WordGroup?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default);

    public Task<WordGroup?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default);

    public Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default);

    public Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default);

    public Task<WordGroup[]> GetWordGroupsAsync(WordGroupFilter wordGroupFilter, CancellationToken cancellationToken = default);

    public Task<WordGroup[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default);

    public Task<WordGroup> AddWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default);

    public Task<bool> ChangeWordGroupStateAsync(WordGroup wordGroup, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default);
}
