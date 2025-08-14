using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface IWordExerciseService
{
    #region CRUD Methods

    public Task<WordExerciseModel?> GetWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default);

    public Task<WordExerciseModel[]> GetWordExercisesAsync(IEnumerable<WordExerciseId> wordExerciseIds, CancellationToken cancellationToken = default);

    public Task<WordExerciseModel[]> GetAllWordExercisesAsync(CancellationToken cancellationToken = default);

    public Task<WordExerciseModel> AddWordExerciseAsync(WordExerciseModel wordExerciseModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateWordExerciseAsync(WordExerciseModel wordExerciseModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default);

    #endregion

    #region Request & Response Methods

    public Task<GetWordExerciseBatchResponseModel?> GetWordExerciseBatchAsync(GetWordExerciseBatchRequestModel getWordExerciseBatchRequestModel, CancellationToken cancellationToken = default);

    public Task<GenerateWordExerciseBatchResponseModel> GenerateWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, CancellationToken cancellationToken = default);

    public Task<SubmitCompletedWordExerciseDetailsResponseModel> SubmitCompletedWordExerciseDetailsAsync(SubmitCompletedWordExerciseDetailsRequestModel submitCompletedWordExerciseDetailsRequestModel, CancellationToken cancellationToken = default);

    public Task<SubmitFailedWordExerciseDetailsResponseModel> SubmitFailedWordExerciseDetailsAsync(SubmitFailedWordExerciseDetailsRequestModel submitFailedWordExerciseDetailsRequestModel, CancellationToken cancellationToken = default);

    #endregion
}
