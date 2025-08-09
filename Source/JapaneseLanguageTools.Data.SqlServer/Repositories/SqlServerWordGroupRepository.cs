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

public class SqlServerWordGroupRepository : WordGroupRepository
{
    private readonly MainDbContext m_context;

    public SqlServerWordGroupRepository(MainDbContext context, SqlServerWordRepository sqlServerWordRepository, TimeProvider timeProvider)
        : base(context, sqlServerWordRepository, timeProvider)
    {
        m_context = context;
    }

    /// <inheritdoc />
    public override async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default)
    {
        return await GetWordGroupsAsync(wordGroupIds, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        wordGroupIds = wordGroupIds.Distinct();
        using DataTable wordGroupIdsDataTable = wordGroupIds
            .Select(wordGroupId => wordGroupId.Value)
            .ToDataTable();

        SqlParameter wordGroupIdsParameter = new("WordGroupIds", SqlDbType.Structured) { TypeName = "dbo.udt_Int32UnorderedSet", Value = wordGroupIdsDataTable };

        string query = $"SELECT * FROM [dbo].[ufn_GetWordGroups] (@{wordGroupIdsParameter.ParameterName})";

        IQueryable<WordGroup> wordGroupsQueryable = m_context.WordGroups.FromSqlRaw(query, wordGroupIdsParameter);

        wordGroupsQueryable = wordGroupsQueryable.Include(wordGroup => wordGroup.Words)
            .ThenInclude(word => word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);

        if (asNoTracking)
            wordGroupsQueryable = wordGroupsQueryable.AsNoTracking();

        wordGroupsQueryable = wordGroupsQueryable.AsSingleQuery();

        WordGroup[] wordGroups = await wordGroupsQueryable.ToArrayAsync(cancellationToken);

        return wordGroups;
    }

    /// <inheritdoc />
    public override async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default)
    {
        return await GetWordGroupsAsync(wordGroupCaptions, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        wordGroupCaptions = wordGroupCaptions.Distinct();
        using DataTable wordGroupCaptionsDataTable = wordGroupCaptions
            .Select(wordGroupCaption => wordGroupCaption.Value)
            .ToDataTable();

        SqlParameter wordGroupCaptionsParameter = new("WordGroupCaptions", SqlDbType.Structured) { TypeName = "dbo.udt_String256UnorderedSet", Value = wordGroupCaptionsDataTable };

        string query = $"SELECT * FROM [dbo].[ufn_GetWordGroupsByCaption] (@{wordGroupCaptionsParameter.ParameterName})";

        IQueryable<WordGroup> wordGroupsQueryable = m_context.WordGroups.FromSqlRaw(query, wordGroupCaptionsParameter);

        wordGroupsQueryable = wordGroupsQueryable.Include(wordGroup => wordGroup.Words)
            .ThenInclude(word => word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);

        if (asNoTracking)
            wordGroupsQueryable = wordGroupsQueryable.AsNoTracking();

        wordGroupsQueryable = wordGroupsQueryable.AsSingleQuery();

        WordGroup[] wordGroups = await wordGroupsQueryable.ToArrayAsync(cancellationToken);

        return wordGroups;
    }

    /// <inheritdoc />
    public override async Task<WordGroup> AddWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        wordGroup.Id = default(int);

        for (int wordIndex = 0; wordIndex < wordGroup.Words.Count; wordIndex++)
        {
            Word word = wordGroup.Words[wordIndex];

            word.Id = word.Id == default(int)
                ? -1 * wordIndex
                : word.Id;

            word.WordGroupId = wordGroup.Id;

            foreach (WordTag wordTag in word.WordTags)
                wordTag.WordId = word.Id;

            ;
        }

        using DataTable wordGroupsDataTable = YieldEnumerableHelpers.Yield(wordGroup)
            .ToDataTable();

        ICustomQueryParameter wordCustomQueryParameter = wordGroup.Words
            .ToCustomQueryParameter();

        ICustomQueryParameter wordTagsCustomQueryParameter = wordGroup.Words
            .SelectMany(word => word.WordTags)
            .ToCustomQueryParameter();

        WordGroupId wordGroupId;

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
            WordGroups = wordGroupsDataTable,
            Words = wordCustomQueryParameter,
            WordTags = wordTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_AddWordGroups";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        IEnumerable<int> wordGroupIdValues = await connection.QueryAsync<int>(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        wordGroupId = wordGroupIdValues
            .Select(wordGroupIdValue => new WordGroupId(wordGroupIdValue))
            .Single();

        wordGroup = await GetWordGroupAsync(wordGroupId, cancellationToken)
            ?? throw new UnreachableException("The word with this Id has just been inserted.");

        return wordGroup;
    }

    /// <inheritdoc />
    public override async Task<bool> UpdateWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        for (int wordIndex = 0; wordIndex < wordGroup.Words.Count; wordIndex++)
        {
            Word word = wordGroup.Words[wordIndex];

            word.Id = word.Id == default(int)
                ? -1 * wordIndex
                : word.Id;

            foreach (WordTag wordTag in word.WordTags)
                wordTag.WordId = word.Id;

            ;
        }

        using DataTable wordGroupsDataTable = YieldEnumerableHelpers.Yield(wordGroup)
            .ToDataTable();

        ICustomQueryParameter wordCustomQueryParameter = wordGroup.Words
            .ToCustomQueryParameter();

        ICustomQueryParameter wordTagsCustomQueryParameter = wordGroup.Words
            .SelectMany(word => word.WordTags)
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
            WordGroups = wordGroupsDataTable,
            Words = wordCustomQueryParameter,
            WordTags = wordTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_UpdateWordGroups";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
