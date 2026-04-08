namespace DTOs.Asset;

/// <summary>
/// Response DTO representing an asset.
/// </summary>
public class AssetDto
{
    /// <summary>
    /// Unique identifier of the asset.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Name of the asset.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Optional description of the asset.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Optional serial number of the asset.
    /// </summary>
    public string? SerialNumber { get; init; }

    /// <summary>
    /// Current availability status (Available, Loaned, Reserved).
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Indicates whether the asset is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Identifier of the asset category.
    /// </summary>
    public int AssetCategoryId { get; init; }

    /// <summary>
    /// Name of the asset category.
    /// </summary>
    public required string AssetCategoryName { get; init; }
}
