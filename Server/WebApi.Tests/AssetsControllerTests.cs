using Application.Services;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using Xunit;

namespace WebApi.Tests;

public class AssetsControllerTests
{
    private readonly Mock<IAssetService> mockAssetService = new();
    private readonly Mock<ILoanService> mockLoanService = new();
    private readonly AssetsController controller;

    public AssetsControllerTests()
    {
        controller = new AssetsController(mockAssetService.Object, mockLoanService.Object);
    }

    #region GetAll

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithPaginatedListOfAssets()
    {
        // Arrange
        var paginatedList = new PaginatedList<AssetDto>
        {
            Data =
            [
                new() { Id = 1, Name = "ThinkPad T14s", Status = "Available", AssetCategoryName = "Electronics" },
                new() { Id = 2, Name = "MacBook Pro", Status = "Loaned", AssetCategoryName = "Electronics" }
            ],
            TotalCount = 2,
            PageIndex = 1,
            PageSize = 10
        };
        mockAssetService.Setup(s => s.GetAssetsAsync(null, null, 1, 10, default)).ReturnsAsync(paginatedList);

        // Act
        IActionResult result = await controller.GetAll(null, null, 1, 10, default);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        PaginatedList<AssetDto> returnValue = Assert.IsType<PaginatedList<AssetDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Data.Count);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenAssetExists()
    {
        // Arrange
        AssetDto asset = new() { Id = 1, Name = "ThinkPad T14s", Status = "Available", AssetCategoryName = "Electronics" };
        mockAssetService.Setup(s => s.GetAssetByIdAsync(1, default)).ReturnsAsync(asset);

        // Act
        IActionResult result = await controller.GetById(1, default);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        AssetDto returnValue = Assert.IsType<AssetDto>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenAssetDoesNotExist()
    {
        // Arrange
        mockAssetService.Setup(s => s.GetAssetByIdAsync(It.IsAny<int>(), default)).ReturnsAsync((AssetDto?)null);

        // Act
        IActionResult result = await controller.GetById(999, default);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenAssetIsCreated()
    {
        // Arrange
        CreateAssetDto createDto = new() { Name = "New Laptop", Description = "Brand new", AssetCategoryId = 1 };
        AssetDto createdAsset = new() { Id = 1, Name = "New Laptop", Description = "Brand new", Status = "Available", AssetCategoryName = "Electronics" };
        mockAssetService.Setup(s => s.CreateAssetAsync(createDto, default)).ReturnsAsync(createdAsset);

        // Act
        IActionResult result = await controller.Create(createDto, default);

        // Assert
        CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result);
        AssetDto returnValue = Assert.IsType<AssetDto>(createdResult.Value);
        Assert.Equal("New Laptop", returnValue.Name);
    }

    #endregion
}
