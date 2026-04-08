using DTOs.Asset;

namespace Application.Services;

/// <summary>
/// Service interface for asset category operations.
/// </summary>
public interface IAssetCategoryService
{
    /// <summary>
    /// Retrieves all asset categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all asset categories.</returns>
    Task<List<AssetCategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an asset category by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    Task<AssetCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new asset category.
    /// </summary>
    /// <param name="dto">The category creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category.</returns>
    Task<AssetCategoryDto> CreateCategoryAsync(CreateAssetCategoryDto dto, CancellationToken cancellationToken = default);
}
