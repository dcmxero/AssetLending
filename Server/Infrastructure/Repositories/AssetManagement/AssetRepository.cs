using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for asset write operations.
/// </summary>
public sealed class AssetRepository(ApplicationDbContext context)
    : IAssetRepository
{
    public async Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Assets.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await context.Assets.AddAsync(asset, cancellationToken);
    }
}
