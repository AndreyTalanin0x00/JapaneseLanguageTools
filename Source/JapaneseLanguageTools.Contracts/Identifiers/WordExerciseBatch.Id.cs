using System;

namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct WordExerciseBatchId
{
    public Guid Value { get; init; }

    public WordExerciseBatchId(Guid value)
    {
        Value = value;
    }

    public static WordExerciseBatchId CreateRandom()
    {
        Guid value = Guid.NewGuid();
        WordExerciseBatchId wordExerciseBatchId = new(value);
        return wordExerciseBatchId;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
