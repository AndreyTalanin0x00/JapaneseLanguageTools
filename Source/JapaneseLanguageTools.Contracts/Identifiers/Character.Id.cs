namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct CharacterId
{
    public int Value { get; init; }

    public CharacterId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
