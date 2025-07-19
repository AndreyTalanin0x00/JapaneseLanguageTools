using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Contracts.Models;

public class WordExerciseRerunModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int WordExerciseId { get; set; }

    public int RequiredChallengeCount { get; set; }

    public int ContinuousChallengeCount { get; set; }

    public int TotalChallengeCount { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset InitiallyScheduledOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset RepeatedlyScheduledOn { get; set; }

    public WordExerciseModel? WordExercise { get; set; }
}
