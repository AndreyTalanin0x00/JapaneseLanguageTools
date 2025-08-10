namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct CharacterGroupCaption
{
    public string Value { get; init; }

    public CharacterGroupCaption(string value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
