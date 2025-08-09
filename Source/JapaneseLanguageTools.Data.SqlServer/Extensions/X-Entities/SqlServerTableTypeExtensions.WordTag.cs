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

internal static class WordTagSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<WordTag> wordTags)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(WordTag.WordId), typeof(int));
        dataTable.Columns.Add(nameof(WordTag.TagId), typeof(int));

        foreach (WordTag wordTag in wordTags)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(wordTag.WordId),
                DbValueConvert.ToDbValue(wordTag.TagId),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    public static ICustomQueryParameter ToCustomQueryParameter(this IEnumerable<WordTag> wordTags)
    {
        WordTagSqlRecordCollection wordTagRecordCollection = new(wordTags);

        ICustomQueryParameter customQueryParameter = ((IEnumerable<SqlDataRecord>)wordTagRecordCollection)
            .AsTableValuedParameter(WordTagSqlRecordCollection.TableTypeName);

        return customQueryParameter;
    }

    private class WordTagSqlRecordCollection : Collection<WordTag>, IEnumerable<SqlDataRecord>
    {
        public const string TableTypeName = "dbo.udt_WordTag";

        public WordTagSqlRecordCollection(IEnumerable<WordTag> wordTagsEnumerable)
            : base(wordTagsEnumerable is not IList<WordTag> wordTagsList ? wordTagsEnumerable.ToArray() : wordTagsList)
        {
        }

        #region IEnumerable<SqlDataRecord> Explicit Interface Implementation

        [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlMetaData[] columnMetadataArray = new SqlMetaData[]
            {
                new(nameof(WordTag.WordId), SqlDbType.Int),
                new(nameof(WordTag.TagId), SqlDbType.Int),
            };

            SqlDataRecord record = new(columnMetadataArray);

            foreach (WordTag wordTag in this)
            {
                record.SetInt32(record.GetOrdinal(nameof(WordTag.WordId)), wordTag.WordId);
                record.SetInt32(record.GetOrdinal(nameof(WordTag.TagId)), wordTag.TagId);

                yield return record;
            }
        }

        #endregion
    }
}
