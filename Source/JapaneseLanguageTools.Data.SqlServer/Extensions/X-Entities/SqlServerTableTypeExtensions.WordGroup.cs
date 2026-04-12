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

internal static class WordGroupSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<WordGroup> wordGroups)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(WordGroup.Id), typeof(int));
        dataTable.Columns.Add(nameof(WordGroup.Caption), typeof(string));
        dataTable.Columns.Add(nameof(WordGroup.Comment), typeof(string));
        dataTable.Columns.Add(nameof(WordGroup.Enabled), typeof(bool));
        dataTable.Columns.Add(nameof(WordGroup.AlwaysUse), typeof(bool));
        dataTable.Columns.Add(nameof(WordGroup.Hidden), typeof(bool));
        dataTable.Columns.Add(nameof(WordGroup.CreatedOn), typeof(DateTimeOffset));
        dataTable.Columns.Add(nameof(WordGroup.UpdatedOn), typeof(DateTimeOffset));

        foreach (WordGroup wordGroup in wordGroups)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(wordGroup.Id),
                DbValueConvert.ToDbValue(wordGroup.Caption),
                DbValueConvert.ToDbValue(wordGroup.Comment),
                DbValueConvert.ToDbValue(wordGroup.Enabled),
                DbValueConvert.ToDbValue(wordGroup.AlwaysUse),
                DbValueConvert.ToDbValue(wordGroup.Hidden),
                DbValueConvert.ToDbValue(wordGroup.CreatedOn),
                DbValueConvert.ToDbValue(wordGroup.UpdatedOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }
}
