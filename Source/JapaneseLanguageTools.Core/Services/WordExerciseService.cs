using System;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class WordExerciseService : IWordExerciseService
{
    public WordExerciseService()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<GetWordExerciseBatchResponseModel?> GetWordExerciseBatchAsync(GetWordExerciseBatchRequestModel getWordExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<GenerateWordExerciseBatchResponseModel> GenerateWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<SubmitCompletedWordExerciseDetailsResponseModel> SubmitCompletedWordExerciseDetailsAsync(SubmitCompletedWordExerciseDetailsRequestModel submitCompletedWordExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<SubmitFailedWordExerciseDetailsResponseModel> SubmitFailedWordExerciseDetailsAsync(SubmitFailedWordExerciseDetailsRequestModel submitFailedWordExerciseDetailsRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
