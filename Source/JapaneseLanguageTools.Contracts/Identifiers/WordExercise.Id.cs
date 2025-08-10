namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct WordExerciseId
{
    public int Value { get; init; }

    public WordExerciseId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
