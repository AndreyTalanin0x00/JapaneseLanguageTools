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

public class SqlServerCharacterExerciseRepository : CharacterExerciseRepository
{
    private readonly MainDbContext m_context;

    public SqlServerCharacterExerciseRepository(MainDbContext context, SqlServerCharacterExerciseRerunRepository sqlServerCharacterExerciseRerunRepository)
        : base(context, sqlServerCharacterExerciseRerunRepository)
    {
        m_context = context;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public override async Task<CharacterExercise> AddCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default)
    {
        characterExercise.Id = default(int);

        foreach (CharacterExerciseRerun characterExerciseRerun in characterExercise.CharacterExerciseReruns)
            characterExerciseRerun.CharacterExerciseId = characterExercise.Id;

        using DataTable characterExercisesDataTable = YieldEnumerableHelpers.Yield(characterExercise)
            .ToDataTable();

        using DataTable characterExerciseRerunsDataTable = characterExercise.CharacterExerciseReruns
            .ToDataTable();

        CharacterExerciseId characterExerciseId;

        using DbConnectionHandle connectionHandle =
            await m_context.Database.GetDbConnectionHandleAsync(cancellationToken);
        DbConnection connection = connectionHandle.Connection;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        using DbTransactionHandle transactionHandle =
            await m_context.Database.GetDbTransactionHandleAsync(async (cancellationToken) => await m_context.Database.BeginTransactionAsync(cancellationToken), cancellationToken);
        DbTransaction transaction = transactionHandle.Transaction;
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

        SqlParameter resultCharacterExerciseIdParameter;
        SqlParameter[] parameters = new SqlParameter[]
        {
            new("CharacterExercises", SqlDbType.Structured) { TypeName = "dbo.udt_CharacterExercise", Value = characterExercisesDataTable, },
            new("CharacterExerciseReruns", SqlDbType.Structured) { TypeName = "dbo.udt_CharacterExerciseRerun", Value = characterExerciseRerunsDataTable },
            resultCharacterExerciseIdParameter = new SqlParameter($"ResultCharacterExerciseId", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        string commandText = @$"
            DECLARE @CharacterExerciseIdMap AS TABLE
            (
                [Id] INT NOT NULL,
                [PreviousId] INT NOT NULL PRIMARY KEY
            );

            MERGE INTO [dbo].[CharacterExercise] AS [target]
            USING @{"CharacterExercises"} AS [source]
            ON [target].[Id] = [source].[Id]
            WHEN NOT MATCHED THEN INSERT
            (
                [CharacterId],
                [InstanceData],
                [GeneratedOn]
            )
            VALUES
            (
                [source].[CharacterId],
                [source].[InstanceData],
                [source].[GeneratedOn]
            )
            OUTPUT [inserted].[Id], [source].[Id]
            INTO @CharacterExerciseIdMap ([Id], [PreviousId]);

            DECLARE @CharacterExerciseIds AS [dbo].[udt_Int32UnorderedSet];

            INSERT INTO @CharacterExerciseIds ([Value])
            SELECT COALESCE([characterExerciseIdMap].[Id], [characterExercise].[Id])
            FROM @{"CharacterExercises"} AS [characterExercise]
            LEFT JOIN @CharacterExerciseIdMap AS [characterExerciseIdMap] ON [characterExerciseIdMap].[PreviousId] = [characterExercise].[Id];

            DECLARE @CharacterExerciseRerunsUpdated AS [dbo].[udt_CharacterExerciseRerun];

            INSERT INTO @CharacterExerciseRerunsUpdated
            (
                [Id],
                [CharacterExerciseId],
                [RequiredChallengeCount],
                [ContinuousChallengeCount],
                [TotalChallengeCount],
                [InitiallyScheduledOn],
                [RepeatedlyScheduledOn]
            )
            SELECT
                [characterExerciseRerun].[Id],
                COALESCE([characterExerciseIdMap].[Id], [characterExerciseRerun].[CharacterExerciseId]),
                [characterExerciseRerun].[RequiredChallengeCount],
                [characterExerciseRerun].[ContinuousChallengeCount],
                [characterExerciseRerun].[TotalChallengeCount],
                [characterExerciseRerun].[InitiallyScheduledOn],
                [characterExerciseRerun].[RepeatedlyScheduledOn]
            FROM @{"CharacterExerciseReruns"} AS [characterExerciseRerun]
            LEFT JOIN @CharacterExerciseIdMap AS [characterExerciseIdMap] ON [characterExerciseIdMap].[PreviousId] = [characterExerciseRerun].[CharacterExerciseId];

            MERGE INTO [dbo].[CharacterExerciseRerun] AS [target]
            USING @{"CharacterExerciseRerunsUpdated"} AS [source]
            ON [target].[Id] = [source].[Id]
            WHEN NOT MATCHED THEN INSERT
            (
                [CharacterExerciseId],
                [RequiredChallengeCount],
                [ContinuousChallengeCount],
                [TotalChallengeCount],
                [InitiallyScheduledOn],
                [RepeatedlyScheduledOn]
            )
            VALUES
            (
                [source].[CharacterExerciseId],
                [source].[RequiredChallengeCount],
                [source].[ContinuousChallengeCount],
                [source].[TotalChallengeCount],
                [source].[InitiallyScheduledOn],
                [source].[RepeatedlyScheduledOn]
            );

            SELECT TOP (1) @{"ResultCharacterExerciseId"} = [Value] FROM @CharacterExerciseIds;";

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

        int characterExerciseIdValue = DbValueConvert.ToValueType<int>(resultCharacterExerciseIdParameter.Value);

        characterExerciseId = new CharacterExerciseId(characterExerciseIdValue);

        characterExercise = await GetCharacterExerciseAsync(characterExerciseId, cancellationToken)
            ?? throw new UnreachableException("The character exercise with this Id has just been inserted.");

        return characterExercise;
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    public override async Task<bool> UpdateCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default)
    {
        using DataTable characterExercisesDataTable = YieldEnumerableHelpers.Yield(characterExercise)
            .ToDataTable();

        using DataTable characterExerciseRerunsDataTable = characterExercise.CharacterExerciseReruns
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
            new("CharacterExercises", SqlDbType.Structured) { TypeName = "[dbo].[udt_CharacterExercise]", Value = characterExercisesDataTable, },
            new("CharacterExerciseReruns", SqlDbType.Structured) { TypeName = "[dbo].[udt_CharacterExerciseRerun]", Value = characterExerciseRerunsDataTable },
        };

        string commandText = @$"
            UPDATE [target]
            SET
                [target].[CharacterId] = [source].[CharacterId],
                [target].[InstanceData] = [source].[InstanceData],
                [target].[GeneratedOn] = [source].[GeneratedOn]
            FROM [dbo].[CharacterExercise] AS [target]
            INNER JOIN @{"CharacterExercises"} AS [source] ON [target].[Id] = [source].[Id];

            MERGE INTO [dbo].[CharacterExerciseRerun] AS [target]
            USING @{"CharacterExerciseReruns"} AS [source]
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
                [CharacterExerciseId],
                [RequiredChallengeCount],
                [ContinuousChallengeCount],
                [TotalChallengeCount],
                [InitiallyScheduledOn],
                [RepeatedlyScheduledOn]
            )
            VALUES
            (
                [source].[CharacterExerciseId],
                [source].[RequiredChallengeCount],
                [source].[ContinuousChallengeCount],
                [source].[TotalChallengeCount],
                [source].[InitiallyScheduledOn],
                [source].[RepeatedlyScheduledOn]
            )
            WHEN NOT MATCHED BY SOURCE
                AND [target].[CharacterExerciseId] IN (SELECT [characterExercise].[Id] FROM @{"CharacterExercises"} AS [characterExercise])
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
