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

public class SqlServerCharacterRepository : CharacterRepository
{
    private readonly MainDbContext m_context;

    public SqlServerCharacterRepository(MainDbContext context, TimeProvider timeProvider)
        : base(context, timeProvider)
    {
        m_context = context;
    }

    /// <inheritdoc />
    public override async Task<Character[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default)
    {
        return await GetCharactersAsync(characterIds, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public async Task<Character[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        characterIds = characterIds.Distinct();
        using DataTable characterIdsDataTable = characterIds
            .Select(catalogId => catalogId.Value)
            .ToDataTable();

        SqlParameter characterIdsParameter = new("CharacterIds", SqlDbType.Structured) { TypeName = "dbo.udt_Int32UnorderedSet", Value = characterIdsDataTable };

        string query = $"SELECT * FROM [dbo].[ufn_GetCharacters] (@{characterIdsParameter.ParameterName})";

        IQueryable<Character> charactersQueryable = m_context.Characters.FromSqlRaw(query, characterIdsParameter);

        charactersQueryable = charactersQueryable.Include(character => character.CharacterGroup);

        charactersQueryable = charactersQueryable.Include(character => character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);

        if (asNoTracking)
            charactersQueryable = charactersQueryable.AsNoTracking();

        charactersQueryable = charactersQueryable.AsSingleQuery();

        Character[] characters = await charactersQueryable.ToArrayAsync(cancellationToken);

        return characters;
    }

    /// <inheritdoc />
    public override async Task<Character> AddCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await AddCharacterAsync(character, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<Character> AddCharacterAsync(Character character, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        if (!saveChangesImmediately)
            throw new NotSupportedException($"This repository method can not be called with the {nameof(saveChangesImmediately)} parameter set to false.");

        character.Id = default(int);

        foreach (CharacterTag characterTag in character.CharacterTags)
            characterTag.CharacterId = character.Id;

        ICustomQueryParameter characterCustomQueryParameter = YieldEnumerableHelpers.Yield(character)
            .ToCustomQueryParameter();

        ICustomQueryParameter characterTagsCustomQueryParameter = character.CharacterTags
            .ToCustomQueryParameter();

        CharacterId characterId;

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
            Characters = characterCustomQueryParameter,
            CharacterTags = characterTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_AddCharacters";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        IEnumerable<int> characterIdValues = await connection.QueryAsync<int>(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        characterId = characterIdValues
            .Select(characterIdValue => new CharacterId(characterIdValue))
            .Single();

        character = await GetCharacterAsync(characterId, cancellationToken)
            ?? throw new UnreachableException("The character with this Id has just been inserted.");

        return character;
    }

    /// <inheritdoc />
    public override Task<bool> UpdateCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return UpdateCharacterAsync(character, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<bool> UpdateCharacterAsync(Character character, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        if (!saveChangesImmediately)
            throw new NotSupportedException($"This repository method can not be called with the {nameof(saveChangesImmediately)} parameter set to false.");

        ICustomQueryParameter characterCustomQueryParameter = YieldEnumerableHelpers.Yield(character)
            .ToCustomQueryParameter();

        ICustomQueryParameter characterTagsCustomQueryParameter = character.CharacterTags
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
            Characters = characterCustomQueryParameter,
            CharacterTags = characterTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_UpdateCharacters";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
