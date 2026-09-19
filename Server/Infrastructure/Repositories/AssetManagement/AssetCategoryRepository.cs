using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for asset category write operations.
/// </summary>
public sealed class AssetCategoryRepository(ApplicationDbContext context)
    : IAssetCategoryRepository
{
    public async Task AddAsync(AssetCategory category, CancellationToken cancellationToken = default)
    {
        await context.AssetCategories.AddAsync(category, cancellationToken);
    }
}
