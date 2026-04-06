using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JapaneseLanguageTools.Core.Services.Visitors.Abstractions;

namespace JapaneseLanguageTools.Core.Services.Visitors;

public class CharacterExerciseBatchVisitorSequence : ICharacterExerciseBatchVisitorSequence
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly Type[] s_characterExerciseBatchVisitorTypes = new Type[]
    {
        typeof(RegularChallengeByExplicitlyDistributedTagsCharacterExerciseBatchVisitor),
        typeof(RepeatedChallengeByPlaceholderCharacterExerciseBatchVisitor),
        typeof(RepeatedChallengeByExceededRerunThresholdPlaceholderCharacterExerciseBatchVisitor),
        typeof(AlwaysPresentRandomChallengeByPlaceholderCharacterExerciseBatchVisitor),
        typeof(RegularChallengeCharacterExerciseBatchVisitor),
        typeof(RepeatedChallengeByOriginalCharacterMatchCharacterExerciseBatchVisitor),
        typeof(OrderShuffleCharacterExerciseBatchVisitor),
    };

    private readonly ICharacterExerciseBatchVisitor[] m_characterExerciseBatchVisitors;

    public CharacterExerciseBatchVisitorSequence(IEnumerable<ICharacterExerciseBatchVisitor> characterExerciseBatchVisitors)
    {
        static Type KeySelector(ICharacterExerciseBatchVisitor characterExerciseBatchVisitor) => characterExerciseBatchVisitor.GetType();

        Dictionary<Type, ICharacterExerciseBatchVisitor> characterExerciseBatchVisitorsByType = characterExerciseBatchVisitors.ToDictionary(KeySelector);

        m_characterExerciseBatchVisitors = new ICharacterExerciseBatchVisitor[s_characterExerciseBatchVisitorTypes.Length];

        for (int index = 0; index < s_characterExerciseBatchVisitorTypes.Length; index++)
        {
            Type characterExerciseBatchVisitorType = s_characterExerciseBatchVisitorTypes[index];

            if (!characterExerciseBatchVisitorsByType.TryGetValue(characterExerciseBatchVisitorType, out ICharacterExerciseBatchVisitor? characterExerciseBatchVisitor))
                throw new InvalidOperationException($"The {characterExerciseBatchVisitorType.FullName} service is not registered in the Dependency Injection container and can not be used to construct a sequence.");

            m_characterExerciseBatchVisitors[index] = characterExerciseBatchVisitor;
        }
    }

    /// <inheritdoc />
    public IEnumerator<ICharacterExerciseBatchVisitor> GetEnumerator()
    {
        IEnumerable<ICharacterExerciseBatchVisitor> characterExerciseBatchVisitorsEnumerable = m_characterExerciseBatchVisitors.AsEnumerable();
        IEnumerator<ICharacterExerciseBatchVisitor> characterExerciseBatchVisitorsEnumerator = characterExerciseBatchVisitorsEnumerable.GetEnumerator();

        return characterExerciseBatchVisitorsEnumerator;
    }

    #region IEnumerable Explicit Interface Implementation

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}
