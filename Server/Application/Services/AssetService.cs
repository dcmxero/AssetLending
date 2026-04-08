using Application.Mappers;
using Domain.Common;
using Domain.Enums;
using DTOs.Asset;
using DTOs.Common;
using Infrastructure.Repositories.AssetManagement;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset management operations including creation, updates, and activation.
/// </summary>
public sealed class AssetService(
    IAssetRepository assetRepository,
    IUnitOfWork unitOfWork,
    ILogger<AssetService> logger)
    : IAssetService
{
    public async Task<PaginatedList<AssetDto>> GetAssetsAsync(AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await assetRepository.GetAssetsAsync(status, categoryId, page, pageSize, cancellationToken);
        return new PaginatedList<AssetDto>
        {
            Data = [.. items.Select(a => a.ToDto())],
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }

    public async Task<AssetDto?> GetAssetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        return asset?.ToDto();
    }

    public async Task<AssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken cancellationToken = default)
    {
        var asset = dto.ToDomain();
        await assetRepository.AddAsync(asset, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation("Created asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return asset.ToDto();
    }

    public async Task<Result<AssetDto>> UpdateAssetAsync(int id, UpdateAssetDto dto, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<AssetDto>($"Asset with ID {id} not found.");
        }

        asset.Name = dto.Name;
        asset.Description = dto.Description;
        asset.SerialNumber = dto.SerialNumber;
        asset.AssetCategoryId = dto.AssetCategoryId;

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict updating asset {AssetId}", id);
            return Result.Failure<AssetDto>("The asset was modified by another user. Please try again.");
        }

        logger.LogInformation("Updated asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return Result.Success(asset.ToDto());
    }

    public async Task<Result<AssetDto>> DeactivateAssetAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<AssetDto>($"Asset with ID {id} not found.");
        }

        var deactivateResult = asset.Deactivate();
        if (!deactivateResult.IsSuccess)
        {
            logger.LogWarning("Failed to deactivate asset {AssetId}: {Error}", id, deactivateResult.Error);
            return Result.Failure<AssetDto>(deactivateResult.Error!);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict deactivating asset {AssetId}", id);
            return Result.Failure<AssetDto>("The asset was modified by another user. Please try again.");
        }

        logger.LogInformation("Deactivated asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return Result.Success(asset.ToDto());
    }

    public async Task<Result<AssetDto>> ActivateAssetAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<AssetDto>($"Asset with ID {id} not found.");
        }

        var activateResult = asset.Activate();
        if (!activateResult.IsSuccess)
        {
            logger.LogWarning("Failed to activate asset {AssetId}: {Error}", id, activateResult.Error);
            return Result.Failure<AssetDto>(activateResult.Error!);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict activating asset {AssetId}", id);
            return Result.Failure<AssetDto>("The asset was modified by another user. Please try again.");
        }

        logger.LogInformation("Activated asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return Result.Success(asset.ToDto());
    }
}
