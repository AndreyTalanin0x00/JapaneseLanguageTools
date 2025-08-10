namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct WordExerciseRerunId
{
    public int Value { get; init; }

    public WordExerciseRerunId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
