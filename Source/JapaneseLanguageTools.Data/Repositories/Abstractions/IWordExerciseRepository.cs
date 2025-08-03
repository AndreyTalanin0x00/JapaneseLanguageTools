using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface IWordExerciseRepository
{
    public Task<WordExercise?> GetWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default);

    public Task<WordExercise[]> GetWordExercisesAsync(IEnumerable<WordExerciseId> wordExerciseIds, CancellationToken cancellationToken = default);

    public Task<WordExercise[]> GetWordExercisesAsync(WordExerciseFilter wordExerciseFilter, CancellationToken cancellationToken = default);

    public Task<WordExercise[]> GetAllWordExercisesAsync(CancellationToken cancellationToken = default);

    public Task<WordExercise> AddWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default);
}
