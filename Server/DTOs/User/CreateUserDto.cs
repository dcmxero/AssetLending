using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

/// <summary>
/// Request DTO for creating a new user.
/// </summary>
public class CreateUserDto
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
    /// Email address of the user. Must be a valid email format.
    /// </summary>
    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public required string Email { get; set; }
}
