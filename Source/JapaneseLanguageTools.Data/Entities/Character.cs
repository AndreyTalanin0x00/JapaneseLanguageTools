using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Data.Entities;

public class Character
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? CharacterGroupId { get; set; }

    [StringLength(16)]
    public string Symbol { get; set; } = string.Empty;

    public CharacterTypes Type { get; set; }

    [StringLength(32)]
    public string? Pronunciation { get; set; }

    [StringLength(32)]
    public string? Syllable { get; set; }

    [StringLength(256)]
    public string? Onyomi { get; set; }

    [StringLength(256)]
    public string? Kunyomi { get; set; }

    [StringLength(512)]
    public string? Meaning { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset UpdatedOn { get; set; }

    public CharacterGroup? CharacterGroup { get; set; }

    public IList<CharacterTag> CharacterTags { get; set; } = [];
}
