using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Data.Entities;

public class CharacterExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    [StringLength(2048)]
    public string? InstanceData { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset GeneratedOn { get; set; }

    public IList<CharacterExerciseRerun> CharacterExerciseReruns { get; set; } = [];

    public Character? Character { get; set; }
}
