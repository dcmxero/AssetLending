using Domain.Models.Identity;
using DTOs.User;

namespace Application.Mappers;

/// <summary>
/// Provides mapping extension methods between <see cref="User"/> domain entities and user DTOs.
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Maps a <see cref="User"/> domain entity to a <see cref="UserDto"/>.
    /// </summary>
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email
    };

    /// <summary>
    /// Maps a <see cref="CreateUserDto"/> to a new <see cref="User"/> domain entity.
    /// </summary>
    public static User ToDomain(this CreateUserDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email
    };
}
