using System.ComponentModel.DataAnnotations;

namespace Domain.Models.AssetManagement;

/// <summary>
/// Represents a category of assets (e.g. Electronics, Tools, Office Equipment).
/// </summary>
public class AssetCategory
    : DbEntity
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

    /// <summary>
    /// Collection of assets belonging to this category.
    /// </summary>
    public virtual ICollection<Asset> Assets { get; set; } = [];
}
