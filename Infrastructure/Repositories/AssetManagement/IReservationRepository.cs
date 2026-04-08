using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for reservation-specific data access operations.
/// </summary>
public interface IReservationRepository
    : IGenericRepository<Reservation>
{
    /// <summary>
    /// Retrieves the active (non-cancelled) reservation for a specific asset, if one exists.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active reservation if found; otherwise, null.</returns>
    Task<Reservation?> GetActiveByAssetIdAsync(int assetId, CancellationToken cancellationToken = default);
}
