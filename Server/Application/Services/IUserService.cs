using Domain.Common;
using DTOs.Common;
using DTOs.User;

namespace Application.Services;

/// <summary>
/// Service interface for user management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a paginated list of all users.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of users.</returns>
    Task<PaginatedList<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user. Returns failure if email is already taken.
    /// </summary>
    /// <param name="dto">The user creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with created user; or failure with error message.</returns>
    Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
}
