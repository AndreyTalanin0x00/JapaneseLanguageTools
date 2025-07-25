using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Data.Entities;

public class CharacterExerciseRerun
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int CharacterExerciseId { get; set; }

    public int RequiredChallengeCount { get; set; }

    public int ContinuousChallengeCount { get; set; }

    public int TotalChallengeCount { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset InitiallyScheduledOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset RepeatedlyScheduledOn { get; set; }

    public CharacterExercise? CharacterExercise { get; set; }
}
