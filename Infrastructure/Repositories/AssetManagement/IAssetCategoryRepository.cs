using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for asset category data access operations.
/// </summary>
public interface IAssetCategoryRepository
    : IGenericRepository<AssetCategory>
{
}
