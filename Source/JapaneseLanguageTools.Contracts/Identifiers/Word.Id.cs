namespace JapaneseLanguageTools.Contracts.Identifiers;

public readonly record struct WordId
{
    public int Value { get; init; }

    public WordId(int value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}
