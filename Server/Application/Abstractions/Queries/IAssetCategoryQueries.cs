using DTOs.Asset;

namespace Application.Abstractions.Queries;

/// <summary>
/// Read-side contract for asset category data.
/// </summary>
public interface IAssetCategoryQueries
{
    /// <summary>
    /// Retrieves all asset categories ordered by name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of asset categories.</returns>
    Task<List<AssetCategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single asset category by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    Task<AssetCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
}
