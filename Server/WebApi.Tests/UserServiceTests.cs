using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Services;
using Domain.Common;
using Domain.Models.Identity;
using DTOs.User;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers user creation, including the duplicate-email rule.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> users = new();
    private readonly Mock<IUserQueries> queries = new();
    private readonly Mock<IUnitOfWork> unitOfWork = new();

    private UserService CreateService() => new(
        users.Object,
        queries.Object,
        unitOfWork.Object,
        NullLogger<UserService>.Instance);

    private static CreateUserDto Request(string email = "jana@example.com") => new()
    {
        FirstName = "Jana",
        LastName = "Novakova",
        Email = email
    };

    [Fact]
    public async Task CreateUser_SavesTheUser_WhenTheEmailIsFree()
    {
        User? saved = null;
        users.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => saved = u);

        var result = await CreateService().CreateUserAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.Equal("jana@example.com", saved!.Email);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_RefusesAnEmailThatIsAlreadyTaken()
    {
        users
            .Setup(r => r.GetByEmailAsync("jana@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = 1, FirstName = "Jana", LastName = "Stara", Email = "jana@example.com" });

        var result = await CreateService().CreateUserAsync(Request());

        Assert.Equal(ResultErrorKind.RuleViolation, result.ErrorKind);
        users.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetUserById_PassesThroughToTheQuerySide()
    {
        queries
            .Setup(q => q.GetUserByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto { Id = 7, FirstName = "Jana", LastName = "Novakova", Email = "jana@example.com" });

        var user = await CreateService().GetUserByIdAsync(7);

        Assert.Equal(7, user!.Id);
    }
}
