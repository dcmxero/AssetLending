using System.ComponentModel.DataAnnotations;
using DTOs.Common;

namespace DTOs.Asset;

/// <summary>
/// Request DTO for creating a new loan (checking out an asset).
/// </summary>
public class CreateLoanDto
{
    /// <summary>
    /// Identifier of the asset to borrow.
    /// </summary>
    [Required]
    public int AssetId { get; set; }

    /// <summary>
    /// Identifier of the user borrowing the asset.
    /// </summary>
    [Required]
    public int BorrowedById { get; set; }

    /// <summary>
    /// Expected return date.
    /// </summary>
    [Required]
    [FutureDate]
    public DateTime DueDate { get; set; }
}
