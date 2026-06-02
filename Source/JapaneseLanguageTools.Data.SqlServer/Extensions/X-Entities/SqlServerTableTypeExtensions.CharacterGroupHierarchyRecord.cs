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

internal static class CharacterGroupHierarchyRecordSqlServerTableTypeExtensions
{
    public static ICustomQueryParameter ToCustomQueryParameter(this IEnumerable<CharacterGroupHierarchyRecord> characterGroupHierarchyRecords)
    {
        CharacterGroupHierarchyRecordSqlRecordCollection characterGroupHierarchyRecordRecordCollection = new(characterGroupHierarchyRecords);

        ICustomQueryParameter customQueryParameter = ((IEnumerable<SqlDataRecord>)characterGroupHierarchyRecordRecordCollection)
            .AsTableValuedParameter(CharacterGroupHierarchyRecordSqlRecordCollection.TableTypeName);

        return customQueryParameter;
    }

    private class CharacterGroupHierarchyRecordSqlRecordCollection : Collection<CharacterGroupHierarchyRecord>, IEnumerable<SqlDataRecord>
    {
        public const string TableTypeName = "dbo.udt_CharacterGroupHierarchyRecord";

        public CharacterGroupHierarchyRecordSqlRecordCollection(IEnumerable<CharacterGroupHierarchyRecord> characterGroupHierarchyRecordsEnumerable)
            : base(characterGroupHierarchyRecordsEnumerable is not IList<CharacterGroupHierarchyRecord> characterGroupHierarchyRecordsList ? characterGroupHierarchyRecordsEnumerable.ToArray() : characterGroupHierarchyRecordsList)
        {
        }

        #region IEnumerable<SqlDataRecord> Explicit Interface Implementation

        [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlMetaData[] columnMetadataArray = new SqlMetaData[]
            {
                new(nameof(CharacterGroupHierarchyRecord.CharacterGroupId), SqlDbType.Int),
                new(nameof(CharacterGroupHierarchyRecord.NestedCharacterGroupId), SqlDbType.Int),
                new(nameof(CharacterGroupHierarchyRecord.PreventRecursiveIncludes), SqlDbType.Bit),
            };

            SqlDataRecord record = new(columnMetadataArray);

            foreach (CharacterGroupHierarchyRecord characterGroupHierarchyRecord in this)
            {
                record.SetValue(record.GetOrdinal(nameof(CharacterGroupHierarchyRecord.CharacterGroupId)), DbValueConvert.ToDbValue(characterGroupHierarchyRecord.CharacterGroupId));
                record.SetValue(record.GetOrdinal(nameof(CharacterGroupHierarchyRecord.NestedCharacterGroupId)), DbValueConvert.ToDbValue(characterGroupHierarchyRecord.NestedCharacterGroupId));
                record.SetValue(record.GetOrdinal(nameof(CharacterGroupHierarchyRecord.PreventRecursiveIncludes)), DbValueConvert.ToDbValue(characterGroupHierarchyRecord.PreventRecursiveIncludes));

                yield return record;
            }
        }

        #endregion
    }
}
