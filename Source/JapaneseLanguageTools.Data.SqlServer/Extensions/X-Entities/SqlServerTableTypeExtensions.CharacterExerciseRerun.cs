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

internal static class CharacterExerciseRerunSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<CharacterExerciseRerun> characterExerciseReruns)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(CharacterExerciseRerun.Id), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExerciseRerun.CharacterExerciseId), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExerciseRerun.RequiredChallengeCount), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExerciseRerun.ContinuousChallengeCount), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExerciseRerun.TotalChallengeCount), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExerciseRerun.InitiallyScheduledOn), typeof(DateTimeOffset));
        dataTable.Columns.Add(nameof(CharacterExerciseRerun.RepeatedlyScheduledOn), typeof(DateTimeOffset));

        foreach (CharacterExerciseRerun characterExerciseRerun in characterExerciseReruns)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(characterExerciseRerun.Id),
                DbValueConvert.ToDbValue(characterExerciseRerun.CharacterExerciseId),
                DbValueConvert.ToDbValue(characterExerciseRerun.RequiredChallengeCount),
                DbValueConvert.ToDbValue(characterExerciseRerun.ContinuousChallengeCount),
                DbValueConvert.ToDbValue(characterExerciseRerun.TotalChallengeCount),
                DbValueConvert.ToDbValue(characterExerciseRerun.InitiallyScheduledOn),
                DbValueConvert.ToDbValue(characterExerciseRerun.RepeatedlyScheduledOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }
}
