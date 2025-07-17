using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Contracts.Models;

public class WordExerciseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int WordId { get; set; }

    [StringLength(2048)]
    public string? InstanceData { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset GeneratedOn { get; set; }

    public IList<WordExerciseRerunModel> WordExerciseReruns { get; set; } = [];

    public WordModel? Word { get; set; }
}
