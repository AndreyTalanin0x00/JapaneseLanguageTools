namespace JapaneseLanguageTools.Data.Entities;

public class CharacterTag
{
    public int CharacterId { get; set; }

    public int TagId { get; set; }

    public Character? Character { get; set; }

    public Tag? Tag { get; set; }
}
