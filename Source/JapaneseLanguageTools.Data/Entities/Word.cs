using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Data.Entities;

public class Word
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? WordGroupId { get; set; }

    [StringLength(256)]
    public string Characters { get; set; } = string.Empty;

    public CharacterTypes CharacterTypes { get; set; }

    [StringLength(512)]
    public string? Pronunciation { get; set; }

    [StringLength(512)]
    public string? Furigana { get; set; }

    [StringLength(512)]
    public string? Okurigana { get; set; }

    [StringLength(512)]
    public string? Meaning { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset UpdatedOn { get; set; }

    public WordGroup? WordGroup { get; set; }

    public IList<WordTag> WordTags { get; set; } = [];
}
