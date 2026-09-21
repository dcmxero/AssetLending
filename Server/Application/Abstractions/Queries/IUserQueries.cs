using DTOs.Common;
using DTOs.User;

namespace Application.Abstractions.Queries;

/// <summary>
/// Read-side contract for user data.
/// </summary>
public interface IUserQueries
{
    /// <summary>
    /// Retrieves a paginated page of users ordered by name.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A page of users.</returns>
    Task<PaginatedList<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single user by their identifier.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
}
