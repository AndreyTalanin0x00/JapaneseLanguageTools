using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Contracts.Models;

public class CharacterExerciseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int CharacterId { get; set; }

    [StringLength(2048)]
    public string? InstanceData { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset GeneratedOn { get; set; }

    public IList<CharacterExerciseRerunModel> CharacterExerciseReruns { get; set; } = [];

    public CharacterModel? Character { get; set; }
}
