using System;
using System.Collections.Generic;

namespace JapaneseLanguageTools.Contracts.Models;

public class WordExerciseBatchModel
{
    public Guid Id { get; set; }

    public IList<WordExerciseModel> Items { get; set; } = [];
}
