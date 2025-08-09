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

internal static class CharacterSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<Character> characters)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(Character.Id), typeof(int));
        dataTable.Columns.Add(nameof(Character.CharacterGroupId), typeof(int));
        dataTable.Columns.Add(nameof(Character.Symbol), typeof(string));
        dataTable.Columns.Add(nameof(Character.Type), typeof(int));
        dataTable.Columns.Add(nameof(Character.Pronunciation), typeof(string));
        dataTable.Columns.Add(nameof(Character.Syllable), typeof(string));
        dataTable.Columns.Add(nameof(Character.Onyomi), typeof(string));
        dataTable.Columns.Add(nameof(Character.Kunyomi), typeof(string));
        dataTable.Columns.Add(nameof(Character.Meaning), typeof(string));
        dataTable.Columns.Add(nameof(Character.CreatedOn), typeof(DateTimeOffset));
        dataTable.Columns.Add(nameof(Character.UpdatedOn), typeof(DateTimeOffset));

        foreach (Character character in characters)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(character.Id),
                DbValueConvert.ToDbValue(character.CharacterGroupId),
                DbValueConvert.ToDbValue(character.Symbol),
                DbValueConvert.ToDbValue(character.Type),
                DbValueConvert.ToDbValue(character.Pronunciation),
                DbValueConvert.ToDbValue(character.Syllable),
                DbValueConvert.ToDbValue(character.Onyomi),
                DbValueConvert.ToDbValue(character.Kunyomi),
                DbValueConvert.ToDbValue(character.Meaning),
                DbValueConvert.ToDbValue(character.CreatedOn),
                DbValueConvert.ToDbValue(character.UpdatedOn),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    public static ICustomQueryParameter ToCustomQueryParameter(this IEnumerable<Character> characters)
    {
        CharacterSqlRecordCollection characterRecordCollection = new(characters);

        ICustomQueryParameter customQueryParameter = ((IEnumerable<SqlDataRecord>)characterRecordCollection)
            .AsTableValuedParameter(CharacterSqlRecordCollection.TableTypeName);

        return customQueryParameter;
    }

    private class CharacterSqlRecordCollection : Collection<Character>, IEnumerable<SqlDataRecord>
    {
        public const string TableTypeName = "dbo.udt_Character";

        public CharacterSqlRecordCollection(IEnumerable<Character> charactersEnumerable)
            : base(charactersEnumerable is not IList<Character> charactersList ? charactersEnumerable.ToArray() : charactersList)
        {
        }

        #region IEnumerable<SqlDataRecord> Explicit Interface Implementation

        [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlMetaData[] columnMetadataArray = new SqlMetaData[]
            {
                new(nameof(Character.Id), SqlDbType.Int),
                new(nameof(Character.CharacterGroupId), SqlDbType.Int),
                new(nameof(Character.Symbol), SqlDbType.NVarChar, 16),
                new(nameof(Character.Type), SqlDbType.Int),
                new(nameof(Character.Pronunciation), SqlDbType.NVarChar, 32),
                new(nameof(Character.Syllable), SqlDbType.NVarChar, 32),
                new(nameof(Character.Onyomi), SqlDbType.NVarChar, 256),
                new(nameof(Character.Kunyomi), SqlDbType.NVarChar, 256),
                new(nameof(Character.Meaning), SqlDbType.NVarChar, 512),
                new(nameof(Character.CreatedOn), SqlDbType.DateTimeOffset),
                new(nameof(Character.UpdatedOn), SqlDbType.DateTimeOffset),
            };

            SqlDataRecord record = new(columnMetadataArray);

            foreach (Character character in this)
            {
                record.SetInt32(record.GetOrdinal(nameof(Character.Id)), character.Id);
                record.SetValue(record.GetOrdinal(nameof(Character.CharacterGroupId)), DbValueConvert.ToDbValue(character.CharacterGroupId));
                record.SetString(record.GetOrdinal(nameof(Character.Symbol)), character.Symbol);
                record.SetInt32(record.GetOrdinal(nameof(Character.Type)), (int)character.Type);
                record.SetValue(record.GetOrdinal(nameof(Character.Pronunciation)), DbValueConvert.ToDbValue(character.Pronunciation));
                record.SetValue(record.GetOrdinal(nameof(Character.Syllable)), DbValueConvert.ToDbValue(character.Syllable));
                record.SetValue(record.GetOrdinal(nameof(Character.Onyomi)), DbValueConvert.ToDbValue(character.Onyomi));
                record.SetValue(record.GetOrdinal(nameof(Character.Kunyomi)), DbValueConvert.ToDbValue(character.Kunyomi));
                record.SetValue(record.GetOrdinal(nameof(Character.Meaning)), DbValueConvert.ToDbValue(character.Meaning));
                record.SetDateTimeOffset(record.GetOrdinal(nameof(Character.CreatedOn)), character.CreatedOn);
                record.SetDateTimeOffset(record.GetOrdinal(nameof(Character.UpdatedOn)), character.UpdatedOn);

                yield return record;
            }
        }

        #endregion
    }
}
