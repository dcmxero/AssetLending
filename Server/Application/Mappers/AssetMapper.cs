using Domain.Models.AssetManagement;
using DTOs.Asset;

namespace Application.Mappers;

/// <summary>
/// Provides mapping extension methods from asset DTOs to <see cref="Asset"/> domain entities.
/// </summary>
public static class AssetMapper
{
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
