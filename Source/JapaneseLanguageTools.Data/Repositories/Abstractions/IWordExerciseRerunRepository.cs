using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface IWordExerciseRerunRepository
{
    public Task<WordExerciseRerun?> GetWordExerciseRerunAsync(WordExerciseRerunId wordExerciseRerunId, CancellationToken cancellationToken = default);

    public Task<WordExerciseRerun[]> GetWordExerciseRerunsAsync(IEnumerable<WordExerciseRerunId> wordExerciseRerunIds, CancellationToken cancellationToken = default);

    public Task<WordExerciseRerun[]> GetWordExerciseRerunsAsync(WordExerciseRerunFilter wordExerciseRerunFilter, CancellationToken cancellationToken = default);

    public Task<WordExerciseRerun[]> GetAllWordExerciseRerunsAsync(CancellationToken cancellationToken = default);

    public Task<WordExerciseRerun> AddWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordExerciseRerunAsync(WordExerciseRerunId wordExerciseRerunId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, CancellationToken cancellationToken = default);
}
