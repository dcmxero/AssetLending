using Application.Services;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Extensions;

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
    /// Retrieves a paginated list of all reservations, newest first.
    /// </summary>
    /// <param name="paging">Page number and page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of reservations.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all reservations (paginated)")]
    [ProducesResponseType(typeof(PaginatedList<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PageRequest paging, CancellationToken cancellationToken = default)
    {
        var reservations = await reservationService.GetAllReservationsAsync(paging.Page, paging.PageSize, cancellationToken);
        return Ok(reservations);
    }

    /// <summary>
    /// Retrieves the reservations that are still in force.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active reservations.</returns>
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get reservations that have neither been cancelled nor run out")]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var reservations = await reservationService.GetActiveReservationsAsync(cancellationToken);
        return Ok(reservations);
    }

    /// <summary>
    /// Retrieves a reservation by its identifier.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reservation if found; otherwise, 404.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get reservation by ID")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var reservation = await reservationService.GetReservationByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }

    /// <summary>
    /// Creates a new reservation for an asset.
    /// </summary>
    /// <param name="dto">The reservation creation data containing asset and user identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created reservation with 201 status; or 409 if the asset is not available.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a reservation for an asset")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto, CancellationToken cancellationToken)
    {
        var result = await reservationService.CreateReservationAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
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
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await reservationService.CancelReservationAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }
}
