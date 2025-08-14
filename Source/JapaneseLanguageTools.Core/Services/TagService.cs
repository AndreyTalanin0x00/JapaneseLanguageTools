using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class TagService : ITagService
{
    public TagService()
    {
        throw new NotImplementedException();
    }

    public Task<TagModel?> GetTagAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TagModel?> GetTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TagModel[]> GetTagsAsync(IEnumerable<TagId> tagIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TagModel[]> GetTagsAsync(IEnumerable<TagCaption> tagCaptions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TagModel[]> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TagModel> AddTagAsync(TagModel tagModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTagAsync(TagModel tagModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveTagAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
