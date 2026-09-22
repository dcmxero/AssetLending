using DTOs.Asset;
using DTOs.Common;

namespace Application.Abstractions.Queries;

/// <summary>
/// Read-side contract for reservation data.
/// </summary>
public interface IReservationQueries
{
    /// <summary>
    /// Retrieves the reservations that are neither cancelled nor past their validity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of reservations still in force.</returns>
    Task<List<ReservationDto>> GetActiveReservationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated page of all reservations, newest first.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A page of reservations.</returns>
    Task<PaginatedList<ReservationDto>> GetAllReservationsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single reservation by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the reservation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reservation if found; otherwise, null.</returns>
    Task<ReservationDto?> GetReservationByIdAsync(int id, CancellationToken cancellationToken = default);
}
