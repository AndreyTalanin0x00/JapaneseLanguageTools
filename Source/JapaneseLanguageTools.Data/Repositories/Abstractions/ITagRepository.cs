using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface ITagRepository
{
    public Task<Tag?> GetTagAsync(TagId tagId, CancellationToken cancellationToken = default);

    public Task<Tag?> GetTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default);

    public Task<Tag[]> GetTagsAsync(IEnumerable<TagId> tagIds, CancellationToken cancellationToken = default);

    public Task<Tag[]> GetTagsAsync(IEnumerable<TagCaption> tagCaptions, CancellationToken cancellationToken = default);

    public Task<Tag[]> GetTagsAsync(TagFilter tagFilter, CancellationToken cancellationToken = default);

    public Task<Tag[]> GetAllTagsAsync(CancellationToken cancellationToken = default);

    public Task<Tag> AddTagAsync(Tag tag, CancellationToken cancellationToken = default);

    public Task<bool> UpdateTagAsync(Tag tag, CancellationToken cancellationToken = default);

    public Task<bool> RemoveTagAsync(TagId tagId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default);

    public Task<bool> RemoveTagAsync(Tag tag, CancellationToken cancellationToken = default);
}
