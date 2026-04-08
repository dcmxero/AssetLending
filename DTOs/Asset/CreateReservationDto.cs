using System.ComponentModel.DataAnnotations;
using DTOs.Common;

namespace DTOs.Asset;

/// <summary>
/// Request DTO for creating a new reservation.
/// </summary>
public class CreateReservationDto
{
    /// <summary>
    /// Identifier of the asset to reserve.
    /// </summary>
    [Required]
    public int AssetId { get; set; }

    /// <summary>
    /// Identifier of the user reserving the asset.
    /// </summary>
    [Required]
    public int ReservedById { get; set; }

    /// <summary>
    /// Date and time until which the reservation should be valid.
    /// </summary>
    [Required]
    [FutureDate]
    public DateTime ReservedUntil { get; set; }
}
