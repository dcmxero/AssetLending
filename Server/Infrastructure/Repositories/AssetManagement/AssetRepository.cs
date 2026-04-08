using Domain.Enums;
using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for asset-specific data access operations.
/// </summary>
public sealed class AssetRepository(ApplicationDbContext context)
    : GenericRepository<Asset>(context), IAssetRepository
{
    public override async Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Assets
            .Include(a => a.AssetCategory)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<(List<Asset> Items, int TotalCount)> GetAssetsAsync(AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive);

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.AssetCategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ([.. items], totalCount);
    }
}
