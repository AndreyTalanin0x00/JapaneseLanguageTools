using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class WordGroupService : IWordGroupService
{
    public WordGroupService()
    {
        throw new NotImplementedException();
    }

    public Task<WordGroupModel?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordGroupModel?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordGroupModel[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordGroupModel> AddWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangeWordGroupStateAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
