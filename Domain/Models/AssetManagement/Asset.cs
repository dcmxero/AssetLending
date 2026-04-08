using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;

namespace Domain.Models.AssetManagement;

/// <summary>
/// Represents a physical asset that can be loaned or reserved (e.g. laptop, tool, monitor).
/// </summary>
public class Asset
    : DbEntity
{
    /// <summary>
    /// Name of the asset.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    /// <summary>
    /// Optional description of the asset.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Optional serial number. Must be unique if provided.
    /// </summary>
    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Current availability status of the asset.
    /// </summary>
    public AssetStatus Status { get; set; } = AssetStatus.Available;

    /// <summary>
    /// Indicates whether the asset is active. Inactive assets cannot be loaned or reserved.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Foreign key to the asset category.
    /// </summary>
    public int AssetCategoryId { get; set; }

    /// <summary>
    /// Navigation property to the asset category.
    /// </summary>
    public virtual AssetCategory AssetCategory { get; set; } = null!;

    /// <summary>
    /// Concurrency token for optimistic concurrency control.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    /// <summary>
    /// Transitions the asset to <see cref="AssetStatus.Loaned"/> status.
    /// </summary>
    /// <returns>Success if the asset was available; failure with error message otherwise.</returns>
    public Result Checkout()
    {
        if (!IsActive)
        {
            return Result.Failure($"Asset '{Name}' is inactive and cannot be checked out.");
        }

        if (Status != AssetStatus.Available)
        {
            return Result.Failure($"Asset '{Name}' is not available for checkout. Current status: {Status}.");
        }

        Status = AssetStatus.Loaned;
        return Result.Success();
    }

    /// <summary>
    /// Transitions a reserved asset directly to <see cref="AssetStatus.Loaned"/> status.
    /// Only the user who made the reservation can perform this action.
    /// </summary>
    /// <param name="reservedByUserId">The ID of the user who holds the reservation.</param>
    /// <param name="requestingUserId">The ID of the user requesting the checkout.</param>
    /// <returns>Success if the asset was reserved by the requesting user; failure otherwise.</returns>
    public Result CheckoutFromReservation(int reservedByUserId, int requestingUserId)
    {
        if (!IsActive)
        {
            return Result.Failure($"Asset '{Name}' is inactive and cannot be checked out.");
        }

        if (Status != AssetStatus.Reserved)
        {
            return Result.Failure($"Asset '{Name}' is not currently reserved. Current status: {Status}.");
        }

        if (reservedByUserId != requestingUserId)
        {
            return Result.Failure($"Asset '{Name}' is reserved by another user.");
        }

        Status = AssetStatus.Loaned;
        return Result.Success();
    }

    /// <summary>
    /// Transitions the asset back to <see cref="AssetStatus.Available"/> status after being loaned.
    /// </summary>
    /// <returns>Success if the asset was loaned; failure with error message otherwise.</returns>
    public Result Return()
    {
        if (Status != AssetStatus.Loaned)
        {
            return Result.Failure($"Asset '{Name}' is not currently loaned. Current status: {Status}.");
        }

        Status = AssetStatus.Available;
        return Result.Success();
    }

    /// <summary>
    /// Transitions the asset to <see cref="AssetStatus.Reserved"/> status.
    /// </summary>
    /// <returns>Success if the asset was available; failure with error message otherwise.</returns>
    public Result Reserve()
    {
        if (!IsActive)
        {
            return Result.Failure($"Asset '{Name}' is inactive and cannot be reserved.");
        }

        if (Status != AssetStatus.Available)
        {
            return Result.Failure($"Asset '{Name}' is not available for reservation. Current status: {Status}.");
        }

        Status = AssetStatus.Reserved;
        return Result.Success();
    }

    /// <summary>
    /// Cancels the reservation and transitions the asset back to <see cref="AssetStatus.Available"/> status.
    /// </summary>
    /// <returns>Success if the asset was reserved; failure with error message otherwise.</returns>
    public Result CancelReservation()
    {
        if (Status != AssetStatus.Reserved)
        {
            return Result.Failure($"Asset '{Name}' is not currently reserved. Current status: {Status}.");
        }

        Status = AssetStatus.Available;
        return Result.Success();
    }

    /// <summary>
    /// Deactivates the asset (soft delete). Only available assets can be deactivated.
    /// </summary>
    /// <returns>Success if the asset was deactivated; failure otherwise.</returns>
    public Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure($"Asset '{Name}' is already inactive.");
        }

        if (Status != AssetStatus.Available)
        {
            return Result.Failure($"Asset '{Name}' cannot be deactivated while it is {Status}.");
        }

        IsActive = false;
        return Result.Success();
    }

    /// <summary>
    /// Reactivates a previously deactivated asset.
    /// </summary>
    /// <returns>Success if the asset was reactivated; failure otherwise.</returns>
    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure($"Asset '{Name}' is already active.");
        }

        IsActive = true;
        return Result.Success();
    }
}
