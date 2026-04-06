using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class WordExerciseBatchVisitorSequence : IWordExerciseBatchVisitorSequence
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly Type[] s_wordExerciseBatchVisitorTypes = new Type[]
    {
        typeof(RegularChallengeByExplicitlyDistributedTagsWordExerciseBatchVisitor),
        typeof(RepeatedChallengeByPlaceholderWordExerciseBatchVisitor),
        typeof(RepeatedChallengeByExceededRerunThresholdPlaceholderWordExerciseBatchVisitor),
        typeof(AlwaysPresentRandomChallengeByPlaceholderWordExerciseBatchVisitor),
        typeof(RegularChallengeWordExerciseBatchVisitor),
        typeof(RepeatedChallengeByOriginalWordMatchWordExerciseBatchVisitor),
        typeof(OrderShuffleWordExerciseBatchVisitor),
    };

    private readonly IWordExerciseBatchVisitor[] m_wordExerciseBatchVisitors;

    public WordExerciseBatchVisitorSequence(IEnumerable<IWordExerciseBatchVisitor> wordExerciseBatchVisitors)
    {
        static Type KeySelector(IWordExerciseBatchVisitor wordExerciseBatchVisitor) => wordExerciseBatchVisitor.GetType();

        Dictionary<Type, IWordExerciseBatchVisitor> wordExerciseBatchVisitorsByType = wordExerciseBatchVisitors.ToDictionary(KeySelector);

        m_wordExerciseBatchVisitors = new IWordExerciseBatchVisitor[s_wordExerciseBatchVisitorTypes.Length];

        for (int index = 0; index < s_wordExerciseBatchVisitorTypes.Length; index++)
        {
            Type wordExerciseBatchVisitorType = s_wordExerciseBatchVisitorTypes[index];

            if (!wordExerciseBatchVisitorsByType.TryGetValue(wordExerciseBatchVisitorType, out IWordExerciseBatchVisitor? wordExerciseBatchVisitor))
                throw new InvalidOperationException($"The {wordExerciseBatchVisitorType.FullName} service is not registered in the Dependency Injection container and can not be used to construct a sequence.");

            m_wordExerciseBatchVisitors[index] = wordExerciseBatchVisitor;
        }
    }

    /// <inheritdoc />
    public IEnumerator<IWordExerciseBatchVisitor> GetEnumerator()
    {
        IEnumerable<IWordExerciseBatchVisitor> wordExerciseBatchVisitorsEnumerable = m_wordExerciseBatchVisitors.AsEnumerable();
        IEnumerator<IWordExerciseBatchVisitor> wordExerciseBatchVisitorsEnumerator = wordExerciseBatchVisitorsEnumerable.GetEnumerator();

        return wordExerciseBatchVisitorsEnumerator;
    }

    #region IEnumerable Explicit Interface Implementation

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}
