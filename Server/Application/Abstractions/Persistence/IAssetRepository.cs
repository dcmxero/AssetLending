using Domain.Models.AssetManagement;

namespace Application.Abstractions.Persistence;

/// <summary>
/// Repository interface for asset write operations.
/// </summary>
public interface IAssetRepository
{
    /// <summary>
    /// Retrieves an asset by its identifier for modification.
    /// </summary>
    /// <param name="id">The identifier of the asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The asset if found; otherwise, null.</returns>
    Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new asset to the context. Changes are not persisted until <see cref="IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <param name="asset">The asset to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
}
