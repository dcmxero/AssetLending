namespace Domain.Enums;

/// <summary>
/// Represents the current availability status of an asset.
/// </summary>
public enum AssetStatus
{
    /// <summary>
    /// Asset is available for loan or reservation.
    /// </summary>
    Available,

    /// <summary>
    /// Asset is currently loaned to a user.
    /// </summary>
    Loaned,

    /// <summary>
    /// Asset is reserved for a user.
    /// </summary>
    Reserved
}
