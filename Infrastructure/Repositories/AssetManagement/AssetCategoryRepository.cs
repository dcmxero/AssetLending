using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for asset category data access operations.
/// </summary>
public sealed class AssetCategoryRepository(ApplicationDbContext context)
    : GenericRepository<AssetCategory>(context), IAssetCategoryRepository
{
}
