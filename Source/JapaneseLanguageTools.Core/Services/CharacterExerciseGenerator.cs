using System;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class CharacterExerciseGenerator : ICharacterExerciseGenerator
{
    /// <inheritdoc />
    public Task<GenerateCharacterExerciseBatchFullResponseModel> GenerateCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
