using Domain.Models.Identity;

namespace Infrastructure.Repositories.Identity;

/// <summary>
/// Repository interface for user-specific data access operations.
/// </summary>
public interface IUserRepository
    : IGenericRepository<User>
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of all users.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of users and total count.</returns>
    Task<(List<User> Items, int TotalCount)> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
