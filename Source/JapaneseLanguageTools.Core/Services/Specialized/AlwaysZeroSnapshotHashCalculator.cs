using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Core.Services.Abstractions;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Services.Specialized;

public class AlwaysZeroSnapshotHashCalculator : ISnapshotHashCalculator
{
    private static readonly Lazy<string> s_emptySnapshotHash = new(CalculateEmptySnapshotHash);

    /// <inheritdoc />
    public Task<string> CalculateSnapshotHashAsync(CancellationToken cancellationToken = default)
    {
        Task<string> completedTask = Task.FromResult(s_emptySnapshotHash.Value);

        return completedTask;
    }

    private static string CalculateEmptySnapshotHash()
    {
        byte[] emptySnapshotHashBytes = Enumerable.Repeat((byte)0x00, SHA256.HashSizeInBytes)
            .ToArray();

        string emptySnapshotHash = Convert.ToHexString(emptySnapshotHashBytes);

        return emptySnapshotHash;
    }
}
