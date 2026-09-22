using Application.Services;
using Domain.Common;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers the translation from a failed result to an HTTP status code.
/// </summary>
public class ResultErrorMappingTests
{
    private readonly Mock<IAssetService> mockAssetService = new();
    private readonly Mock<ILoanService> mockLoanService = new();
    private readonly AssetsController assetsController;
    private readonly LoansController loansController;

    public ResultErrorMappingTests()
    {
        assetsController = new AssetsController(mockAssetService.Object, mockLoanService.Object);
        loansController = new LoansController(mockLoanService.Object);
    }

    [Fact]
    public async Task NotFoundKind_MapsTo404_WhateverTheMessageSays()
    {
        mockAssetService
            .Setup(s => s.DeactivateAssetAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.NotFound<AssetDto>("No such asset."));

        var response = await assetsController.Deactivate(7, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal("No such asset.", Assert.IsType<ErrorResponse>(notFound.Value).Error);
    }

    [Fact]
    public async Task RuleViolationKind_MapsTo409_EvenWhenTheMessageMentionsNotFound()
    {
        mockAssetService
            .Setup(s => s.DeactivateAssetAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AssetDto>("Asset 'Scanner' was not found available and cannot be deactivated."));

        var response = await assetsController.Deactivate(7, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async Task ConcurrencyKind_MapsTo409()
    {
        mockAssetService
            .Setup(s => s.ActivateAssetAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.ConcurrencyConflict<AssetDto>());

        Assert.IsType<ConflictObjectResult>(await assetsController.Activate(7, CancellationToken.None));
    }

    [Fact]
    public async Task CreateLoan_ForMissingAsset_MapsTo404()
    {
        mockLoanService
            .Setup(s => s.CreateLoanAsync(It.IsAny<CreateLoanDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.NotFound<LoanDto>("Asset with ID 999 not found."));

        var response = await loansController.Create(new CreateLoanDto { AssetId = 999, BorrowedById = 1, DueDate = DateTime.UtcNow.AddDays(1) }, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task ReturnLoan_ForUnavailableAsset_MapsTo409()
    {
        mockLoanService
            .Setup(s => s.ReturnAssetAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<LoanDto>("This loan has already been returned."));

        Assert.IsType<ConflictObjectResult>(await loansController.ReturnAsset(4, CancellationToken.None));
    }
}
