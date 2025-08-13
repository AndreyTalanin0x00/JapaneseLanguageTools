using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface ITagService
{
    public Task<TagModel?> GetTagAsync(TagId tagId, CancellationToken cancellationToken = default);

    public Task<TagModel?> GetTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default);

    public Task<TagModel[]> GetTagsAsync(IEnumerable<TagId> tagIds, CancellationToken cancellationToken = default);

    public Task<TagModel[]> GetTagsAsync(IEnumerable<TagCaption> tagCaptions, CancellationToken cancellationToken = default);

    public Task<TagModel[]> GetAllTagsAsync(CancellationToken cancellationToken = default);

    public Task<TagModel> AddTagAsync(TagModel tagModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateTagAsync(TagModel tagModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveTagAsync(TagId tagId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default);
}
