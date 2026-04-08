namespace DTOs.Asset;

/// <summary>
/// Response DTO representing an asset category.
/// </summary>
public class AssetCategoryDto
{
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Name of the category.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Optional description of the category.
    /// </summary>
    public string? Description { get; init; }
}
