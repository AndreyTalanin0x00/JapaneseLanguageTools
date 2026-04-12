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

internal static class CharacterExerciseSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<CharacterExercise> characterExercises)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(CharacterExercise.Id), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExercise.CharacterId), typeof(int));
        dataTable.Columns.Add(nameof(CharacterExercise.InstanceData), typeof(string));
        dataTable.Columns.Add(nameof(CharacterExercise.GeneratedOn), typeof(DateTimeOffset));

        foreach (CharacterExercise characterExercise in characterExercises)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(characterExercise.Id),
                DbValueConvert.ToDbValue(characterExercise.CharacterId),
                DbValueConvert.ToDbValue(characterExercise.InstanceData),
                DbValueConvert.ToDbValue(characterExercise.GeneratedOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }
}
