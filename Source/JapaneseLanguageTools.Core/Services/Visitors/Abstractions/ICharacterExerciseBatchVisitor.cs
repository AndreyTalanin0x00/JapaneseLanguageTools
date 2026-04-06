using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;

namespace JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

public interface ICharacterExerciseBatchVisitor
{
    public Task VisitCharacterExerciseBatchAsync(GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CharacterExerciseBatchModel characterExerciseBatchModel, CharacterExerciseGeneratorContext characterExerciseGeneratorContext, CancellationToken cancellationToken = default);
}
