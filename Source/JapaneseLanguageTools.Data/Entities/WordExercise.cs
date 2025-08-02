using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Data.Entities;

public class WordExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int WordId { get; set; }

    [StringLength(2048)]
    public string? InstanceData { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset GeneratedOn { get; set; }

    public IList<WordExerciseRerun> WordExerciseReruns { get; set; } = [];

    public Word? Word { get; set; }
}
