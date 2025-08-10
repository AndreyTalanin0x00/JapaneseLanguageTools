namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct WordGroupCaption
{
    public string Value { get; init; }

    public WordGroupCaption(string value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
