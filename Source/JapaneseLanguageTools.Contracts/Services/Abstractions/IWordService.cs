using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface IWordService
{
    public Task<WordModel?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default);

    public Task<WordModel[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default);

    public Task<WordModel[]> GetAllWordsAsync(CancellationToken cancellationToken = default);

    public Task<WordModel> AddWordAsync(WordModel wordModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordAsync(WordModel wordModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default);
}
