using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface ICharacterExerciseRerunRepository
{
    public Task<CharacterExerciseRerun?> GetCharacterExerciseRerunAsync(CharacterExerciseRerunId characterExerciseRerunId, CancellationToken cancellationToken = default);

    public Task<CharacterExerciseRerun[]> GetCharacterExerciseRerunsAsync(IEnumerable<CharacterExerciseRerunId> characterExerciseRerunIds, CancellationToken cancellationToken = default);

    public Task<CharacterExerciseRerun[]> GetCharacterExerciseRerunsAsync(CharacterExerciseRerunFilter characterExerciseRerunFilter, CancellationToken cancellationToken = default);

    public Task<CharacterExerciseRerun[]> GetAllCharacterExerciseRerunsAsync(CancellationToken cancellationToken = default);

    public Task<CharacterExerciseRerun> AddCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterExerciseRerunAsync(CharacterExerciseRerunId characterExerciseRerunId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, CancellationToken cancellationToken = default);
}
