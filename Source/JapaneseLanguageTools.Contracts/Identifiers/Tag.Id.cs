namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct TagId
{
    public int Value { get; init; }

    public TagId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
