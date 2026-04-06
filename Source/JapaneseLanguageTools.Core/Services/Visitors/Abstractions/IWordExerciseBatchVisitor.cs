using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;

namespace JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

public interface IWordExerciseBatchVisitor
{
    public Task VisitWordExerciseBatchAsync(GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, WordExerciseBatchModel wordExerciseBatchModel, WordExerciseGeneratorContext wordExerciseGeneratorContext, CancellationToken cancellationToken = default);
}
