using Domain.Common;
using DTOs.Asset;
using DTOs.Common;

namespace Application.Services;

/// <summary>
/// Service interface for asset reservation operations.
/// </summary>
public interface IReservationService
{
    /// <summary>
    /// Retrieves the reservations that are still in force.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active reservations.</returns>
    Task<List<ReservationDto>> GetActiveReservationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of all reservations.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of reservations.</returns>
    Task<PaginatedList<ReservationDto>> GetAllReservationsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a reservation by its identifier.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reservation if found; otherwise, null.</returns>
    Task<ReservationDto?> GetReservationByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new reservation for an asset.
    /// </summary>
    /// <param name="dto">The reservation creation data containing asset and user identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with created reservation; or failure if asset/user not found or asset not available.</returns>
    Task<Result<ReservationDto>> CreateReservationAsync(CreateReservationDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an existing reservation, making the asset available again.
    /// </summary>
    /// <param name="reservationId">The identifier of the reservation to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with updated reservation; or failure if not found or already cancelled.</returns>
    Task<Result<ReservationDto>> CancelReservationAsync(int reservationId, CancellationToken cancellationToken = default);
}
