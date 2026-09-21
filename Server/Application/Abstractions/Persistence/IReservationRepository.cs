using Domain.Models.AssetManagement;

namespace Application.Abstractions.Persistence;

/// <summary>
/// Repository interface for reservation write operations.
/// </summary>
public interface IReservationRepository
{
    /// <summary>
    /// Retrieves a reservation by its identifier, together with the asset and holder its domain methods act on.
    /// </summary>
    /// <param name="id">The identifier of the reservation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reservation if found; otherwise, null.</returns>
    Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the reservation currently held on an asset, if one exists.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reservation if found; otherwise, null.</returns>
    Task<Reservation?> GetActiveByAssetIdAsync(int assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new reservation to the context. Changes are not persisted until <see cref="IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <param name="reservation">The reservation to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
