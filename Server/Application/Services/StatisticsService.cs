using Application.Abstractions.Queries;
using DTOs.Common;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for exposing aggregated lending system statistics.
/// </summary>
public sealed class StatisticsService(
    IStatisticsQueries statisticsQueries,
    ILogger<StatisticsService> logger)
    : IStatisticsService
{
    /// <inheritdoc />
    public async Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Computing system statistics");

        return await statisticsQueries.GetStatisticsAsync(cancellationToken);
    }
}
