using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

using AndreyTalanin0x00.DbNullHelpers;

using JapaneseLanguageTools.Data.Entities;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Data.SqlServer.Extensions;

internal static class WordExerciseRerunSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<WordExerciseRerun> wordExerciseReruns)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(WordExerciseRerun.Id), typeof(int));
        dataTable.Columns.Add(nameof(WordExerciseRerun.WordExerciseId), typeof(int));
        dataTable.Columns.Add(nameof(WordExerciseRerun.RequiredChallengeCount), typeof(int));
        dataTable.Columns.Add(nameof(WordExerciseRerun.ContinuousChallengeCount), typeof(int));
        dataTable.Columns.Add(nameof(WordExerciseRerun.TotalChallengeCount), typeof(int));
        dataTable.Columns.Add(nameof(WordExerciseRerun.InitiallyScheduledOn), typeof(DateTimeOffset));
        dataTable.Columns.Add(nameof(WordExerciseRerun.RepeatedlyScheduledOn), typeof(DateTimeOffset));

        foreach (WordExerciseRerun wordExerciseRerun in wordExerciseReruns)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(wordExerciseRerun.Id),
                DbValueConvert.ToDbValue(wordExerciseRerun.WordExerciseId),
                DbValueConvert.ToDbValue(wordExerciseRerun.RequiredChallengeCount),
                DbValueConvert.ToDbValue(wordExerciseRerun.ContinuousChallengeCount),
                DbValueConvert.ToDbValue(wordExerciseRerun.TotalChallengeCount),
                DbValueConvert.ToDbValue(wordExerciseRerun.InitiallyScheduledOn),
                DbValueConvert.ToDbValue(wordExerciseRerun.RepeatedlyScheduledOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }
}
