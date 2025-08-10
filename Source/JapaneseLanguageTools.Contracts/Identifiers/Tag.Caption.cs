namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct TagCaption
{
    public string Value { get; init; }

    public TagCaption(string value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
