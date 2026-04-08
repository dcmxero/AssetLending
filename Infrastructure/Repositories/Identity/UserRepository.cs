using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Identity;

/// <summary>
/// Repository for user-specific data access operations.
/// </summary>
public sealed class UserRepository(ApplicationDbContext context)
    : GenericRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<(List<User> Items, int TotalCount)> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Users.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ([.. items], totalCount);
    }
}
