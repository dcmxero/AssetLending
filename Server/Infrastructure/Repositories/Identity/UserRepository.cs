using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Identity;

/// <summary>
/// Repository for user write operations.
/// </summary>
public sealed class UserRepository(ApplicationDbContext context)
    : GenericRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
