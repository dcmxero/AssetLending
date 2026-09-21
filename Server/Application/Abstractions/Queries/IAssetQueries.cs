using Domain.Enums;
using DTOs.Asset;
using DTOs.Common;

namespace Application.Abstractions.Queries;

/// <summary>
/// Read-side contract for asset data.
/// </summary>
public interface IAssetQueries
{
    /// <summary>
    /// Retrieves a paginated page of active assets, optionally filtered by status and category.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A page of assets.</returns>
    Task<PaginatedList<AssetDto>> GetAssetsAsync(AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single asset by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The asset if found; otherwise, null.</returns>
    Task<AssetDto?> GetAssetByIdAsync(int id, CancellationToken cancellationToken = default);
}
