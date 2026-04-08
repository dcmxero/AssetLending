using Domain.Common;
using Domain.Enums;
using Domain.Models.Identity;

namespace Domain.Models.AssetManagement;

/// <summary>
/// Represents a loan record — tracks who borrowed which asset and when.
/// </summary>
public class Loan
    : DbEntity
{
    /// <summary>
    /// Foreign key to the borrowed asset.
    /// </summary>
    public int AssetId { get; set; }

    /// <summary>
    /// Navigation property to the borrowed asset.
    /// </summary>
    public virtual Asset Asset { get; set; } = null!;

    /// <summary>
    /// Foreign key to the user who borrowed the asset.
    /// </summary>
    public int BorrowedById { get; set; }

    /// <summary>
    /// Navigation property to the user who borrowed the asset.
    /// </summary>
    public virtual User BorrowedBy { get; set; } = null!;

    /// <summary>
    /// Date and time when the asset was borrowed.
    /// </summary>
    public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Expected return date.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Actual return date. Null while the loan is active.
    /// </summary>
    public DateTime? ReturnedAt { get; set; }

    /// <summary>
    /// Current status of the loan.
    /// </summary>
    public LoanStatus Status { get; set; } = LoanStatus.Active;

    /// <summary>
    /// Marks the loan as returned and records the return timestamp.
    /// </summary>
    /// <returns>Success if the loan was active; failure with error message otherwise.</returns>
    public Result MarkReturned()
    {
        if (Status == LoanStatus.Returned)
        {
            return Result.Failure("This loan has already been returned.");
        }

        Status = LoanStatus.Returned;
        ReturnedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
