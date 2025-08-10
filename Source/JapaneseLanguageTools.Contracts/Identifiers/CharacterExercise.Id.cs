namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct CharacterExerciseId
{
    public int Value { get; init; }

    public CharacterExerciseId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
