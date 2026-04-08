namespace DTOs.Asset;

/// <summary>
/// Response DTO representing a loan record.
/// </summary>
public class LoanDto
{
    /// <summary>
    /// Unique identifier of the loan.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Identifier of the borrowed asset.
    /// </summary>
    public int AssetId { get; init; }

    /// <summary>
    /// Name of the borrowed asset.
    /// </summary>
    public required string AssetName { get; init; }

    /// <summary>
    /// Identifier of the user who borrowed the asset.
    /// </summary>
    public int BorrowedById { get; init; }

    /// <summary>
    /// Full name of the user who borrowed the asset.
    /// </summary>
    public required string BorrowedByName { get; init; }

    /// <summary>
    /// Date and time when the asset was borrowed.
    /// </summary>
    public DateTime BorrowedAt { get; init; }

    /// <summary>
    /// Expected return date.
    /// </summary>
    public DateTime DueDate { get; init; }

    /// <summary>
    /// Actual return date. Null while the loan is active.
    /// </summary>
    public DateTime? ReturnedAt { get; init; }

    /// <summary>
    /// Current status of the loan (Active, Returned).
    /// </summary>
    public required string Status { get; init; }
}
