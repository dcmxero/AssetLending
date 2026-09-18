using System.Linq.Expressions;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries;

/// <summary>
/// Reads asset category data and projects it into DTOs within the database query.
/// </summary>
public sealed class AssetCategoryQueries(ApplicationDbContext context)
    : IAssetCategoryQueries
{
    private static readonly Expression<Func<AssetCategory, AssetCategoryDto>> ToDto = category => new AssetCategoryDto
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };

    public async Task<List<AssetCategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.AssetCategories
            .OrderBy(c => c.Name)
            .Select(ToDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.AssetCategories
            .Where(c => c.Id == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
