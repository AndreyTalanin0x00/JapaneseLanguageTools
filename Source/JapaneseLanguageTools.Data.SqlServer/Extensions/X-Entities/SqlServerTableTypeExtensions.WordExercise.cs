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

internal static class WordExerciseSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<WordExercise> wordExercises)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(WordExercise.Id), typeof(int));
        dataTable.Columns.Add(nameof(WordExercise.WordId), typeof(int));
        dataTable.Columns.Add(nameof(WordExercise.InstanceData), typeof(string));
        dataTable.Columns.Add(nameof(WordExercise.GeneratedOn), typeof(DateTimeOffset));

        foreach (WordExercise wordExercise in wordExercises)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(wordExercise.Id),
                DbValueConvert.ToDbValue(wordExercise.WordId),
                DbValueConvert.ToDbValue(wordExercise.InstanceData),
                DbValueConvert.ToDbValue(wordExercise.GeneratedOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }
}
