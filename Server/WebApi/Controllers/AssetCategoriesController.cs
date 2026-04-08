using Application.Services;
using DTOs.Asset;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controller for managing asset categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssetCategoriesController(IAssetCategoryService categoryService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves all asset categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all asset categories.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all asset categories")]
    [ProducesResponseType(typeof(List<AssetCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAllCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves an asset category by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise, 404.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get asset category by ID")]
    [ProducesResponseType(typeof(AssetCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    /// <summary>
    /// Creates a new asset category.
    /// </summary>
    /// <param name="dto">The category creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category with 201 status.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new asset category")]
    [ProducesResponseType(typeof(AssetCategoryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAssetCategoryDto dto, CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateCategoryAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }
}
