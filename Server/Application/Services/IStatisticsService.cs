using DTOs.Common;

namespace Application.Services;

/// <summary>
/// Service interface for retrieving aggregated system statistics.
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Retrieves aggregated statistics about the lending system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Aggregated statistics DTO.</returns>
    Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
