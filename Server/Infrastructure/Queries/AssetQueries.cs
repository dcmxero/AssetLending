using System.Linq.Expressions;
using Domain.Enums;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries;

/// <summary>
/// Reads asset data and projects it into DTOs within the database query.
/// </summary>
public sealed class AssetQueries(ApplicationDbContext context)
    : IAssetQueries
{
    private static readonly Expression<Func<Asset, AssetDto>> ToDto = asset => new AssetDto
    {
        Id = asset.Id,
        Name = asset.Name,
        Description = asset.Description,
        SerialNumber = asset.SerialNumber,
        Status = asset.Status.ToString(),
        IsActive = asset.IsActive,
        AssetCategoryId = asset.AssetCategoryId,
        AssetCategoryName = asset.AssetCategory.Name
    };

    public async Task<PaginatedList<AssetDto>> GetAssetsAsync(AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Assets.Where(a => a.IsActive);

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
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PaginatedList<AssetDto>
        {
            Data = items,
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }

    public async Task<AssetDto?> GetAssetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Assets
            .Where(a => a.Id == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
