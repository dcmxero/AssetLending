using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for asset-specific data access operations.
/// </summary>
public interface IAssetRepository
    : IGenericRepository<Asset>
{
    /// <summary>
    /// Retrieves a paginated list of assets, optionally filtered by status. Only active assets are returned.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of assets and total count.</returns>
    Task<(List<Asset> Items, int TotalCount)> GetAssetsAsync(Domain.Enums.AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default);
}
