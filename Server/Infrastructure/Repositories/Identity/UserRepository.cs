using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Identity;

/// <summary>
/// Repository for user write operations.
/// </summary>
public sealed class UserRepository(ApplicationDbContext context)
    : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FindAsync([id], cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }
}
