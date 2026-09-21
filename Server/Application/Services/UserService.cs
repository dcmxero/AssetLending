using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Mappers;
using Domain.Common;
using DTOs.Common;
using DTOs.User;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for user management operations.
/// </summary>
public sealed class UserService(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IUnitOfWork unitOfWork,
    ILogger<UserService> logger)
    : IUserService
{
    public async Task<PaginatedList<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await userQueries.GetUsersAsync(page, pageSize, cancellationToken);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await userQueries.GetUserByIdAsync(id, cancellationToken);
    }

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Attempted to create user with duplicate email '{Email}'", dto.Email);
            return Result.Failure<UserDto>($"User with email '{dto.Email}' already exists.");
        }

        var user = dto.ToDomain();
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation("Created user '{Email}' with ID {UserId}", user.Email, user.Id);

        return Result.Success(user.ToDto());
    }
}
