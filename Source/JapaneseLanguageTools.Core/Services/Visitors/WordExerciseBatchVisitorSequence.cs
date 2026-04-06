using System;
using System.Collections;
using System.Collections.Generic;

using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class WordExerciseBatchVisitorSequence : IWordExerciseBatchVisitorSequence
{
    /// <inheritdoc />
    public IEnumerator<IWordExerciseBatchVisitor> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    #region IEnumerable Explicit Interface Implementation

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}
