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

internal static class CharacterGroupSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<CharacterGroup> characterGroups)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(CharacterGroup.Id), typeof(int));
        dataTable.Columns.Add(nameof(CharacterGroup.Caption), typeof(string));
        dataTable.Columns.Add(nameof(CharacterGroup.Comment), typeof(string));
        dataTable.Columns.Add(nameof(CharacterGroup.Enabled), typeof(bool));
        dataTable.Columns.Add(nameof(CharacterGroup.AlwaysUse), typeof(bool));
        dataTable.Columns.Add(nameof(CharacterGroup.Hidden), typeof(bool));
        dataTable.Columns.Add(nameof(CharacterGroup.CreatedOn), typeof(DateTimeOffset));
        dataTable.Columns.Add(nameof(CharacterGroup.UpdatedOn), typeof(DateTimeOffset));

        foreach (CharacterGroup characterGroup in characterGroups)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(characterGroup.Id),
                DbValueConvert.ToDbValue(characterGroup.Caption),
                DbValueConvert.ToDbValue(characterGroup.Comment),
                DbValueConvert.ToDbValue(characterGroup.Enabled),
                DbValueConvert.ToDbValue(characterGroup.AlwaysUse),
                DbValueConvert.ToDbValue(characterGroup.Hidden),
                DbValueConvert.ToDbValue(characterGroup.CreatedOn),
                DbValueConvert.ToDbValue(characterGroup.UpdatedOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }
}
