using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using AndreyTalanin0x00.DbNullHelpers;

using Dapper;

using JapaneseLanguageTools.Data.Entities;

using Microsoft.Data.SqlClient.Server;

using static Dapper.SqlMapper;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Data.SqlServer.Extensions;

internal static class WordGroupHierarchyRecordSqlServerTableTypeExtensions
{
    public static ICustomQueryParameter ToCustomQueryParameter(this IEnumerable<WordGroupHierarchyRecord> wordGroupHierarchyRecords)
    {
        WordGroupHierarchyRecordSqlRecordCollection wordGroupHierarchyRecordRecordCollection = new(wordGroupHierarchyRecords);

        ICustomQueryParameter customQueryParameter = ((IEnumerable<SqlDataRecord>)wordGroupHierarchyRecordRecordCollection)
            .AsTableValuedParameter(WordGroupHierarchyRecordSqlRecordCollection.TableTypeName);

        return customQueryParameter;
    }

    private class WordGroupHierarchyRecordSqlRecordCollection : Collection<WordGroupHierarchyRecord>, IEnumerable<SqlDataRecord>
    {
        public const string TableTypeName = "dbo.udt_WordGroupHierarchyRecord";

        public WordGroupHierarchyRecordSqlRecordCollection(IEnumerable<WordGroupHierarchyRecord> wordGroupHierarchyRecordsEnumerable)
            : base(wordGroupHierarchyRecordsEnumerable is not IList<WordGroupHierarchyRecord> wordGroupHierarchyRecordsList ? wordGroupHierarchyRecordsEnumerable.ToArray() : wordGroupHierarchyRecordsList)
        {
        }

        #region IEnumerable<SqlDataRecord> Explicit Interface Implementation

        [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlMetaData[] columnMetadataArray = new SqlMetaData[]
            {
                new(nameof(WordGroupHierarchyRecord.WordGroupId), SqlDbType.Int),
                new(nameof(WordGroupHierarchyRecord.NestedWordGroupId), SqlDbType.Int),
                new(nameof(WordGroupHierarchyRecord.PreventRecursiveIncludes), SqlDbType.Bit),
            };

            SqlDataRecord record = new(columnMetadataArray);

            foreach (WordGroupHierarchyRecord wordGroupHierarchyRecord in this)
            {
                record.SetValue(record.GetOrdinal(nameof(WordGroupHierarchyRecord.WordGroupId)), DbValueConvert.ToDbValue(wordGroupHierarchyRecord.WordGroupId));
                record.SetValue(record.GetOrdinal(nameof(WordGroupHierarchyRecord.NestedWordGroupId)), DbValueConvert.ToDbValue(wordGroupHierarchyRecord.NestedWordGroupId));
                record.SetValue(record.GetOrdinal(nameof(WordGroupHierarchyRecord.PreventRecursiveIncludes)), DbValueConvert.ToDbValue(wordGroupHierarchyRecord.PreventRecursiveIncludes));

                yield return record;
            }
        }

        #endregion
    }
}
