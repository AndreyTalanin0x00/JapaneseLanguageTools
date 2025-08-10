using System;

namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct CharacterExerciseBatchId
{
    public Guid Value { get; init; }

    public CharacterExerciseBatchId(Guid value)
    {
        Value = value;
    }

    public static CharacterExerciseBatchId CreateRandom()
    {
        Guid value = Guid.NewGuid();
        CharacterExerciseBatchId characterExerciseBatchId = new(value);
        return characterExerciseBatchId;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
