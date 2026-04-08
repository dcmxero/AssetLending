using Application.Services;
using Domain.Enums;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controller for managing loanable assets (laptops, tools, monitors, etc.).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssetsController(IAssetService assetService, ILoanService loanService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of assets, optionally filtered by status and/or category.
    /// </summary>
    /// <param name="status">Optional status filter (Available, Loaned, Reserved).</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of assets.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all assets (paginated, with optional status and category filter)")]
    [ProducesResponseType(typeof(PaginatedList<AssetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(AssetStatus? status, int? categoryId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) { page = 1; }
        if (pageSize < 1) { pageSize = 10; }
        if (pageSize > 100) { pageSize = 100; }

        var assets = await assetService.GetAssetsAsync(status, categoryId, page, pageSize, cancellationToken);
        return Ok(assets);
    }

    /// <summary>
    /// Retrieves an asset by its identifier.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The asset if found; otherwise, 404.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get asset by ID")]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var asset = await assetService.GetAssetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return NotFound();
        }

        return Ok(asset);
    }

    /// <summary>
    /// Creates a new asset with Available status.
    /// </summary>
    /// <param name="dto">The asset creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created asset with 201 status.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new asset")]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAssetDto dto, CancellationToken cancellationToken)
    {
        var asset = await assetService.CreateAssetAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
    }

    /// <summary>
    /// Updates an existing asset's properties.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="dto">The updated asset data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated asset; or 404/409 on failure.</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an asset")]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAssetDto dto, CancellationToken cancellationToken)
    {
        var result = await assetService.UpdateAssetAsync(id, dto, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(new ErrorResponse { Error = result.Error! });
            }

            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deactivates an asset (soft delete). Only available assets can be deactivated.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deactivated asset; or 404/409 on failure.</returns>
    [HttpPatch("{id}/deactivate")]
    [SwaggerOperation(Summary = "Deactivate an asset (soft delete)")]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var result = await assetService.DeactivateAssetAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(new ErrorResponse { Error = result.Error! });
            }

            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Reactivates a previously deactivated asset.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The activated asset; or 404/409 on failure.</returns>
    [HttpPatch("{id}/activate")]
    [SwaggerOperation(Summary = "Activate an asset")]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        var result = await assetService.ActivateAssetAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(new ErrorResponse { Error = result.Error! });
            }

            return Conflict(new ErrorResponse { Error = result.Error! });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a paginated loan history for a specific asset.
    /// </summary>
    /// <param name="id">The asset identifier.</param>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of loans for the asset.</returns>
    [HttpGet("{id}/loans")]
    [SwaggerOperation(Summary = "Get loan history for an asset")]
    [ProducesResponseType(typeof(PaginatedList<LoanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoanHistory(int id, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) { page = 1; }
        if (pageSize < 1) { pageSize = 10; }
        if (pageSize > 100) { pageSize = 100; }

        var loans = await loanService.GetLoansByAssetIdAsync(id, page, pageSize, cancellationToken);
        return Ok(loans);
    }
}
