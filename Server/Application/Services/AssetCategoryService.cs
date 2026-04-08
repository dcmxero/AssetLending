using Application.Mappers;
using DTOs.Asset;
using Infrastructure.Repositories.AssetManagement;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset category operations.
/// </summary>
public sealed class AssetCategoryService(
    IAssetCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ILogger<AssetCategoryService> logger)
    : IAssetCategoryService
{
    /// <inheritdoc />
    public async Task<List<AssetCategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAll()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return [.. categories.Select(c => c.ToDto())];
    }

    /// <inheritdoc />
    public async Task<AssetCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        return category?.ToDto();
    }

    /// <inheritdoc />
    public async Task<AssetCategoryDto> CreateCategoryAsync(CreateAssetCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = dto.ToDomain();
        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation("Created asset category '{CategoryName}' with ID {CategoryId}", category.Name, category.Id);

        return category.ToDto();
    }
}
