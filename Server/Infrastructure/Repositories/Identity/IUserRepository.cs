using Domain.Models.Identity;

namespace Infrastructure.Repositories.Identity;

/// <summary>
/// Repository interface for user write operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their identifier for modification.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the context. Changes are not persisted until <see cref="UnitOfWork.IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
