using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Mappers;
using DTOs.Asset;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset category operations.
/// </summary>
public sealed class AssetCategoryService(
    IAssetCategoryRepository categoryRepository,
    IAssetCategoryQueries categoryQueries,
    IUnitOfWork unitOfWork,
    ILogger<AssetCategoryService> logger)
    : IAssetCategoryService
{
    /// <inheritdoc />
    public async Task<List<AssetCategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await categoryQueries.GetAllCategoriesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AssetCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await categoryQueries.GetCategoryByIdAsync(id, cancellationToken);
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
