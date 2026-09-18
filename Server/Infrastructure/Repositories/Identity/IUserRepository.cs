using Domain.Models.Identity;

namespace Infrastructure.Repositories.Identity;

/// <summary>
/// Repository interface for user write operations.
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
}
