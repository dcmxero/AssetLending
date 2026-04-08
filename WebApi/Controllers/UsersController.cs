using Application.Services;
using DTOs.Common;
using DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controller for managing users who can borrow and reserve assets.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of users.
    /// </summary>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of users.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all users (paginated)")]
    [ProducesResponseType(typeof(PaginatedList<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) { page = 1; }
        if (pageSize < 1) { pageSize = 10; }
        if (pageSize > 100) { pageSize = 100; }

        var users = await userService.GetUsersAsync(page, pageSize, cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Retrieves a user by their identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found; otherwise, 404.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto">The user creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user with 201 status; or 409 if the email is already taken.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await userService.CreateUserAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}
