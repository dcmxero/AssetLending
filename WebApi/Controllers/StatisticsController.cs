using Application.Services;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controller for retrieving aggregated system statistics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService statisticsService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves aggregated statistics about the lending system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Aggregated statistics.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get aggregated system statistics")]
    [ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
    {
        var statistics = await statisticsService.GetStatisticsAsync(cancellationToken);
        return Ok(statistics);
    }
}
