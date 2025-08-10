namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct CharacterExerciseRerunId
{
    public int Value { get; init; }

    public CharacterExerciseRerunId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
