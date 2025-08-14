using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface ICharacterExerciseService
{
    #region CRUD Methods

    public Task<CharacterExerciseModel?> GetCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default);

    public Task<CharacterExerciseModel[]> GetCharacterExercisesAsync(IEnumerable<CharacterExerciseId> characterExerciseIds, CancellationToken cancellationToken = default);

    public Task<CharacterExerciseModel[]> GetAllCharacterExercisesAsync(CancellationToken cancellationToken = default);

    public Task<CharacterExerciseModel> AddCharacterExerciseAsync(CharacterExerciseModel characterExerciseModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterExerciseAsync(CharacterExerciseModel characterExerciseModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default);

    #endregion

    #region Request & Response Methods

    public Task<GetCharacterExerciseBatchResponseModel?> GetCharacterExerciseBatchAsync(GetCharacterExerciseBatchRequestModel getCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default);

    public Task<GenerateCharacterExerciseBatchResponseModel> GenerateCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default);

    public Task<SubmitCompletedCharacterExerciseDetailsResponseModel> SubmitCompletedCharacterExerciseDetailsAsync(SubmitCompletedCharacterExerciseDetailsRequestModel submitCompletedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken = default);

    public Task<SubmitFailedCharacterExerciseDetailsResponseModel> SubmitFailedCharacterExerciseDetailsAsync(SubmitFailedCharacterExerciseDetailsRequestModel submitFailedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken = default);

    #endregion
}
