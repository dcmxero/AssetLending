using System.ComponentModel.DataAnnotations;

namespace DTOs.Asset;

/// <summary>
/// Request DTO for updating an existing asset.
/// </summary>
public class UpdateAssetDto
{
    /// <summary>
    /// Updated name of the asset.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    /// <summary>
    /// Updated description of the asset.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Updated serial number. Must be unique if provided.
    /// </summary>
    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Identifier of the asset category.
    /// </summary>
    [Required]
    public int AssetCategoryId { get; set; }
}
