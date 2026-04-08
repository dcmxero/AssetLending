namespace DTOs.User;

/// <summary>
/// Response DTO representing a user.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// First name of the user.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    public required string Email { get; init; }
}
