using Application.Services;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controller for managing asset loans (checkout and return operations).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoansController(ILoanService loanService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of all loans.
    /// </summary>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of all loans.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all loans (paginated)")]
    [ProducesResponseType(typeof(PaginatedList<LoanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) { page = 1; }
        if (pageSize < 1) { pageSize = 10; }
        if (pageSize > 100) { pageSize = 100; }

        var loans = await loanService.GetAllLoansAsync(page, pageSize, cancellationToken);
        return Ok(loans);
    }

    /// <summary>
    /// Retrieves all currently active loans with asset and user details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active loans.</returns>
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get all active loans")]
    [ProducesResponseType(typeof(List<LoanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveLoans(CancellationToken cancellationToken)
    {
        var loans = await loanService.GetActiveLoansAsync(cancellationToken);
        return Ok(loans);
    }

    /// <summary>
    /// Retrieves all overdue loans (active loans past their due date).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of overdue loans.</returns>
    [HttpGet("overdue")]
    [SwaggerOperation(Summary = "Get all overdue loans")]
    [ProducesResponseType(typeof(List<LoanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdueLoans(CancellationToken cancellationToken)
    {
        var loans = await loanService.GetOverdueLoansAsync(cancellationToken);
        return Ok(loans);
    }

    /// <summary>
    /// Creates a new loan by checking out an asset to a user.
    /// </summary>
    /// <param name="dto">The loan creation data containing asset and user identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created loan with 201 status; or 409 if the asset is not available.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a loan (checkout an asset)")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateLoanDto dto, CancellationToken cancellationToken)
    {
        var result = await loanService.CreateLoanAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return CreatedAtAction(null, new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Returns a loaned asset, marking the loan as returned and the asset as available.
    /// </summary>
    /// <param name="id">The loan identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated loan with 200 status; or 409 if the loan was already returned.</returns>
    [HttpPut("{id}/return")]
    [SwaggerOperation(Summary = "Return a loaned asset")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReturnAsset(int id, CancellationToken cancellationToken)
    {
        var result = await loanService.ReturnAssetAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return Ok(result.Value);
    }
}
