using Application.Services;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controller for managing asset reservations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IReservationService reservationService)
    : ControllerBase
{
    /// <summary>
    /// Creates a new reservation for an asset.
    /// </summary>
    /// <param name="dto">The reservation creation data containing asset and user identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created reservation with 201 status; or 409 if the asset is not available.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a reservation for an asset")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto, CancellationToken cancellationToken)
    {
        var result = await reservationService.CreateReservationAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return CreatedAtAction(null, new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Cancels an existing reservation, making the asset available again.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated reservation with 200 status; or 409 if already cancelled.</returns>
    [HttpPut("{id}/cancel")]
    [SwaggerOperation(Summary = "Cancel a reservation")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await reservationService.CancelReservationAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return Ok(result.Value);
    }
}
