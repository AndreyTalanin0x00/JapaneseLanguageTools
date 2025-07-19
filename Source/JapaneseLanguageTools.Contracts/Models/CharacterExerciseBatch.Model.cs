using System;
using System.Collections.Generic;

namespace JapaneseLanguageTools.Contracts.Models;

public class CharacterExerciseBatchModel
{
    public Guid Id { get; set; }

    public IList<CharacterExerciseModel> Items { get; set; } = [];
}
