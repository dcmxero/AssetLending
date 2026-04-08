using Domain.Common;
using Domain.Models.Identity;

namespace Domain.Models.AssetManagement;

/// <summary>
/// Represents a reservation of an asset for a specific user and time period.
/// </summary>
public class Reservation
    : DbEntity
{
    /// <summary>
    /// Foreign key to the reserved asset.
    /// </summary>
    public int AssetId { get; set; }

    /// <summary>
    /// Navigation property to the reserved asset.
    /// </summary>
    public virtual Asset Asset { get; set; } = null!;

    /// <summary>
    /// Foreign key to the user who reserved the asset.
    /// </summary>
    public int ReservedById { get; set; }

    /// <summary>
    /// Navigation property to the user who reserved the asset.
    /// </summary>
    public virtual User ReservedBy { get; set; } = null!;

    /// <summary>
    /// Date and time when the reservation was created.
    /// </summary>
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time until which the reservation is valid.
    /// </summary>
    public DateTime ReservedUntil { get; set; }

    /// <summary>
    /// Indicates whether the reservation has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Indicates whether the reservation has expired (past ReservedUntil date).
    /// </summary>
    public bool IsExpired => !IsCancelled && ReservedUntil < DateTime.UtcNow;

    /// <summary>
    /// Cancels the reservation.
    /// </summary>
    /// <returns>Success if the reservation was active; failure with error message otherwise.</returns>
    public Result Cancel()
    {
        if (IsCancelled)
        {
            return Result.Failure("This reservation has already been cancelled.");
        }

        IsCancelled = true;
        return Result.Success();
    }
}
