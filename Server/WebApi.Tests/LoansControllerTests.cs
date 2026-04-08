using Application.Services;
using Domain.Common;
using DTOs.Asset;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using Xunit;

namespace WebApi.Tests;

public class LoansControllerTests
{
    private readonly Mock<ILoanService> mockLoanService = new();
    private readonly LoansController controller;

    public LoansControllerTests()
    {
        controller = new LoansController(mockLoanService.Object);
    }

    #region GetActiveLoans

    [Fact]
    public async Task GetActiveLoans_ReturnsOkResult_WithActiveLoans()
    {
        // Arrange
        List<LoanDto> loans =
        [
            new() { Id = 1, AssetId = 1, AssetName = "ThinkPad", BorrowedById = 1, BorrowedByName = "Ján Novák", DueDate = DateTime.UtcNow.AddDays(7), Status = "Active" }
        ];
        mockLoanService.Setup(s => s.GetActiveLoansAsync(default)).ReturnsAsync(loans);

        // Act
        IActionResult result = await controller.GetActiveLoans(default);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<LoanDto> returnValue = Assert.IsType<List<LoanDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    #endregion

    #region Create (Checkout)

    [Fact]
    public async Task Create_ReturnsCreatedResult_WhenCheckoutSucceeds()
    {
        // Arrange
        CreateLoanDto createDto = new() { AssetId = 1, BorrowedById = 1, DueDate = DateTime.UtcNow.AddDays(7) };
        LoanDto createdLoan = new() { Id = 1, AssetId = 1, AssetName = "ThinkPad", BorrowedById = 1, BorrowedByName = "Ján Novák", DueDate = createDto.DueDate, Status = "Active" };
        mockLoanService.Setup(s => s.CreateLoanAsync(createDto, default)).ReturnsAsync(Result.Success(createdLoan));

        // Act
        IActionResult result = await controller.Create(createDto, default);

        // Assert
        CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result);
        LoanDto returnValue = Assert.IsType<LoanDto>(createdResult.Value);
        Assert.Equal("Active", returnValue.Status);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenAssetNotAvailable()
    {
        // Arrange
        CreateLoanDto createDto = new() { AssetId = 1, BorrowedById = 1, DueDate = DateTime.UtcNow.AddDays(7) };
        mockLoanService.Setup(s => s.CreateLoanAsync(createDto, default)).ReturnsAsync(Result.Failure<LoanDto>("Asset is not available."));

        // Act
        IActionResult result = await controller.Create(createDto, default);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    #endregion

    #region ReturnAsset

    [Fact]
    public async Task ReturnAsset_ReturnsOkResult_WhenReturnSucceeds()
    {
        // Arrange
        LoanDto returnedLoan = new() { Id = 1, AssetId = 1, AssetName = "ThinkPad", BorrowedById = 1, BorrowedByName = "Ján Novák", ReturnedAt = DateTime.UtcNow, Status = "Returned" };
        mockLoanService.Setup(s => s.ReturnAssetAsync(1, default)).ReturnsAsync(Result.Success(returnedLoan));

        // Act
        IActionResult result = await controller.ReturnAsset(1, default);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        LoanDto returnValue = Assert.IsType<LoanDto>(okResult.Value);
        Assert.Equal("Returned", returnValue.Status);
    }

    [Fact]
    public async Task ReturnAsset_ReturnsConflict_WhenAlreadyReturned()
    {
        // Arrange
        mockLoanService.Setup(s => s.ReturnAssetAsync(1, default)).ReturnsAsync(Result.Failure<LoanDto>("Already returned."));

        // Act
        IActionResult result = await controller.ReturnAsset(1, default);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    #endregion
}
