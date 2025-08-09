using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.EntityFrameworkCore.Connections;
using AndreyTalanin0x00.EntityFrameworkCore.Transactions;
using AndreyTalanin0x00.YieldHelpers;

using Dapper;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories;
using JapaneseLanguageTools.Data.SqlServer.Extensions;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using static Dapper.SqlMapper;

namespace JapaneseLanguageTools.Data.SqlServer.Repositories;

public class SqlServerCharacterGroupRepository : CharacterGroupRepository
{
    private readonly MainDbContext m_context;

    public SqlServerCharacterGroupRepository(MainDbContext context, SqlServerCharacterRepository sqlServerCharacterRepository, TimeProvider timeProvider)
        : base(context, sqlServerCharacterRepository, timeProvider)
    {
        m_context = context;
    }

    /// <inheritdoc />
    public override async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default)
    {
        return await GetCharacterGroupsAsync(characterGroupIds, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        characterGroupIds = characterGroupIds.Distinct();
        using DataTable characterGroupIdsDataTable = characterGroupIds
            .Select(characterGroupId => characterGroupId.Value)
            .ToDataTable();

        SqlParameter characterGroupIdsParameter = new("CharacterGroupIds", SqlDbType.Structured) { TypeName = "dbo.udt_Int32UnorderedSet", Value = characterGroupIdsDataTable };

        string query = $"SELECT * FROM [dbo].[ufn_GetCharacterGroups] (@{characterGroupIdsParameter.ParameterName})";

        IQueryable<CharacterGroup> characterGroupsQueryable = m_context.CharacterGroups.FromSqlRaw(query, characterGroupIdsParameter);

        characterGroupsQueryable = characterGroupsQueryable.Include(characterGroup => characterGroup.Characters)
            .ThenInclude(character => character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);

        if (asNoTracking)
            characterGroupsQueryable = characterGroupsQueryable.AsNoTracking();

        characterGroupsQueryable = characterGroupsQueryable.AsSingleQuery();

        CharacterGroup[] characterGroups = await characterGroupsQueryable.ToArrayAsync(cancellationToken);

        return characterGroups;
    }

    /// <inheritdoc />
    public override async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default)
    {
        return await GetCharacterGroupsAsync(characterGroupCaptions, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        characterGroupCaptions = characterGroupCaptions.Distinct();
        using DataTable characterGroupCaptionsDataTable = characterGroupCaptions
            .Select(characterGroupCaption => characterGroupCaption.Value)
            .ToDataTable();

        SqlParameter characterGroupCaptionsParameter = new("CharacterGroupCaptions", SqlDbType.Structured) { TypeName = "dbo.udt_String256UnorderedSet", Value = characterGroupCaptionsDataTable };

        string query = $"SELECT * FROM [dbo].[ufn_GetCharacterGroupsByCaption] (@{characterGroupCaptionsParameter.ParameterName})";

        IQueryable<CharacterGroup> characterGroupsQueryable = m_context.CharacterGroups.FromSqlRaw(query, characterGroupCaptionsParameter);

        characterGroupsQueryable = characterGroupsQueryable.Include(characterGroup => characterGroup.Characters)
            .ThenInclude(character => character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);

        if (asNoTracking)
            characterGroupsQueryable = characterGroupsQueryable.AsNoTracking();

        characterGroupsQueryable = characterGroupsQueryable.AsSingleQuery();

        CharacterGroup[] characterGroups = await characterGroupsQueryable.ToArrayAsync(cancellationToken);

        return characterGroups;
    }

    /// <inheritdoc />
    public override async Task<CharacterGroup> AddCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        characterGroup.Id = default(int);

        for (int characterIndex = 0; characterIndex < characterGroup.Characters.Count; characterIndex++)
        {
            Character character = characterGroup.Characters[characterIndex];

            character.Id = character.Id == default(int)
                ? -1 * characterIndex
                : character.Id;

            character.CharacterGroupId = characterGroup.Id;

            foreach (CharacterTag characterTag in character.CharacterTags)
                characterTag.CharacterId = character.Id;

            ;
        }

        using DataTable characterGroupsDataTable = YieldEnumerableHelpers.Yield(characterGroup)
            .ToDataTable();

        ICustomQueryParameter characterCustomQueryParameter = characterGroup.Characters
            .ToCustomQueryParameter();

        ICustomQueryParameter characterTagsCustomQueryParameter = characterGroup.Characters
            .SelectMany(character => character.CharacterTags)
            .ToCustomQueryParameter();

        CharacterGroupId characterGroupId;

        using DbConnectionHandle connectionHandle =
            await m_context.Database.GetDbConnectionHandleAsync(cancellationToken);
        DbConnection connection = connectionHandle.Connection;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbTransactionHandle transactionHandle =
            await m_context.Database.GetDbTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
        DbTransaction transaction = transactionHandle.Transaction;
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        object dynamicParametersObject = new
        {
            CharacterGroups = characterGroupsDataTable,
            Characters = characterCustomQueryParameter,
            CharacterTags = characterTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_AddCharacterGroups";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        IEnumerable<int> characterGroupIdValues = await connection.QueryAsync<int>(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        characterGroupId = characterGroupIdValues
            .Select(characterGroupIdValue => new CharacterGroupId(characterGroupIdValue))
            .Single();

        characterGroup = await GetCharacterGroupAsync(characterGroupId, cancellationToken)
            ?? throw new UnreachableException("The character with this Id has just been inserted.");

        return characterGroup;
    }

    /// <inheritdoc />
    public override async Task<bool> UpdateCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        for (int characterIndex = 0; characterIndex < characterGroup.Characters.Count; characterIndex++)
        {
            Character character = characterGroup.Characters[characterIndex];

            character.Id = character.Id == default(int)
                ? -1 * characterIndex
                : character.Id;

            foreach (CharacterTag characterTag in character.CharacterTags)
                characterTag.CharacterId = character.Id;

            ;
        }

        using DataTable characterGroupsDataTable = YieldEnumerableHelpers.Yield(characterGroup)
            .ToDataTable();

        ICustomQueryParameter characterCustomQueryParameter = characterGroup.Characters
            .ToCustomQueryParameter();

        ICustomQueryParameter characterTagsCustomQueryParameter = characterGroup.Characters
            .SelectMany(character => character.CharacterTags)
            .ToCustomQueryParameter();

        using DbConnectionHandle connectionHandle =
            await m_context.Database.GetDbConnectionHandleAsync(cancellationToken);
        DbConnection connection = connectionHandle.Connection;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbTransactionHandle transactionHandle =
            await m_context.Database.GetDbTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
        DbTransaction transaction = transactionHandle.Transaction;
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        object dynamicParametersObject = new
        {
            CharacterGroups = characterGroupsDataTable,
            Characters = characterCustomQueryParameter,
            CharacterTags = characterTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_UpdateCharacterGroups";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
