namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct WordGroupId
{
    public int Value { get; init; }

    public WordGroupId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
