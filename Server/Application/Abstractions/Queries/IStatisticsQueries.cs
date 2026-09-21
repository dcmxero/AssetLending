using DTOs.Common;

namespace Application.Abstractions.Queries;

/// <summary>
/// Read-side contract for aggregated lending system statistics.
/// </summary>
public interface IStatisticsQueries
{
    /// <summary>
    /// Computes the aggregated statistics of the lending system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregated statistics.</returns>
    Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
