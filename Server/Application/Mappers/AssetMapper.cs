using Domain.Models.AssetManagement;
using DTOs.Asset;

namespace Application.Mappers;

/// <summary>
/// Provides mapping extension methods between <see cref="Asset"/> domain entities and asset DTOs.
/// </summary>
public static class AssetMapper
{
    /// <summary>
    /// Maps an <see cref="Asset"/> domain entity to an <see cref="AssetDto"/>.
    /// </summary>
    public static AssetDto ToDto(this Asset asset) => new()
    {
        Id = asset.Id,
        Name = asset.Name,
        Description = asset.Description,
        SerialNumber = asset.SerialNumber,
        Status = asset.Status.ToString(),
        IsActive = asset.IsActive,
        AssetCategoryId = asset.AssetCategoryId,
        AssetCategoryName = asset.AssetCategory?.Name ?? ""
    };

    /// <summary>
    /// Maps a <see cref="CreateAssetDto"/> to a new <see cref="Asset"/> domain entity.
    /// </summary>
    public static Asset ToDomain(this CreateAssetDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        SerialNumber = dto.SerialNumber,
        AssetCategoryId = dto.AssetCategoryId
    };
}
