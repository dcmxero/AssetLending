using Domain.Models.AssetManagement;
using DTOs.Asset;

namespace Application.Mappers;

/// <summary>
/// Provides mapping extension methods between AssetCategory entities and DTOs.
/// </summary>
public static class AssetCategoryMapper
{
    /// <summary>
    /// Maps an AssetCategory entity to a DTO.
    /// </summary>
    public static AssetCategoryDto ToDto(this AssetCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };

    /// <summary>
    /// Maps a CreateAssetCategoryDto to a new AssetCategory entity.
    /// </summary>
    public static AssetCategory ToDomain(this CreateAssetCategoryDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description
    };
}
