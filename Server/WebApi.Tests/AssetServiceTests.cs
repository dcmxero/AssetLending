using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Services;
using Domain.Common;
using Domain.Enums;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers asset creation, updates and the activation pair, including the read-back
/// that keeps a response describing what was persisted.
/// </summary>
public class AssetServiceTests
{
    private readonly Mock<IAssetRepository> assets = new();
    private readonly Mock<IAssetQueries> queries = new();
    private readonly Mock<IUnitOfWork> unitOfWork = new();

    private readonly Asset asset = new() { Id = 1, Name = "ThinkPad", AssetCategoryId = 1 };

    private AssetService CreateService() => new(
        assets.Object,
        queries.Object,
        unitOfWork.Object,
        NullLogger<AssetService>.Instance);

    private void GivenAssetExists()
    {
        assets.Setup(r => r.GetByIdAsync(asset.Id, It.IsAny<CancellationToken>())).ReturnsAsync(asset);
    }

    private void GivenTheQuerySideReturns(string name = "ThinkPad", string categoryName = "Electronics")
    {
        queries
            .Setup(q => q.GetAssetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssetDto
            {
                Id = asset.Id,
                Name = name,
                Status = nameof(AssetStatus.Available),
                AssetCategoryId = asset.AssetCategoryId,
                AssetCategoryName = categoryName
            });
    }

    [Fact]
    public async Task CreateAsset_AnswersWithThePersistedAsset_NotTheInMemoryOne()
    {
        GivenTheQuerySideReturns(categoryName: "Electronics");

        var result = await CreateService().CreateAssetAsync(new CreateAssetDto { Name = "ThinkPad", AssetCategoryId = 1 });

        Assert.Equal("Electronics", result.AssetCategoryName);
        assets.Verify(r => r.AddAsync(It.IsAny<Asset>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsset_AnswersWithTheCategoryItWasMovedTo()
    {
        GivenAssetExists();
        GivenTheQuerySideReturns(categoryName: "Tools");

        var result = await CreateService().UpdateAssetAsync(asset.Id, new UpdateAssetDto { Name = "ThinkPad", AssetCategoryId = 2 });

        Assert.True(result.IsSuccess);
        Assert.Equal(2, asset.AssetCategoryId);
        Assert.Equal("Tools", result.Value!.AssetCategoryName);
    }

    [Fact]
    public async Task UpdateAsset_ReportsNotFound_WhenTheAssetIsMissing()
    {
        var result = await CreateService().UpdateAssetAsync(404, new UpdateAssetDto { Name = "Nothing", AssetCategoryId = 1 });

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsset_ReportsConcurrencyConflict_WhenTheSaveLosesTheRace()
    {
        GivenAssetExists();
        unitOfWork.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConcurrencyConflictException(new InvalidOperationException()));

        var result = await CreateService().UpdateAssetAsync(asset.Id, new UpdateAssetDto { Name = "ThinkPad", AssetCategoryId = 1 });

        Assert.Equal(ResultErrorKind.ConcurrencyConflict, result.ErrorKind);
    }

    [Fact]
    public async Task DeactivateAsset_RefusesWhileTheAssetIsLoaned()
    {
        GivenAssetExists();
        asset.Status = AssetStatus.Loaned;

        var result = await CreateService().DeactivateAssetAsync(asset.Id);

        Assert.Equal(ResultErrorKind.RuleViolation, result.ErrorKind);
        Assert.True(asset.IsActive);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeactivateAsset_SoftDeletesAnAvailableAsset()
    {
        GivenAssetExists();
        GivenTheQuerySideReturns();

        var result = await CreateService().DeactivateAssetAsync(asset.Id);

        Assert.True(result.IsSuccess);
        Assert.False(asset.IsActive);
    }

    [Fact]
    public async Task ActivateAsset_RefusesAnAssetThatIsAlreadyActive()
    {
        GivenAssetExists();

        var result = await CreateService().ActivateAssetAsync(asset.Id);

        Assert.Equal(ResultErrorKind.RuleViolation, result.ErrorKind);
    }

    [Fact]
    public async Task ActivateAsset_BringsBackADeactivatedAsset()
    {
        GivenAssetExists();
        GivenTheQuerySideReturns();
        asset.Deactivate();

        var result = await CreateService().ActivateAssetAsync(asset.Id);

        Assert.True(result.IsSuccess);
        Assert.True(asset.IsActive);
    }

    [Fact]
    public async Task DeactivateAsset_ReportsNotFound_WhenTheAssetIsMissing()
    {
        var result = await CreateService().DeactivateAssetAsync(404);

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
    }
}
