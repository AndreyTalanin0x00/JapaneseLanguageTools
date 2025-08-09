using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.DbNullHelpers;
using AndreyTalanin0x00.EntityFrameworkCore.Transactions;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLanguageTools.Data.SqlServer.Repositories;

public class SqlServerTagRepository : TagRepository
{
    private readonly MainDbContext m_context;

    public SqlServerTagRepository(MainDbContext context, TimeProvider timeProvider)
        : base(context, timeProvider)
    {
        m_context = context;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public override async Task<Tag> AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        TagId tagId;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbContextTransactionHandle contextTransactionHandle =
            await m_context.Database.GetDbContextTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        SqlParameter resultTagIdParameter;
        SqlParameter[] parameters = new SqlParameter[]
        {
            new(nameof(Tag.Caption),SqlDbType.NVarChar, 256) { Value = tag.Caption },
            new(nameof(Tag.PlaceholderMarker),SqlDbType.NVarChar, 2048) { Value = tag.PlaceholderMarker },
            resultTagIdParameter = new SqlParameter($"Result{nameof(Tag.Id)}", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        string query = @$"
            DECLARE @InsertedTags AS TABLE
            (
                [Id] INT NOT NULL
            );

            INSERT INTO [dbo].[Tag] ([Caption], [PlaceholderMarker])
            OUTPUT [inserted].[Id] INTO @InsertedTags ([Id])
            VALUES (@{nameof(Tag.Caption)}, @{nameof(Tag.PlaceholderMarker)});

            SELECT TOP (1) @{resultTagIdParameter.ParameterName} = [insertedTag].[Id]
            FROM @InsertedTags AS [insertedTag];";

        await m_context.Database.ExecuteSqlRawAsync(query, parameters, cancellationToken);

        if (contextTransactionHandle.ContextTransactionOwnedByHandle)
            await contextTransactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        int tagIdValue = DbValueConvert.ToValueType<int>(resultTagIdParameter.Value);

        tagId = new TagId(tagIdValue);

        tag = await GetTagAsync(tagId, cancellationToken)
            ?? throw new UnreachableException("The tag with this Id has just been inserted.");

        return tag;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public override async Task<bool> UpdateTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbContextTransactionHandle contextTransactionHandle =
            await m_context.Database.GetDbContextTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        SqlParameter[] parameters = new SqlParameter[]
        {
            new(nameof(Tag.Id),SqlDbType.Int) { Value = tag.Id },
            new(nameof(Tag.Caption),SqlDbType.NVarChar, 256) { Value = tag.Caption },
            new(nameof(Tag.PlaceholderMarker),SqlDbType.NVarChar, 2048) { Value = tag.PlaceholderMarker },
        };

        string query = $@"
            UPDATE [dbo].[Tag]
            SET
                [Caption] = @{nameof(Tag.Caption)},
                [PlaceholderMarker] = @{nameof(Tag.PlaceholderMarker)}
            WHERE [Id] = @{nameof(Tag.Id)};";

        await m_context.Database.ExecuteSqlRawAsync(query, parameters, cancellationToken);

        if (contextTransactionHandle.ContextTransactionOwnedByHandle)
            await contextTransactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
