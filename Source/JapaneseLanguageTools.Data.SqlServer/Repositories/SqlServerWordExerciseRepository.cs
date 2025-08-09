using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.DbNullHelpers;
using AndreyTalanin0x00.EntityFrameworkCore.Connections;
using AndreyTalanin0x00.EntityFrameworkCore.Transactions;
using AndreyTalanin0x00.YieldHelpers;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories;
using JapaneseLanguageTools.Data.SqlServer.Extensions;

using Microsoft.Data.SqlClient;

namespace JapaneseLanguageTools.Data.SqlServer.Repositories;

public class SqlServerWordExerciseRepository : WordExerciseRepository
{
    private readonly MainDbContext m_context;

    public SqlServerWordExerciseRepository(MainDbContext context, SqlServerWordExerciseRerunRepository sqlServerWordExerciseRerunRepository)
        : base(context, sqlServerWordExerciseRerunRepository)
    {
        m_context = context;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public override async Task<WordExercise> AddWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default)
    {
        wordExercise.Id = default(int);

        foreach (WordExerciseRerun wordExerciseRerun in wordExercise.WordExerciseReruns)
            wordExerciseRerun.WordExerciseId = wordExercise.Id;

        using DataTable wordExercisesDataTable = YieldEnumerableHelpers.Yield(wordExercise)
            .ToDataTable();

        using DataTable wordExerciseRerunsDataTable = wordExercise.WordExerciseReruns
            .ToDataTable();

        WordExerciseId wordExerciseId;

        using DbConnectionHandle connectionHandle =
            await m_context.Database.GetDbConnectionHandleAsync(cancellationToken);
        DbConnection connection = connectionHandle.Connection;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbTransactionHandle transactionHandle =
            await m_context.Database.GetDbTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
        DbTransaction transaction = transactionHandle.Transaction;
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        SqlParameter resultWordExerciseIdParameter;
        SqlParameter[] parameters = new SqlParameter[]
        {
            new("WordExercises", SqlDbType.Structured) { TypeName = "[dbo].[udt_WordExercise]", Value = wordExercisesDataTable, },
            new("WordExerciseReruns", SqlDbType.Structured) { TypeName = "[dbo].[udt_WordExerciseRerun]", Value = wordExerciseRerunsDataTable },
            resultWordExerciseIdParameter = new SqlParameter($"ResultWordExerciseId", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        string commandText = @$"
            DECLARE @WordExerciseIdMap AS TABLE
            (
                [Id] INT NOT NULL,
                [PreviousId] INT NOT NULL PRIMARY KEY
            );

            MERGE INTO [dbo].[WordExercise] AS [target]
            USING @{"WordExercises"} AS [source]
            ON [target].[Id] = [source].[Id]
            WHEN NOT MATCHED THEN INSERT
            (
                [WordId],
                [InstanceData],
                [GeneratedOn]
            )
            VALUES
            (
                [source].[WordId],
                [source].[InstanceData],
                [source].[GeneratedOn]
            )
            OUTPUT [inserted].[Id], [source].[Id]
            INTO @WordExerciseIdMap ([Id], [PreviousId]);

            DECLARE @WordExerciseIds AS [dbo].[udt_Int32UnorderedSet];

            INSERT INTO @WordExerciseIds ([Value])
            SELECT COALESCE([wordExerciseIdMap].[Id], [wordExercise].[Id])
            FROM @{"WordExercises"} AS [wordExercise]
            LEFT JOIN @WordExerciseIdMap AS [wordExerciseIdMap] ON [wordExerciseIdMap].[PreviousId] = [wordExercise].[Id];

            DECLARE @WordExerciseRerunsUpdated AS [dbo].[udt_WordExerciseRerun];

            INSERT INTO @WordExerciseRerunsUpdated
            (
                [Id],
                [WordExerciseId],
                [RequiredChallengeCount],
                [ContinuousChallengeCount],
                [TotalChallengeCount],
                [InitiallyScheduledOn],
                [RepeatedlyScheduledOn]
            )
            SELECT
                [wordExerciseRerun].[Id],
                COALESCE([wordExerciseIdMap].[Id], [wordExerciseRerun].[WordExerciseId]),
                [wordExerciseRerun].[RequiredChallengeCount],
                [wordExerciseRerun].[ContinuousChallengeCount],
                [wordExerciseRerun].[TotalChallengeCount],
                [wordExerciseRerun].[InitiallyScheduledOn],
                [wordExerciseRerun].[RepeatedlyScheduledOn]
            FROM @{"WordExerciseReruns"} AS [wordExerciseRerun]
            LEFT JOIN @WordExerciseIdMap AS [wordExerciseIdMap] ON [wordExerciseIdMap].[PreviousId] = [wordExerciseRerun].[WordExerciseId];

            MERGE INTO [dbo].[WordExerciseRerun] AS [target]
            USING @{"WordExerciseRerunsUpdated"} AS [source]
            ON [target].[Id] = [source].[Id]
            WHEN NOT MATCHED THEN INSERT
            (
                [WordExerciseId],
                [RequiredChallengeCount],
                [ContinuousChallengeCount],
                [TotalChallengeCount],
                [InitiallyScheduledOn],
                [RepeatedlyScheduledOn]
            )
            VALUES
            (
                [source].[WordExerciseId],
                [source].[RequiredChallengeCount],
                [source].[ContinuousChallengeCount],
                [source].[TotalChallengeCount],
                [source].[InitiallyScheduledOn],
                [source].[RepeatedlyScheduledOn]
            );

            SELECT TOP (1) @{"ResultWordExerciseId"} = [Value] FROM @WordExerciseIds;";

        CommandType commandType = CommandType.Text;

        SqlCommand command = new()
        {
            Connection = (SqlConnection)connection,
            Transaction = (SqlTransaction?)transaction,
            CommandText = commandText,
            CommandType = commandType,
        };

        foreach (SqlParameter parameter in parameters)
            command.Parameters.Add(parameter);

        await command.ExecuteNonQueryAsync(cancellationToken);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        int wordExerciseIdValue = DbValueConvert.ToValueType<int>(resultWordExerciseIdParameter.Value);

        wordExerciseId = new WordExerciseId(wordExerciseIdValue);

        wordExercise = await GetWordExerciseAsync(wordExerciseId, cancellationToken)
            ?? throw new UnreachableException("The word exercise with this Id has just been inserted.");

        return wordExercise;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public override async Task<bool> UpdateWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default)
    {
        using DataTable wordExercisesDataTable = YieldEnumerableHelpers.Yield(wordExercise)
            .ToDataTable();

        using DataTable wordExerciseRerunsDataTable = wordExercise.WordExerciseReruns
            .ToDataTable();

        using DbConnectionHandle connectionHandle =
            await m_context.Database.GetDbConnectionHandleAsync(cancellationToken);
        DbConnection connection = connectionHandle.Connection;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbTransactionHandle transactionHandle =
            await m_context.Database.GetDbTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
        DbTransaction transaction = transactionHandle.Transaction;
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        SqlParameter[] parameters = new SqlParameter[]
        {
            new("WordExercises", SqlDbType.Structured) { TypeName = "[dbo].[udt_WordExercise]", Value = wordExercisesDataTable, },
            new("WordExerciseReruns", SqlDbType.Structured) { TypeName = "[dbo].[udt_WordExerciseRerun]", Value = wordExerciseRerunsDataTable },
        };

        string commandText = @$"
            UPDATE [target]
            SET
                [target].[WordId] = [source].[WordId],
                [target].[InstanceData] = [source].[InstanceData],
                [target].[GeneratedOn] = [source].[GeneratedOn]
            FROM [dbo].[WordExercise] AS [target]
            INNER JOIN @{"WordExercises"} AS [source] ON [target].[Id] = [source].[Id];

            MERGE INTO [dbo].[WordExerciseRerun] AS [target]
            USING @{"WordExerciseReruns"} AS [source]
            ON [target].[Id] = [source].[Id]
            WHEN MATCHED THEN UPDATE
            SET
                [target].[RequiredChallengeCount] = [source].[RequiredChallengeCount],
                [target].[ContinuousChallengeCount] = [source].[ContinuousChallengeCount],
                [target].[TotalChallengeCount] = [source].[TotalChallengeCount],
                [target].[InitiallyScheduledOn] = [source].[InitiallyScheduledOn],
                [target].[RepeatedlyScheduledOn] = [source].[RepeatedlyScheduledOn]
            WHEN NOT MATCHED THEN INSERT
            (
                [WordExerciseId],
                [RequiredChallengeCount],
                [ContinuousChallengeCount],
                [TotalChallengeCount],
                [InitiallyScheduledOn],
                [RepeatedlyScheduledOn]
            )
            VALUES
            (
                [source].[WordExerciseId],
                [source].[RequiredChallengeCount],
                [source].[ContinuousChallengeCount],
                [source].[TotalChallengeCount],
                [source].[InitiallyScheduledOn],
                [source].[RepeatedlyScheduledOn]
            )
            WHEN NOT MATCHED BY SOURCE
                AND [target].[WordExerciseId] IN (SELECT [wordExercise].[Id] FROM @{"WordExercises"} AS [wordExercise])
                THEN DELETE;

            ;";

        CommandType commandType = CommandType.Text;

        SqlCommand command = new()
        {
            Connection = (SqlConnection)connection,
            Transaction = (SqlTransaction?)transaction,
            CommandText = commandText,
            CommandType = commandType,
        };

        foreach (SqlParameter parameter in parameters)
            command.Parameters.Add(parameter);

        await command.ExecuteNonQueryAsync(cancellationToken);

        if (transactionHandle.ContextTransactionOwnedByHandle)
            await transactionHandle.ContextTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
