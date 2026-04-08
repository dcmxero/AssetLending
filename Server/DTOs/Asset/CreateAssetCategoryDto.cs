using System.ComponentModel.DataAnnotations;

namespace DTOs.Asset;

/// <summary>
/// Request DTO for creating a new asset category.
/// </summary>
public class CreateAssetCategoryDto
{
    /// <summary>
    /// Name of the category.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Optional description of the category.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}
