using System;
using System.Collections;
using System.Collections.Generic;

using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class CharacterExerciseBatchVisitorSequence : ICharacterExerciseBatchVisitorSequence
{
    /// <inheritdoc />
    public IEnumerator<ICharacterExerciseBatchVisitor> GetEnumerator()
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
