using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for asset write operations.
/// </summary>
public interface IAssetRepository
    : IGenericRepository<Asset>
{
}
