using System.Threading;
using System.Threading.Tasks;

namespace JapaneseLanguageTools.Core.Services.Abstractions;

public interface ISnapshotHashCalculator
{
    public Task<string> CalculateSnapshotHashAsync(CancellationToken cancellationToken = default);
}
