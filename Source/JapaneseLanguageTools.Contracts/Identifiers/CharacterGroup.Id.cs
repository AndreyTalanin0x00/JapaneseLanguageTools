namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct CharacterGroupId
{
    public int Value { get; init; }

    public CharacterGroupId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
