using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Mappers;
using Domain.Common;
using Domain.Enums;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset management operations including creation, updates, and activation.
/// </summary>
public sealed class AssetService(
    IAssetRepository assetRepository,
    IAssetQueries assetQueries,
    IUnitOfWork unitOfWork,
    ILogger<AssetService> logger)
    : IAssetService
{
    public async Task<PaginatedList<AssetDto>> GetAssetsAsync(AssetStatus? status, int? categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await assetQueries.GetAssetsAsync(status, categoryId, page, pageSize, cancellationToken);
    }

    public async Task<AssetDto?> GetAssetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await assetQueries.GetAssetByIdAsync(id, cancellationToken);
    }

    public async Task<AssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken cancellationToken = default)
    {
        var asset = dto.ToDomain();
        await assetRepository.AddAsync(asset, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation("Created asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return await ReadBackAsync(asset.Id, cancellationToken);
    }

    public async Task<Result<AssetDto>> UpdateAssetAsync(int id, UpdateAssetDto dto, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return Result.NotFound<AssetDto>($"Asset with ID {id} not found.");
        }

        asset.Name = dto.Name;
        asset.Description = dto.Description;
        asset.SerialNumber = dto.SerialNumber;
        asset.AssetCategoryId = dto.AssetCategoryId;

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            logger.LogWarning("Concurrency conflict updating asset {AssetId}", id);
            return Result.ConcurrencyConflict<AssetDto>();
        }

        logger.LogInformation("Updated asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return Result.Success(await ReadBackAsync(asset.Id, cancellationToken));
    }

    public async Task<Result<AssetDto>> DeactivateAssetAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return Result.NotFound<AssetDto>($"Asset with ID {id} not found.");
        }

        var deactivateResult = asset.Deactivate();
        if (!deactivateResult.IsSuccess)
        {
            logger.LogWarning("Failed to deactivate asset {AssetId}: {Error}", id, deactivateResult.Error);
            return Result.Failure<AssetDto>(deactivateResult);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            logger.LogWarning("Concurrency conflict deactivating asset {AssetId}", id);
            return Result.ConcurrencyConflict<AssetDto>();
        }

        logger.LogInformation("Deactivated asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return Result.Success(await ReadBackAsync(asset.Id, cancellationToken));
    }

    public async Task<Result<AssetDto>> ActivateAssetAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return Result.NotFound<AssetDto>($"Asset with ID {id} not found.");
        }

        var activateResult = asset.Activate();
        if (!activateResult.IsSuccess)
        {
            logger.LogWarning("Failed to activate asset {AssetId}: {Error}", id, activateResult.Error);
            return Result.Failure<AssetDto>(activateResult);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            logger.LogWarning("Concurrency conflict activating asset {AssetId}", id);
            return Result.ConcurrencyConflict<AssetDto>();
        }

        logger.LogInformation("Activated asset '{AssetName}' with ID {AssetId}", asset.Name, asset.Id);

        return Result.Success(await ReadBackAsync(asset.Id, cancellationToken));
    }

    /// <summary>
    /// Reads a written asset back through the query side so the response carries its persisted category name.
    /// </summary>
    private async Task<AssetDto> ReadBackAsync(int id, CancellationToken cancellationToken)
    {
        var asset = await assetQueries.GetAssetByIdAsync(id, cancellationToken);
        return asset ?? throw new InvalidOperationException($"Asset with ID {id} disappeared after being saved.");
    }
}
