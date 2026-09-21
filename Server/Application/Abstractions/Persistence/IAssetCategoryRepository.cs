using Domain.Models.AssetManagement;

namespace Application.Abstractions.Persistence;

/// <summary>
/// Repository interface for asset category write operations.
/// </summary>
public interface IAssetCategoryRepository
{
    /// <summary>
    /// Adds a new asset category to the context. Changes are not persisted until <see cref="IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <param name="category">The category to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(AssetCategory category, CancellationToken cancellationToken = default);
}
