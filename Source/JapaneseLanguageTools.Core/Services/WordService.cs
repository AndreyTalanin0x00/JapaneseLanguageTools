using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class WordService : IWordService
{
    public WordService()
    {
        throw new NotImplementedException();
    }

    public Task<WordModel?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordModel[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordModel[]> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WordModel> AddWordAsync(WordModel wordModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateWordAsync(WordModel wordModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
