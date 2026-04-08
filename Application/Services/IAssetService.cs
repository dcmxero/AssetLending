using Domain.Common;
using Domain.Enums;
using DTOs.Asset;
using DTOs.Common;

namespace Application.Services;

/// <summary>
/// Service interface for asset management operations.
/// </summary>
public interface IAssetService
{
    /// <summary>
    /// Retrieves a paginated list of assets, optionally filtered by status and/or category.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of assets.</returns>
    Task<PaginatedList<AssetDto>> GetAssetsAsync(AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an asset by its identifier.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The asset if found; otherwise, null.</returns>
    Task<AssetDto?> GetAssetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new asset with Available status.
    /// </summary>
    /// <param name="dto">The asset creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created asset.</returns>
    Task<AssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing asset's properties.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="dto">The updated asset data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with updated asset; or failure with error message.</returns>
    Task<Result<AssetDto>> UpdateAssetAsync(int id, UpdateAssetDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates an asset (soft delete). Only available assets can be deactivated.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with deactivated asset; or failure with error message.</returns>
    Task<Result<AssetDto>> DeactivateAssetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivates a previously deactivated asset.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with activated asset; or failure with error message.</returns>
    Task<Result<AssetDto>> ActivateAssetAsync(int id, CancellationToken cancellationToken = default);
}
