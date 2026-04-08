namespace DTOs.Asset;

/// <summary>
/// Response DTO representing a reservation.
/// </summary>
public class ReservationDto
{
    /// <summary>
    /// Unique identifier of the reservation.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Identifier of the reserved asset.
    /// </summary>
    public int AssetId { get; init; }

    /// <summary>
    /// Name of the reserved asset.
    /// </summary>
    public required string AssetName { get; init; }

    /// <summary>
    /// Identifier of the user who reserved the asset.
    /// </summary>
    public int ReservedById { get; init; }

    /// <summary>
    /// Full name of the user who reserved the asset.
    /// </summary>
    public required string ReservedByName { get; init; }

    /// <summary>
    /// Date and time when the reservation was created.
    /// </summary>
    public DateTime ReservedAt { get; init; }

    /// <summary>
    /// Date and time until which the reservation is valid.
    /// </summary>
    public DateTime ReservedUntil { get; init; }

    /// <summary>
    /// Indicates whether the reservation has been cancelled.
    /// </summary>
    public bool IsCancelled { get; init; }

    /// <summary>
    /// Indicates whether the reservation has expired.
    /// </summary>
    public bool IsExpired { get; init; }
}
