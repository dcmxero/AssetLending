using System.Linq.Expressions;
using Domain.Models.Identity;
using DTOs.Common;
using DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries;

/// <summary>
/// Reads user data and projects it into DTOs within the database query.
/// </summary>
public sealed class UserQueries(ApplicationDbContext context)
    : IUserQueries
{
    private static readonly Expression<Func<User, UserDto>> ToDto = user => new UserDto
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email
    };

    public async Task<PaginatedList<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await context.Users.CountAsync(cancellationToken);

        var items = await context.Users
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PaginatedList<UserDto>
        {
            Data = items,
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.Id == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
