using Domain.Common;
using DTOs.Asset;

namespace Application.Services;

/// <summary>
/// Service interface for asset reservation operations.
/// </summary>
public interface IReservationService
{
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
