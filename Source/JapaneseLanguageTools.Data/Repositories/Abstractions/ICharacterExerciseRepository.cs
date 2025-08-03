using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface ICharacterExerciseRepository
{
    public Task<CharacterExercise?> GetCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default);

    public Task<CharacterExercise[]> GetCharacterExercisesAsync(IEnumerable<CharacterExerciseId> characterExerciseIds, CancellationToken cancellationToken = default);

    public Task<CharacterExercise[]> GetCharacterExercisesAsync(CharacterExerciseFilter characterExerciseFilter, CancellationToken cancellationToken = default);

    public Task<CharacterExercise[]> GetAllCharacterExercisesAsync(CancellationToken cancellationToken = default);

    public Task<CharacterExercise> AddCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default);
}
