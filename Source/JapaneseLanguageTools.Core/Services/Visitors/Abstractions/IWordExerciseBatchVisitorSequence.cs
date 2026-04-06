using System.Collections.Generic;

namespace JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

public interface IWordExerciseBatchVisitorSequence : IEnumerable<IWordExerciseBatchVisitor>
{
}
