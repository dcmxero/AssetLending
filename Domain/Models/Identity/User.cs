using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Identity;

/// <summary>
/// Represents a user who can borrow or reserve assets.
/// </summary>
public class User
    : DbEntity
{
    /// <summary>
    /// First name of the user.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string LastName { get; set; }

    /// <summary>
    /// Email address of the user. Must be unique.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public required string Email { get; set; }

    /// <summary>
    /// Full name of the user (computed from FirstName and LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Collection of loans associated with this user.
    /// </summary>
    public virtual ICollection<AssetManagement.Loan> Loans { get; set; } = [];

    /// <summary>
    /// Collection of reservations associated with this user.
    /// </summary>
    public virtual ICollection<AssetManagement.Reservation> Reservations { get; set; } = [];
}
