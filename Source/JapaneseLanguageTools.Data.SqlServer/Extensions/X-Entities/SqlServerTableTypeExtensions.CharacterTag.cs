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

internal static class CharacterTagSqlServerTableTypeExtensions
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public static DataTable ToDataTable(this IEnumerable<CharacterTag> characterTags)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add(nameof(CharacterTag.CharacterId), typeof(int));
        dataTable.Columns.Add(nameof(CharacterTag.TagId), typeof(int));

        foreach (CharacterTag characterTag in characterTags)
        {
            object[] rowValues = new object[]
            {
                DbValueConvert.ToDbValue(characterTag.CharacterId),
                DbValueConvert.ToDbValue(characterTag.TagId),
            };

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    public static ICustomQueryParameter ToCustomQueryParameter(this IEnumerable<CharacterTag> characterTags)
    {
        CharacterTagSqlRecordCollection characterTagRecordCollection = new(characterTags);

        ICustomQueryParameter customQueryParameter = ((IEnumerable<SqlDataRecord>)characterTagRecordCollection)
            .AsTableValuedParameter(CharacterTagSqlRecordCollection.TableTypeName);

        return customQueryParameter;
    }

    private class CharacterTagSqlRecordCollection : Collection<CharacterTag>, IEnumerable<SqlDataRecord>
    {
        public const string TableTypeName = "dbo.udt_CharacterTag";

        public CharacterTagSqlRecordCollection(IEnumerable<CharacterTag> characterTagsEnumerable)
            : base(characterTagsEnumerable is not IList<CharacterTag> characterTagsList ? characterTagsEnumerable.ToArray() : characterTagsList)
        {
        }

        #region IEnumerable<SqlDataRecord> Explicit Interface Implementation

        [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlMetaData[] columnMetadataArray = new SqlMetaData[]
            {
                new(nameof(CharacterTag.CharacterId), SqlDbType.Int),
                new(nameof(CharacterTag.TagId), SqlDbType.Int),
            };

            SqlDataRecord record = new(columnMetadataArray);

            foreach (CharacterTag characterTag in this)
            {
                record.SetInt32(record.GetOrdinal(nameof(CharacterTag.CharacterId)), characterTag.CharacterId);
                record.SetInt32(record.GetOrdinal(nameof(CharacterTag.TagId)), characterTag.TagId);

                yield return record;
            }
        }

        #endregion
    }
}
