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

public class SqlServerWordRepository : WordRepository
{
    private readonly MainDbContext m_context;

    public SqlServerWordRepository(MainDbContext context, TimeProvider timeProvider)
        : base(context, timeProvider)
    {
        m_context = context;
    }

    /// <inheritdoc />
    public override async Task<Word[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default)
    {
        return await GetWordsAsync(wordIds, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public async Task<Word[]> GetWordsAsync(IEnumerable<WordId> wordIds, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        wordIds = wordIds.Distinct();
        using DataTable wordIdsDataTable = wordIds
            .Select(catalogId => catalogId.Value)
            .ToDataTable();

        SqlParameter wordIdsParameter = new("WordIds", SqlDbType.Structured) { TypeName = "dbo.udt_Int32UnorderedSet", Value = wordIdsDataTable };

        string query = $"SELECT * FROM [dbo].[ufn_GetWords] (@{wordIdsParameter.ParameterName})";

        IQueryable<Word> wordsQueryable = m_context.Words.FromSqlRaw(query, wordIdsParameter);

        wordsQueryable = wordsQueryable.Include(word => word.WordGroup);

        wordsQueryable = wordsQueryable.Include(word => word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);

        if (asNoTracking)
            wordsQueryable = wordsQueryable.AsNoTracking();

        wordsQueryable = wordsQueryable.AsSingleQuery();

        Word[] words = await wordsQueryable.ToArrayAsync(cancellationToken);

        return words;
    }

    /// <inheritdoc />
    public override async Task<Word> AddWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await AddWordAsync(word, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<Word> AddWordAsync(Word word, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        if (!saveChangesImmediately)
            throw new NotSupportedException($"This repository method can not be called with the {nameof(saveChangesImmediately)} parameter set to false.");

        word.Id = default(int);

        foreach (WordTag wordTag in word.WordTags)
            wordTag.WordId = word.Id;

        ICustomQueryParameter wordCustomQueryParameter = YieldEnumerableHelpers.Yield(word)
            .ToCustomQueryParameter();

        ICustomQueryParameter wordTagsCustomQueryParameter = word.WordTags
            .ToCustomQueryParameter();

        WordId wordId;

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
            Words = wordCustomQueryParameter,
            WordTags = wordTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_AddWords";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        IEnumerable<int> wordIdValues = await connection.QueryAsync<int>(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        wordId = wordIdValues
            .Select(wordIdValue => new WordId(wordIdValue))
            .Single();

        word = await GetWordAsync(wordId, cancellationToken)
            ?? throw new UnreachableException("The word with this Id has just been inserted.");

        return word;
    }

    /// <inheritdoc />
    public override Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return UpdateWordAsync(word, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<bool> UpdateWordAsync(Word word, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        if (!saveChangesImmediately)
            throw new NotSupportedException($"This repository method can not be called with the {nameof(saveChangesImmediately)} parameter set to false.");

        ICustomQueryParameter wordCustomQueryParameter = YieldEnumerableHelpers.Yield(word)
            .ToCustomQueryParameter();

        ICustomQueryParameter wordTagsCustomQueryParameter = word.WordTags
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
            Words = wordCustomQueryParameter,
            WordTags = wordTagsCustomQueryParameter,
        };

        DynamicParameters dynamicParameters = new(dynamicParametersObject);

        const string storedProcedure = "dbo.sp_UpdateWords";

        CommandDefinition commandDefinition = new(storedProcedure, dynamicParameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
