using System;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class CharacterExerciseService : ICharacterExerciseService
{
    public CharacterExerciseService()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<GetCharacterExerciseBatchResponseModel?> GetCharacterExerciseBatchAsync(GetCharacterExerciseBatchRequestModel getCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<GenerateCharacterExerciseBatchResponseModel> GenerateCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<SubmitCompletedCharacterExerciseDetailsResponseModel> SubmitCompletedCharacterExerciseDetailsAsync(SubmitCompletedCharacterExerciseDetailsRequestModel submitCompletedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<SubmitFailedCharacterExerciseDetailsResponseModel> SubmitFailedCharacterExerciseDetailsAsync(SubmitFailedCharacterExerciseDetailsRequestModel submitFailedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
