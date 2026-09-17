using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for asset write operations.
/// </summary>
public sealed class AssetRepository(ApplicationDbContext context)
    : GenericRepository<Asset>(context), IAssetRepository
{
}
