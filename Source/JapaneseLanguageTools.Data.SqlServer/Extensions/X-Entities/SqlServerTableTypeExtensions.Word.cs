using System;
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

internal static class WordSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<Word> words)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(Word.Id), typeof(int));
        dataTable.Columns.Add(nameof(Word.WordGroupId), typeof(int));
        dataTable.Columns.Add(nameof(Word.Characters), typeof(string));
        dataTable.Columns.Add(nameof(Word.CharacterTypes), typeof(int));
        dataTable.Columns.Add(nameof(Word.Pronunciation), typeof(string));
        dataTable.Columns.Add(nameof(Word.Furigana), typeof(string));
        dataTable.Columns.Add(nameof(Word.Okurigana), typeof(string));
        dataTable.Columns.Add(nameof(Word.Meaning), typeof(string));
        dataTable.Columns.Add(nameof(Word.CreatedOn), typeof(DateTimeOffset));
        dataTable.Columns.Add(nameof(Word.UpdatedOn), typeof(DateTimeOffset));

        foreach (Word word in words)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(word.Id),
                DbValueConvert.ToDbValue(word.WordGroupId),
                DbValueConvert.ToDbValue(word.Characters),
                DbValueConvert.ToDbValue(word.CharacterTypes),
                DbValueConvert.ToDbValue(word.Pronunciation),
                DbValueConvert.ToDbValue(word.Furigana),
                DbValueConvert.ToDbValue(word.Okurigana),
                DbValueConvert.ToDbValue(word.Meaning),
                DbValueConvert.ToDbValue(word.CreatedOn),
                DbValueConvert.ToDbValue(word.UpdatedOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    public static ICustomQueryParameter ToCustomQueryParameter(this IEnumerable<Word> words)
    {
        WordSqlRecordCollection wordRecordCollection = new(words);

        ICustomQueryParameter customQueryParameter = ((IEnumerable<SqlDataRecord>)wordRecordCollection)
            .AsTableValuedParameter(WordSqlRecordCollection.TableTypeName);

        return customQueryParameter;
    }

    private class WordSqlRecordCollection : Collection<Word>, IEnumerable<SqlDataRecord>
    {
        public const string TableTypeName = "dbo.udt_Word";

        public WordSqlRecordCollection(IEnumerable<Word> wordsEnumerable)
            : base(wordsEnumerable is not IList<Word> wordsList ? wordsEnumerable.ToArray() : wordsList)
        {
        }

        #region IEnumerable<SqlDataRecord> Explicit Interface Implementation

        [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlMetaData[] columnMetadataArray = new SqlMetaData[]
            {
                new(nameof(Word.Id), SqlDbType.Int),
                new(nameof(Word.WordGroupId), SqlDbType.Int),
                new(nameof(Word.Characters), SqlDbType.NVarChar, 256),
                new(nameof(Word.CharacterTypes), SqlDbType.Int),
                new(nameof(Word.Pronunciation), SqlDbType.NVarChar, 512),
                new(nameof(Word.Furigana), SqlDbType.NVarChar, 512),
                new(nameof(Word.Okurigana), SqlDbType.NVarChar, 512),
                new(nameof(Word.Meaning), SqlDbType.NVarChar, 512),
                new(nameof(Word.CreatedOn), SqlDbType.DateTimeOffset),
                new(nameof(Word.UpdatedOn), SqlDbType.DateTimeOffset),
            };

            SqlDataRecord record = new(columnMetadataArray);

            foreach (Word word in this)
            {
                record.SetInt32(record.GetOrdinal(nameof(Word.Id)), word.Id);
                record.SetValue(record.GetOrdinal(nameof(Word.WordGroupId)), DbValueConvert.ToDbValue(word.WordGroupId));
                record.SetString(record.GetOrdinal(nameof(Word.Characters)), word.Characters);
                record.SetInt32(record.GetOrdinal(nameof(Word.CharacterTypes)), (int)word.CharacterTypes);
                record.SetValue(record.GetOrdinal(nameof(Word.Pronunciation)), DbValueConvert.ToDbValue(word.Pronunciation));
                record.SetValue(record.GetOrdinal(nameof(Word.Furigana)), DbValueConvert.ToDbValue(word.Furigana));
                record.SetValue(record.GetOrdinal(nameof(Word.Okurigana)), DbValueConvert.ToDbValue(word.Okurigana));
                record.SetValue(record.GetOrdinal(nameof(Word.Meaning)), DbValueConvert.ToDbValue(word.Meaning));
                record.SetDateTimeOffset(record.GetOrdinal(nameof(Word.CreatedOn)), word.CreatedOn);
                record.SetDateTimeOffset(record.GetOrdinal(nameof(Word.UpdatedOn)), word.UpdatedOn);

                yield return record;
            }
        }

        #endregion
    }
}
