using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface IWordRepository
{
    public Task<Word?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default);

    public Task<Word[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default);

    public Task<Word[]> GetWordsAsync(WordFilter wordFilter, CancellationToken cancellationToken = default);

    public Task<Word[]> GetAllWordsAsync(CancellationToken cancellationToken = default);

    public Task<Word> AddWordAsync(Word word, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordAsync(Word word, CancellationToken cancellationToken = default);
}
